using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerState
{
    Moving,
    Hit
}
public class PlayerMovement : MonoBehaviour
{
    private int layerMask;
    public float moveSpeed = 5f;

    [HideInInspector]
    public PlayerState currentState;

    private int currentWeapon = 0;
    public Transform weaponHand;



    public Rigidbody2D rb;

    [HideInInspector]
    public Direction direction = Direction.Down;

    private bool up, down, left, right;

    public Animator animator;

    //private bool touchingWall = false;

    private Vector2 lastPos, currentPos;

    public float rayLength = .5f;

    public LayerMask enemyLayers;
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
        if (transform.childCount < 2)
        {
            SwapGear();
        }
    }

    public void SwapGear()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name != "WeaponHand")
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
        for (int i = 0; i < GetComponentInParent<HudScript>().equippedGear.Length; i++)
        {
            if (GetComponentInParent<HudScript>().equippedGear[i] != Gear.Empty)
            {
                GameObject gear = (GameObject)Instantiate(Resources.Load("Gear/" + GetComponentInParent<HudScript>().equippedGear[i].ToString()), transform);
                gear.name = GetComponentInParent<HudScript>().equippedGear[i].ToString();
            }
        }
    }

    private void Controls()
    {
        AnimatorClipInfo[] animatorClipInfo = animator.GetCurrentAnimatorClipInfo(0);
        string clipName = animatorClipInfo[0].clip.name;

        if (Input.GetButtonDown("WeaponAttack") && !clipName.Contains("Attack"))
        {
            animator.SetTrigger("Attack");
            transform.GetChild(currentWeapon);
            transform.Find(GetComponent<HudScript>().equippedGear[0].ToString()).GetComponent<Animator>().SetTrigger("Attacking");
            //GetComponent<HudScript>().equippedGear[currentWeapon].GetComponent<Animator>().SetTrigger("Attacking");
            transform.Find(GetComponent<HudScript>().equippedGear[0].ToString()).GetComponent<Animator>().speed = animator.speed = 0;
            animator.SetFloat("Speed", 0f);
        }
        else
        {
            if (Input.GetButton("WeaponAttack"))
            {

                //GetComponent<HudScript>().weaponList[currentWeapon].SetActive(true);
                transform.Find(GetComponent<HudScript>().equippedGear[0].ToString()).transform.position = weaponHand.transform.position;

                switch (direction)
                {
                    case Direction.Right:
                        transform.Find(GetComponent<HudScript>().equippedGear[0].ToString()).GetComponent<Animator>().SetFloat("Horizontal", 1);
                        transform.Find(GetComponent<HudScript>().equippedGear[0].ToString()).GetComponent<Animator>().SetFloat("Vertical", 0);
                        break;
                    case Direction.Left:
                        transform.Find(GetComponent<HudScript>().equippedGear[0].ToString()).GetComponent<Animator>().SetFloat("Horizontal", -1);
                        transform.Find(GetComponent<HudScript>().equippedGear[0].ToString()).GetComponent<Animator>().SetFloat("Vertical", 0);
                        break;
                    case Direction.Down:
                        transform.Find(GetComponent<HudScript>().equippedGear[0].ToString()).GetComponent<Animator>().SetFloat("Horizontal", 0);
                        transform.Find(GetComponent<HudScript>().equippedGear[0].ToString()).GetComponent<Animator>().SetFloat("Vertical", -1);
                        break;
                    case Direction.Up:
                        transform.Find(GetComponent<HudScript>().equippedGear[0].ToString()).GetComponent<Animator>().SetFloat("Horizontal", 0);
                        transform.Find(GetComponent<HudScript>().equippedGear[0].ToString()).GetComponent<Animator>().SetFloat("Vertical", 1);
                        break;
                }


                movement = Vector2.zero;
                chargeTimer += Time.deltaTime;
            }
            else if (Input.GetButtonUp("WeaponAttack"))
            {
                transform.Find(GetComponent<HudScript>().equippedGear[0].ToString()).transform.position = weaponHand.transform.position;
                transform.Find(GetComponent<HudScript>().equippedGear[0].ToString()).GetComponent<Animator>().speed = animator.speed = 1;
            }
            else
            {
                Movement();
            }
        }

        if (Input.GetButtonDown("SwapElement") && !clipName.Contains("Attack"))
        {
            GetComponent<HudScript>().SwapElements();
        }
        if (Input.GetButtonDown("SwapWeapon") && !clipName.Contains("Attack"))
        {
            GetComponent<HudScript>().SwapWeapon();
        }
    }

    private void Attack()
    {
        transform.Find(GetComponent<HudScript>().equippedGear[0].ToString()).GetComponent<WeaponScript>().WeaponAttack(chargeTimer, enemyLayers);
        chargeTimer = 0f;
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

        if (clipName.Contains("Attack"))
        {
            transform.Find(GetComponent<HudScript>().equippedGear[0].ToString()).transform.position = weaponHand.transform.position;
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
        if (currentState != PlayerState.Hit && !PauseScript.isPaused)
        {
            Controls();

            if (transform.Find(GetComponent<HudScript>().equippedGear[0].ToString()).GetComponent<WeaponScript>().attackRange.gameObject.activeInHierarchy)
            {
                Attack();
            }
        }

    }

    private void FixedUpdate()
    {
        if (currentState != PlayerState.Hit && !PauseScript.isPaused)
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
}