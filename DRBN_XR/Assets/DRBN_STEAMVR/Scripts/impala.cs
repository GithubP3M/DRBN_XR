using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implicit Membrane Potential for Lipid Assembly (IMPALA) simulation.
/// Applies position-dependent forces to simulate membrane insertion and hydrophobic interactions.
/// </summary>
/// <remarks>
/// Attach this script to a trigger collider representing the membrane region.
/// Objects tagged "helix" receive membrane-specific viscosity and hydrophobic forces.
/// Child transforms tagged "hydrophobic" receive additional insertion forces.
/// </remarks>
public class impala : MonoBehaviour {

	//public GameObject[] gotag;

	public float viscosity = 0.5f;

	/// <summary>Exponential decay constant for the membrane potential.</summary>
	float a=1.99f;
	
	/// <summary>Current z-position (y in Unity) of the colliding object.</summary>
	float z;
	
	/// <summary>Reference z-position for the membrane center in nm.</summary>
	float z_0=1.575f;
	
	/// <summary>Calculated membrane insertion coefficient.</summary>
	float C_z;
	
	/// <summary>Secondary membrane coefficient (unused).</summary>
	float C_zb;
	
	/// <summary>Direction modifier based on which side of membrane the object is on.</summary>
	float modifier;
	
	/// <summary>Y-position of the trigger collider (membrane plane).</summary>
	float Trigger_z;

	/// <summary>Rigidbody of the currently colliding object.</summary>
	Rigidbody rb;
	
	/// <summary>Linear velocity of the colliding Rigidbody.</summary>
	Vector3 rbv;
	
	/// <summary>Angular velocity of the colliding Rigidbody.</summary>
	Vector3 rbav;
	
	/// <summary>Position of hydrophobic child transforms.</summary>
	Vector3 phobicpos;
	
	/// <summary>Array of child transforms to check for hydrophobic tags.</summary>
	Transform[] gotag;

	[Header("Membrane damping")]
	public float membraneLinearDamping = 5.0f;
	public float membraneAngularDamping = 5.0f;

	struct DampingState
	{
		public float linear;
		public float angular;
	}

	readonly Dictionary<Rigidbody, DampingState> originalDamping = new Dictionary<Rigidbody, DampingState>();

	/// <summary>
	/// Initializes the membrane trigger position.
	/// </summary>
	void Start () {
		Trigger_z = this.gameObject.transform.position.y;
	}
	
	/// <summary>
	/// Update is called once per frame (currently unused).
	/// </summary>
	void Update () {
		
	}

	void OnTriggerEnter(Collider collider)
	{
		if (!collider) return;
		if (collider.gameObject.tag != "helix") return;

		Rigidbody enteredRb = collider.GetComponent<Rigidbody>();
		if (!enteredRb) return;

		if (!originalDamping.ContainsKey(enteredRb))
		{
			originalDamping[enteredRb] = new DampingState
			{
				linear = enteredRb.linearDamping,
				angular = enteredRb.angularDamping
			};
		}

		enteredRb.linearDamping = membraneLinearDamping;
		enteredRb.angularDamping = membraneAngularDamping;
	}

	void OnTriggerExit(Collider collider)
	{
		if (!collider) return;
		Rigidbody exitedRb = collider.GetComponent<Rigidbody>();
		if (!exitedRb) return;

		if (originalDamping.TryGetValue(exitedRb, out var state))
		{
			exitedRb.linearDamping = state.linear;
			exitedRb.angularDamping = state.angular;
			originalDamping.Remove(exitedRb);
		}
	}

	/// <summary>
	/// Determines the force direction modifier based on which side of the membrane the object is on.
	/// </summary>
	/// <returns>-1 if below membrane plane, +1 if above.</returns>
	float switchmod(){
		//Debug.Log ("switch " + Trigger_z + this.name);
		if (z < Trigger_z) {
			modifier = -1;
		} else {
			modifier = 1;
		}
		return modifier;
	}

