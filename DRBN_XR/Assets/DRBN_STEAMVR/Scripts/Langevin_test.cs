using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.InputSystem;

public class Langevin_test : MonoBehaviour {

    public List<Rigidbody> GOS;
    public List<GameObject> GOmol;
    
    /*Langevin variables*/
    public static float temp = 300.0f;
    public static double kB = 1.38 * Math.Pow(10.0, -23.0);
    public static double relax = 1e-11; //Pa.s-1 =6.6cPoise
    public static double dt = 1e-15;
    public static float maxSpeed = 0.01f;
    public static double Avogadro =6.022f*Math.Pow(10.0, 23.0);

    //make a list of objects that are tagged with the "molecule" tag
    (List<Rigidbody>,List<GameObject>) CountObjects()
    {
	GOmol = GameObject.FindGameObjectsWithTag("molecule").ToList<GameObject>();
        foreach (GameObject go in GOmol)
            {GOS.AddRange(go.GetComponents<Rigidbody>());}
        
  
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
       float mean = (minValue + maxValue) / 2.0f;
       float sigma = (maxValue - mean) / 3.0f;
       return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
    }
    
    Vector3 langevin_tr(Rigidbody arg1)
        {
            Vector3 argvb = arg1.linearVelocity;
            Vector3 randomVec = new Vector3(RandomGaussian(),RandomGaussian(),RandomGaussian());
            double mass = arg1.mass/Avogadro;
	    double sigma = Math.Sqrt(2.0*mass*temp*kB/(relax*dt))*Math.Pow(10.0, 9.0); //e-8
	    randomVec = randomVec*(float)sigma;
	    double drag = -mass*Math.Pow(10.0, 9.0)/relax; //e-11
	    Vector3 addF = randomVec+argvb*(float)drag;//
	    
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
                /*Your Langevin code here*/
                Vector3 addF = langevin_tr(GO);	
                GO.AddForce(addF);
            }
        }
    


    // Use this for initialization
    void Start()
    {
        CountObjects();
    }

	
	// Update is called once per frame
	void FixedUpdate ()
    {    

        RndF();
    }}
