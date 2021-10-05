using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem: MonoBehaviour
{
    public int health;
    public float damageCooldownAmount = .25f;
    private float damageCooldown = 0f;

    public Health dispHealth;
    
    public void Startup(int maxHp)
    {
        health = maxHp;

        if(dispHealth != null)
        {
            health = dispHealth.health;
        }
    }

    private void Update()
    {
        if(damageCooldown < 0)
        {
            damageCooldown = 0;
        }
        else if(damageCooldown <= damageCooldownAmount && damageCooldown > 0)
        {
            damageCooldown -= Time.deltaTime;
        }
    }
    public void Damage(int damage)
    {
        
        if (damageCooldown <= 0f)
        {
            health -= damage;
            damageCooldown = damageCooldownAmount;
            print(transform.name + " took " + damage + " damage\n" + health + " hp left");
        }
        
        if(health <= 0)
        {
            Destroy(gameObject);
        }
        
    }
}
