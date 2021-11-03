using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Gear
{
    Empty,
    Staff,
    Spear,
    Tome,
    Broom,
    Elemental_Rod,
    Stick,
    Wand
}

public enum Element
{
    Dark,
    Fire,
    Ice,
    Light
}

public class HudScript : HealthSystem
{
    private int hitPerCandi = 3;
    private int maxCandi = 12;
    private int maxMana = 100;
    public int numOfCandi = 3;
    public float currentMana;

    
    
    public GameObject origin;

    [SerializeField]
    private ManaBar manaBar;

    [SerializeField]
    private Image[] elementIcon;

    [HideInInspector]
    public List<GameObject> candiHolders;

    public Sprite fullCandi,
                  oneBite,
                  twoBites,
                  emptyCandy;

    public Element[] currentEle;

    

    //public List<GameObject> weaponList = new List<GameObject>();
    //public List<Image> allGear = new List<Image>();
    public Image[] equipSlot = new Image[3];
    public Gear[] equippedGear = new Gear[3];

    

    private void Start()
    {
        health = numOfCandi * hitPerCandi;
        manaBar.SetMana(maxMana);
        currentMana = maxMana;
        //elementIcon.sprite = Resources.Load()
    }

    public void SwapElements()
    {
        Element eleHolder = currentEle[0];

        currentEle[0] = currentEle[1];
        currentEle[1] = eleHolder;
    }

    public void SwapWeapon()
    {
        Gear gearHolder = equippedGear[0];

        equippedGear[0] = equippedGear[1];
        equippedGear[1] = gearHolder;

        FindObjectOfType<PlayerMovement>().SwapGear();
    }

    private void UpdateHud()
    {
        UpdateHealth();
        UpdateGear();

        Sprite[] sprites;

        sprites = Resources.LoadAll<Sprite>("Sprites/HUD/Element Medallions");

        for (int j = 0; j < currentEle.Length; j++)
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                if (currentEle[j].ToString() + " Medallion" == sprites[i].name)
                {
                    elementIcon[j].sprite = sprites[i];
                }
            }

        }
    }

    private void UpdateHealth()
    {
        if (numOfCandi < 3)
        {
            numOfCandi = 3;
        }

        if (numOfCandi > maxCandi)
        {
            numOfCandi = maxCandi;
            ClearCandi();
        }

        if (numOfCandi < candiHolders.Count)
        {
            ClearCandi();
        }

        if(currentMana > maxMana)
        {
            currentMana = maxMana;
        }

        while (candiHolders.Count < numOfCandi)
        {
            for (int i = 0; i < numOfCandi && numOfCandi - candiHolders.Count > 0; i++)
            {
                GameObject heart = Instantiate(origin) as GameObject;
                heart.SetActive(true);

                heart.transform.SetParent(origin.transform.parent, false);

                candiHolders.Add(heart.gameObject);
            }
        }

        if (health > numOfCandi * hitPerCandi)
        {
            health = numOfCandi * hitPerCandi;
        }

        for (int i = 0; i < numOfCandi; i++)
        {
            if (health - i * hitPerCandi >= hitPerCandi)
            {
                candiHolders[i].GetComponent<Image>().sprite = fullCandi;
            }
            else if (health - i * hitPerCandi == 1)
            {
                candiHolders[i].GetComponent<Image>().sprite = twoBites;
            }
            else if (health - i * hitPerCandi == 2)
            {
                candiHolders[i].GetComponent<Image>().sprite = oneBite;
            }
            //else if (health - i * 4 == 3)
            //{
            //    heartContainers[i].GetComponent<Image>().sprite = oneBite;
            //}
            else
            {
                candiHolders[i].GetComponent<Image>().sprite = emptyCandy;
            }
        }
    }

    private void UpdateGear()
    {
        Color invisible = Color.white, opaque = Color.white;
        invisible.a = 0;
        opaque.a = 1;

        Sprite[] sprites;

        sprites = Resources.LoadAll<Sprite>("Sprites/HUD/Gear Icons");

        for (int i = 0; i < equippedGear.Length; i++)
        {
            if (equippedGear[i] == Gear.Empty)
            {
                equipSlot[i].color = invisible;
                break;
            }
            for (int j = 0; j < sprites.Length; j++)
            {
                if(sprites[j].name == equippedGear[i].ToString())
                {
                    equipSlot[i].color = opaque;

                    equipSlot[i].sprite = sprites[j];
                    //equipSlot[i].SetNativeSize();
                    break;
                }
            }
        }

        equipSlot[0].SetNativeSize();
    }

    

    private void ClearCandi()
    {
        for (int i = 0; i < candiHolders.Count; i++)
        {
            Destroy(candiHolders[i]);
        }
        candiHolders.Clear();
    }
    void Update()
    {
        UpdateHud();

        if(currentMana < maxMana)
        {
            currentMana += Time.deltaTime;
        }

        manaBar.UpdateMana(currentMana);

        if (health <= 0)
        {
            Destroy(gameObject);
        }

        ResetCooldowns();
    }
}
