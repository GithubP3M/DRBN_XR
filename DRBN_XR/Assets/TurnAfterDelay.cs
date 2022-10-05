using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnAfterDelay : MonoBehaviour
{
    public Transform target;
    public int Delay;
    public float speed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //this.transform.rotation = CopyRotationfrom.rotation;

        // Determine which direction to rotate towards
        Vector3 targetDirection = target.position - transform.position;

        // The step size is equal to speed times frame time.
        float singleStep = speed * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

        // Draw a ray pointing at our target in
        Debug.DrawRay(transform.position, newDirection, Color.red);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        this.transform.rotation = Quaternion.LookRotation(newDirection);
        this.transform.position = target.position;
    }
}
