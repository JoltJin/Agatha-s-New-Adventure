using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spells : MonoBehaviour
{
    public int damage;
    public int velocity;
    public float duration;
    public float statusTime;
    public int statusDamage;

    public Status status;
    public SpellType spellType;

    public enum SpellType
    {
        PassThrough,
        Explode,
        StickTo
    }

    private void Start()
    {
        StartCoroutine(MagicBurnout());
        if (GetComponentInParent<PlayerMovement>().direction == PlayerMovement.Direction.Up)
        {
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
        }
        else if (GetComponentInParent<PlayerMovement>().direction == PlayerMovement.Direction.Down)
        {
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, -90.0f);
        }
        else if (GetComponentInParent<PlayerMovement>().direction == PlayerMovement.Direction.Left)
        {
            //GetComponent<SpriteRenderer>().flipX = true;
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);
        }
    }

    IEnumerator MagicBurnout()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}
