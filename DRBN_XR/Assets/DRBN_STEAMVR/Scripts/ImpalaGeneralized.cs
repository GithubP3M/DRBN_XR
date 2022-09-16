using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpalaGeneralized : MonoBehaviour {

	//public GameObject[] gotag;

	float a=1.99f;
	float z;
	float z_0=1.575f; //nm
	float C_z;
	float C_zb;
	float modifier;
	float Trigger_z;

	Rigidbody rb;
	Vector3 rbv;
	Vector3 rbav;
	Vector3 phobicpos;
	Transform[] gotag;

	// Use this for initialization
	void Start () {
		

	}
	
	// Update is called once per frame
	void Update () {


	}

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

 //   private void OnDrawGizmos()
 //   {
	//	Gizmos.color = Color.yellow;
	//	Gizmos.DrawSphere(new Vector3(0, Trigger_z, 0), 0.1f);
	//}
    

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

			rbv = rb.velocity;
			rbav = rb.angularVelocity;

			rb.velocity = rbv * 0.5f; // membrane is more viscous 
			rb.angularVelocity = rbav * 0.5f; // membrane is more viscous 

			Vector3 projected = collider.gameObject.transform.position;
			Vector3 projector = this.gameObject.transform.position;
			Vector3 projection = projected - projector;

			//Debug.DrawLine(this.gameObject.transform.position, this.gameObject.transform.position + projection, Color.green);

			Vector3 ProjectVec = Vector3.Project(projection, up);

			//Debug.DrawLine(this.gameObject.transform.position, this.gameObject.transform.position + ProjectVec, Color.magenta);

			



			Vector3 Frb = (up * CalcCz (z,m));
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
			Vector3 Frb = (dn * CalcCz (z,m));
			rb.AddForce (Frb); //disable temporarily for debugging purpose
			Debug.DrawLine (rb.position, rb.position + Frb, Color.white);
		}
	}
}
