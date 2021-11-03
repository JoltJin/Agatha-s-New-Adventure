using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem: MonoBehaviour
{
    public int health;
    private float damageCooldownAmount = .15f;
    private float damageCooldown = 0f;
    private float statusCooldown = 0f;
    private int knockbackDist = 5;
    private float knockbackTime = .2f;
    //public float statChance = 0f;

    private bool canHeal = true;
    private float healCooldown = .25f;

    public Status status;

    public List<Status> immunity = new List<Status>();
    private float statDur;

    public enum Status
    {
        Normal,
        Poisoned,
        Cursed,
        Burned,
        Frozen,
        Wet,
        Weak
    }

    public void HealthUp(int maxHp)
    {
        health = maxHp;
    }

    private void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }

        ResetCooldowns();
    }

    public void ResetCooldowns()
    {
        if (damageCooldown < 0)
        {
            damageCooldown = 0;
        }
        else if (damageCooldown <= damageCooldownAmount && damageCooldown > 0)
        {
            damageCooldown -= Time.deltaTime;
        }

        if (statusCooldown < 0)
        {
            statusCooldown = 0;
        }
        else if (statusCooldown <= damageCooldownAmount && statusCooldown > 0)
        {
            statusCooldown -= Time.deltaTime;
        }
    }

    public void Heal(int healAmount)
    {
        if (canHeal)
        {
            health += healAmount;
            StartCoroutine(HealTimer());
        }
    }

    IEnumerator HealTimer()
    {
        canHeal = false;
        yield return new WaitForSeconds(healCooldown);
        canHeal = true;
    }

    //basic physical attacks
    public void Damage(int damage, Transform damageSource)
    {
        if (damageCooldown <= 0f)
        {
            health -= damage;
            damageCooldown = damageCooldownAmount;
            Vector2 direction = transform.position - damageSource.position;
            direction = direction.normalized * knockbackDist;

            StartCoroutine(Knocked(direction));
        }

    }

    //physical charged attacks
    public void Damage(int damage, Element element, Transform damageSource, HealthSystem attacker)
    {
        Damage(0, damageSource);

        Status inflicted = Status.Normal;
        if(element == Element.Dark)
        {
            inflicted = Status.Poisoned;
        }
        else if (element == Element.Fire)
        {
            inflicted = Status.Burned;
        }
        else if (element == Element.Ice)
        {
            inflicted = Status.Frozen;
        }
        else if (element == Element.Light)
        {
            inflicted = Status.Weak;
        }
        
        Damage(damage, inflicted, 1f, 0, attacker);
    }

    //elemental attacks
    public void Damage(int damage, Status inflictedStatus, float statusDuration, int statusDamage, HealthSystem attacker)
    {
        if(statusCooldown <= 0f)
        {
            health -= damage;
            statusCooldown = damageCooldownAmount;

            if (immunity.Count > 0 && immunity.Contains(inflictedStatus))
            {
                return;
            }
            else
            {
                if(status == inflictedStatus)
                {
                    statDur += statusDuration;
                }
                else
                {
                    if(status == Status.Burned && inflictedStatus == Status.Frozen)
                    {
                        statDur += statusDuration;
                        statusDamage = 0;
                        inflictedStatus = Status.Wet;
                    }
                    else if (status == Status.Frozen && inflictedStatus == Status.Burned)
                    {
                        statDur += statusDuration / 2;
                        health -= damage/2;
                        inflictedStatus = Status.Wet;
                    }
                    else if (status == Status.Frozen && inflictedStatus == Status.Poisoned)
                    {
                        statDur += statusDuration/2;
                        inflictedStatus = Status.Frozen;
                    }
                    else if (status == Status.Weak && inflictedStatus == Status.Poisoned || status == Status.Poisoned && inflictedStatus == Status.Weak)
                    {
                        statDur += statusDuration / 2;
                        attacker.Heal(damage/2);
                        inflictedStatus = Status.Cursed;
                    }
                    else if (status == Status.Poisoned && inflictedStatus == Status.Burned || status == Status.Weak && inflictedStatus == Status.Burned)
                    {
                        statDur = 0;
                        health -= damage / 4;
                        inflictedStatus = Status.Normal;
                    }
                    else if (status == Status.Wet && inflictedStatus == Status.Weak)
                    {
                        statDur += statusDuration;
                        inflictedStatus = Status.Wet;
                    }
                    else if (status == Status.Wet && inflictedStatus == Status.Poisoned)
                    {
                        statDur -= statusDuration / 2;
                        statusDamage *= 2;
                        inflictedStatus = Status.Cursed;
                    }
                    else if (status == Status.Wet && inflictedStatus == Status.Frozen)
                    {
                        statDur += statusDuration/2;
                        statusDamage *= 2;
                        inflictedStatus = Status.Frozen;
                    }
                    else if (status == Status.Wet && inflictedStatus == Status.Burned)
                    {
                        statDur -= statusDuration;
                        statusDamage = 0;
                        health += damage / 2;
                        inflictedStatus = Status.Wet;
                    }
                    else if (status == Status.Cursed && inflictedStatus == Status.Weak)
                    {
                        statDur += statusDuration;
                        health += damage / 2;
                        inflictedStatus = Status.Cursed;
                    }
                    else if (status == Status.Cursed && inflictedStatus == Status.Poisoned)
                    {
                        statDur -= statusDuration /2;
                        attacker.Heal(damage / 2);
                        inflictedStatus = Status.Cursed;
                    }
                    else if (status == Status.Cursed && inflictedStatus == Status.Frozen)
                    {
                        statDur += statusDuration * 2;
                        statusDamage *= 2;
                        inflictedStatus = Status.Frozen;
                    }
                    else if (status == Status.Cursed && inflictedStatus == Status.Burned)
                    {
                        statDur -= statusDuration / 2;
                        health -= damage / 2;
                        statusDamage /= 2;
                        inflictedStatus = Status.Cursed;
                    }
                    else
                    {
                        statDur = statusDuration;
                    }

                    StopCoroutine("StatusDamage");
                    status = inflictedStatus;

                    if(status != Status.Normal)
                    {
                        StartCoroutine(StatusDamage(statusDamage));
                    }
                    else
                    {
                        statDur = 0;
                    }
                }
            }
        }
    }

    private IEnumerator StatusDamage(int damage)
    {
        while(statDur > 0)
        {
            health -= damage;
            statDur -= 1;
            yield return new WaitForSeconds(1);
        }
        status = Status.Normal;
    }

    private IEnumerator Knocked(Vector2 direction)
    {
        if(gameObject != null)
        {
            GetComponent<Rigidbody2D>().AddForce(direction, ForceMode2D.Impulse);

            if(GetComponent<EnemyScript>())
            {
                GetComponent<EnemyScript>().currentState = EnemyState.Hit;
            }
            else if (GetComponent<PlayerMovement>())
            {
                GetComponent<PlayerMovement>().currentState = PlayerState.Hit;
            }
            yield return new WaitForSeconds(knockbackTime);

            GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            if (GetComponent<EnemyScript>())
            {
                GetComponent<EnemyScript>().currentState = EnemyState.Chasing;
            }
            else if (GetComponent<PlayerMovement>())
            {
                GetComponent<PlayerMovement>().currentState = PlayerState.Moving;
            }
        }
    }
}
