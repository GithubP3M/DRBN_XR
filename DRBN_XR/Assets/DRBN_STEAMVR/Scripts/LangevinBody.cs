using UnityEngine;

/// <summary>
/// Simple 3D Langevin dynamics integrator.
/// Attach to any GameObject you want to be thermostatted.
/// </summary>
[RequireComponent(typeof(Transform))]
public class LangevinBody : MonoBehaviour
{
    /// <summary>
    /// Reference to the global Langevin manager holding shared simulation parameters.
    /// </summary>
    public Langevin_TL manager;

    [Header("Integration")]
    /// <summary>
    /// Current velocity used by the Transform-based integrator path.
    /// If a <see cref="Rigidbody"/> is present, its velocity is used instead.
    /// </summary>
    public Vector3 velocity;            // Current velocity
    public bool useFixedDeltaTime = true;

    /// <summary>
    /// Cached velocity for debug visualization.
    /// </summary>
    public Vector3 lastVelocity;

    /// <summary>
    /// Cached total force applied in the last integration step (deterministic + random).
    /// </summary>
    public Vector3 lastForceAdded;

    /// <summary>
    /// Cached deterministic force applied in the last integration step.
    /// </summary>
    public Vector3 lastForceDeterministic;

    /// <summary>
    /// Cached random (stochastic) force applied in the last integration step.
    /// </summary>
    public Vector3 lastForceRandom;

    /// <summary>
    /// Effective radius estimated from this GameObject's collider bounds in world space.
    /// Used when the manager enables viscosity-based drag.
    /// </summary>
    public float effectiveRadius;

    // Optional external deterministic force in world units
    public Vector3 externalForce = Vector3.zero;

    System.Random rng;
    float invSqrtMass;
    Rigidbody rb;
    Collider cachedCollider;

    void Awake()
    {
        rng = new System.Random();
        rb = GetComponent<Rigidbody>();
        cachedCollider = GetComponentInChildren<Collider>();
        if (!manager)
        {
            manager = Langevin_TL.Instance;
        }
        RecomputeCachedParameters();
    }

    public void SetManager(Langevin_TL m)
    {
        manager = m;
        RecomputeCachedParameters();
    }

    public void RecomputeCachedParameters()
    {
        float mass = manager ? manager.mass : 1.0e-23f;
        invSqrtMass = 1.0f / Mathf.Sqrt(mass);
        if (rb)
        {
            rb.mass = mass;
        }
        UpdateEffectiveRadius();
    }

    /// <summary>
    /// Updates <see cref="effectiveRadius"/> from collider bounds.
    /// The estimate uses the maximum bounds extent as a sphere radius proxy.
    /// </summary>
    public void UpdateEffectiveRadius()
    {
        if (!cachedCollider)
        {
            cachedCollider = GetComponentInChildren<Collider>();
        }
        if (!cachedCollider)
        {
            effectiveRadius = 0f;
            return;
        }

        Vector3 e = cachedCollider.bounds.extents;
        effectiveRadius = Mathf.Max(e.x, Mathf.Max(e.y, e.z));
    }

    float GetEffectiveGamma(float fallbackGamma)
    {
        if (!manager) return fallbackGamma;
        if (!manager.useViscosityDrag) return fallbackGamma;
        if (effectiveRadius <= 0f) return fallbackGamma;

        float g = 6.0f * Mathf.PI * manager.viscosity * effectiveRadius;
        if (manager.clampForces)
        {
            if (g < 0f) g = 0f;
            if (g > manager.maxGamma) g = manager.maxGamma;
        }
        return g;
    }

    bool IsFinite(float f)
    {
        return !float.IsNaN(f) && !float.IsInfinity(f);
    }

    bool IsFinite(Vector3 v)
    {
        return IsFinite(v.x) && IsFinite(v.y) && IsFinite(v.z);
    }

    Vector3 ClampMagnitude(Vector3 v, float maxMag)
    {
        if (maxMag <= 0f) return Vector3.zero;
        float mag = v.magnitude;
        if (!IsFinite(mag) || mag == 0f) return v;
        if (mag <= maxMag) return v;
        return v * (maxMag / mag);
    }

    void FixedUpdate()
    {
        float dt = useFixedDeltaTime ? Time.fixedDeltaTime : Time.deltaTime;
        if (rb)
        {
            StepLangevinRigidbody(dt);
        }
        else
        {
            StepLangevinTransform(dt);
        }
    }

