﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereColliderPopulate : MonoBehaviour
{
    public GameObject PopulateGO;
    private Vector3[] VertList;
    private Vector3[] NormList;
    private SphereCollider[] Population;
    private GameObject[] Debugsphere;
    
    // Start is called before the first frame update
    void Start()
    {
        VertList = ExtractVert(PopulateGO);
        NormList = ExtractNorm(PopulateGO);
        Debug.Log(VertList.Length + " Length");

        Populate(VertList, NormList);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Vector3[] ExtractVert(GameObject PopulateGO) 
    {
        Mesh PopulateMesh = PopulateGO.GetComponent<MeshFilter>().mesh;
        VertList = PopulateMesh.vertices;

        Vector3[] vworld = new Vector3[VertList.Length];
        for (int i = 0; i<VertList.Length; i++)
        {
            vworld[i] = transform.TransformPoint(VertList[i]);
        }
        

        return vworld;
    }

    Vector3[] ExtractNorm(GameObject PopulateGO)
    {
        Mesh PopulateMesh = PopulateGO.GetComponent<MeshFilter>().mesh;
        NormList = PopulateMesh.normals;

        return NormList;
    }

    void Populate(Vector3[] VertList, Vector3[] NormList) 
    {
        for (int i =0; i<=VertList.Length; i++)
        {
            GameObject ColliderOrientation = new GameObject();
            ColliderOrientation.transform.parent=PopulateGO.transform;
            SphereCollider Sphere = ColliderOrientation.AddComponent<SphereCollider>();
            Sphere.radius = 0.05f;
            SphereCollider Sphere_Trig = ColliderOrientation.AddComponent<SphereCollider>();
            Sphere_Trig.radius = 0.1f;
            Sphere_Trig.isTrigger = true;

            ColliderOrientation.transform.position = VertList[i];
            ColliderOrientation.transform.localRotation = Quaternion.LookRotation(NormList[i]);

            ColliderOrientation.AddComponent<ImpalaGeneralized>();

            //GameObject DSphere = new GameObject();
            GameObject DSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            DSphere.transform.position = VertList[i];
            DSphere.transform.localScale = new Vector3 (0.1f, 0.1f, 0.1f) ;
            //Debugsphere[i] = DSphere;

            //SphereCollider Sphere = PopulateGO.AddComponent<SphereCollider>();
            //Sphere.center = v;
            //Sphere.radius = 0.05f;

            //SphereCollider Sphere_Trig = PopulateGO.AddComponent<SphereCollider>();
            //Sphere_Trig.center = v;
            //Sphere_Trig.radius = 0.1f;
            //Sphere_Trig.isTrigger = true;
        }
    }
}