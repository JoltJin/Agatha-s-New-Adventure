using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceMagic : Spells
{
    private void Awake()
    {
        if(name.Contains("3"))
        {
            transform.position = transform.parent.position;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            collision.gameObject.GetComponent<HealthSystem>().Damage(0, status, statusTime / 2, statusDamage, transform.GetComponentInParent<HealthSystem>());
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            collision.gameObject.GetComponent<HealthSystem>().Damage(damage, status, statusTime, statusDamage, transform.GetComponentInParent<HealthSystem>());

            if (spellType == SpellType.StickTo)
            {
                GetComponent<Rigidbody2D>().drag = 8f;
            }
        }
    }
}
