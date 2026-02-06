using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper/manager to control which GameObjects are under Langevin dynamics.
/// </summary>
public class Langevin_TL : MonoBehaviour
{
    public static Langevin_TL Instance { get; private set; }

    [Header("Physical parameters")]
    /// <summary>
    /// Global particle mass used by <see cref="LangevinBody"/>.
    /// In Unity-units mode this is in arbitrary (Unity) mass units; in SI-like mode this is kg.
    /// </summary>
    public float mass = 4.5e-23f;

    /// <summary>
    /// Friction coefficient.
    /// In the force formulation (Rigidbody mode), this is a force-per-velocity coefficient so that friction is <c>-gamma * v</c>.
    /// </summary>
    public float gamma = 1.0f;

    /// <summary>
    /// If true, derive a per-body friction coefficient from collider size using a Stokes-like model:
    /// <c>gamma_i = 6 * pi * viscosity * radius_i</c>.
    /// This overrides <see cref="gamma"/> on a per-body basis.
    /// </summary>
    public bool useViscosityDrag = false;

    /// <summary>
    /// Unity-scaled dynamic viscosity used when <see cref="useViscosityDrag"/> is true.
    /// Units are arbitrary but must be consistent with your chosen Unity units (length/time/mass).
    /// </summary>
    public float viscosity = 1.0f;

    /// <summary>
    /// If true, clamp derived parameters (e.g. per-body gamma) and applied forces to avoid NaN/Infinity and unstable spikes.
    /// </summary>
    public bool clampForces = true;

    /// <summary>
    /// Upper bound for the effective friction coefficient used by bodies (useful when collider bounds are huge).
    /// </summary>
    public float maxGamma = 1000.0f;

    /// <summary>
    /// Upper bound for the magnitude of forces applied in Rigidbody mode.
    /// </summary>
    public float maxForceMagnitude = 1000.0f;

    /// <summary>
    /// Temperature in Kelvin (used only when <see cref="useUnityUnits"/> is false).
    /// </summary>
    public float temperature = 300.0f;

    /// <summary>
    /// Boltzmann constant (used only when <see cref="useUnityUnits"/> is false).
    /// This can be rescaled to your chosen unit system.
    /// </summary>
    public float boltzmann = 1.380649e-23f;

    /// <summary>
    /// If true, treat the simulation as non-dimensionalized in "Unity units".
    /// In this mode, thermal energy is <c>kB_unity * temperature</c> instead of <c>boltzmann * temperature</c>.
    /// </summary>
    public bool useUnityUnits = false;

    /// <summary>
    /// Unity-scaled Boltzmann constant (used only when <see cref="useUnityUnits"/> is true).
    /// Effective thermal energy becomes <c>kB_unity * temperature</c>.
    /// Set to ~1 for visible fluctuations while still allowing temperature to modulate intensity.
    /// </summary>
    public float kB_unity = 1.0f;

    /// <summary>
    /// Returns the effective thermal energy kBT based on current settings.
    /// </summary>
    public float EffectiveKBT => useUnityUnits ? (kB_unity * temperature) : (boltzmann * temperature);

    [Header("Timestep")]
    public bool useGlobalVarsDt = false;
    public float minFixedDeltaTime = 1.0e-6f;
    public float maxFixedDeltaTime = 0.05f;

    [Header("Automatic discovery")]
    public bool findAllOnStart = true;

    [Header("Explicitly managed bodies")]
    public List<LangevinBody> bodies = new List<LangevinBody>();

    [Header("Debug")]
    public bool debugDraw = false;
    public float debugVelocityScale = 1.0f;
    public float debugForceScale = 1.0f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            return;
        }
        Instance = this;
    }

    void Start()
    {
        ApplyFixedDeltaTime();
        if (findAllOnStart)
        {
            FindAllBodiesInScene();
        }
        NotifyParametersChanged();
    }

    void ApplyFixedDeltaTime()
    {
        if (!useGlobalVarsDt) return;

        float dt = GlobalVars.dt;
        if (float.IsNaN(dt) || float.IsInfinity(dt)) return;

        if (dt < minFixedDeltaTime) dt = minFixedDeltaTime;
        if (dt > maxFixedDeltaTime) dt = maxFixedDeltaTime;

        Time.fixedDeltaTime = dt;
    }

    void FixedUpdate()
    {
        if (!debugDraw) return;

        float dt = Time.fixedDeltaTime;
        foreach (var b in bodies)
        {
            if (!b) continue;
            Debug.DrawRay(b.transform.position, b.lastVelocity * debugVelocityScale, Color.green, dt, false);
            Debug.DrawRay(b.transform.position, b.lastForceDeterministic * debugForceScale, Color.blue, dt, false);
            Debug.DrawRay(b.transform.position, b.lastForceRandom * debugForceScale, Color.red, dt, false);
        }
    }

    void OnDrawGizmos()
    {
        if (!debugDraw) return;

        var drawBodies = bodies;
        if (drawBodies == null || drawBodies.Count == 0)
        {
            drawBodies = new List<LangevinBody>(FindObjectsByType<LangevinBody>(FindObjectsSortMode.None));
        }

        foreach (var b in drawBodies)
        {
            if (!b) continue;
            Vector3 p = b.transform.position;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(p, p + b.lastVelocity * debugVelocityScale);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(p, p + b.lastForceDeterministic * debugForceScale);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(p, p + b.lastForceRandom * debugForceScale);
        }
    }

    public void FindAllBodiesInScene()
    {
        bodies.Clear();
        bodies.AddRange(FindObjectsByType<LangevinBody>(FindObjectsSortMode.None));
        foreach (var b in bodies)
        {
            if (!b) continue;
            b.SetManager(this);
        }
    }

    public void AddBody(GameObject go)
    {
        if (!go) return;
        LangevinBody body = go.GetComponent<LangevinBody>();
        if (!body)
        {
            body = go.AddComponent<LangevinBody>();
        }
        body.SetManager(this);
        if (!bodies.Contains(body))
        {
            bodies.Add(body);
        }
    }

    public void RemoveBody(GameObject go)
    {
        if (!go) return;
        LangevinBody body = go.GetComponent<LangevinBody>();
        if (body && bodies.Contains(body))
        {
            bodies.Remove(body);
        }
    }

    // Example: set same parameters for all registered bodies
    public void SetGlobalParameters(
        float mass,
        float gamma,
        float temperature
    )
    {
        this.mass = mass;
        this.gamma = gamma;
        this.temperature = temperature;
        NotifyParametersChanged();
    }

    public void NotifyParametersChanged()
    {
        foreach (var b in bodies)
        {
            if (!b) continue;
            b.RecomputeCachedParameters();
        }
    }

    // Example: apply a uniform external force to all bodies
    public void SetGlobalForce(Vector3 force)
    {
        foreach (var b in bodies)
        {
            if (!b) continue;
            b.SetExternalForce(force);
        }
    }
}
