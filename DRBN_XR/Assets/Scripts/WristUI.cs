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
    public GameObject simulation;
    public TMP_Dropdown mdropdown;
    public List<GameObject> PrefabList; //using GameObject type for ease of selection in assets, but don't forget that Instantiate uses Transform and not GameObject
    public Slider TempSlider;
    public Text temperature;
    public Langevin_dial langevin_Dial;
    public double Temp;

    private Transform spawn;
    private MolSpawn_dial spawnGO;
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
    }

    public void SpawnMolecule()
    {
        Debug.Log(DropdownValue);

        int counter;
        counter = molcounter.molecules.Count;
        List<Transform> MolCount;
        MolCount = molcounter.molecules;
        spawnGO = this.gameObject.GetComponent<MolSpawn_dial>();
        Debug.Log(this.gameObject.name + " name");
        Debug.Log(spawnpoint.name);

        //simulation.GetComponent<Langevin_dial_v2>().

        if (counter < molcounter.limit)
        {
            Vector3 loc = spawnpoint.transform.position;
            //Quaternion rot = gameObject.transform.rotation;
            Quaternion rot = spawnpoint.transform.rotation;
            spawn = Instantiate(PrefabList[DropdownValue].transform, loc, rot);
            spawn.name = PrefabList[DropdownValue].name + "_" + counter.ToString();
        }
    }

    public void ChangeTemp()
    {
        Temp = Langevin_dial.Temp;
        Debug.Log(TempSlider.value);
        Langevin_dial.Temp = Mathf.Lerp(TempSlider.minValue, TempSlider.maxValue, TempSlider.normalizedValue / 100f);
        temperature.text = Langevin_dial.Temp.ToString();
        //temperature.text = TempSlider.value.ToString();
        //Mathf.Lerp(0f, 10000f, e.normalizedValue / 100f);
        Debug.Log("langevin_Dial.Temp " + Temp);
    }

    //public void SpawnMolecule()
    //{
    //    int counter;
    //    counter = molcounter.molecules.Count;
    //    List<Transform> MolCount;
    //    MolCount = molcounter.molecules;
    //    spawnGO = this.gameObject.GetComponent<MolSpawn_dial>();
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
