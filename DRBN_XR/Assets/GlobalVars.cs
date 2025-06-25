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
         public static string pathTrajectoryFolder;
        
	void Awake ()
	{
	  var sr = new StreamReader("/home/joel/Documents/integrinalpha/files.config");
          var fileContents = sr.ReadToEnd();
          sr.Close();
          var lines = fileContents.Split("\n");

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
	  }	
        }

}
