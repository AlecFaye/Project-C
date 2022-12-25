using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void SetMaxValue(float value) {
        slider.maxValue = value;
    }

    public void SetCurrentValue(float value) {
        slider.value = value;
    }

    public void Hide(bool hide) {
        foreach (Image image in this.gameObject.GetComponentsInChildren<Image>()) {
            image.enabled = hide;
        }
    }


}
