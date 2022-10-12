using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
//using VRTK; //relic from past versions 

public class resetLocation : MonoBehaviour {
    private Vector3 initpos;
    private Vector3 initrot;
    public float timeLeft = 30;
    private float notimeLeft;

    // Use this for initialization
    void Start () {
        initpos = this.gameObject.transform.position;
        initrot = this.gameObject.transform.eulerAngles;
        //this.gameObject.GetComponent<VRTK_InteractableObject>().InteractableObjectGrabbed += new InteractableObjectEventHandler(ObjectGrabbed); ; // replace these lines by XR versions
        //this.gameObject.GetComponent<XRGrabInteractable>().isActiveAndEnabled += new InteractableObjectEventHandler(ObjectGrabbed);
    }
    public void Update()
    {
        reset();
    }

    //private void ObjectGrabbed(object sender, InteractableObjectEventArgs e)
    //{
    //    Debug.Log("Im Grabbed");
    //}

    public void activated() {
        notimeLeft = timeLeft;
        //Debug.Log("ACTIVATED");
        //Debug.Log("initpos " + initpos);
    }

    // Update is called once per frame
    
    public void reset () {
        //Debug.Log("something is happening");
        
        if (Vector3.Distance(this.gameObject.transform.position,initpos)>=0.1)
        {
            //Debug.Log(initpos+" initpos");
            //Debug.Log(this.gameObject.transform.position+" localpos");
            notimeLeft -= Time.deltaTime;
            Debug.Log("timeleft " + notimeLeft);
            if (notimeLeft < 0)
            {
                this.gameObject.transform.position = initpos;
                this.gameObject.transform.eulerAngles = initrot;
                this.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                notimeLeft = timeLeft;
            }
        }
	}
}
