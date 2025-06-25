using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;



public class SaveSnapShotNoPhysics : MonoBehaviour
{
    int nextActionTime = 0;
    int period = 3;


    void Update()
    {
        if (Time.time >= nextActionTime)
        {
            periodicsaveJSON((int)Time.time);
            nextActionTime += period;
        }
    }

    public void periodicsaveJSON(int step)
    {
        List<SavedataHierarchy> savefile = new List<SavedataHierarchy>();

        List<Rigidbody> RBlist = new List<Rigidbody>();

        List<Coords> GOVertscoords = new List<Coords>();
        
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag ("molecule");
        foreach(var molecule in gameObjects)
        {
            Rigidbody[] rigidBodies = molecule.GetComponentsInChildren<Rigidbody>();
        
            foreach(var rigidBody in rigidBodies)
            {
                savefile.Add(new SavedataHierarchy(rigidBody.transform.parent.name,rigidBody.name, rigidBody.position, rigidBody.rotation));
            }
        }

        
        RBListContainerHierarchy container = new RBListContainerHierarchy(savefile);
        string json = JsonUtility.ToJson(container, true);

        Debug.Log("json " + json);
        Debug.Log("step " + step);

        
        File.WriteAllText(GlobalVars.pathTrajectoryFolder+"/gamesave_list_"+ step.ToString() + ".jsonbrn", json);

    }
}
