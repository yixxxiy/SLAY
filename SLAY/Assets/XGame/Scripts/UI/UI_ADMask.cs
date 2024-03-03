using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace XGame
{
    public class UI_ADMaskData
    {
        public float WaitTime;
        public System.Action Cb;
    }

    public class UI_ADMask : UIView
    {
        UI_ADMaskData mData;
        public override UILayers Layer
        {
            get
            {
                return UILayers.LoadingLayer;
            }
        }

        public override void OnInit()
        {

        }
        void Close()
        {
            this.CloseSelf();
            mData.Cb?.Invoke();
        }

        public override void OnShow(object msg)
        {
            mData = msg as UI_ADMaskData;
            if (mData.WaitTime > 0)
            {
                Invoke("Close", mData.WaitTime);
            }

        }
        public override void OnHide()
        {

        }

    }
}