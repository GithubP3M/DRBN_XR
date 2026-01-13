using UnityEngine;
//using System.Collections;
using System;
using System.Linq;

/// <summary>
/// Implements a Langevin thermostat for molecular dynamics simulation.
/// Applies stochastic and friction forces to all Rigidbody objects in the scene.
/// </summary>
/// <remarks>
/// Keyboard controls:
/// - O: Unfreeze all objects (disable isKinematic)
/// - P: Freeze all objects (enable isKinematic)
/// - I: Toggle between normal and 30x time scale
/// - [+]: Increase temperature by 100K
/// - [-]: Decrease temperature by 100K
/// </remarks>
public class Langevin : MonoBehaviour {

    /// <summary>Array of all Rigidbody objects in the scene.</summary>
    public Rigidbody[] GOS;
    
    /// <summary>Thrust magnitude (unused legacy parameter).</summary>
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
    
    /// <summary>Simulation temperature in Kelvin.</summary>
    public static double temp = 0.0f;
    
    /// <summary>Boltzmann constant in simulation units.</summary>
    public static double kB = 1.38f * Math.Pow(10.0f, 23.0f);
    
    /// <summary>Viscosity of the medium.</summary>
    public static double viscosity = 6.6e-3;
    
    /// <summary>Effective mass parameter.</summary>
    public static double Ma = (13e6f * 1.7f * Math.Pow(10.0f, -1f)) / 2.0f;
    
    /// <summary>Friction coefficient derived from Stokes drag.</summary>
    public static double friction = 6f * Math.PI * viscosity * 20f * Math.Pow(10f, -9f);
    
    /// <summary>Characteristic timescale (mass/friction).</summary>
    public static double dt = Ma / friction;
    
    /// <summary>Standard deviation of stochastic force from fluctuation-dissipation theorem.</summary>
    public static double sigma = Math.Sqrt(6.0f * friction * kB * temp / dt);
    
    /// <summary>Float cast of sigma for use with Unity Vector3.</summary>
    public static float sigmaf = (float)sigma;
    
    /// <summary>Float cast of friction for use with Unity Vector3.</summary>
    public static float frictionf = (float)friction;

    /// <summary>
    /// Finds and stores all Rigidbody objects in the scene.
    /// </summary>
    /// <returns>Array of all Rigidbody components found.</returns>
    UnityEngine.Rigidbody[] CountObjects()
    {
        //print("checking... ");
        GOS = FindObjectsOfType(typeof(Rigidbody)) as Rigidbody[];
        //Rigidbody[] GOS = FindObjectsOfType(typeof(Rigidbody)) as Rigidbody[];
        /*
        foreach (Rigidbody GO in GOS)
        {
            StartCoroutine(CheckMoving(GO));
            print(GO.name + " " + GO.velocity.magnitude);
            Debug.Log(GO.name);
        }
        */
        return GOS;
    }

    
    /// <summary>
    /// Calculates the Langevin force for a single Rigidbody.
    /// Combines stochastic thermal noise with velocity-dependent friction.
    /// </summary>
    /// <param name="arg1">The Rigidbody to calculate forces for.</param>
    /// <param name="arg2">Sigma - standard deviation of stochastic force.</param>
    /// <param name="arg3">Friction coefficient.</param>
    /// <returns>The combined Langevin force vector to apply.</returns>
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


    /*
    #Arg1: corps, arg2: sigma, arg3: friction
    def langevin_tr(arg1,arg2,arg3):
	argvb=arg1.getLinearVel()
#	v = np.random.multivariate_normal([0,0,0],[[1,0,0],[0,1,0],[0,0,1]])
#	v /= np.sqrt(v.dot(v))
	"""argfx=gauss(0.,arg2)-arg3*argvb[0]
	argfy=gauss(0.,arg2)-arg3*argvb[1]
	argfz=gauss(0.,arg2)-arg3*argvb[2]"""
#	argfx=arg2*v[0]-arg3*argvb[0]
#	argfy=arg2*v[1]-arg3*argvb[1]
#	argfz=arg2*v[2]-arg3*argvb[2]
	
	argfx=2.*arg2*(.5-random())-arg3*argvb[0]
	argfy=2.*arg2*(.5-random())-arg3*argvb[1]
	argfz=2.*arg2*(.5-random())-arg3*argvb[2]
	arg1.addForce((argfx,argfy,argfz))
    */

