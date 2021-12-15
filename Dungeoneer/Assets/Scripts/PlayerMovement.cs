using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerState
{
    Moving,
    Hit,
    Interacting,
    Flying
}
public class PlayerMovement : MonoBehaviour
{
    private HudScript playerInfo;
    private GameObject player;
    public BoxCollider2D playerHitbox;
    public Transform shadow;
    public SpriteRenderer heldItemSprite;
    private int layerMask;
    public float moveSpeed = 5f;
    public float currentSpeed = 5f;
    private bool floatDirection = false; // true float up false float down

    private float itemCharge = 0;

    //[HideInInspector]
    public PlayerState currentState;

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

    private Vector2 realPos;
    private Vector2 hoverPos;

    [HideInInspector]
    public bool isFlying = false, isInvisible = false;
    public bool isCasting = false;
    
    private bool canLand = true;
     private int slotnum = 0;

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    private Vector2 movement;

    private void Awake()
    {
        playerInfo = FindObjectOfType<HudScript>();
        player = transform.GetChild(0).gameObject;
        playerHitbox = player.GetComponent<BoxCollider2D>();
        layerMask = LayerMask.GetMask("Wall");
        shadow.gameObject.SetActive(false);
        if (transform.GetChild(0).childCount < 2)
        {
            SwapGear();
        }
    }

    private void Update()
    {
        if (!GameObject.Find("Pause Menu") && !isCasting)
        {
            CanLand();

            if (currentState != PlayerState.Hit && currentState != PlayerState.Interacting)
            {
                Controls();

                if (player.transform.Find(playerInfo.equippedGear[slotnum].ToString()) && player.transform.Find(playerInfo.equippedGear[slotnum].ToString()).GetComponent<WeaponScript>() && player.transform.Find(playerInfo.equippedGear[slotnum].ToString()).GetComponent<WeaponScript>().attackRange.gameObject.activeInHierarchy)
                {
                    Attack();
                }
            }
        }
        else if(isCasting)
        {
            animator.speed = 0;
            movement = Vector2.zero;
        }

        if (currentState == PlayerState.Flying)
        {
            Levitation();
            currentSpeed = moveSpeed * 1.25f;
        }
        else
        {
            if (isFlying)
            {
                EndLevitation();
            }
            currentSpeed = moveSpeed;
        }

        if (isInvisible && PlayerData.currentMana > 0)
        {
            PlayerData.currentMana -= Time.deltaTime * 7;
        }
        else if (isInvisible && PlayerData.currentMana <= 0)
        {
            Cloaked();
        }
    }

