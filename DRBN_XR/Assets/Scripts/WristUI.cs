using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class WristUI : MonoBehaviour
{
    public GameObject wristUI;
    public bool activeWristUI = true;


    // Start is called before the first frame update
    void Start()
    {
        DisplayWristUI();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void SpawnMolecule()
    {
        spawn = Instantiate(prefab, loc, rot);
        spawn.name = prefab.name + "_" + counter.ToString();
    }

    public void MenuPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
            DisplayWristUI();
    }

    public void DisplayWristUI() 
    { 
    if(activeWristUI)
        {
            wristUI.SetActive(false);
            activeWristUI = false;
        }
    else if(!activeWristUI)
        {
            wristUI.SetActive(true);
            activeWristUI = true;
        }
    }
}
