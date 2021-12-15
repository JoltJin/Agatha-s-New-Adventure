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
    Wand,
    Whip,
    Bow,
    Cloak
}

public enum Element
{
    Dark,
    Fire,
    Ice,
    Light
}
public enum Spell
{
    None,
    Teleport,
    Mana_Shield,
    Time_Freeze,


    Last
}

public enum EquipmentType
{
    Weapon,
    Item,
    Special,
    Other
}

public class HudScript : MonoBehaviour
{
    private HealthSystem playerHealth;
    private int hitPerCandi = 3;// how many hits per health unit
    [SerializeField]private int numOfCandi = 3; //max starting health
    private int maxCandi = 12;// total maximum health
    public Signal healthSignal;// signal to update health amount
    public GameObject origin; //original health icon
    
    private int maxMana = 100;
    //[HideInInspector]
    //public float currentMana;
    [SerializeField]
    private ManaBar manaBar;
    
    [SerializeField]
    private Image[] elementIcon;
    [HideInInspector]
    //public Element[] currentEle = new Element[2];

    public Image[] equipSlot = new Image[3];
    //[HideInInspector]
    public Gear[] equippedGear = new Gear[3];

    [SerializeField]
    private Image[] spellIcon = new Image[2];

    //public Spell currentSpell;

    //public List<Spell> learnedSpells = new List<Spell>();

    [HideInInspector]
    public List<GameObject> candiHolders;
    public Sprite fullCandi,
                  oneBite,
                  twoBites,
                  emptyCandy;
    
    private void Start()
    {
        playerHealth = GameObject.Find("Agatha").GetComponent<HealthSystem>();
        playerHealth.health = numOfCandi * hitPerCandi;
        manaBar.SetMana(maxMana);

        //PlayerData.currentMana = maxMana;

        playerHealth.health = PlayerData.health;
        UpdateHud();

        if(PlayerData.learnedSpells.Count < 1)
        {
            PlayerData.learnedSpells.Add(Spell.None);

            PlayerData.learnedSpells.Add(Spell.Mana_Shield);
            PlayerData.learnedSpells.Add(Spell.Teleport);
            PlayerData.learnedSpells.Add(Spell.Time_Freeze);
        }

        if(PlayerData.weaponInventory.Count < 1)
        {
            PlayerData.weaponInventory.Add(Gear.Staff);
        }


    }

    void Update()
    {
        if(PlayerData.currentMana < maxMana)
        {
            PlayerData.currentMana += Time.deltaTime;
        }

        if(PlayerData.currentMana < 0)
        {
            PlayerData.currentMana = 0;
        }
        if(PlayerData.currentMana > maxMana)
        {
            PlayerData.currentMana = maxMana;
        }
        
        manaBar.UpdateMana(PlayerData.currentMana);
    }

    private void UpdateHud()
    {
        UpdateHealth();
        UpdateGear();
        UpdateElement();
    }

    public void SwapElements()
    {
        Element eleHolder = PlayerData.currentEle[0];

        PlayerData.currentEle[0] = PlayerData.currentEle[1];
        PlayerData.currentEle[1] = eleHolder;

        UpdateElement();
    }

    public void UpdateElement()
    {
        Sprite[] sprites;

        sprites = Resources.LoadAll<Sprite>("Sprites/Menus/HUD/Element Medallions");

        for (int j = 0; j < PlayerData.currentEle.Length; j++)
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                if (PlayerData.currentEle[j].ToString() + " Medallion" == sprites[i].name)
                {
                    elementIcon[j].sprite = sprites[i];
                }
            }
        }

        Color invisible = Color.white, opaque = Color.white;
        invisible.a = 0;
        opaque.a = 1;

        if(PlayerData.currentEle[0] == PlayerData.currentEle[1])
        {
            elementIcon[1].color = invisible;
        }
        else
        {
            elementIcon[1].color = opaque;
        }
    }

    public void UpdateSpell(bool bookOpen, int slot)
    {
        Color invisible = Color.white, opaque = Color.white;
        opaque.a = 1;

        spellIcon[slot].color = opaque;

        Sprite[] sprites;

        sprites = Resources.LoadAll<Sprite>("Sprites/Menus/HUD/Spell Icons");

        for (int i = 0; i < sprites.Length; i++)
        {
            if (PlayerData.currentSpell.ToString() == sprites[i].name)
            {
                spellIcon[slot].sprite = sprites[i];
            }
        }
    }

    public void UpdateSpell()
    {
        Color invisible = Color.white, opaque = Color.white;
        invisible.a = 0;
        foreach (Image itemSlot in spellIcon)
        {
            itemSlot.color = invisible;
        }
    }

    public void SwapWeapon()
    {
        Gear gearHolder = equippedGear[0];

        equippedGear[0] = equippedGear[1];
        equippedGear[1] = gearHolder;

        FindObjectOfType<PlayerMovement>().SwapGear();

        UpdateGear();
    }

    public void UpdateGear()
    {
        Color invisible = Color.white, opaque = Color.white;
        invisible.a = 0;
        opaque.a = 1;

        Sprite[] sprites;

        sprites = Resources.LoadAll<Sprite>("Sprites/Menus/HUD/Gear Icons");

        UpdateSpell();
        for (int i = 0; i < equippedGear.Length; i++)
        {
            if (equippedGear[i] == Gear.Empty)
            {
                equipSlot[i].color = invisible;
                //break;
                continue;
            }
            for (int j = 0; j < sprites.Length; j++)
            {
                if(sprites[j].name == equippedGear[i].ToString())
                {
                    equipSlot[i].color = opaque;

                    equipSlot[i].sprite = sprites[j];
                    continue;
                }else if (equippedGear[i] == Gear.Tome)
                {
                    equipSlot[i].color = opaque;

                    if (PlayerData.currentSpell == Spell.None)
                    {
                        if(sprites[j].name == "Tome Closed")
                        {
                            equipSlot[i].sprite = sprites[j];
                        }
                    }
                    else
                    {
                        if (sprites[j].name == "Tome Open")
                        {
                            equipSlot[i].sprite = sprites[j];
                            UpdateSpell(true, i - 1);
                        }
                    }
                }
            }
        }
        equipSlot[0].SetNativeSize();
        equipSlot[1].SetNativeSize();
        equipSlot[2].SetNativeSize();

        FindObjectOfType<PlayerMovement>().SwapGear();
    }

    public void UpdateHealth()
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

        if (playerHealth.health > numOfCandi * hitPerCandi)
        {
            playerHealth.health = numOfCandi * hitPerCandi;
        }

        for (int i = 0; i < numOfCandi; i++)
        {
            if (playerHealth.health - i * hitPerCandi >= hitPerCandi)
            {
                candiHolders[i].GetComponent<Image>().sprite = fullCandi;
            }
            else if (playerHealth.health - i * hitPerCandi == 1)
            {
                candiHolders[i].GetComponent<Image>().sprite = twoBites;
            }
            else if (playerHealth.health - i * hitPerCandi == 2)
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

            PlayerData.health = playerHealth.health;
        }
    }

    private void ClearCandi()
    {
        for (int i = 0; i < candiHolders.Count; i++)
        {
            Destroy(candiHolders[i]);
        }
        candiHolders.Clear();
    }
}