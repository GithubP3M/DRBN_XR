using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject[] objectsToToggle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    
    public void SelectMenu(GameObject menu)
    {
        foreach (GameObject obj in objectsToToggle)
        {
            obj.GetComponent<Image>().enabled = false;
        }
        menu.GetComponent<Image>().enabled = true;
    }

}
