using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class WristUI : MonoBehaviour
{
    public GameObject wristUI;
    public bool activeWristUI = true;

    public Transform prefab;
    private Transform spawn;
    public GameObject spawnpoint;
    private MolSpawn spawnGO;


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
        int counter;
        counter = molcounter.molecules.Count;
        List<Transform> MolCount;
        MolCount = molcounter.molecules;
        spawnGO = this.gameObject.GetComponent<MolSpawn>();
        Debug.Log(spawnGO.gameObject.name + " name !!!");
        spawnpoint = spawnGO.spawnpoint;
        Debug.Log(spawnGO.gameObject.name+" name !!!");
        
        if (counter < molcounter.limit)
        {
            Vector3 loc = spawnpoint.transform.position;
            Quaternion rot = gameObject.transform.rotation;
            spawn = Instantiate(prefab, loc, rot);
            spawn.name = prefab.name + "_" + counter.ToString();
        }
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
