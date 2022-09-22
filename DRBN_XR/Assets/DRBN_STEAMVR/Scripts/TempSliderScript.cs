using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempSliderScript : MonoBehaviour
{
    public Text myText;
    
    private Langevin_dial langevin_Dial;

    public Slider tempslider;

    private void Start()
    {
        tempslider.onValueChanged.AddListener((v)=> {
            myText.text = v.ToString("0.00");
        });
    }

    private void Update()
    {
        myText.text=tempslider.value.ToString();
    }
}

    