    private void FixedUpdate()
    {
        if (currentState != PlayerState.Hit && !PauseScript.isPaused)
        {
            rb.MovePosition(rb.position + movement * currentSpeed * Time.fixedDeltaTime);

            currentPos = realPos = rb.transform.position;
            hoverPos = new Vector2(realPos.x, realPos.y + 2);

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
    
    public void SwapGear()
    {
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            if (transform.GetChild(0).GetChild(i).name != "WeaponHand")
            {
                Destroy(transform.GetChild(0).GetChild(i).gameObject);
            }
        }
        for (int i = 0; i < playerInfo.equippedGear.Length; i++)
        {
            if (playerInfo.equippedGear[i] != Gear.Empty)
            {
                GameObject gear = (GameObject)Instantiate(Resources.Load("Prefabs/Gear/" + playerInfo.equippedGear[i].ToString()), transform.GetChild(0));
                gear.name = playerInfo.equippedGear[i].ToString();
            }
        }
    }

    public void HoldItem()
    {
        animator.SetBool("HoldItem", !animator.GetBool("HoldItem"));
        
        if (currentState != PlayerState.Interacting)
        {
            currentState = PlayerState.Interacting;

            Sprite[] sprites;

            sprites = Resources.LoadAll<Sprite>("Sprites/Menus/HUD/Gear Icons");

            for (int i = 0; i < sprites.Length; i++)
            {
                
                if ((PlayerData.displayItemName == "Tome" && sprites[i].name == "Tome Closed") || sprites[i].name == PlayerData.displayItemName)
                {
                    heldItemSprite.sprite = sprites[i];
                    break;
                }
            }
        }
        else
        {
            currentState = PlayerState.Moving;

            heldItemSprite.sprite = null;
            PlayerData.displayItemName = "";
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

        if (clipName.Contains("Attack"))
        {
            player.transform.Find(playerInfo.equippedGear[slotnum].ToString()).transform.position = weaponHand.transform.position;
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

        //if (upRay && upRay.collider.tag == "Wall" && up)
        //{
        //    animator.SetFloat("Direction", 0.25f);
        //}
        //else if (downRay && downRay.collider.tag == "Wall" && down)
        //{
        //    animator.SetFloat("Direction", 0.5f);
        //}
        //else if (leftRay && leftRay.collider.tag == "Wall" && left)
        //{
        //    animator.SetFloat("Direction", 0.75f);
        //}
        //else if (rightRay && rightRay.collider.tag == "Wall" && right)
        //{
        //    animator.SetFloat("Direction", 1f);
        //}
        bool canPass = false;
        if (isFlying)
        {
            canPass = true;
        }

        if (upRay && upRay.collider.tag == "Flyable")
        {
            Physics2D.IgnoreCollision(upRay.collider, GetComponent<Collider2D>(), canPass);
        }
        else if (downRay && downRay.collider.tag == "Flyable")
        {
            Physics2D.IgnoreCollision(downRay.collider, GetComponent<Collider2D>(), canPass);
        }
        else if (leftRay && leftRay.collider.tag == "Flyable")
        {
            Physics2D.IgnoreCollision(leftRay.collider, GetComponent<Collider2D>(), canPass);
        }
        else if (rightRay && rightRay.collider.tag == "Flyable")
        {
            Physics2D.IgnoreCollision(rightRay.collider, GetComponent<Collider2D>(), canPass);
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

    private void Controls()
    {
        if(playerInfo.equippedGear[slotnum] == Gear.Empty)
        {
            slotnum = 0;
        }
        AnimatorClipInfo[] animatorClipInfo = animator.GetCurrentAnimatorClipInfo(0);
        string clipName = animatorClipInfo[0].clip.name;

        AnimatorClipInfo[] weaponClipInfo = player.transform.Find(playerInfo.equippedGear[slotnum].ToString()).GetComponent<Animator>().GetCurrentAnimatorClipInfo(0);
        string weaponClip = animatorClipInfo[0].clip.name;
        if (Input.GetButtonDown("ItemOneAttack") || Input.GetButtonDown("ItemTwoAttack"))
        {

            if (Input.GetButtonDown("ItemOneAttack"))
            {
                slotnum = 1;
            }
            else
            {
                slotnum = 2;
            }

            if(playerInfo.equippedGear[slotnum] == Gear.Empty)
            {
                return;
            }

            if (currentState == PlayerState.Moving)
            {
                if (playerInfo.equippedGear[slotnum] == Gear.Broom)
                {
                    currentState = PlayerState.Flying;
                }
                else if (playerInfo.equippedGear[slotnum] == Gear.Cloak)
                {
                    if (!isInvisible && PlayerData.currentMana >= 5)
                    {
                        PlayerData.currentMana -= 5;
                        Cloaked();
                    }
                    else if (isInvisible)
                    {
                        Cloaked();
                    }
                }
                else
                {
                    //if (!player.transform.Find(playerInfo.equippedGear[slotnum].ToString()).GetComponent<GearType>().secondaryEffect)
                    //{
                    //    player.transform.Find(playerInfo.equippedGear[slotnum].ToString()).GetComponent<GearType>().useItem = true;
                    //    itemCharge = 0;
                    //}
                }
            }
            else if (playerInfo.equippedGear[slotnum] == Gear.Broom && currentState == PlayerState.Flying)
            {
                currentState = PlayerState.Moving;
            }
        }
        else if (Input.GetButton("ItemOneAttack") || Input.GetButton("ItemTwoAttack"))
        {
            itemCharge += Time.deltaTime;
        }
        else if (Input.GetButtonUp("ItemOneAttack") || Input.GetButtonUp("ItemTwoAttack"))
        {
            if (slotnum > 0)
            {
                ItemCharger();
            }

            if (!player.transform.Find(playerInfo.equippedGear[slotnum].ToString()).GetComponent<GearType>().secondaryEffect)
            {
                if (itemCharge > 0)
                {
                    player.transform.Find(playerInfo.equippedGear[slotnum].ToString()).GetComponent<GearType>().useItem = true;
                }
                itemCharge = 0;
            }
        }
        else if (Input.GetButtonDown("WeaponAttack"))
        {
            slotnum = 0;
        }
        
        //if you are using your weapon or a weapon item and you are not flying or being hit
        if ((Input.GetButtonDown("WeaponAttack") || (Input.GetButtonDown("ItemOneAttack") || Input.GetButtonDown("ItemTwoAttack")) && player.transform.Find(playerInfo.equippedGear[slotnum].ToString()).GetComponent<GearType>().gearType == GearTypes.Attack) && !clipName.Contains("Attack") && currentState != PlayerState.Flying && !isFlying)
        {
            animator.SetTrigger("Attack");
            player.transform.Find(playerInfo.equippedGear[slotnum].ToString()).GetComponent<Animator>().SetTrigger("Attacking");
            player.transform.Find(playerInfo.equippedGear[slotnum].ToString()).GetComponent<Animator>().speed = animator.speed = 0;
            animator.SetFloat("Speed", 0f);

            switch (direction)
            {
                case Direction.Right:
                    player.transform.Find(playerInfo.equippedGear[slotnum].ToString()).GetComponent<Animator>().SetFloat("Horizontal", 1);
                    player.transform.Find(playerInfo.equippedGear[slotnum].ToString()).GetComponent<Animator>().SetFloat("Vertical", 0);
                    break;
                case Direction.Left:
                    player.transform.Find(playerInfo.equippedGear[slotnum].ToString()).GetComponent<Animator>().SetFloat("Horizontal", -1);
                    player.transform.Find(playerInfo.equippedGear[slotnum].ToString()).GetComponent<Animator>().SetFloat("Vertical", 0);
                    break;
                case Direction.Down:
                    player.transform.Find(playerInfo.equippedGear[slotnum].ToString()).GetComponent<Animator>().SetFloat("Horizontal", 0);
                    player.transform.Find(playerInfo.equippedGear[slotnum].ToString()).GetComponent<Animator>().SetFloat("Vertical", -1);
                    break;
                case Direction.Up:
                    player.transform.Find(playerInfo.equippedGear[slotnum].ToString()).GetComponent<Animator>().SetFloat("Horizontal", 0);
                    player.transform.Find(playerInfo.equippedGear[slotnum].ToString()).GetComponent<Animator>().SetFloat("Vertical", 1);
                    break;
            }
        }
        else
        {
            if ((Input.GetButton("WeaponAttack") || (Input.GetButton("ItemOneAttack") || Input.GetButton("ItemTwoAttack")) && player.transform.Find(playerInfo.equippedGear[slotnum].ToString()).GetComponent<GearType>().gearType == GearTypes.Attack) && clipName.Contains("Attack") && currentState != PlayerState.Flying && !isFlying)
            {
                player.transform.Find(playerInfo.equippedGear[slotnum].ToString()).transform.position = weaponHand.transform.position;


                movement = Vector2.zero;
                chargeTimer += Time.deltaTime;
                
            }
            else if ((Input.GetButtonUp("WeaponAttack") || (Input.GetButtonUp("ItemOneAttack") || Input.GetButtonUp("ItemTwoAttack")) && player.transform.Find(playerInfo.equippedGear[slotnum].ToString()).GetComponent<GearType>().gearType == GearTypes.Attack) && currentState != PlayerState.Flying && !isFlying)
            {
                player.transform.Find(playerInfo.equippedGear[slotnum].ToString()).transform.position = weaponHand.transform.position;
                player.transform.Find(playerInfo.equippedGear[slotnum].ToString()).GetComponent<Animator>().speed = animator.speed = 1;
                
            }
            else
            {
                if (weaponClip.Contains("Attack"))
                {
                    player.transform.Find(playerInfo.equippedGear[slotnum].ToString()).GetComponent<Animator>().speed = animator.speed = 1;
                }
                else
                {
                    chargeTimer = 0;
                }

                Movement();

            }
        }

        if (Input.GetButtonDown("SwapElement") && !clipName.Contains("Attack"))
        {
            playerInfo.SwapElements();
        }
        //if (Input.GetButtonDown("SwapWeapon") && !clipName.Contains("Attack"))
        //{
        //    playerInfo.SwapWeapon();
        //}
    }

    private void Attack()
    {
        player.transform.Find(playerInfo.equippedGear[slotnum].ToString()).GetComponent<WeaponScript>().WeaponAttack(chargeTimer, enemyLayers);
        chargeTimer = 0f;
    }

    private void ItemCharger()
    {
        if(itemCharge > 0.5f)
        {
            isCasting = true;
            player.transform.Find(playerInfo.equippedGear[slotnum].ToString()).GetComponent<GearType>().secondaryEffect = true;
            itemCharge = 0;
        }
    }
    
    public void Cloaked()
    {
        Color transparent = Color.white, visible = Color.white;
        transparent.a = 0.5f;
        visible.a = 1;

        shadow.gameObject.SetActive(!shadow.gameObject.activeInHierarchy);
        isInvisible = !isInvisible;

        playerHitbox.enabled = !playerHitbox.enabled;

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), isInvisible);
        if (isInvisible)
        {
            player.GetComponent<SpriteRenderer>().color = transparent;
        }
        else
        {
            player.GetComponent<SpriteRenderer>().color = visible;
        }

    }

    private void Levitation()
    {
        if (!isFlying)
        {
            animator.speed = 1;
            animator.SetBool("Flying", true);
            player.transform.Find(playerInfo.equippedGear[slotnum].ToString()).GetComponent<Animator>().SetBool("IsFlying", true);
            shadow.gameObject.SetActive(true);
            isFlying = true;
        }
        player.transform.Find(playerInfo.equippedGear[slotnum].ToString()).transform.position = weaponHand.transform.position;

        if (currentState == PlayerState.Flying)
        {
            if (player.transform.localPosition.y <= 1 || player.transform.localPosition.y <= .5f)
            {
                floatDirection = true;
            }
            else if (player.transform.localPosition.y >= 1.5f)
            {
                floatDirection = false;
            }

            if (floatDirection)
            {
                if (player.transform.localPosition.y <= 1)
                {
                    player.transform.localPosition = new Vector2(0, player.transform.localPosition.y + (Time.deltaTime * 1.25f));
                }
                else
                {
                    player.transform.localPosition = new Vector2(0, player.transform.localPosition.y + (Time.deltaTime / 2));
                }
            }
            else
            {
                player.transform.localPosition = new Vector2(0, player.transform.localPosition.y - (Time.deltaTime / 2));
            }
        }
    }

    private void EndLevitation()
    {

        if (player.transform.localPosition.y <= 0.5f && !canLand)
        {
            currentState = PlayerState.Flying;
            Levitation();
        }
        else if (player.transform.localPosition.y > 0)
        {
            player.transform.localPosition = new Vector2(0, player.transform.localPosition.y - (Time.deltaTime * 2));
        }
        else if (player.transform.localPosition.y <= 0)
        {
            player.transform.localPosition = Vector2.zero;
            isFlying = false;
            animator.SetBool("Flying", false);
            player.transform.Find(Gear.Broom.ToString()).GetComponent<Animator>().SetBool("IsFlying", false);
            shadow.gameObject.SetActive(false);
        }
        player.transform.Find(Gear.Broom.ToString()).transform.position = weaponHand.transform.position;
    }

    private void CanLand()
    {
        Collider2D[] objects = new Collider2D[4];
        Physics2D.OverlapBoxNonAlloc(transform.position, GetComponent<BoxCollider2D>().bounds.size, 0f, objects);

        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] != null && objects[i].tag == "Flyable")
            {
                canLand = false;
                break;
            }
            else
            {
                canLand = true;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag == "Flyable" && currentState == PlayerState.Flying)
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }
}