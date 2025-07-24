using System.Security.Cryptography;
using UnityEngine;



public class distance_mesh : MonoBehaviour
{
    public MeshFilter membrane;

    float a = 1.99f;
    float z;
    float z_0 = 1.575f; //nm
    float C_z;
    float C_zb;
    float modifier;
    float Trigger_z;


    //Vector3 point = GameObject().rigidbody.position;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Vector3 origin=transform.position;
        Vector3 dn = new Vector3(0f, -1f, 0f);
        Vector3 up = new Vector3(0f, 1f, 0f);

        int impaLayer = 1 << LayerMask.NameToLayer("Impala");
        float maxDistance = 0.1f;
        Ray ray = new Ray(origin, dn);

        //if (Physics.Raycast(origin, direction, out hit, maxDistance))
        //if (Physics.Raycast(origin, dn, out hit,impaLayer,maxDistance))
        if (Physics.Raycast(origin, dn, out hit, maxDistance, impaLayer    ))
        //if (Physics.Raycast(origin, dn, out hit, impaLayer))
            {
            Rigidbody rb = GetComponent<Rigidbody>();

            float D = Vector3.Distance(origin, hit.point);
            Vector3 N = hit.normal;

            Debug.DrawLine(origin, hit.point, Color.blue);
            Debug.DrawLine(hit.point, hit.normal, Color.green);
            Debug.Log(D);

            //if (D > 0.12)
            //{
            //    Vector3 Frb = N * CalcCz(D, 1f);
            //    Debug.DrawLine(origin, hit.point, Color.blue);
            //    Debug.DrawLine(hit.point, hit.normal, Color.green);
            //    Debug.DrawLine(origin, origin+Frb, Color.red);
            //    rb.AddForceAtPosition(origin, Frb);
            //}

            if (D < 0.12)
            {
                Vector3 Frb = N * CalcCz(D, -1f);
                Debug.DrawLine(origin, hit.point, Color.blue);
                Debug.DrawLine(hit.point, hit.normal, Color.green);
                Debug.DrawLine(origin, origin+Frb, Color.red);
                //Debug.DrawLine(origin, origin + Frb, Color.red);
                rb.AddForceAtPosition(origin, Frb);
            }
        }

        
    }

    float CalcCz(float z, float modifier)
    {
        if (Mathf.Abs(z) > 1.35f + Trigger_z && Mathf.Abs(z) < 1.8f + Trigger_z)
        {
            C_z = 0.5f - 11f + Mathf.Exp(a * (z - z_0));
            C_z = C_z * modifier;
            //Debug.Log ("medium");
        }
        else if (Mathf.Abs(z) > 1.35f + Trigger_z)
        {
            C_z = 0;
            //Debug.Log ("lo");
        }
        else if (Mathf.Abs(z) < 1.8f + Trigger_z)
        {
            C_z = 1 * modifier;
            //Debug.Log ("hi");
        }
        return C_z;
    }
}


//Vector3 point = ...; // Your point in world space
//Collider meshCollider = ...; // Your mesh's collider
//Vector3 closest = meshCollider.ClosestPoint(point);
//float distance = Vector3.Distance(point, closest);