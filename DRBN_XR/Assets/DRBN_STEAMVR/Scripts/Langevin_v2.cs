using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.InputSystem;

public class Langevin_v2 : MonoBehaviour {

    public List<Rigidbody> GOS;
	GameObject[] GOmol;
    public List<GameObject> GOmolist;
    static float thrust = 100;

    /*Langevin variables*/
    /*
    public static double temp = 310.0f;
    public static double kB = 1.38f * Math.Pow(10.0f, -23.0f);
    public static double viscosity = 6.6e-3;
    public static double Ma = (13e6 * 1.7 * Math.Pow(10.0, -27)) / 2.0;
    public static double friction = 6 * Math.PI* viscosity * 20 * Math.Pow(10, -9);
    public static double dt = Ma / friction;
    public static double sigma = Math.Sqrt(6.0f * friction * kB * temp / dt);
    public static float sigmaf = (float)sigma;
    public static float frictionf = (float)friction;
    */
    public static double temp = 300.0f;
    public static double kB = 1.38f * Math.Pow(10.0f, 23.0f);
	public static double viscosity = 6.6e-3; //Pa.s-1 =6.6cPoise
    public static double Ma = (13e6f * 1.7f * Math.Pow(10.0f, -1f)) / 2.0f;
    public static double friction = 6f * Math.PI * viscosity * 20f * Math.Pow(10f, -9f);
    public static double dt = Ma / friction;
    public static double sigma = Math.Sqrt(6.0f * friction * kB * temp / dt);
    public static float sigmaf = (float)sigma;
    public static float frictionf = (float)friction;
    public static float maxSpeed = 0.01f;

    //make a list of objects that are tagged with the "molecule" tag
    (List<Rigidbody>,List<GameObject>) CountObjects()
    {
        //print("checking... ");
        GOmol = GameObject.FindGameObjectsWithTag("molecule") as GameObject[];
        GOmolist = GOmol.ToList<GameObject>();
        for (int i = 0; i <= GOmol.Length - 1; i++)
        {
            //Debug.Log (GOmol[i].GetComponents<Rigidbody>());
            GOS.AddRange(GOmol[i].GetComponents<Rigidbody>());
        }
        return (GOS,GOmolist);
    }

    
    Vector3 langevin_tr(Rigidbody arg1,float arg2,float arg3)
        {
            Vector3 argvb = arg1.linearVelocity;
            Vector3 randvec = UnityEngine.Random.insideUnitSphere;

            float rx = randvec[0];
            float ry = randvec[1];
            float rz = randvec[2];
            float argfx = 2f * arg2 * rx - arg3 * argvb[0];
            float argfy = 2f * arg2 * ry - arg3 * argvb[1];
            float argfz = 2f * arg2 * rz - arg3 * argvb[2];
            Vector3 addF = new Vector3(argfx, argfy, argfz);
            //Debug.Log(rx+" rx");
            //Debug.Log(ry+" ry");
            //Debug.Log(rz+" rz");
            //Debug.Log(randvec);
            return addF;
        }
    
    void OnGUI()
    {
        //GUI.Label(new Rect(0, 0, 1000, 100), "temp " + temp.ToString());
        GUI.Label(new Rect(0, 10, 1000, 100), "sigma " + sigmaf.ToString("E3"));
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
                Vector3 addF = langevin_tr(GO,sigmaf,frictionf);
				//Debug.Log ("addF "+addF);
				//Debug.DrawLine(GO.transform.position,GO.transform.position+addF,Color.blue);
				//Debug.DrawLine(GO.transform.position,GO.transform.position+GO.GetComponent<Rigidbody>().velocity,Color.blue);

                //GO.AddForce(addF*0.01f);

                GO.AddForce(addF);

                if (GO.linearVelocity.magnitude > maxSpeed)
                {
                    Debug.LogWarning("adjusting speed " + GO.linearVelocity);
                    GO.linearVelocity = GO.linearVelocity.normalized * maxSpeed;
                    Debug.LogWarning("adjusted speed " + GO.linearVelocity);
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

	void Start ()
    {
        
	}
	
	// Update is called once per frame
	void Update ()
    {
        //if (Input.GetMouseButtonDown(1))
        //{
        //CountObjects();
        //}

// new input system UnityEngine.InputSystems
        if(Keyboard.current.uKey.wasPressedThisFrame)
        {
            Debug.Log("doing something");
            CountObjects();
        }


// obsolete?
        //if (Input.GetKeyDown("u")) //quickfix
        //{
        //    Debug.Log("doing something");
        //    CountObjects();
        //}

        //if (Input.GetKeyDown("o"))
        //{
        //    bool isK = GOS.Any(p => p.isKinematic == true);
        //    Debug.Log(isK + " popo touche O");
        //    //    bool isK = GOS.Any(p => p.isKinematic == true);
        //    if (isK == true)
        //    {
        //        foreach (Rigidbody GO in GOS)
        //        {
        //            GO.isKinematic = false;
        //        }
        //    }
        //}

        //if (Input.GetKeyDown("p"))
        //{
        //    //freeze using kinematic
        //    bool isK = GOS.Any(p => p.isKinematic == true);
        //    Debug.Log(isK + " popo touche P");
        //        //if (GO.isKinematic)
        //        if (isK == false)
        //        {
        //            foreach (Rigidbody GO in GOS)
        //            {
        //                GO.isKinematic = true;
        //            }
        //        }
        //        //else if (!GO.isKinematic)

        //    //freeze using timescale
        //    /*
        //    if (Time.timeScale == 0)
        //    {
        //        Time.timeScale = 1;
        //    }
        //    else if (Time.timeScale == 1)
        //    {
        //        Time.timeScale = 0;
        //    }
        //    */
        //}

        //if (Input.GetKeyDown("i"))
        //{
        //    if (Time.timeScale == 1.0f)
        //    {
        //        Time.timeScale = 5.0f;
        //        Time.fixedDeltaTime = 1 / Time.timeScale;
        //        Debug.Log(Time.timeScale);
        //    }
        //    else if (Time.timeScale > 1.0f)
        //    {
        //        Time.timeScale = 1.0f;
        //        Time.fixedDeltaTime = 0.02F * Time.timeScale;
        //        Debug.Log(Time.timeScale);
        //    }
        //}

        //if (Input.GetKeyDown("[+]"))
        //{
        //    if (temp >= 0.0f && temp < 10000)
        //    {
        //        temp = temp+100;
        //        sigma = Math.Sqrt(6.0f * friction * kB * temp / dt);
        //        sigmaf = (float)sigma;
        //        //Debug.Log("popo touche + " + temp);
        //    }
        //}

        //if (Input.GetKeyDown("[-]"))
        //{
        //    if (temp > 0.0f && temp <= 10000)
        //    {
        //        if (temp - 100 < 0)
        //        {
        //            temp = 0;
        //        }
        //        else
        //        {
        //            temp = temp - 100;
        //        }
        //        sigma = Math.Sqrt(6.0f * friction * kB * temp / dt);
        //        sigmaf = (float)sigma;
        //        //Debug.Log("popo touche - " + temp);
        //    }
        //}


        RndF();
    }
}
