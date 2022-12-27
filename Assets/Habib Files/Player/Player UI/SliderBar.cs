using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderBar : MonoBehaviour
{
    private Slider slider;
    private List<Image> images = new List<Image>();


    private void Awake() {
        slider = this.GetComponent<Slider>();
        
        foreach (Image image in this.gameObject.GetComponentsInChildren<Image>()) {
            images.Add(image);
        }
        
    }

    public void SetMaxValue(float value) {
        slider.maxValue = value;
    }

    public void SetCurrentValue(float value) {
        slider.value = value;
    }

    public void ToggleHide(bool IsHide) {
        foreach (Image image in images) {
            image.enabled = IsHide;
        }
    }


}
