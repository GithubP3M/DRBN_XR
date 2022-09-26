using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.InputSystem;

public class Langevin_dial_v2 : MonoBehaviour
{

    //public Rigidbody[] GOS;
    public List<Rigidbody> GOS;
    GameObject[] GOmol;
    public List<GameObject> GOmolist;
    //public List<Rigidbody> RBS;
    static float thrust = 100;

    /*Langevin variables*/
    public static double temp = 000.0d;
    //public double temp { get; set; }
    public static double Temp  // static makes Temp a member of Langevin it can then be called in other functions with Langevin_dial_v2.Temp
    {
        get
        {
            return temp;
        }
        set
        {
            temp = value;
        }
    }
    public static double kB = 1.38f * Math.Pow(10.0f, 23.0f);
    public static double viscosity = 6.6e-3;
    public static double Ma = (13e6f * 1.7f * Math.Pow(10.0f, -1f)) / 2.0f;
    public static double friction = 6f * Math.PI * viscosity * 20f * Math.Pow(10f, -9f);
    public static double dt = Ma / friction;
    public static double sigma = Math.Sqrt(6.0f * friction * kB * temp / dt);
    public static float sigmaf = (float)sigma;
    public static float frictionf = (float)friction;
    public static float maxSpeed = 0.01f;

    (List<Rigidbody>, List<GameObject>) CountObjects()
    {
        //print("checking... ");
        GOmol = GameObject.FindGameObjectsWithTag("molecule") as GameObject[];
        GOmolist = GOmol.ToList<GameObject>();
        for (int i = 0; i <= GOmol.Length - 1; i++)
        {
            //Debug.Log (GOmol[i].GetComponents<Rigidbody>());
            GOS.AddRange(GOmol[i].GetComponents<Rigidbody>());
        }
        return (GOS, GOmolist);
    }


    Vector3 langevin_tr(Rigidbody arg1, float arg2, float arg3)
    {
        Vector3 argvb = arg1.velocity;
        Vector3 randvec = UnityEngine.Random.insideUnitSphere;

        float rx = randvec[0];
        float ry = randvec[1];
        float rz = randvec[2];
        float argfx = 2f * arg2 * rx - arg3 * argvb[0];
        float argfy = 2f * arg2 * ry - arg3 * argvb[1];
        float argfz = 2f * arg2 * rz - arg3 * argvb[2];
        Vector3 addF = new Vector3(argfx, argfy, argfz);
        return addF;
    }



    // Use this for initialization

    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 1000, 100), "temp " + temp.ToString());
        //GUI.Label(new Rect(0, 10, 1000, 100), "sigma " + sigmaf.ToString("E3"));
        GUI.Label(new Rect(0, 20, 1000, 100), "friction " + frictionf.ToString("E3"));
    }

    void RndF()
    {
        //Debug.Log(GOS.Length+" HAHA");
        //print(GOS[0].name);
        //print(GOS[1].name);
        //print(GOS[GOS.Length-1].name);

        foreach (Rigidbody GO in GOS)
        {
            //if (GO.transform.root.name != "char_shadow")
            {


                //GO.AddForce(UnityEngine.Random.insideUnitSphere * thrust);
                /*Your Langevin code here*/
                Vector3 addF = langevin_tr(GO, sigmaf, frictionf);
                //Debug.Log ("addF "+addF);
                //Debug.DrawLine(GO.transform.position,GO.transform.position+addF,Color.blue);
                //Debug.DrawLine(GO.transform.position,GO.transform.position+GO.GetComponent<Rigidbody>().velocity,Color.blue);

                //GO.AddForce(addF*0.01f);

                GO.AddForce(addF);

                if (GO.velocity.magnitude > maxSpeed)
                {
                    Debug.LogWarning("adjusting speed " + GO.velocity);
                    GO.velocity = GO.velocity.normalized * maxSpeed;
                    Debug.LogWarning("adjusted speed " + GO.velocity);
                }
                //Debug.Log(addF+GO.name);

                //Debug.Log(GO.transform.root + " parent");
                //Debug.Log(GO.velocity);
            }
        }
    }


    // Use this for initialization
    void Awake()
    {
        CountObjects();
    }

    void Start()
    {
        ////in a pinch, this works now
        //GOS = GameObject.FindGameObjectsWithTag("molecule") as GameObject[];

        //for (int i=0; i<=GOS.Length; i++)
        //{
        //    //Debug.Log("int i " + i + " GOS.Length " + GOS.Length);
        //    //Debug.Log("GOS length " + GOS[i].name );
        //    Rigidbody[] RB = (GOS[i].GetComponentsInChildren<Rigidbody>());
        //    //Debug.Log("RB Count " + RB.Count());
        //    List<Rigidbody> RBList = RB.ToList<Rigidbody>();
        //    RBS.AddRange(RBList);
        //    Debug.Log("RBS Count " + RBS.Count);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        //CountObjects();
        //Debug.Log("oy!");
        RndF();
    }
}
