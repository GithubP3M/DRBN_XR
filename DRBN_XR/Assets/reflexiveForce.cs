using UnityEngine;

public class reflexiveForce : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
     void OnTriggerEnter(Collider collider)
    {
            Rigidbody rb = this.gameObject.GetComponent<Rigidbody>();
            rb.linearVelocity = -rb.linearVelocity;
    }
}
