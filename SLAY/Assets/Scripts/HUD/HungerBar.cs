using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XGame;

public class HungerBar : MonoBehaviour
{
    Slider slider;

    private void Awake()
    {
        this.RegisterEvent<HungerUpdatedEvent>(HungerUpdated);

        slider = GetComponent<Slider>();
        slider.minValue = 0;
    }

    private void OnDestroy()
    {
        this.UnRegisterEvent<HungerUpdatedEvent>();
    }

    void HungerUpdated(HungerUpdatedEvent e)
    {
        if (slider.maxValue != e.max)
        {
            slider.maxValue = e.max;
        }

        if (slider.value != e.value)
        {
            slider.value = e.value;
        }
    }
}
