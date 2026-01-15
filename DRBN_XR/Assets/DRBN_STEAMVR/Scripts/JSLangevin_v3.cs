using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.InputSystem;
using System.IO;

/// <summary>
/// Implements a Langevin dynamics integrator for molecular simulation in Unity.
/// Applies stochastic forces to Rigidbody components to simulate Brownian motion at a given temperature.
/// </summary>
/// <remarks>
/// GameObjects must be tagged with "molecule" to be included in the simulation.
/// Each child Rigidbody of tagged GameObjects will receive Langevin thermostat forces.
/// </remarks>
public class JSLangevin_v3 : MonoBehaviour {

    /// <summary>List of all Rigidbody components participating in the simulation.</summary>
    public List<Rigidbody> GOS;
    
    /// <summary>List of parent GameObjects tagged as "molecule".</summary>
    public List<GameObject> GOmol;
    
    /// <summary>Simulation temperature in Kelvin.</summary>
    public float temp = 300.0f;
    
    /// <summary>Boltzmann constant in simulation units (Distance:nm, Time:ns, Mass:kDa, Temperature:K).</summary>
    public static float kB = 8.31f;
    
    /// <summary>Friction coefficient for the Langevin thermostat.</summary>
    public float friction = 28.655f;
    
    /// <summary>Integration timestep from global variables.</summary>
    public static float dt = GlobalVars.dt;
    
  
    

    /// <summary>
    /// Collects all GameObjects tagged with "molecule" and their child Rigidbody components.
    /// </summary>
    /// <returns>A tuple containing the list of Rigidbodies and the list of molecule GameObjects.</returns>
    (List<Rigidbody>,List<GameObject>) CountObjects()
    {
	GOmol = GameObject.FindGameObjectsWithTag("molecule").ToList<GameObject>();
        foreach (GameObject go in GOmol)
            {GOS.AddRange(go.GetComponentsInChildren<Rigidbody>());}
        
        return (GOS,GOmol);
    }
    /// <summary>
    /// Applies a single Langevin integration step to a Rigidbody.
    /// Adds stochastic and friction forces according to the Langevin equation.
    /// </summary>
    /// <param name="domain">The Rigidbody to integrate.</param>
    void integrate(Rigidbody domain)
    {
        MathNet.Numerics.Distributions.Normal normalDist = new MathNet.Numerics.Distributions.Normal(0,1);
        float coll_constant = friction/domain.mass;
        float sigma = (float)Math.Sqrt(2*kB*temp*coll_constant/domain.mass);
        Vector3 randomVector = new Vector3((float)normalDist.Sample(),(float)normalDist.Sample(),(float)normalDist.Sample());
        domain.AddForce((float)Math.Sqrt(dt)*sigma*randomVector-dt*coll_constant*domain.linearVelocity,ForceMode.VelocityChange);
    }
    
    
    /// <summary>
    /// Displays the current simulation time on screen.
    /// </summary>
    void OnGUI()
    {
        var t = GlobalVars.i*dt;
        GUI.Label(new Rect(0, 0, 1000, 100), "time " + t.ToString() + "ns");
    }

    /// <summary>
    /// Iterates through all Rigidbodies and applies Langevin integration.
    /// </summary>
    void LangevinSolver()
    {
      foreach (Rigidbody GO in GOS)
      {    
         integrate(GO);
      }
    }
    /// <summary>
    /// Calculates and logs the instantaneous temperature based on kinetic energy.
    /// Uses the equipartition theorem: T = 2*KE / ((3N-3)*kB).
    /// </summary>
    void PrintTemperature()
    {
      float temperature;
      float kin_eng = 0;
      int n_domains = GOS.Count;
      foreach (Rigidbody GO in GOS)
      {
         kin_eng += 0.5f*GO.mass*Vector3.Dot(GO.linearVelocity,GO.linearVelocity);
      }
      temperature = 2*kin_eng / ((3*n_domains-3)*kB);
      Debug.Log(temperature);
   
    }
    
    /// <summary>
    /// Initializes the simulation by collecting molecule objects and assigning
    /// initial velocities from a Maxwell-Boltzmann distribution.
    /// </summary>
    void Start()
    {
        Debug.Log(Time.fixedDeltaTime);
        CountObjects();
        foreach (Rigidbody GO in GOS)
        {
          var std = Math.Sqrt(kB*temp/GO.mass);
          MathNet.Numerics.Distributions.Normal normalDist = new MathNet.Numerics.Distributions.Normal(0,std);
          Vector3 newVelocity = new Vector3((float)normalDist.Sample(),(float)normalDist.Sample(),(float)normalDist.Sample());
          GO.AddForce(newVelocity,ForceMode.VelocityChange);
        }
    }
    /// <summary>
    /// Called every fixed timestep. Runs the Langevin solver, periodically logs temperature,
    /// and stops the simulation after 600000 iterations.
    /// </summary>
    void FixedUpdate ()
    {   
        if (GlobalVars.i > 0)
        {
          LangevinSolver();
        }
        if (GlobalVars.i % 500 == 0 || GlobalVars.i == 0)
        {
            PrintTemperature();
        }
     if (GlobalVars.i >= 600000)
     { 
       UnityEditor.EditorApplication.isPlaying = false;
      }
    }
}
