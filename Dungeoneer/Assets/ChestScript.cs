using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestScript : InteractableObjects
{
    public Item item;

    public Animator animator;
    
    new private void Update()
    {
        base.Update();

        if (inUse)
        {
            textInput = item.itemDesc;

            ShowHideTextBox();
            inUse = false;
            
            if(!finish)
            {
                //canInteract = false;
                //lockPlayer = false;

                if (item.equipmentType == EquipmentType.Item)
                {
                    PlayerData.itemInventory.Add((Gear)System.Enum.Parse(typeof(Gear), item.itemName));
                }
                else if (item.equipmentType == EquipmentType.Special)
                {
                    PlayerData.specialInventory.Add((Gear)System.Enum.Parse(typeof(Gear), item.itemName));
                }
                else if (item.equipmentType == EquipmentType.Weapon)
                {
                    PlayerData.weaponInventory.Add((Gear)System.Enum.Parse(typeof(Gear), item.itemName));
                }
                PlayerData.displayItemName = item.itemName;

                showItem.Raise();

                animator.SetBool("Open", true);

                finish = true;
            }
            else
            {
                showItem.Raise();
                finish = false;
                canInteract = false;
                lockPlayer = false;
            }
        }
    }
}