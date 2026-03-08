using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClockMenu : MonoBehaviour
{
    public TextMeshProUGUI clockText;
    public TextMeshProUGUI chronoText;
    public Image runButtonImage;
    public Sprite pauseIcon;
    public Sprite playIcon;
    private bool runChrono = false;

    private float time;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        while(true) {
            yield return new WaitForSeconds(1f);
            OutputTime();
        }
    }

    void OutputTime() {
        if (clockText)
        {
            clockText.text = TimeSpan.FromSeconds(Time.time).ToString(@"mm\:ss");
        }
    }

    public void OnClickRun()
    {
        runChrono = !runChrono;
        runButtonImage.sprite = runChrono ? pauseIcon : playIcon;
    }

    public void OnClickClear()
    {
        runChrono = false;
        chronoText.text="00:00,00";
        time = 0f;
        runButtonImage.sprite = playIcon;
    }

    private void Update()
    {
        if(runChrono) {
            time+= Time.deltaTime;
            if (chronoText)
            {
                chronoText.text = TimeSpan.FromSeconds(time).ToString(@"mm\:ss\,ff");
            }
        }
    }
}