    /// <summary>
    /// Displays temperature, sigma, and friction values on screen.
    /// </summary>
    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 1000, 100), "temp " + temp.ToString());
        GUI.Label(new Rect(0, 10, 1000, 100), "sigma " + sigmaf.ToString("E3"));
        GUI.Label(new Rect(0, 20, 1000, 100), "friction " + frictionf.ToString("E3"));
    }

    /// <summary>
    /// Applies Langevin forces to all Rigidbodies except those under "char_shadow".
    /// Called every frame to maintain thermal equilibrium.
    /// </summary>
    void RndF()
    {
        //Debug.Log(GOS.Length+" HAHA");
        //print(GOS[0].name);
        //print(GOS[1].name);
        //print(GOS[GOS.Length-1].name);

        foreach (Rigidbody GO in GOS)
        {
            if (GO.transform.root.name != "char_shadow")
            {
                //GO.AddForce(UnityEngine.Random.insideUnitSphere * thrust);
                /*Your Langevin code here*/
                Vector3 addF = langevin_tr(GO,sigmaf,frictionf);
                GO.AddForce(addF);
                //Debug.Log(addF+GO.name);

                //Debug.Log(GO.transform.root + " parent");
                //Debug.Log(GO.velocity);
            }
        }
    }


    /// <summary>
    /// Initializes the simulation by finding all Rigidbodies.
    /// </summary>
    void Awake()
    {
        CountObjects();
    }

    /// <summary>
    /// Called on the frame when the script is enabled (currently unused).
    /// </summary>
	void Start ()
    {
        
	}
	
    /// <summary>
    /// Called every frame. Handles keyboard input for simulation control
    /// and applies Langevin forces to all objects.
    /// </summary>
	void Update ()
    {
        //if (Input.GetMouseButtonDown(1))
        //{
            CountObjects();
        //}
        
        if (Input.GetKeyDown("o"))
        {
            bool isK = GOS.Any(p => p.isKinematic == true);
            Debug.Log(isK + " popo touche O");
            //    bool isK = GOS.Any(p => p.isKinematic == true);
            if (isK == true)
            {
                foreach (Rigidbody GO in GOS)
                {
                    GO.isKinematic = false;
                }
            }
        }

        if (Input.GetKeyDown("p"))
        {
            //freeze using kinematic
            bool isK = GOS.Any(p => p.isKinematic == true);
            Debug.Log(isK + " popo touche P");
                //if (GO.isKinematic)
                if (isK == false)
                {
                    foreach (Rigidbody GO in GOS)
                    {
                        GO.isKinematic = true;
                    }
                }
                //else if (!GO.isKinematic)

            //freeze using timescale
            /*
            if (Time.timeScale == 0)
            {
                Time.timeScale = 1;
            }
            else if (Time.timeScale == 1)
            {
                Time.timeScale = 0;
            }
            */
        }

        if (Input.GetKeyDown("i"))
        {
            if (Time.timeScale == 1.0f)
            {
                Time.timeScale = 30.0f;
                Time.fixedDeltaTime = 1 / Time.timeScale;
                Debug.Log(Time.timeScale);
            }
            else if (Time.timeScale > 1.0f)
            {
                Time.timeScale = 1.0f;
                Time.fixedDeltaTime = 0.02F * Time.timeScale;
                Debug.Log(Time.timeScale);
            }
        }

        if (Input.GetKeyDown("[+]"))
        {
            if (temp >= 0.0f && temp < 10000)
            {
                temp = temp+100;
                sigma = Math.Sqrt(6.0f * friction * kB * temp / dt);
                sigmaf = (float)sigma;
                //Debug.Log("popo touche + " + temp);
            }
        }

        if (Input.GetKeyDown("[-]"))
        {
            if (temp > 0.0f && temp <= 10000)
            {
                if (temp - 100 < 0)
                {
                    temp = 0;
                }
                else
                {
                    temp = temp - 100;
                }
                sigma = Math.Sqrt(6.0f * friction * kB * temp / dt);
                sigmaf = (float)sigma;
                //Debug.Log("popo touche - " + temp);
            }
        }


        RndF();
    }
}
