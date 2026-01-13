using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generalized IMPALA (Implicit Membrane Potential for Lipid Assembly) simulation.
/// Applies position-dependent forces to simulate membrane insertion with arbitrary membrane orientation.
/// </summary>
/// <remarks>
/// Unlike the base impala class, this version uses the GameObject's forward vector to define
/// the membrane normal, allowing for non-horizontal membrane planes.
/// Objects on layer 11 receive membrane-specific viscosity and hydrophobic forces.
/// Child transforms tagged "hydrophobic" receive additional insertion forces.
/// </remarks>
public class ImpalaGeneralized : MonoBehaviour {

	//public GameObject[] gotag;

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
	
	/// <summary>Y-position used as reference for the membrane plane.</summary>
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

	/// <summary>
	/// Called when the script instance is being loaded (currently unused).
	/// </summary>
	void Start () {
		

	}
	
	/// <summary>
	/// Update is called once per frame (currently unused).
	/// </summary>
	void Update () {


	}

	/// <summary>
	/// Determines the force direction modifier based on which side of the membrane the object is on.
	/// </summary>
	/// <returns>-1 if below membrane plane, +1 if above.</returns>
	float switchmod(){
        //Debug.Log("switch " + Trigger_z + " " + this.name);

		

        if (z < Trigger_z)
        {
            modifier = -1;
        }
        else
        {
            modifier = 1;
        }
        return modifier;
	}

	/// <summary>
	/// Draws a yellow sphere gizmo at the membrane trigger position in the editor.
	/// </summary>
    private void OnDrawGizmos()
    {
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(new Vector3(0, Trigger_z, 0), 0.1f);
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

    

	//bool triggered = false;
	/// <summary>
	/// Called every frame while a collider stays in the membrane trigger.
	/// Applies viscosity damping and membrane insertion forces using the GameObject's forward vector
	/// as the membrane normal direction. Objects on layer 11 receive hydrophobic forces.
	/// </summary>
	/// <param name="collider">The collider that is inside the membrane trigger.</param>
    void OnTriggerStay (Collider collider) {
		//triggered = true;
		z = collider.gameObject.transform.position.y;

		
		

		//Debug.Log(collider.gameObject.name + " my name is");
		//Debug.Log(this.gameObject.name + " his name is");
		rb = collider.GetComponent<Rigidbody> ();
		gotag = collider.gameObject.transform.GetComponentsInChildren<Transform> ();
		var m = switchmod ();
		
		Vector3 up = this.gameObject.transform.forward.normalized; //because of course, up is forward... 
		Vector3 dn = this.gameObject.transform.forward.normalized*-1;

		//if (collider.gameObject.tag=="helix"){
		if (collider.gameObject.layer==11){
			//Debug.Log ("z " + z);

			rbv = rb.linearVelocity;
			rbav = rb.angularVelocity;

			rb.linearVelocity = rbv * 0.5f; // membrane is more viscous 
			rb.angularVelocity = rbav * 0.5f; // membrane is more viscous 

			Vector3 projected = collider.gameObject.transform.position;
			Vector3 projector = this.gameObject.transform.position;
			Vector3 projection = projected - projector;

			//Debug.DrawLine(this.gameObject.transform.position, this.gameObject.transform.position + projection, Color.green);

			Vector3 ProjectVec = Vector3.Project(projection, up);

			//Debug.DrawLine(this.gameObject.transform.position, this.gameObject.transform.position + ProjectVec, Color.magenta);

			



			Vector3 Frb = (dn * CalcCz (z,m));
			rb.AddForce (Frb); 
			Debug.DrawLine (rb.position, rb.position + Frb, Color.black);
			
			//Debug.DrawLine(rb.position, rb.position + up, Color.green);
			//Debug.DrawLine(rb.position, rb.position + rt, Color.red);
			//Debug.DrawLine(rb.position, rb.position + up, Color.blue);


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
			rb.AddForce (Frb); //disable temporarily for debugging purpose
			Debug.DrawLine (rb.position, rb.position + Frb, Color.white);
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
