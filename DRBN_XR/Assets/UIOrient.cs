using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOrient : MonoBehaviour
{
    public GameObject FollowCam;
    private Quaternion camRot;
    public float timer = 0.0f;
    public float waitingTime = 3.0f;
    
 
    
    void followCam() 
    {
        camRot = new Quaternion();

        camRot = FollowCam.transform.rotation;

        

        this.transform.rotation = camRot;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > waitingTime)
        {
            timer = 0f;
            followCam();
        }
    }
}
