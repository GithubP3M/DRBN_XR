using UnityEngine;
using System;
using System.Linq;

public class Langevin_UD : MonoBehaviour
{
    public Rigidbody[] GOS;

    // Reduced (dimensionless/Unity) Langevin parameters
    // These are NOT SI values; they are the parameters of the underdamped Langevin
    // equation in Unity units: m d v / dt = -γ m v + √(2 γ m T / dt) ξ
    [Header("Reduced Langevin parameters")]
    public float mass = 1f;         // m̃
    public float gamma = 1f;        // γ̃ [1/s]
    public float temperature = 10f; // T̃, gives D̃ = T̃/γ̃ ≈ 10 if gamma = 1

    // For GUI display
    public static float sigmaf;     // last computed σ
    public static float frictionf;  // γ m

    void Awake()
    {
        CountObjects();

        foreach (Rigidbody rb in GOS)
        {
            if (rb != null && rb.transform.root.name != "char_shadow")
            {
                rb.mass = mass;
                rb.linearDamping = 0f; // we handle friction ourselves
            }
        }

        frictionf = gamma * mass;
    }

    Rigidbody[] CountObjects()
    {
        GOS = FindObjectsByType<Rigidbody>(FindObjectsSortMode.InstanceID);
        return GOS;
    }

    // Gaussian(0,1) using Box–Muller
    float GaussianRandom()
    {
        float u1 = UnityEngine.Random.value;
        float u2 = UnityEngine.Random.value;
        return Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
    }

    // Underdamped Langevin force in reduced units
    // F = -γ m v + √(2 γ m T / dt) R
    Vector3 LangevinForce(Rigidbody rb)
    {
        float dt = Time.fixedDeltaTime;
        Vector3 v = rb.linearVelocity;

        // friction part
        float friction = gamma * mass;
        Vector3 frictionForce = -friction * v;

        // noise part (per component)
        float sigma = Mathf.Sqrt(2f * gamma * mass * temperature / dt);
        Vector3 randomForce = sigma * new Vector3(
            GaussianRandom(),
            GaussianRandom(),
            GaussianRandom()
        );

        sigmaf = sigma;
        frictionf = friction;

        return frictionForce + randomForce;
    }

    void FixedUpdate()
    {
        // apply Langevin forces each physics step
        foreach (Rigidbody rb in GOS)
        {
            if (rb != null && !rb.isKinematic && rb.transform.root.name != "char_shadow")
            {
                Vector3 F = LangevinForce(rb);
                rb.AddForce(F, ForceMode.Force);
            }
        }
    }

    void Update()
    {
        // Optionally refresh list (you can remove this if your particle set is static)
        CountObjects();

        // Unfreeze
        if (Input.GetKeyDown("o"))
        {
            bool isK = GOS.Any(p => p.isKinematic == true);
            if (isK)
            {
                foreach (Rigidbody GO in GOS)
                    GO.isKinematic = false;
            }
        }

        // Freeze
        if (Input.GetKeyDown("p"))
        {
            bool isK = GOS.Any(p => p.isKinematic == true);
            if (!isK)
            {
                foreach (Rigidbody GO in GOS)
                    GO.isKinematic = true;
            }
        }

        // Time scale toggle (keep dt small for better statistics)
        if (Input.GetKeyDown("i"))
        {
            if (Time.timeScale == 1.0f)
            {
                Time.timeScale = 30.0f;
                // keep a reasonable fixedDeltaTime; do NOT increase dt when speeding up
                Time.fixedDeltaTime = 0.02f; 
            }
            else if (Time.timeScale > 1.0f)
            {
                Time.timeScale = 1.0f;
                Time.fixedDeltaTime = 0.02f;
            }
        }

        // Temperature control in reduced units (changes diffusion D = T / γ)
        if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            if (temperature < 1e4f)
                temperature += 1f;
        }

        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            if (temperature > 0f)
                temperature = Mathf.Max(0f, temperature - 1f);
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 300, 20), $"T̃ (reduced) = {temperature:F1}");
        GUI.Label(new Rect(0, 20, 300, 20), $"γ̃ = {gamma:F3}  m̃ = {mass:F3}");
        GUI.Label(new Rect(0, 40, 300, 20), $"Friction = γ̃ m̃ = {frictionf:E3}");
        GUI.Label(new Rect(0, 60, 300, 20), $"σ (per comp) = {sigmaf:E3}");
    }
}
