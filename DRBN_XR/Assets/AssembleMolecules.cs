using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

public class AssembleMolecules : MonoBehaviour
{       
        
        int fixedUpdateIdx = 0;
	void Awake ()
	{
	 
	  var srmasses = new StreamReader(GlobalVars.pathMassesFile);
          var fileContentsMasses = srmasses.ReadToEnd();
          srmasses.Close();
          var linesMasses = fileContentsMasses.Split("\n");
          var srjoints = new StreamReader(GlobalVars.pathJointsFile);
          var fileContentsJoints = srjoints.ReadToEnd();
          srjoints.Close();
          var linesJoints = fileContentsJoints.Split("\n");
          
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag ("molecule");
        foreach(var molecule in gameObjects)
        {
           
           foreach (Transform childTransform in molecule.transform)
	   {
   		Rigidbody rb = childTransform.gameObject.AddComponent<Rigidbody>();
   		childTransform.gameObject.AddComponent<CheckCollision>();
   		rb.useGravity=false;
   		foreach(var line in linesMasses)
          	{
          	    if (line.Split(",")[0] ==childTransform.gameObject.name) 
          	    {
          	       rb.mass = float.Parse(line.Split(",")[1]);
          	    }
          	}
          	foreach (Transform colliderTransform in childTransform.GetChild(0))
          	{
          	colliderTransform.gameObject.GetComponent<MeshRenderer>().enabled=false;
          	 MeshCollider collider = colliderTransform.gameObject.AddComponent<MeshCollider>();
          	 collider.convex = true;
   		 collider.isTrigger =false;
   		 //collider.hasModifiableContacts = true;
          	 }
   		
   		
   		
	   }	
        foreach (Transform childTransform in molecule.transform)
	   {
	      foreach(var line in linesJoints)
          	{
          	    if (line.Split(",")[0] ==childTransform.gameObject.name) 
          	    {  
          	       Transform connectedDomain = molecule.transform.Find(line.Split(",")[1]);
          	       ConfigurableJoint joint = childTransform.gameObject.AddComponent<ConfigurableJoint>();
         	       joint.connectedBody = connectedDomain.gameObject.GetComponent<Rigidbody>();
          	       joint.xMotion = ConfigurableJointMotion.Locked;
   		       joint.yMotion = ConfigurableJointMotion.Locked;
   		       joint.zMotion = ConfigurableJointMotion.Locked;
   		       joint.anchor = childTransform.gameObject.transform.GetChild(1).transform.localPosition;
   		       joint.enableCollision=true;
          	    }
         	}
	   }
	   
	   if (molecule.name == "chainalpha")
	   {
	   foreach (Transform childTransform in molecule.transform)
	   {
	     childTransform.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0,0,0),ForceMode.Acceleration);
	   }
	   }
	}
	}
	
	
}
