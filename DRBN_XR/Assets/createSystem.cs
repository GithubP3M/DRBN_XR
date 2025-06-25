using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.InputSystem;

public class createSystem : MonoBehaviour
{
    private GameObject s;
    private int step = 0;
   // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        for (int i = 0; i<1000; i++)
	{
	  s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
	  s.AddComponent<reflexiveForce>();
          s.transform.position = new Vector3 (UnityEngine.Random.value*100-100, UnityEngine.Random.value*100-150, UnityEngine.Random.value*40-0);
          s.transform.localScale = new Vector3(1, 1, 1);
          s.tag = "solvent";
          Rigidbody rigidbody = s.AddComponent<Rigidbody>();
          Collider collider = s.GetComponent<Collider>();
          collider.isTrigger = true;
          rigidbody.useGravity = false;
          rigidbody.mass = 10f;
          rigidbody.linearVelocity = new Vector3 (UnityEngine.Random.value*50-250,UnityEngine.Random.value*50-150,UnityEngine.Random.value*50-25);
          //rigidbody.linearVelocity = new Vector3 (0,0,0);
    	}
    }

 //   void FixedUpdate()
 //   {
 //       Vector3 sum = new Vector3(0,0,0);
 //       float velocity = 0;
 //       GameObject[] spheres = GameObject.FindGameObjectsWithTag("molecule");
 //       foreach(var sphere in spheres)
 //       {
 //          Rigidbody rb = sphere.GetComponent<Rigidbody>();
 //          sum += rb.linearVelocity;
 //       }
 //       Vector3 mean = sum/spheres.Length;
        
 //       foreach(var sphere in spheres)
 //       {
 //          Rigidbody rb = sphere.GetComponent<Rigidbody>();
 //          Vector3 difference = rb.linearVelocity-mean;
 //          velocity += rb.mass*difference.sqrMagnitude;
 //       }
 //       if(step%20==0)
 //       {
 //        Debug.Log(velocity/(3*spheres.Length*Langevin_v3.kB));
 //       }
 //       step+=1;
 //   }}
 }
