using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class WristUI : MonoBehaviour
{
    public GameObject wristUI;
    public bool activeWristUI = true;
    public Transform prefab;
    public GameObject spawnpoint;
    public TMP_Dropdown mdropdown;

    private Transform spawn;
    private MolSpawn spawnGO;
    private int DropdownValue;

    
    


    // Start is called before the first frame update
    void Start()
    {
        DisplayWristUI();

        //mdropdown = this.gameObject.GetComponent<Dropdown>();
        //Debug.Log(this.gameObject.name);
        //mdropdown.onValueChanged.AddListener(delegate {
        //    SpawnSelected(mdropdown);
        //});
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void SpawnSelected(TMP_Dropdown change)
    {
        Debug.Log(change.value);
        DropdownValue = change.value;
        
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
        Debug.Log(DropdownValue);

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

    //public void SpawnMolecule()
    //{
    //    int counter;
    //    counter = molcounter.molecules.Count;
    //    List<Transform> MolCount;
    //    MolCount = molcounter.molecules;
    //    spawnGO = this.gameObject.GetComponent<MolSpawn>();
    //    Debug.Log(this.gameObject.name +" name");
    //    Debug.Log(spawnpoint.name);

    //    if (counter < molcounter.limit)
    //    {
    //        Vector3 loc = spawnpoint.transform.position;
    //        Quaternion rot = gameObject.transform.rotation;
    //        spawn = Instantiate(prefab, loc, rot);
    //        spawn.name = prefab.name + "_" + counter.ToString();
    //    }
    //}

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
