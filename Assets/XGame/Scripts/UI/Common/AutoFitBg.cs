using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace XGame
{
    public class AutoFitBg : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            GetComponent<RectTransform>().anchorMin = new Vector2(0, -(UIUtil.NotchHeight) / (Screen.height - UIUtil.NotchHeight));
            // - UIUtil.NotchHeight
            transform.localPosition = new Vector3(0, UIUtil.NotchHeight * 1920 / Screen.height / 2f, 0);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

