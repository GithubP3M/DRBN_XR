/// @file MolSpawn_dial.cs
/// @brief Handles molecule spawning via UI dial controls.
/// @details Provides functionality to instantiate molecule prefabs at designated
///          spawn points, respecting molecule count limits and integrating with
///          the Langevin physics simulation system.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages molecule spawning through UI dial interactions.
/// </summary>
/// <remarks>
/// Spawns molecule prefabs at a designated spawn point, tracks molecule counts,
/// and registers spawned objects with the Langevin physics simulation.
/// </remarks>
public class MolSpawn_dial : MonoBehaviour {

	/// <summary>
	/// The initial scale of spawned molecules (unused in current implementation).
	/// </summary>
	Vector3 minScale;

	/// <summary>
	/// The target scale for spawned molecules.
	/// </summary>
	public Vector3 maxScale;

	/// <summary>
	/// Speed multiplier for scaling operations.
	/// </summary>
	public float speed = 2f;

	/// <summary>
	/// Duration in seconds for scaling transitions.
	/// </summary>
	public float duration = 5f;

	/// <summary>
	/// The molecule prefab to instantiate when spawning.
	/// </summary>
	public Transform prefab;

	/// <summary>
	/// Reference to the most recently spawned molecule instance.
	/// </summary>
	private Transform spawn;

	/// <summary>
	/// The GameObject defining the spawn location and rotation.
	/// </summary>
    public GameObject spawnpoint;


	//Rigidbody[] children;

	/// <summary>
	/// Spawns a new molecule instance at the spawn point location.
	/// </summary>
	/// <remarks>
	/// <para>Only spawns if the current molecule count is below the limit defined in molcounter.</para>
	/// <para>Registers the spawned molecule's Rigidbodies with the Langevin_dial physics simulation.</para>
	/// <para>Assigns a unique name to each spawned molecule based on the counter.</para>
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
            //Quaternion rot = gameObject.transform.rotation;
            Quaternion rot = spawnpoint.transform.rotation;
            spawn = Instantiate(prefab, loc, rot);
            spawn.name = prefab.name+"_"+counter.ToString();
            Debug.Log("plop");
            Debug.Log("rot " + rot);

            // recover Langevin GOS gameobject list and append the spawned gameobjects
            Langevin_dial Lange = GameObject.FindGameObjectWithTag("Physics_Sim").GetComponent<Langevin_dial>();

            Lange.CountMOFOS();

            // add line for mol counter
            //List<Transform> MolCount = GameObject.Find("Simulation").GetComponent<molcounter>().molecules;

            Debug.Log("Before " + Lange.RBS.Count);
            //Lange.GOS.Add(
            Debug.Log(spawn.transform.name);
            Rigidbody[] GOarray = spawn.gameObject.GetComponentsInChildren<Rigidbody>();
            Lange.RBS.AddRange(GOarray);
            MolCount.Add(spawn);
            Debug.Log("After  " + Lange.RBS.Count);
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
