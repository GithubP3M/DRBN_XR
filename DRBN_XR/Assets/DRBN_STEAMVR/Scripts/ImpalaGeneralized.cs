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
		//obsolete for generalized impala Trigger_z = this.gameObject.transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	float switchmod(){
        //Debug.Log("switch " + Trigger_z + this.name);
        

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

 //   private void OnDrawGizmos()
 //   {
	//	Gizmos.DrawSphere(rb.gameObject.transform.localPosition, 0.01f);
	//	Gizmos.DrawSphere(this.gameObject.transform.localPosition, 0.01f);
	//}

	//bool triggered = false; 
    void OnTriggerStay (Collider collider) {
		//triggered = true;
		z = collider.gameObject.transform.position.y;
		//Debug.Log(collider.gameObject.name + " my name is");
		//Debug.Log(this.gameObject.name + " his name is");
		rb = collider.GetComponent<Rigidbody> ();
		gotag = collider.gameObject.transform.GetComponentsInChildren<Transform> ();
		var m = switchmod ();

		//Defined by the gameobject Z axis in generalized Impala
		//Vector3 dn = new Vector3 (0f, -1f, 0f);
		//Vector3 up = new Vector3 (0f, 1f, 0f);


		Vector3 dn = collider.gameObject.transform.localRotation.eulerAngles;
		Vector3 up = Quaternion.Inverse(collider.gameObject.transform.localRotation).eulerAngles;
		

		//Debug.Log(dn + " dn");
		//Debug.Log(up + " up");

		//if (collider.gameObject.tag=="helix"){
		if (collider.gameObject.layer==11){
			//Debug.Log ("z " + z);

			rbv = rb.velocity;
			rbav = rb.angularVelocity;

			rb.velocity = rbv * 0.5f; // membrane is more viscous 
			rb.angularVelocity = rbav * 0.5f; // membrane is more viscous 



			Vector3 Frb = (dn * CalcCz (z,m));
            //rb.AddForce (Frb); //disable temporarily for debugging purpose
            Debug.DrawLine(rb.position, rb.position + Frb, Color.black);
            Debug.DrawLine(rb.position, rb.position + dn, Color.blue);
            Debug.DrawLine(rb.position, rb.position + up, Color.red);


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
			//Vector3 Frb = (up * CalcCz (z,m));
			//rb.AddForce (Frb); //disable temporarily for debugging purpose
			//Debug.DrawLine (rb.position, rb.position + Frb, Color.white);
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
