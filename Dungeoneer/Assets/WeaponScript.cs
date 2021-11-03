using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    public int physicalDamage, magicDamage;
    public float firstCharge, secondCharge, thirdCharge;
    private int attackBuff;
    public Transform attackRange;
    public float attackSize;
    public bool isRanged = false;
    public int mpRegen;
    private float regenCooldownAmount = .1f, 
                  regenCooldown = 0;

    virtual public void WeaponAttack(float chargeTime, LayerMask enemyLayers)
    {
        Vector2 direction = Vector2.zero;
        int castLevel =  0, chargeLevel =  0;

        int magicCost = 0;

        if(GetComponentInParent<HudScript>().currentEle[0] == Element.Dark)
        {
            magicCost = 10;
        }
        else if (GetComponentInParent<HudScript>().currentEle[0] == Element.Fire)
        {
            magicCost = 15;
        }
        else if (GetComponentInParent<HudScript>().currentEle[0] == Element.Ice)
        {
            magicCost = 10;
        }
        else if (GetComponentInParent<HudScript>().currentEle[0] == Element.Light)
        {
            magicCost = 8;
        }

        if (GetComponentInParent<PlayerMovement>().direction == PlayerMovement.Direction.Up)
        {
            direction = Vector2.up;
        }
        else if (GetComponentInParent<PlayerMovement>().direction == PlayerMovement.Direction.Down)
        {
            direction = Vector2.down;
        }
        else if (GetComponentInParent<PlayerMovement>().direction == PlayerMovement.Direction.Left)
        {
            direction = Vector2.left;
        }
        else if (GetComponentInParent<PlayerMovement>().direction == PlayerMovement.Direction.Right)
        {
            direction = Vector2.right;
        }

        if (chargeTime < firstCharge)// if not charged at all
        {
            attackBuff = 1;
        }
        else if (chargeTime >= firstCharge && chargeTime < secondCharge)// if charged to first level
        {
            attackBuff = 2;
            
            if(GetComponentInParent<HudScript>().currentMana >= magicCost && isRanged)
            {
                GetComponentInParent<HudScript>().currentMana -= magicCost;
                castLevel = 1;
            }
            chargeLevel = 1;
        }
        else if (chargeTime >= secondCharge && chargeTime < thirdCharge)// if charged to second level
        {
            attackBuff = 3;

            if (GetComponentInParent<HudScript>().currentMana >= magicCost * 1.5 && isRanged)
            {
                GetComponentInParent<HudScript>().currentMana -= (magicCost * 1.5f);
                castLevel = 2;
            }
            chargeLevel = 2;
        }
        else// if fully charged
        {
            attackBuff = 5;

            if (GetComponentInParent<HudScript>().currentMana >= magicCost * 2 && isRanged)
            {
                GetComponentInParent<HudScript>().currentMana -= (magicCost * 2);
                castLevel = 3;
            }
            chargeLevel = 3;
        }

        if(isRanged && castLevel > 0)
        {
            GameObject magic = (GameObject)Instantiate(Resources.Load("Sprites/Magic/" + GetComponentInParent<HudScript>().currentEle[0].ToString() + " " + castLevel), attackRange.position, attackRange.rotation, transform.parent);
            Rigidbody2D rb = magic.GetComponent<Rigidbody2D>();
            rb.AddForce(direction * magic.GetComponent<Spells>().velocity, ForceMode2D.Impulse);
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackRange.position, attackSize, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            if(chargeLevel > 0 && !isRanged)
            {
                enemy.GetComponent<HealthSystem>().Damage(physicalDamage * attackBuff, GetComponentInParent<HudScript>().currentEle[0], transform, transform.GetComponentInParent<HealthSystem>());
            }
            else
            {
                enemy.GetComponent<HealthSystem>().Damage(physicalDamage * attackBuff, transform);
            }
            if(regenCooldown <= 0)
            {
                GetComponentInParent<HudScript>().currentMana += (mpRegen * attackBuff);
                regenCooldown = regenCooldownAmount;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (regenCooldown < 0)
        {
            regenCooldown = 0;
        }
        else if (regenCooldown <= regenCooldownAmount && regenCooldown > 0)
        {
            regenCooldown -= Time.deltaTime;
        }
    }
}
