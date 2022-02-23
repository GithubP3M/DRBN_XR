using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WristUI : MonoBehaviour
{
    public GameObject wristUI;
    public bool activeWristUI = true;

    public Transform prefab;
    private Transform spawn;
    public GameObject spawnpoint;
    private MolSpawn spawnGO;

    Dropdown mdropdown;

    // Start is called before the first frame update
    void Start()
    {
        DisplayWristUI();

        mdropdown = this.gameObject.GetComponent<Dropdown>();
        mdropdown.onValueChanged.AddListener(delegate {
            SpawnSelected(mdropdown);
        });
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void SpawnSelected(Dropdown change)
    {
        Debug.Log(mdropdown.value);
        
        int counter;
        counter = molcounter.molecules.Count;
        List<Transform> MolCount;
        MolCount = molcounter.molecules;
        spawnGO = this.gameObject.GetComponent<MolSpawn>();
        Debug.Log(this.gameObject.name + " name");
        Debug.Log(spawnpoint.name);



        if (counter < molcounter.limit)
        {
            Vector3 loc = spawnpoint.transform.position;
            Quaternion rot = gameObject.transform.rotation;
            spawn = Instantiate(prefab, loc, rot);
            spawn.name = prefab.name + "_" + counter.ToString();
        }
    }

    public void SpawnMolecule()
    {
        int counter;
        counter = molcounter.molecules.Count;
        List<Transform> MolCount;
        MolCount = molcounter.molecules;
        spawnGO = this.gameObject.GetComponent<MolSpawn>();
        Debug.Log(this.gameObject.name +" name");
        Debug.Log(spawnpoint.name);
        
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
