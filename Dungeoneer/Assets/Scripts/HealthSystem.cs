using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public enum SpecialStatus
    {
        None,
        Time_Freeze
    }

public class HealthSystem: MonoBehaviour
{
    public int health;
    private float damageCooldownAmount = .2f;
    private float damageCooldown = 0f;
    private float statusCooldown = 0f;
    public int knockbackDist = 5;
    private float knockbackTime = .2f;
    //public float statChance = 0f;

    private bool canHeal = true;
    private float healCooldown = .25f;

    public bool ignoreDamage = false;

    public Status status;

    //public List<SpecialStatus> specialStatus = new List<SpecialStatus>();
    public SpecialStatus specialStatus;
    public List<Status> immunity = new List<Status>();
    private float statDur;



    public void HealthMax(int maxHp)
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

        if (GetComponent<HudScript>())
        {
            GetComponent<HudScript>().healthSignal.Raise();
        }
    }

    public void ReduceDamage(int reductionAmount)
    {
        health += reductionAmount;

        if (GetComponent<HudScript>())
        {
            GetComponent<HudScript>().healthSignal.Raise();
        }
    }

    IEnumerator HealTimer()
    {
        canHeal = false;
        yield return new WaitForSeconds(healCooldown);
        canHeal = true;
    }

    public void TakeDamage(int damage)
    {
        if (!ignoreDamage)
        {
            health -= damage;
        }

        if (GetComponentInParent<PlayerMovement>())
        {
            FindObjectOfType<HudScript>().healthSignal.Raise();
        }
    }

    //basic physical attacks
    public void Damage(int damage, Transform damageSource, float knockbackMultiplier)
    {
        if (damageCooldown <= 0f)
        {
            TakeDamage(damage);

            if(GetComponentInParent<PlayerMovement>())
            {
                PlayerData.currentMana -= damage * 2;
            }

            if (GetComponent<HudScript>())
            {
                GetComponent<HudScript>().healthSignal.Raise();
            }

            damageCooldown = damageCooldownAmount;
            Vector2 direction = transform.position - damageSource.position;
            direction = direction.normalized * knockbackDist;

            StartCoroutine(Knocked(direction, knockbackMultiplier));
        }
    }

    //physical charged attacks
    public void Damage(int damage, Element element, Transform damageSource, HealthSystem attacker)
    {
        Damage(0, damageSource, 1f);

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
            TakeDamage(damage);
            
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
                        TakeDamage(damage/2);
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
                        TakeDamage(damage / 4);
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
                        ReduceDamage(damage / 2);
                        inflictedStatus = Status.Wet;
                    }
                    else if (status == Status.Cursed && inflictedStatus == Status.Weak)
                    {
                        statDur += statusDuration;
                        ReduceDamage(damage / 2);
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
                        TakeDamage(damage / 2);
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
            TakeDamage(damage);
            statDur -= 1;
            yield return new WaitForSeconds(1);
        }
        status = Status.Normal;
    }

    public void SpecialAilment(float specialDuration, SpecialStatus statusType)
    {
        
        if(statusType == SpecialStatus.Time_Freeze && specialStatus != SpecialStatus.Time_Freeze)
        {
            specialStatus = statusType;

            StartCoroutine(SpecialStatusTimer(specialDuration));
        }
    }

    private IEnumerator SpecialStatusTimer(float duration)
    {
        int knockDistHolder = knockbackDist;
        float knockTimeHolder = knockbackTime;
        
        GetComponent<Animator>().speed = 0;
        knockbackTime = knockbackDist = 0;

        yield return new WaitForSeconds(duration);

        knockbackDist = knockDistHolder;
        knockbackTime = knockTimeHolder;
        GetComponent<Animator>().speed = 1;
        //specialStatus.RemoveAt(slotNum);
        specialStatus = SpecialStatus.None;
    }

    private IEnumerator Knocked(Vector2 direction, float knockbackMultiplier)
    {
        if(gameObject != null)
        {
            if(GetComponentInParent<PlayerMovement>())
            {
                transform.parent.GetComponent<Rigidbody2D>().AddForce(direction, ForceMode2D.Impulse);
            }
            else
            {
                GetComponent<Rigidbody2D>().AddForce(direction, ForceMode2D.Impulse);
            }

            if(GetComponent<EnemyScript>())
            {
                GetComponent<EnemyScript>().currentState = EnemyState.Hit;
            }
            else if (GetComponentInParent<PlayerMovement>())
            {
                GetComponentInParent<PlayerMovement>().currentState = PlayerState.Hit;
            }
            yield return new WaitForSeconds(knockbackTime * knockbackMultiplier);

            if (GetComponentInParent<PlayerMovement>())
            {
                transform.parent.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
            else
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }

            if (GetComponent<EnemyScript>())
            {
                GetComponent<EnemyScript>().currentState = EnemyState.Chasing;
            }
            else if (GetComponentInParent<PlayerMovement>())
            {
                GetComponentInParent<PlayerMovement>().currentState = PlayerState.Moving;
            }
        }
    }
}
