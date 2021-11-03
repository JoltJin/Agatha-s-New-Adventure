using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//for testing
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    public GameObject pauseMenu;
    public Animator pauseWindow;
    public Animator pauseCover;
    public static bool isPaused;
    public GameObject firstButton;
    private InventoryItem heldItem;

    private int elementSlot = 0;
    private Element[] elements = new Element[2];

    private HudScript hudScript;

    [SerializeField]
    private GameObject equipmentHolder, elementHolder;

    [SerializeField]
    private Transform[] equipmentPosition = new Transform[2];

    // Start is called before the first frame update
    void Start()
    {
        hudScript = FindObjectOfType<HudScript>();
        pauseMenu.SetActive(false);
        CloseSubMenu();
    }

    // Update is called once per frame
    void Update()
    {
        ////
        if(Input.GetButton("Reset"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        ////
        if (Input.GetButtonDown("Pause") && !pauseMenu.activeInHierarchy)
        {
            pauseMenu.SetActive(true);
            isPaused = true;
            Time.timeScale = 0;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstButton);
        }
        else if (Input.GetButtonDown("Pause") && !equipmentHolder.activeInHierarchy && !elementHolder.activeInHierarchy)
        {
            HidePause();
        }

        if(isPaused && EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(firstButton);
        }

        if (Input.GetButtonDown("ItemAttack") && isPaused)
        {
            if(equipmentHolder.activeInHierarchy || elementHolder.activeInHierarchy)
            {
                CloseSubMenu();
            }
            else
            {
                HidePause();
            }
        }
    }

    private void HidePause()
    {
        pauseWindow.SetTrigger("Pause");
        pauseCover.SetTrigger("Pause");
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            isPaused = false;
            firstButton = EventSystem.current.currentSelectedGameObject;
        }
    }
    
    public void SwapGear()
    {
        if(EventSystem.current.currentSelectedGameObject.name == "Gear 1")
        {
            //if you are just swapping positions
            if(hudScript.equippedGear[1] == firstButton.GetComponentInChildren<InventoryItem>().currentGear)
            {
                hudScript.equippedGear[1] = hudScript.equippedGear[0];
            }
            hudScript.equippedGear[0] = firstButton.GetComponentInChildren<InventoryItem>().currentGear;
        }
        else if (EventSystem.current.currentSelectedGameObject.name == "Gear 2")
        {
            //if you are just swapping positions
            if (hudScript.equippedGear[0] == firstButton.GetComponentInChildren<InventoryItem>().currentGear)
            {
                hudScript.equippedGear[0] = hudScript.equippedGear[1];
            }
            hudScript.equippedGear[1] = firstButton.GetComponentInChildren<InventoryItem>().currentGear;
        }

        CloseSubMenu();

        EventSystem.current.SetSelectedGameObject(firstButton);
    }

    public void SwapElement()
    {
        elements[elementSlot] = (Element)System.Enum.Parse(typeof(Element), EventSystem.current.currentSelectedGameObject.name);

        elementSlot++;

        if(elementSlot >= 2)
        {
            if(elements[0] == elements[1])
            {
                elementSlot--;
                return;
            }
            for (int i = 0; i < hudScript.currentEle.Length; i++)
            {
                hudScript.currentEle[i] = elements[i];
            }
            CloseSubMenu();
        }

    }

    public void ChangeElement()
    {
        firstButton = EventSystem.current.currentSelectedGameObject;
        CloseSubMenu();
        elementHolder.SetActive(true);
        elementSlot = 0;

        EventSystem.current.SetSelectedGameObject(elementHolder.transform.Find("Dark").gameObject);
    }
    public void ChangePhysical()
    {
        if(EventSystem.current.currentSelectedGameObject.GetComponentInChildren<InventoryItem>().currentGear != Gear.Empty)
        {
            firstButton = EventSystem.current.currentSelectedGameObject;
            CloseSubMenu();
            equipmentHolder.SetActive(true);
            equipmentHolder.transform.position = equipmentPosition[0].position;
            equipmentHolder.transform.Find("Gear Slot 1").GetComponent<Image>().sprite = FindObjectOfType<HudScript>().equipSlot[0].sprite;
            equipmentHolder.transform.Find("Gear Slot 2").GetComponent<Image>().sprite = FindObjectOfType<HudScript>().equipSlot[1].sprite;

            EventSystem.current.SetSelectedGameObject(equipmentHolder.transform.Find("Gear 1").gameObject);
        }
    }
    public void ChangeMagical()
    {
        if (EventSystem.current.currentSelectedGameObject.GetComponentInChildren<InventoryItem>().currentGear != Gear.Empty)
        {
            firstButton = EventSystem.current.currentSelectedGameObject;
            CloseSubMenu();
            equipmentHolder.SetActive(true);
            equipmentHolder.transform.position = equipmentPosition[1].position;
            equipmentHolder.transform.Find("Gear Slot 1").GetComponent<Image>().sprite = FindObjectOfType<HudScript>().equipSlot[0].sprite;
            equipmentHolder.transform.Find("Gear Slot 2").GetComponent<Image>().sprite = FindObjectOfType<HudScript>().equipSlot[1].sprite;

            EventSystem.current.SetSelectedGameObject(equipmentHolder.transform.Find("Gear 1").gameObject);
        }
    }
    public void ChangeItem()
    {
        FindObjectOfType<HudScript>().equippedGear[2] = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<InventoryItem>().currentGear;
    }
    public void CloseSubMenu()
    {
        equipmentHolder.SetActive(false);
        elementHolder.SetActive(false);

        EventSystem.current.SetSelectedGameObject(firstButton);
    }
}
