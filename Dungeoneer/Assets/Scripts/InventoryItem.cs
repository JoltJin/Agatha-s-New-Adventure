using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public Gear currentGear;


    private void Start()
    {
        UpdateItem();
    }

    public void UpdateItem()
    {
        Color invisible = Color.white, opaque = Color.white;
        invisible.a = 0;
        opaque.a = 1;

        Sprite[] sprites;

        sprites = Resources.LoadAll<Sprite>("Sprites/Menus/HUD/Gear Icons");

        if (currentGear == Gear.Empty)
        {
            GetComponent<Image>().color = invisible;
        }
        else
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                if ((currentGear == Gear.Tome && sprites[i].name == "Tome Closed") || sprites[i].name == currentGear.ToString())
                {
                    GetComponent<Image>().color = opaque;
                    GetComponent<Image>().sprite = sprites[i];
                    GetComponent<Image>().SetNativeSize();
                    break;
                }
            }
        }
    }
}