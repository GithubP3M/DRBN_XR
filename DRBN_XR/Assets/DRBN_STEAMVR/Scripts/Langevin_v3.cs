using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.InputSystem;

public class Langevin_v3 : MonoBehaviour {

    public List<Rigidbody> GOS;
    public List<GameObject> GOmol;
    public List<GameObject> GOsol;
    
    /*Langevin variables*/
    public static float temp = 300.0f;
    public static float kB = 1.38f * (float)Math.Pow(10.0f, -5.0f);
    public static float dt = 1e-3f;
    public static float relax = 100*dt; //Pa.s-1 =6.6cPoise   
    public static float maxSpeed = 0.01f;

    //make a list of objects that are tagged with the "molecule" tag
     (List<Rigidbody>,List<GameObject>) CountObjects()
    {
	GOmol = GameObject.FindGameObjectsWithTag("molecule").ToList<GameObject>();
        foreach (GameObject go in GOmol)
            {GOS.AddRange(go.GetComponentsInChildren<Rigidbody>());}
        GOsol = GameObject.FindGameObjectsWithTag("solvent").ToList<GameObject>();
        foreach (GameObject go in GOsol)
            {GOS.AddRange(go.GetComponentsInChildren<Rigidbody>());}
        
  
        return (GOS,GOmol);
    }

    float RandomGaussian(float minValue = -1.0f, float maxValue = 1.0f)
    {
        float u, v, S;

        do
        {
           u = 2.0f * UnityEngine.Random.value - 1.0f;
           v = 2.0f * UnityEngine.Random.value - 1.0f;
           S = u * u + v * v;
        }
        while (S >= 1.0f);

       // Standard Normal Distribution
       float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);

       // Normal Distribution centered between the min and max value
       // and clamped following the "three-sigma rule"
       // float mean = (minValue + maxValue) / 2.0f;
       // float sigma = (maxValue - mean) / 3.0f;
       //return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
       return std;
    }
    
    Vector3 langevin_tr(Rigidbody arg1)
        {
            Vector3 argvb = arg1.linearVelocity;
            Vector3 randomVec = new Vector3(RandomGaussian(),RandomGaussian(),RandomGaussian());
	    float sigma = (float)Math.Sqrt(2.0*arg1.mass*temp*kB/(relax*dt));
	    
	    //Debug.Log(sigma);
	    randomVec = randomVec*sigma;
	    float drag = -arg1.mass/(float)relax;
	    
	    Vector3 addF = randomVec+argvb*drag;
           //Debug.Log(addF);
            return addF;
        }
    
    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 1000, 100), "temp " + temp.ToString());
    }

    void RndF()
    {
      foreach (Rigidbody GO in GOS)
      {
         Vector3 addF = langevin_tr(GO);			
         GO.AddForce(addF*dt/GO.mass,ForceMode.VelocityChange);
      }
    }
    
    void Start()
    {
        CountObjects();
    }
    void FixedUpdate ()
    {    
        RndF();
    }
}
