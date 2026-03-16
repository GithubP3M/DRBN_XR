using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject ArrowButtonClose;
    public GameObject MemoryMenu;
    public GameObject[] objectsToToggle;
    public GameObject[] SubMenuList;
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

        foreach (GameObject obj in SubMenuList)
        {
            obj.SetActive(false);
        }
        menu.GetComponent<Image>().enabled = true;
        ArrowButtonClose.SetActive(false);
    }

    public void SetMemoryMenu(GameObject menu)
    {
        MemoryMenu = menu;
    }
    
    public void ActivateMemoryMenu()
    {
        if (MemoryMenu)
        {
            MemoryMenu.SetActive(true);
        }
    }

}
