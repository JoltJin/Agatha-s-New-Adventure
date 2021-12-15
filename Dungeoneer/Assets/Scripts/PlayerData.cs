using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    //basic info
    public static float currentMana = 100;
    public static int health = 12;

    //gear info
    public static List<Gear> itemInventory = new List<Gear>();
    public static List<Gear> weaponInventory = new List<Gear>();
    public static List<Gear> specialInventory = new List<Gear>();

    public static string displayItemName;


    //hud info
    public static List<Spell> learnedSpells = new List<Spell>();
    public static Element[] currentEle = new Element[2];
    public static Gear[] equippedGear = new Gear[3];
    public static Spell currentSpell;


}
