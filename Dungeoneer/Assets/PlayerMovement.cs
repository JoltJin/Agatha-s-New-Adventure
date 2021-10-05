using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private int layerMask;
    public float moveSpeed = 5f;

    public Rigidbody2D rb;

    public Direction direction = Direction.Down;

    private bool up, down, left, right;

    public Animator animator;

    //private bool touchingWall = false;

    private Vector2 lastPos, currentPos;

    public float rayLength = .5f;

    public LayerMask enemyLayers;
    public Transform attackRange;
    public float attackSize = .5f;
    [SerializeField]
    private float charge1 = .25f, charge2 = .5f, charge3 = .75f;
    private float chargeTimer = 0f;

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    private Vector2 movement;

    private void Start()
    {
        layerMask = LayerMask.GetMask("Wall");

        gameObject.GetComponent<HealthSystem>().Startup(0);
    }

    private void Controls()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            animator.SetTrigger("Attack");
        }
        else
        {
            if (Input.GetButton("Fire1"))
            {
                animator.speed = 0;
                animator.SetFloat("Speed", 0f);
                movement = Vector2.zero;
                chargeTimer += Time.deltaTime;
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                animator.speed = 1;
            }
            else
            {
                Movement();
            }
        }
    }

    private void Attack()
    {
        int damageMod;
        if(chargeTimer <  charge1)
        {
            damageMod = 1;
        }
        else if(chargeTimer >= charge1 && chargeTimer < charge2)
        {
            damageMod = 2;
        }
        else if(chargeTimer >= charge2 && chargeTimer < charge3)
        {
            damageMod = 3;
        }
        else
        {
            damageMod = 5;
        }
        chargeTimer = 0f;
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackRange.position, attackSize, enemyLayers);

        foreach(Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<HealthSystem>().Damage(1 * damageMod);
        }
    }

    private void Movement()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        AnimatorClipInfo[] animatorClipInfo = animator.GetCurrentAnimatorClipInfo(0);
        string clipName = animatorClipInfo[0].clip.name;

        //needs to only happen when not playing attacking
        if (movement == Vector2.zero && clipName.Contains("Walk"))
        {
            animator.speed = 0;
        }
        else
        {
            animator.speed = 1;
        }

        if(clipName.Contains("Attack"))
        {
            movement = Vector2.zero;
            animator.SetFloat("Speed", 0f);
        }
        else
        {
            animator.SetFloat("Speed", movement.sqrMagnitude);
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
        }

        RaycastHit2D upRay = Physics2D.Raycast(transform.position, Vector2.up, rayLength, layerMask);
        RaycastHit2D downRay = Physics2D.Raycast(transform.position, Vector2.down, rayLength, layerMask);
        RaycastHit2D leftRay = Physics2D.Raycast(transform.position, Vector2.left, rayLength, layerMask);
        RaycastHit2D rightRay = Physics2D.Raycast(transform.position, Vector2.right, rayLength, layerMask);

        //Debug.DrawRay(transform.position, Vector2.up * rayLength, Color.red);
        //Debug.DrawRay(transform.position, Vector2.down * rayLength, Color.red);
        //Debug.DrawRay(transform.position, Vector2.left * rayLength, Color.red);
        //Debug.DrawRay(transform.position, Vector2.right * rayLength, Color.red);

        if (upRay && upRay.collider.tag == "Wall" && up)
        {
            animator.SetFloat("Direction", 0.25f);
        }
        else if (downRay && downRay.collider.tag == "Wall" && down)
        {
            animator.SetFloat("Direction", 0.5f);
        }
        else if (leftRay && leftRay.collider.tag == "Wall" && left)
        {
            animator.SetFloat("Direction", 0.75f);
        }
        else if (rightRay && rightRay.collider.tag == "Wall" && right)
        {
            animator.SetFloat("Direction", 1f);
        }

        if (movement.x == -1)
        {
            direction = Direction.Left;
            left = true;
            right = false;
        }
        else if (movement.x == 1)
        {
            direction = Direction.Right;
            left = false;
            right = true;
        }
        else
        {
            left = right = false;
        }

        if (movement.y == 1)
        {
            direction = Direction.Up;
            up = true;
            down = false;
        }
        else if (movement.y == -1)
        {
            direction = Direction.Down;
            up = false;
            down = true;
        }
        else
        {
            up = down = false;
        }

        if (animator.GetFloat("Speed") == 0 && !clipName.Contains("Attack"))
        {
            if (direction == Direction.Up)
            {
                animator.SetFloat("Vertical", 1);
            }
            else if (direction == Direction.Down)
            {
                animator.SetFloat("Vertical", -1);
            }
            else if (direction == Direction.Left)
            {
                animator.SetFloat("Horizontal", -1);
            }
            else if (direction == Direction.Right)
            {
                animator.SetFloat("Horizontal", 1);
            }
        }
    }

    void Update()
    {
        Controls();

        if (attackRange.gameObject.activeInHierarchy)
        {
            Attack();
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        currentPos = rb.transform.position;

        if (movement.x == -1 || movement.x == 1 || movement.y == -1 || movement.y == 1)
        {
            if (currentPos.x == lastPos.x && movement.x != 0 || currentPos.y == lastPos.y && movement.y != 0)
            {
                animator.SetBool("Wall", true);
                animator.SetFloat("Speed", 1);
            }
            else
            {
                animator.SetBool("Wall", false);
            }
        }
        else
        {
            animator.SetBool("Wall", false);
        }

        lastPos = currentPos;
    }
}