	/// <summary>
	/// Calculates the membrane insertion coefficient based on z-position.
	/// Uses an exponential profile to model the hydrophobic core region.
	/// </summary>
	/// <param name="z">The z-position (y in Unity) to evaluate.</param>
	/// <param name="modifier">Direction modifier from switchmod().</param>
	/// <returns>The calculated insertion force coefficient.</returns>
	float CalcCz(float z,float modifier){
		if (Mathf.Abs(z) > 1.35f+Trigger_z && Mathf.Abs(z) < 1.8f+Trigger_z) {
			C_z = 0.5f - 11f + Mathf.Exp (a * (z - z_0));
			C_z = C_z * modifier;
			//Debug.Log ("medium");
		} else if (Mathf.Abs (z) > 1.35f+Trigger_z) {
			C_z = 0;
			//Debug.Log ("lo");
		} else if (Mathf.Abs (z) < 1.8f+Trigger_z) {
			C_z = 1 * modifier;
			//Debug.Log ("hi");
		}
		return C_z;
	}

//  working version
//	float switchmod(){
//		if (z < 0) {
//			modifier = -1;
//		} else {
//			modifier = 1;
//		}
//		return modifier;
//	}


//	float CalcCz(float z,float modifier){
//		if (Mathf.Abs(z) > 1.35f && Mathf.Abs(z) < 1.8f) {
//			C_z = 0.5f - 11f + Mathf.Exp (a * (z - z_0));
//			C_z = C_z * modifier;
//			Debug.Log ("medium");
//		} else if (Mathf.Abs (z) > 1.35f) {
//			C_z = 0;
//			Debug.Log ("lo");
//		} else if (Mathf.Abs (z) < 1.8f) {
//			C_z = 1 * modifier;
//			Debug.Log ("hi");
//		}
//		return C_z;
//	}

	/// <summary>
	/// Called every frame while a collider stays in the membrane trigger.
	/// Applies viscosity damping and membrane insertion forces to objects.
	/// Objects tagged "helix" receive special handling with hydrophobic child forces.
	/// </summary>
	/// <param name="collider">The collider that is inside the membrane trigger.</param>
	void OnTriggerStay (Collider collider) {
		z = collider.gameObject.transform.position.y;
		rb = collider.GetComponent<Rigidbody> ();
		if (!rb) return;
		gotag = collider.gameObject.transform.GetComponentsInChildren<Transform> ();
		var m = switchmod ();

		Vector3 dn = new Vector3 (0f, -1f, 0f);
		Vector3 up = new Vector3 (0f, 1f, 0f);

		if (collider.gameObject.tag=="helix"){
		//if (collider.gameObject.layer==11){
			//Debug.Log ("z " + z);

			Vector3 Frb = (dn * CalcCz (z,m));
			rb.AddForce (Frb);
			Debug.DrawLine (rb.position, rb.position + Frb, Color.black);
			//Debug.Log ("boom ");
			//Debug.Log (rb.position-(rb.position + Frb));

			for (int ht = 0; ht < gotag.Length; ht++) {
				if (gotag [ht].tag == "hydrophobic") {
					phobicpos = gotag [ht].position;
					var zphob = phobicpos.y;

					Vector3 F = (dn * CalcCz (zphob,m));
					rb.AddForceAtPosition (F, phobicpos);
					Debug.DrawLine (phobicpos, phobicpos + F, Color.blue);

				}
			}

		}
		else {
			Vector3 Frb = (up * CalcCz (z,m));
			rb.AddForce (Frb);
			Debug.DrawLine (rb.position, rb.position + Frb, Color.black);
		}

		//	void OnTriggerStay (Collider collider) {
		//		z = collider.gameObject.transform.position.y;
		//		rb = collider.GetComponent<Rigidbody> ();
		//		gotag = collider.gameObject.transform.GetComponentsInChildren<Transform> ();
		//		var m = switchmod ();
		//		//Debug.Log ("z " + z);
		//
		//		rbv = rb.velocity;
		//		rbav = rb.angularVelocity;
		//
		//		rb.velocity = rbv * 0.5f; // membrane is more viscous 
		//		rb.angularVelocity = rbav * 0.5f; // membrane is more viscous 
		//
		//		Vector3 dn = new Vector3 (0f, -1f, 0f);
		//		Vector3 up = new Vector3 (0f, 1f, 0f);
		//
		//		Vector3 Frb = (dn * CalcCz (z,m));
		//		rb.AddForce (Frb);
		//		Debug.DrawLine (rb.position, rb.position + Frb, Color.black);
		//		//Debug.Log ("boom ");
		//		//Debug.Log (rb.position-(rb.position + Frb));
		//
		//		for (int ht = 0; ht < gotag.Length; ht++) {
		//			if (gotag [ht].tag == "hydrophobic") {
		//				phobicpos = gotag [ht].position;
		//				var zphob = phobicpos.y;
		//				if (Mathf.Abs(zphob) < 1.8) {
		//					Vector3 F = (dn * CalcCz (zphob,m));
		//					rb.AddForceAtPosition (F, phobicpos);
		//					Debug.DrawLine (phobicpos, phobicpos + F, Color.blue);
		//				}
		//			}
		//		}
		//
		//	}
	}
}
