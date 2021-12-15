using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GearTypes
{
    Attack,
    Buff,
    Utlity,
}
public class GearType : MonoBehaviour
{
    public GearTypes gearType;
    
    [HideInInspector]
    public bool useItem = false, secondaryEffect;
}
