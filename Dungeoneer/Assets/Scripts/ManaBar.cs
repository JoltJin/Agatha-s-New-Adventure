using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    public Slider slider;

    public void SetMana(int maxMana)
    {
        slider.maxValue = slider.value = maxMana;
    }

    public void UpdateMana(float mana)
    {
        slider.value = mana;
    }
}
