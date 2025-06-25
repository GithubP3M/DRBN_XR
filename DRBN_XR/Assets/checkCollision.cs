using UnityEngine;
using System.Collections;

public class CheckCollision : MonoBehaviour
{
    void OnCollisionEnter(Collision col)
    {
       
        for (int i = 0; i < col.contactCount; i++)
        {
            ContactPoint cp = col.GetContact(i);
            foreach(ConfigurableJoint joint in this.GetComponents<ConfigurableJoint>())
            {
                if (joint.connectedBody == cp.otherCollider.attachedRigidbody)
                {
                  Debug.Log("Ignore collisions between "+cp.thisCollider.name+" from " + cp.thisCollider.attachedRigidbody.name+ " and " +cp.otherCollider.name+" from " + cp.otherCollider.attachedRigidbody.name );
                  Physics.IgnoreCollision(cp.thisCollider, cp.otherCollider);
                }
            }
        }
        Destroy(this);
     }
     
}
