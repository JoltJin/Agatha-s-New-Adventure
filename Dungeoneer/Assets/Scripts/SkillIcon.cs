using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillIcon : MonoBehaviour
{
    public Spell currentSpell;


    private void Start()
    {
        UpdateSpell();
    }

    public void UpdateSpell()
    {
        Color invisible = Color.white, opaque = Color.white;
        invisible.a = 0;
        opaque.a = 1;

        Sprite[] sprites;

        sprites = Resources.LoadAll<Sprite>("Sprites/Menus/HUD/Spell Icons");

        if (currentSpell == Spell.None)
        {
            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Menus/HUD/Tome Page");
        }
        else
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                if (sprites[i].name == currentSpell.ToString())
                {
                    GetComponent<SpriteRenderer>().color = opaque;
                    GetComponent<SpriteRenderer>().sprite = sprites[i];
                    break;
                }
            }
        }
    }
}