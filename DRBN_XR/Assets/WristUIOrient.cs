using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WristUIOrient : MonoBehaviour
{
    public GameObject FollowCam;
    private Quaternion camRot;
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
        followCam();
    }
}
