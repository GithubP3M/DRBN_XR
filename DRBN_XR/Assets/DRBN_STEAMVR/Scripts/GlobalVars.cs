using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;




public class GlobalVars : MonoBehaviour
{       
         public static string pathJointsFile;
         public static string pathMassesFile;
         public static string pathHydrophobicityFile;
         public static string pathTrajectoryFolder;
         public static float dt = 0.001f;
         public static int i = 0;
        
	void Awake ()
	{
	  //var sr = new StreamReader("/home/joel/Documents/KovReplica/files.config");

      var sr = new StreamReader(Path.Combine(Application.streamingAssetsPath, "files.config"));
      //var sr = new StreamReader(Path.Combine(Application.persistentDataPath, "files.config"));
      Debug.Log((Path.Combine(Application.streamingAssetsPath, "files.config"))+"aaaaaaaaaaaaaaaargh");

      var fileContents = sr.ReadToEnd();
      sr.Close();
      var lines = fileContents.Split("\n");
      Time.fixedDeltaTime = 0.001f;

	  foreach(var line in lines)
          {
            if (line.Split("=")[0] =="jointsFile") 
            {
               pathJointsFile = line.Split("=")[1];
            }
            else if (line.Split("=")[0] =="massesFile") 
            {
               pathMassesFile = line.Split("=")[1];
            }
            else if (line.Split("=")[0] =="trajectoryFolder") 
            {
               pathTrajectoryFolder = line.Split("=")[1];
            }
            else if (line.Split("=")[0] =="hydrophobicityFile") 
            {
               pathHydrophobicityFile = line.Split("=")[1];
            }
	  }	
        }
        void FixedUpdate ()
    {    
        i++;
    }
}
