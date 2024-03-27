using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XGame
{
    public class UI_Permanent : UIView
    {
        public override UILayers Layer
        {
            get
            {
                return UILayers.BackgroundLayer;
            }
        }
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