    void StepLangevinTransform(float dt)
    {
        // Simple Euler–Maruyama integration:
        // m dv/dt = F_det - gamma v + R(t)
        // with <R_i(t) R_j(t')> = 2 gamma kT delta_ij delta(t-t')
        // Discrete update:
        // v_{n+1} = v_n + (F_det/m - gamma v_n/m) dt + sqrt(2 gamma kT / m) * sqrt(dt) * N(0,1)

        if (dt <= 0f) return;

        float mass = manager ? manager.mass : 1.0e-23f;
        float gamma = manager ? manager.gamma : 1.0f;
        gamma = GetEffectiveGamma(gamma);
        float kT = manager ? manager.EffectiveKBT : 1.380649e-23f * 300.0f;

        Vector3 vOld = velocity;

        // Deterministic acceleration
        Vector3 aDet = externalForce / mass - (gamma / mass) * vOld;

        // Noise strength: sigma = sqrt(2 gamma kT / m)
        float sigma = Mathf.Sqrt(2.0f * gamma * kT) * invSqrtMass;

        float sqrtDt = Mathf.Sqrt(dt);
        Vector3 noise = new Vector3(
            NextGaussian() * sigma * sqrtDt,
            NextGaussian() * sigma * sqrtDt,
            NextGaussian() * sigma * sqrtDt
        );

        // Velocity update
        Vector3 dvDet = aDet * dt;
        Vector3 dvRnd = noise;
        Vector3 dv = dvDet + dvRnd;
        velocity = vOld + dv;

        lastVelocity = velocity;
        lastForceDeterministic = dvDet * (mass / dt);
        lastForceRandom = dvRnd * (mass / dt);
        lastForceAdded = lastForceDeterministic + lastForceRandom;

        // Position update
        transform.position += velocity * dt;
    }

    void StepLangevinRigidbody(float dt)
    {
        if (dt <= 0f || float.IsNaN(dt) || float.IsInfinity(dt)) return;

        float mass = manager ? manager.mass : rb.mass;
        float gamma = manager ? manager.gamma : 1.0f;
        gamma = GetEffectiveGamma(gamma);
        float kT = manager ? manager.EffectiveKBT : 1.380649e-23f * 300.0f;

        if (!IsFinite(gamma) || gamma < 0f) return;
        if (!IsFinite(kT) || kT < 0f) return;

        Vector3 v = rb.linearVelocity;

        // Deterministic force: F_det - gamma v
        Vector3 fDet = externalForce - gamma * v;

        // Random force with <F_i(t) F_j(t')> = 2 gamma kT delta_ij delta(t-t')
        // Discrete: F_rand = sqrt(2 gamma kT / dt) * N(0,1)
        float sigmaF2 = 2.0f * gamma * kT / dt;
        if (!IsFinite(sigmaF2) || sigmaF2 < 0f) return;
        float sigmaF = Mathf.Sqrt(sigmaF2);
        Vector3 fRnd = new Vector3(
            NextGaussian() * sigmaF,
            NextGaussian() * sigmaF,
            NextGaussian() * sigmaF
        );

        Vector3 fTotal = fDet + fRnd;

        if (!IsFinite(fDet) || !IsFinite(fRnd) || !IsFinite(fTotal))
        {
            return;
        }

        if (manager && manager.clampForces)
        {
            fDet = ClampMagnitude(fDet, manager.maxForceMagnitude);
            fRnd = ClampMagnitude(fRnd, manager.maxForceMagnitude);
            fTotal = ClampMagnitude(fTotal, manager.maxForceMagnitude);
        }

        rb.AddForce(fTotal, ForceMode.Force);

        lastVelocity = rb.linearVelocity;
        lastForceDeterministic = fDet;
        lastForceRandom = fRnd;
        lastForceAdded = fTotal;
    }

    // Box–Muller Gaussian N(0,1)
    float NextGaussian()
    {
        float u1 = 1f - (float)rng.NextDouble();
        float u2 = 1f - (float)rng.NextDouble();
        float r = Mathf.Sqrt(-2.0f * Mathf.Log(u1));
        float theta = 2.0f * Mathf.PI * u2;
        return r * Mathf.Cos(theta);
    }

    // Utility: set a deterministic force from outside
    public void SetExternalForce(Vector3 force)
    {
        externalForce = force;
    }
}
