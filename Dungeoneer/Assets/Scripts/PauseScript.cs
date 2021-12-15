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

    public InventoryItem[] itemInventory = new InventoryItem[9];
    public InventoryItem[] weaponInventory = new InventoryItem[2];
    //public InventoryItem[] specialInventory = new InventoryItem[?];

    private int elementSlot = 0;
    private Element[] elements = new Element[2];

    private HudScript hudScript;

    [SerializeField]
    private GameObject /*equipmentHolder,*/ elementHolder;

    [SerializeField]
    private GameObject highlighter;

    [SerializeField]
    private Transform[] equipmentPosition = new Transform[2];

    [HideInInspector]
    public int slotNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        hudScript = FindObjectOfType<HudScript>();
        highlighter.SetActive(false);
        pauseMenu.SetActive(false);
        CloseSubMenu();
    }

    // Update is called once per frame
    void Update()
    {
        ////
        if (Input.GetButton("Reset"))
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

            RefreshMenuItems();
        }
        else if (Input.GetButtonDown("Pause") && /*!equipmentHolder.activeInHierarchy &&*/ !elementHolder.activeInHierarchy)
        {
            HidePause();
        }

        if (isPaused && EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(firstButton);
        }

        if (isPaused)
        {
            if (Input.GetButtonDown("ItemOneAttack") || Input.GetButtonDown("ItemTwoAttack"))
            {
                if (Input.GetButtonDown("ItemOneAttack"))
                {
                    slotNum = 1;
                }
                else
                {
                    slotNum = 2;
                }

                if (EventSystem.current.currentSelectedGameObject.name.Contains("Inventory"))
                {
                    EventSystem.current.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
                }
            }
            else if(Input.GetButtonDown("WeaponAttack") && !EventSystem.current.currentSelectedGameObject.name.Contains("Inventory"))
            {
                EventSystem.current.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
            }

        }

        //if (Input.GetButtonDown("ItemAttack") && isPaused)
        //{
        //    if(highlighter.activeInHierarchy)
        //    {
        //        elementSlot = 0;
        //        highlighter.SetActive(false);
        //    }
        //    else if(equipmentHolder.activeInHierarchy || elementHolder.activeInHierarchy)
        //    {
        //        CloseSubMenu();
        //    }
        //    //else
        //    //{
        //    //    HidePause();
        //    //}
        //}
    }

    private void RefreshMenuItems()
    {
        for (int i = 0; i < PlayerData.itemInventory.Count; i++)
        {
            itemInventory[i].currentGear = PlayerData.itemInventory[i];
            itemInventory[i].UpdateItem();
        }
        for (int i = 0; i < PlayerData.weaponInventory.Count; i++)
        {
            weaponInventory[i].currentGear = PlayerData.weaponInventory[i];
            weaponInventory[i].UpdateItem();
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
        //if (EventSystem.current.currentSelectedGameObject.name == "Gear 1")
        //{
            ////if you are just swapping positions
            //if(hudScript.equippedGear[1] == firstButton.GetComponentInChildren<InventoryItem>().currentGear)
            //{
            //    hudScript.equippedGear[1] = hudScript.equippedGear[0];
            //}
            hudScript.equippedGear[0] = firstButton.GetComponentInChildren<InventoryItem>().currentGear;
        //}
        //else if (EventSystem.current.currentSelectedGameObject.name == "Gear 2")
        //{
        //    //if you are just swapping positions
        //    if (hudScript.equippedGear[0] == firstButton.GetComponentInChildren<InventoryItem>().currentGear)
        //    {
        //        hudScript.equippedGear[0] = hudScript.equippedGear[1];
        //    }
        //    hudScript.equippedGear[1] = firstButton.GetComponentInChildren<InventoryItem>().currentGear;
        //}

        CloseSubMenu();

        FindObjectOfType<HudScript>().UpdateGear();

        EventSystem.current.SetSelectedGameObject(firstButton);
    }

    public void SwapElement()
    {
        elements[elementSlot] = (Element)System.Enum.Parse(typeof(Element), EventSystem.current.currentSelectedGameObject.name);

        elementSlot++;

        if (elementSlot >= 2)
        {
            if (elements[0] == elements[1])
            {
                elementSlot--;
                return;
            }
            for (int i = 0; i < PlayerData.currentEle.Length; i++)
            {
                PlayerData.currentEle[i] = elements[i];
            }
            CloseSubMenu();

            EventSystem.current.SetSelectedGameObject(firstButton);

            elementSlot = 0;
            highlighter.SetActive(false);

            FindObjectOfType<HudScript>().UpdateElement();

            return;
        }

        highlighter.SetActive(true);
        highlighter.transform.position = EventSystem.current.currentSelectedGameObject.transform.position;
    }

    public void ChangeElement()
    {
        firstButton = EventSystem.current.currentSelectedGameObject;
        CloseSubMenu();
        elementHolder.SetActive(true);
        elementSlot = 0;

        EventSystem.current.SetSelectedGameObject(elementHolder.transform.Find("Dark").gameObject);
    }
    //public void ChangePhysical()
    //{
    //    if(EventSystem.current.currentSelectedGameObject.GetComponentInChildren<InventoryItem>().currentGear != Gear.Empty)
    //    {
    //        firstButton = EventSystem.current.currentSelectedGameObject;
    //        CloseSubMenu();
    //        equipmentHolder.SetActive(true);
    //        equipmentHolder.transform.position = equipmentPosition[0].position;
    //        equipmentHolder.transform.Find("Gear Slot 1").GetComponent<Image>().sprite = FindObjectOfType<HudScript>().equipSlot[0].sprite;
    //        equipmentHolder.transform.Find("Gear Slot 2").GetComponent<Image>().sprite = FindObjectOfType<HudScript>().equipSlot[1].sprite;

    //        EventSystem.current.SetSelectedGameObject(equipmentHolder.transform.Find("Gear 1").gameObject);
    //    }
    //}
    //public void ChangeMagical()
    //{
    //    if (EventSystem.current.currentSelectedGameObject.GetComponentInChildren<InventoryItem>().currentGear != Gear.Empty)
    //    {
    //        firstButton = EventSystem.current.currentSelectedGameObject;
    //        CloseSubMenu();
    //        equipmentHolder.SetActive(true);
    //        equipmentHolder.transform.position = equipmentPosition[1].position;
    //        equipmentHolder.transform.Find("Gear Slot 1").GetComponent<Image>().sprite = FindObjectOfType<HudScript>().equipSlot[0].sprite;
    //        equipmentHolder.transform.Find("Gear Slot 2").GetComponent<Image>().sprite = FindObjectOfType<HudScript>().equipSlot[1].sprite;

    //        EventSystem.current.SetSelectedGameObject(equipmentHolder.transform.Find("Gear 1").gameObject);
    //    }
    //}

    public void ChangeWeapon()
    {
        hudScript.equippedGear[0] = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<InventoryItem>().currentGear;
        hudScript.UpdateGear();

        CloseSubMenu();
    }

    public void ChangeItem()
    {
        if (slotNum == 1)
        {
            //if you are just swapping positions
            if (hudScript.equippedGear[2] == EventSystem.current.currentSelectedGameObject.GetComponentInChildren<InventoryItem>().currentGear)
            {
                hudScript.equippedGear[2] = hudScript.equippedGear[1];
                hudScript.equippedGear[1] = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<InventoryItem>().currentGear;
            }
            else if(FindObjectOfType<PlayerMovement>().isInvisible || FindObjectOfType<PlayerMovement>().isFlying)
            {
                if(hudScript.equippedGear[slotNum] != Gear.Cloak && hudScript.equippedGear[slotNum] != Gear.Broom)
                {
                    hudScript.equippedGear[slotNum] = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<InventoryItem>().currentGear;
                }
            }
        }
        else
        {
            //if you are just swapping positions
            if (hudScript.equippedGear[1] == EventSystem.current.currentSelectedGameObject.GetComponentInChildren<InventoryItem>().currentGear)
            {
                hudScript.equippedGear[1] = hudScript.equippedGear[2];
            }
            else if (FindObjectOfType<PlayerMovement>().isInvisible || FindObjectOfType<PlayerMovement>().isFlying)
            {
                if (hudScript.equippedGear[slotNum] != Gear.Cloak && hudScript.equippedGear[slotNum] != Gear.Broom)
                {
                    hudScript.equippedGear[slotNum] = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<InventoryItem>().currentGear;
                }
            }
        }

        CloseSubMenu();

        hudScript.equippedGear[slotNum] = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<InventoryItem>().currentGear;
        hudScript.UpdateGear();
    }
    public void CloseSubMenu()
    {
        //equipmentHolder.SetActive(false);
        elementHolder.SetActive(false);
        RefreshMenuItems();
        //EventSystem.current.SetSelectedGameObject(firstButton);
    }
}