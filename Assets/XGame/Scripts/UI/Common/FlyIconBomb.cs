using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace XGame
{
    public class FlyIconBomb : MonoBehaviour
    {
        // Start is called before the first frame update
        void Awake()
        {
            var image = transform.Find<Image>("Image");
            var index = 1;
            this.StartTimer(0.03f, 9, () =>
            {
                image.sprite = ResourceUtil.GetSprite(ConstDefine.AtlasCommon, "fly_bomb" + index);
                index++;
            }, endCallBack: () =>
            {
                Destroy(gameObject);
            });
        }
    }
}


