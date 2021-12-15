using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TomeScript : GearType
{
    public Transform teleportLocation, ManaShield, timeFreezeZone;

    private Vector2 teleportOrigin;

    public GameObject spellSwapMenu, previousSpellIcon, nextSpellIcon, currentSpellIcon;

    private HudScript player;

    private Vector2 movement = Vector2.zero;

    private bool moveTeleLocation = false;
    public bool canTele;

    private int previousSpell = 0, nextSpell = 0;
    private bool pressed = false;
    private bool freezeTime = false;

    private void Start()
    {
        teleportLocation.gameObject.SetActive(false);
        ManaShield.gameObject.SetActive(false);
        player = FindObjectOfType<HudScript>();
        teleportOrigin = teleportLocation.localPosition;
    }

    private void Update()
    {
        if (transform.position != transform.parent.position)
        {
            transform.position = transform.parent.position;
        }

        //generic item activation effects
        if (useItem)
        {
            UseItem();
        }
        // mana shield effects
        if (ManaShield.gameObject.activeInHierarchy)
        {
            if (PlayerData.currentMana > 0)
            {
                PlayerData.currentMana -= Time.deltaTime * 2;
            }
            else
            {
                ManaShield.gameObject.SetActive(false);
                GetComponentInParent<HealthSystem>().ignoreDamage = false;
            }
        }
        //teleport effects
        if (moveTeleLocation)
        {
            MoveTeleLocation();

            if (canTele)
            {
                Teleport();
            }
        }

        // changing spell effects
        if (secondaryEffect && PlayerData.learnedSpells.Count > 1)
        {
            DisableSpells();
            SwapSpell();
            MenuControls();
        }
        else if (PlayerData.learnedSpells.Count == 1)
        {
            FindObjectOfType<PlayerMovement>().isCasting = false;
            secondaryEffect = false;
            spellSwapMenu.SetActive(false);
        }
        else
        {
            spellSwapMenu.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (teleportLocation.gameObject.activeInHierarchy)
        {
            //Vector3 viewPos = teleportLocation.localPosition;
            //viewPos.x = Mathf.Clamp(viewPos.x, cameraBounds.x / 2 * -1, cameraBounds.x / 2);
            //viewPos.y = Mathf.Clamp(viewPos.y, cameraBounds.y * 2 * -1 + teleportHeight, cameraBounds.y * 2 - teleportHeight);
            //teleportLocation.localPosition = viewPos;
            Vector3 pos = Camera.main.WorldToViewportPoint(teleportLocation.position);
            pos.x = Mathf.Clamp(pos.x, 0.02f, 0.98f);
            pos.y = Mathf.Clamp(pos.y, 0.009f, 0.999f);
            teleportLocation.position = Camera.main.ViewportToWorldPoint(pos);

            Collider2D[] objects = new Collider2D[4];
            Physics2D.OverlapBoxNonAlloc(teleportLocation.position, teleportLocation.GetComponent<BoxCollider2D>().bounds.size, 0f, objects);

            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] != null)
                {
                    if(objects[i].tag == "Flyable" || objects[i].tag == "Wall")
                    {
                        canTele = false;
                        teleportLocation.GetComponent<SpriteRenderer>().color = Color.red;
                        break;
                    }
                }
                else
                {
                    canTele = true;
                    teleportLocation.GetComponent<SpriteRenderer>().color = Color.green;
                }
            }

            
        }
    }

    private void FixedUpdate()
    {
        if (teleportLocation.gameObject.activeInHierarchy)
        {
            teleportLocation.position += new Vector3(movement.x * GetComponentInParent<PlayerMovement>().moveSpeed * 2 * Time.deltaTime, movement.y * GetComponentInParent<PlayerMovement>().moveSpeed * 2 * Time.deltaTime, 0);
        }
    }

    public void UseItem()
    {
        useItem = false;

        switch (PlayerData.currentSpell)
        {
            case Spell.Mana_Shield:
                if (ManaShield.gameObject.activeInHierarchy)
                {
                    ManaShield.gameObject.SetActive(false);
                    GetComponentInParent<HealthSystem>().ignoreDamage = false;
                }
                else if (PlayerData.currentMana > 5)
                {
                    PlayerData.currentMana -= 4;
                    ManaShield.gameObject.SetActive(true);
                    GetComponentInParent<HealthSystem>().ignoreDamage = true;
                }
                break;
            case Spell.Teleport:
                //cameraBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.localPosition.z));
                Teleport();
                break;
            case Spell.Time_Freeze:
                if (PlayerData.currentMana > 80)
                {
                    FreezeTime();
                    PlayerData.currentMana -= 80;
                }
                break;
            case Spell.None:
                secondaryEffect = true;
                FindObjectOfType<PlayerMovement>().isCasting = true;

                break;
        }
    }

    private void FreezeTime()
    {
        Collider2D[] enemies = new Collider2D[10];
        Physics2D.OverlapBoxNonAlloc(timeFreezeZone.position, timeFreezeZone.GetComponent<BoxCollider2D>().size, 0, enemies, LayerMask.GetMask("Enemy"));

        foreach (Collider2D enemy in enemies)
        {
            if(enemy != null && enemy.transform.GetComponent<HealthSystem>())
            {
                enemy.transform.GetComponent<HealthSystem>().SpecialAilment(3, SpecialStatus.Time_Freeze);
            }
        }
    }
    private void DisableSpells()
    {
        ManaShield.gameObject.SetActive(false);
        GetComponentInParent<HealthSystem>().ignoreDamage = false;
        teleportLocation.position = teleportOrigin;
        teleportLocation.gameObject.SetActive(false);
    }

    public void SwapSpell()
    {
        spellSwapMenu.SetActive(true);

        int spellslot = 0;

        for (int i = 0; i < PlayerData.learnedSpells.Count; i++)
        {
            if (PlayerData.currentSpell == PlayerData.learnedSpells[i])
            {
                spellslot = i;
                break;
            }
        }

        currentSpellIcon.GetComponent<SkillIcon>().currentSpell = PlayerData.currentSpell;

        if (spellslot == 0)
        {
            previousSpell = PlayerData.learnedSpells.Count - 1;
            nextSpell = spellslot + 1;
        }
        else if (spellslot == PlayerData.learnedSpells.Count - 1)
        {
            nextSpell = 0;
            previousSpell = spellslot - 1;
        }
        else
        {
            previousSpell = spellslot - 1;
            nextSpell = spellslot + 1;
        }

        previousSpellIcon.GetComponent<SkillIcon>().currentSpell = PlayerData.learnedSpells[previousSpell];
        nextSpellIcon.GetComponent<SkillIcon>().currentSpell = PlayerData.learnedSpells[nextSpell];

        currentSpellIcon.GetComponent<SkillIcon>().UpdateSpell();
        nextSpellIcon.GetComponent<SkillIcon>().UpdateSpell();
        previousSpellIcon.GetComponent<SkillIcon>().UpdateSpell();
    }

    public void MenuControls()
    {
        if (Input.GetButtonDown("ItemOneAttack") || Input.GetButtonDown("ItemTwoAttack") || Input.GetButtonDown("WeaponAttack"))
        {
            pressed = true;
        }
        if (Input.GetButtonUp("ItemOneAttack") || Input.GetButtonUp("ItemTwoAttack") || Input.GetButtonUp("WeaponAttack"))
        {
            if (pressed)
            {
                secondaryEffect = false;
                spellSwapMenu.SetActive(false);

                player.UpdateSpell();
                if (player.equippedGear[1] == Gear.Tome)
                {
                    player.UpdateSpell(true, 0);
                }
                else
                {
                    player.UpdateSpell(true, 1);
                }

                player.UpdateGear();

                FindObjectOfType<PlayerMovement>().isCasting = false;
                secondaryEffect = false;
                pressed = false;
                return;
            }

        }
        else if (Input.GetButtonDown("Horizontal"))
        {
            if (Input.GetAxisRaw("Horizontal") == -1)
            {
                PlayerData.currentSpell = previousSpellIcon.GetComponent<SkillIcon>().currentSpell;
            }
            else if (Input.GetAxisRaw("Horizontal") == 1)
            {
                PlayerData.currentSpell = nextSpellIcon.GetComponent<SkillIcon>().currentSpell;
            }
        }
    }

    public void Teleport()
    {
        if (!teleportLocation.gameObject.activeInHierarchy && PlayerData.currentMana > 20)
        {
            teleportLocation.gameObject.SetActive(true);
            FindObjectOfType<PlayerMovement>().isCasting = true;
            moveTeleLocation = true;
        }
        else if (teleportLocation.gameObject.activeInHierarchy && Input.GetButtonUp("WeaponAttack"))
        {
            canTele = moveTeleLocation = false;
            teleportLocation.localPosition = teleportOrigin;
            teleportLocation.gameObject.SetActive(false);
            FindObjectOfType<PlayerMovement>().isCasting = false;
        }
        else if (teleportLocation.gameObject.activeInHierarchy && canTele)
        {
            if (Input.GetButtonUp("ItemOneAttack") || Input.GetButtonUp("ItemTwoAttack"))
            {
                FindObjectOfType<PlayerMovement>().gameObject.transform.localPosition = teleportLocation.position;
                FindObjectOfType<PlayerMovement>().gameObject.transform.localPosition += new Vector3(0, teleportLocation.GetComponent<SpriteRenderer>().bounds.size.y * .75f, 0);
                canTele = moveTeleLocation = false;
                teleportLocation.localPosition = teleportOrigin;
                teleportLocation.gameObject.SetActive(false);
                FindObjectOfType<PlayerMovement>().isCasting = false;
                PlayerData.currentMana -= 20;
            }
        }
    }

    public void MoveTeleLocation()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }
}