using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableObjects : MonoBehaviour
{
    [HideInInspector]
    public PlayerMovement player;
    [HideInInspector]
    public bool inUse = false;
    [HideInInspector]
    public bool finish = false;
    private bool inRange = false;

    private GameObject textBox;
    private Text text;
    public string textInput;

    public bool canInteract = true;
    public bool lockPlayer = true;

    public Signal showItem;

    // Start is called before the first frame update
    void Start()
    {
        textBox = GameObject.Find("TextBox");
        text = textBox.GetComponentInChildren<Text>();
        player = FindObjectOfType<PlayerMovement>();
        textBox.SetActive(false);
    }

    // Update is called once per frame
    public void Update()
    {
        if (Input.GetButtonDown("WeaponAttack") && inRange && player.direction == PlayerMovement.Direction.Up)
        {
            if (canInteract)
            {
                inUse = true;
            }
            else
            {
                HideTextBox();
            }

            if (lockPlayer)
            {
                //player.currentState = PlayerState.Interacting;
            }
            else
            {
                //player.currentState = PlayerState.Moving;
            }
        }
    }

    public void ShowHideTextBox()
    {
        textBox.SetActive(!textBox.activeInHierarchy);

        if (textBox.activeInHierarchy)
        {
            text.text = textInput;
        }
    }

    public void HideTextBox()
    {
        textBox.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            inRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            inRange = false;
        }
    }
}