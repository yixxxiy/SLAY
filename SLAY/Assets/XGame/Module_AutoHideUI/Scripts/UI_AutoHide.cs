using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XGame
{
    public class UI_AutoHide : UIView
    {
        public override UILayers Layer
        {
            get
            {
                return UILayers.TempBgLayer;
            }
        }

        public override bool IsSingle => true;

        public override void OnHide()
        {
        }

        public override void OnInit()
        {
        }

        public override void OnShow(object obj)
        {
        }
    }
}
