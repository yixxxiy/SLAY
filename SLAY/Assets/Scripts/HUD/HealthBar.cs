using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    PlayerScript playerScript;
    Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerScript>();
        slider = GetComponent<Slider>();
        slider.minValue = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (slider.maxValue != playerScript.maxHealth)
        {
            slider.maxValue = playerScript.maxHealth;
        }

        if (slider.value != playerScript.health)
        {
            slider.value = playerScript.health;
        }
    }
}
