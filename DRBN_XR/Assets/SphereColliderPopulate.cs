using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereColliderPopulate : MonoBehaviour
{
    public Transform Prefab;
    public GameObject PopulateGO;
    private Vector3[] VertList;
    private Vector3[] NormList;
    private SphereCollider[] Population;
    private GameObject[] Debugsphere;
    public float sphradius = 0.03f;

    

    // Start is called before the first frame update
    void Start()
    {
        VertList = ExtractVert(PopulateGO);
        NormList = ExtractNorm(PopulateGO);
        //Debug.Log(VertList.Length + " Length");

        Populate(VertList, NormList);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Vector3[] ExtractVert(GameObject PopulateGO) 
    {
        Mesh PopulateMesh = PopulateGO.GetComponent<MeshFilter>().mesh;
        Debug.Log("PopulateMesh " + PopulateMesh);
        Debug.Log("PopulateGO " + PopulateGO);
        VertList = PopulateMesh.vertices;
        Debug.Log(VertList.Length + " Length");

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
        for (int i =0; i<VertList.Length; i++)
        {
            GameObject ColliderOrientation = new GameObject("impala collider");
            ColliderOrientation.transform.parent=PopulateGO.transform;
            SphereCollider Sphere = ColliderOrientation.AddComponent<SphereCollider>();
            ColliderOrientation.layer = LayerMask.NameToLayer("Impala");
            //Sphere.radius = 0.03f;
            Sphere.radius = sphradius/2;

            GameObject TriggerOrientation = new GameObject("impala trigger");
            TriggerOrientation.transform.parent = PopulateGO.transform;

            SphereCollider Sphere_Trig = TriggerOrientation.AddComponent<SphereCollider>();
            //Sphere_Trig.radius = 0.03f;
            Sphere_Trig.radius = sphradius/2;
            Sphere_Trig.isTrigger = true;
            

            ColliderOrientation.transform.position = VertList[i];
            ColliderOrientation.transform.localRotation = Quaternion.LookRotation(NormList[i]);

            TriggerOrientation.transform.position = VertList[i];
            TriggerOrientation.transform.localRotation = Quaternion.LookRotation(NormList[i]);

            TriggerOrientation.AddComponent<ImpalaGeneralized>();

            //GameObject DSphere = new GameObject();

            GameObject DSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            

            DSphere.gameObject.name = "Impala Renderer"; //change to something lighter in polygons

            //Transform DSphere = Instantiate(Prefab); // lighter in polygons, maybe change to particles in the future

            //Losphere = DSphere.gameObject.GetComponent<MeshFilter>();
            //Losphere.sharedMesh = Resources.Load<Mesh>("Impalasphere");

            Collider DSphereCollider = DSphere.GetComponent<Collider>();
            Destroy(DSphereCollider);
            DSphere.transform.parent = PopulateGO.transform;
            DSphere.transform.position = VertList[i];
            ///DSphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            DSphere.transform.localScale = new Vector3(sphradius, sphradius, sphradius);
            DSphere.transform.localRotation = Quaternion.LookRotation(NormList[i],Vector3.up);

            //Debug.Log(PopulateGO.GetComponent<MeshRenderer>().materials[0].name);
            Shader Green = Shader.Find("resources/Transparent Green");
            GetComponent<MeshRenderer>().material.shader = Green;

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
