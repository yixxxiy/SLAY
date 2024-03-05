using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HungerBar : MonoBehaviour
{
    public float showDuration = 3.0f;

    PlayerScript playerScript;
    Slider slider;
    CanvasGroup canvasGroup;
    bool show = true;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerScript>();

        slider = GetComponent<Slider>();
        slider.minValue = 0;

        canvasGroup = GetComponent<CanvasGroup>();

        Show();
    }

    // Update is called once per frame
    void Update()
    {
        if (slider.maxValue != playerScript.maxHunger)
        {
            Show();
            slider.maxValue = playerScript.maxHunger;
        }

        if (slider.value != playerScript.hunger)
        {
            Show();
            slider.value = playerScript.hunger;
        }

        if (show && canvasGroup.alpha != 1)
        {
            canvasGroup.alpha = 1;
        }
        if (!show &&  canvasGroup.alpha != 0)
        {
            canvasGroup.alpha = 0;
        }
    }

    void Show()
    {
        show = true;
        CancelInvoke("Hide");
        Invoke("Hide", showDuration);
    }

    void Hide()
    {
        show = false;
    }
}
