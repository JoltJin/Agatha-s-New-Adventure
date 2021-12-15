using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireMagic : Spells
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            collision.gameObject.GetComponent<HealthSystem>().Damage(damage, status, statusTime, statusDamage, transform.GetComponentInParent<HealthSystem>());

            if (spellType == SpellType.Explode)
            {
                transform.localScale *= 1.5f;
                StartCoroutine(Explode());
            }
        }
        else if (collision.tag == "Wall")
        {
            transform.localScale *= 1.25f;
            StartCoroutine(Explode());
        }
    }

    IEnumerator Explode()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        yield return new WaitForSeconds(.25f);
        Destroy(gameObject);
    }
}
