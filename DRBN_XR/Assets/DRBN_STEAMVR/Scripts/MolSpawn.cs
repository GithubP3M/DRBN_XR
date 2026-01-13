using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles spawning of molecule prefabs into the simulation.
/// Instantiates molecules at a designated spawn point and registers them with the physics system.
/// </summary>
/// <remarks>
/// Requires a Langevin_v2 component on a GameObject tagged "Physics_Sim".
/// Spawning is limited by molcounter.limit.
/// </remarks>
public class MolSpawn : MonoBehaviour {
	
	/// <summary>Minimum scale for spawn animation (unused).</summary>
	Vector3 minScale;
	
	/// <summary>Maximum scale for spawn animation.</summary>
	public Vector3 maxScale;
	
	/// <summary>Speed of scale animation.</summary>
	public float speed = 2f;
	
	/// <summary>Duration of scale animation in seconds.</summary>
	public float duration = 5f;

	/// <summary>The molecule prefab to instantiate.</summary>
	public Transform prefab;
	
	/// <summary>Reference to the most recently spawned molecule.</summary>
	private Transform spawn;
	
	/// <summary>The GameObject defining the spawn location.</summary>
    public GameObject spawnpoint;


	//Rigidbody[] children;

	/// <summary>
	/// Spawns a new molecule instance at the spawn point.
	/// Registers the molecule's Rigidbodies with the Langevin physics system
	/// and adds it to the molecule counter.
	/// </summary>
	/// <remarks>
	/// Only spawns if current molecule count is below molcounter.limit.
	/// Called from UI buttons.
	/// </remarks>
	public void SpawnMolecule()
	{
        int counter;
        counter = molcounter.molecules.Count;
        List<Transform> MolCount;
        MolCount = molcounter.molecules;

        if (counter < molcounter.limit)
        {
            //Vector3 loc = gameObject.transform.position;
            Vector3 loc = spawnpoint.transform.position;
            Quaternion rot = gameObject.transform.rotation;
            spawn = Instantiate(prefab, loc, rot);
            spawn.name = prefab.name+"_"+counter.ToString();
            Debug.Log("plop");

            // recover Langevin GOS gameobject list and append the spawned gameobjects
            Langevin_v2 Lange = GameObject.FindGameObjectWithTag("Physics_Sim").GetComponent<Langevin_v2>();

            // add line for mol counter
            //List<Transform> MolCount = GameObject.Find("Simulation").GetComponent<molcounter>().molecules;

            Debug.Log("Before " + Lange.GOS.Count);
            //Lange.GOS.Add(
            Debug.Log(spawn.transform.name);
            Rigidbody[] GOarray = spawn.gameObject.GetComponentsInChildren<Rigidbody>();
            Lange.GOS.AddRange(GOarray);
            MolCount.Add(spawn);
            Debug.Log("After  " + Lange.GOS.Count);
            Debug.Log("counter " + counter);
        }
	}




	// uncomment below if using the function for non UI purpose (testing debugs etc)
//	void Update () {
//		Vector3 loc = gameObject.transform.position;
//		Quaternion rot = gameObject.transform.rotation;
//
//		if (Input.GetKeyDown ("space")) {
//			spawn = Instantiate (prefab, loc, rot);
//			Vector3 currScale = spawn.transform.localScale;
//
//			children = spawn.GetComponentsInChildren<Rigidbody>();
//		}
//	}
}
