using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Idle,
    Chasing,
    Attacking,
    Hit
}

public class EnemyScript: MonoBehaviour
{
    [HideInInspector]
    public Transform target;
    [HideInInspector]
    public Vector2 homeBase;
    [HideInInspector]
    public Rigidbody2D body;
    public int baseAttack;
    public float movementSpeed;
    public float attackRange, chaseRange;

    [HideInInspector]
    public EnemyState currentState;

    [HideInInspector]
    public Animator anim;
}
