/// @file Glue.cs
/// @brief Provides functionality to join GameObjects using physics joints.
/// @details This script creates FixedJoint connections between Rigidbody parent objects.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles joining GameObjects together using FixedJoint components.
/// </summary>
/// <remarks>
/// Attach this script to a GameObject that will manage glue/joint operations.
/// </remarks>
public class Glue : MonoBehaviour {
	

	/// <summary>
	/// Called when the script instance is being loaded.
	/// </summary>
	void Start () {
		
	}
	
	/// <summary>
	/// Called once per frame.
	/// </summary>
	void Update () {
		
	}

	/// <summary>
	/// Joins two GameObjects by creating a FixedJoint between their parent objects.
	/// </summary>
	/// <param name="A">The first Rigidbody whose parent will receive the FixedJoint.</param>
	/// <param name="B">The second Rigidbody whose parent will be connected.</param>
	void JoinGO(Rigidbody A,Rigidbody B){
		GameObject parentA = A.transform.parent.gameObject;
		GameObject parentB = B.transform.parent.gameObject;
		FixedJoint Gojoint = parentA.AddComponent<FixedJoint> ();

	}
}
