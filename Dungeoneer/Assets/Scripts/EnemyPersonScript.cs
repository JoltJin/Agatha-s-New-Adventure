using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPersonScript : EnemyScript
{
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindWithTag("Player").transform.parent;
        body = GetComponent<Rigidbody2D>();
        homeBase = transform.position;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (currentState != EnemyState.Hit && GetComponent<HealthSystem>().specialStatus != SpecialStatus.Time_Freeze)
        {
            CheckDistance();
        }
    }

    private void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.gameObject.CompareTag("Player"))
        {
            if (trigger.GetComponentInParent<PlayerMovement>() && !trigger.GetComponentInParent<PlayerMovement>().isInvisible)
            {
                trigger.GetComponent<HealthSystem>().Damage(baseAttack, transform, 1f);
            }
        }
    }
    private void CheckDistance()
    {
        Vector2 moveTo;

        if (Vector2.Distance(target.position, transform.position) <= attackRange && !target.GetComponentInParent<PlayerMovement>().isInvisible)
        {
            currentState = EnemyState.Attacking;
        }
        else if (Vector2.Distance(target.position, transform.position) <= chaseRange && !target.GetComponentInParent<PlayerMovement>().isInvisible)
        {
            currentState = EnemyState.Chasing;
        }
        else if (Vector2.Distance(target.position, transform.position) > chaseRange || target.GetComponentInParent<PlayerMovement>().isInvisible)
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

        ChangeAnim(moveTo - (Vector2)transform.position);

        body.MovePosition(moveTo);
    }

    private void ChangeAnim(Vector2 direction)
    {
        if(direction != Vector2.zero)
        {
            anim.SetFloat("Speed", 1);
        }
        else
        {
            anim.SetFloat("Speed", 0);
        }
        direction = direction.normalized;
        anim.SetFloat("Horizontal", direction.x);
        anim.SetFloat("Vertical", direction.y);
    }
}