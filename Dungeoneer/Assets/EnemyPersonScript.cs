using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPersonScript : EnemyScript
{
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
        body = GetComponent<Rigidbody2D>();
        homeBase = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (currentState != EnemyState.Hit)
        {
            CheckDistance();
        }
    }

    private void OnTriggerEnter2D(Collider2D trigger)
    {
        if(trigger.gameObject.CompareTag("Player"))
        {
            trigger.GetComponent<HudScript>().Damage(baseAttack, transform);
        }
    }
    private void CheckDistance()
    {
        Vector2 moveTo;


        if (Vector2.Distance(target.position, transform.position) <= attackRange)
        {
            currentState = EnemyState.Attacking;
        }
        else if (Vector2.Distance(target.position, transform.position) <= chaseRange)
        {
            currentState = EnemyState.Chasing;
        }
        else if (Vector2.Distance(target.position, transform.position) > chaseRange)
        {
            currentState = EnemyState.Idle;
        }

        if (currentState == EnemyState.Chasing)
        {
            moveTo = Vector2.MoveTowards(body.position, target.position, movementSpeed * Time.fixedDeltaTime);
        }
        else if (currentState == EnemyState.Attacking)
        {
            moveTo = transform.position;
        }
        else if(currentState == EnemyState.Idle)
        {
            moveTo = Vector2.MoveTowards(transform.position, homeBase, movementSpeed * Time.deltaTime);
        }
        else
        {
            moveTo = transform.position;
        }
        body.MovePosition(moveTo);
    }
}