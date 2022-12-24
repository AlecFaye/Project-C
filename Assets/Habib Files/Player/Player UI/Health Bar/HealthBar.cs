using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void SetMaxValue(float health) {
        slider.maxValue = health;
    }

    public void SetCurrentValue(float health) {
        slider.value = health;
    }


}
