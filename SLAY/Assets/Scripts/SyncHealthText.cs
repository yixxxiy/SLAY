using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SyncHealthText : MonoBehaviour
{
    TextMeshProUGUI text;
    public PalScript palScript;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        var newText = "HP: " + palScript.pal.health.ToString();
        if (text.text != newText)
        {
            text.text = newText;
        }
    }
}
