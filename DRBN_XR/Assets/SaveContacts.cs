﻿using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

public class SaveContacts : MonoBehaviour
{
    int nextActionTime = 0;
    int period = 10;
    
    public List<Vector2> contacts = new List<Vector2>();

    public int[,] MatSize;
    int[,] SaveMat;
    Langevin_v2 Lange = new Langevin_v2();

    // Update is called once per frame
    void Start()
    {
        
    }

    void Update()
    {
        
        
        //if(Time.time > nextActionTime)
        //{
        //    nextActionTime += period;
        //    //periodicsaveTXT(nextActionTime);
        //    contacts.Clear();
        //    Debug.Log("CLEARED");
        //}
        //if (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.s)) // shift + S combo
        if (Input.GetKeyDown("s")) 
        {
            Lange = GameObject.Find("Simulation").GetComponent<Langevin_v2>();

            Debug.Log("Initiating matrix");
            MatSize = new int[Lange.GOS.Count, Lange.GOS.Count];
            Debug.Log("MatSize size " + MatSize.Length);
        }

        if (Input.GetKeyDown("d"))
        {
            Debug.Log("Saving matrix");
            SaveMat = MatSize;

            using (StreamWriter outfile = new StreamWriter(@"C:\Temp\test.csv"))
            {
                for (int x = 0; x < SaveMat.GetLength(0); x++)
                {
                    string content = "";
                    for (int y = 0; y < SaveMat.GetLength(1); y++)
                    {
                        //content += SaveMat[x, y].ToString("0.00") + ";";
                        content += SaveMat[x, y].ToString("0") + ";";
                    }
                    outfile.WriteLine(content);
                }
            }
        }
    }



    //public void modList(Vector2 contact)
    //{
    //    contacts.Add(contact);
    //}

    //public void periodicsaveTXT(int step)
    //{
    //    System.IO.File.AppendAllText("D:/trajectory_data/matrix_" + step.ToString() + ".txt", contacts.);

    //    //string json = JsonUtility.ToJson(contacts);// Been babmboozled, it appears json implementation in unity doesn't allow that

    //    //Debug.Log("writing THAT much " + contacts.Count);

    //    //File.WriteAllText("D:/trajectory_data/matrix_" + step.ToString() + ".jsonbrn", json);
    //}

    public void modMatrix(int i, int j)
    {
        //Debug.Log("i " + i + " j " + j + " MatSize size " + MatSize.Length);
        if (i >= 0)
        {
            if (j >= 0)
            {
                MatSize[i, j] += 1;
            }
        }
        //Debug.Log("MatSize has been modified");
    }
}
