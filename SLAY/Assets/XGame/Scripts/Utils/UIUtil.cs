using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
namespace XGame
{
    public static class UIUtil
    {
       
        private static string MyEscapeURL(string url)
        {
            //%20是空格在url中的编码，这个方法将url中非法的字符转换成%20格式
            return UnityEngine.Networking.UnityWebRequest.EscapeURL(url).Replace("+", "%20");
        }


        public static int GetIndexByWeight(int[] weights)
        {
            var totalWeight = 0;
            foreach (var w in weights)
            {
                totalWeight += w;
            }
            var r = Random.Range(0, totalWeight);
            for (int i = 0; i < weights.Length; ++i)
            {
                r -= weights[i];
                if (r < 0)
                {
                    return i;
                }
            }
            return weights.Length - 1;
        }

        /// <summary>
        /// 设置位置
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="midInterval"></param>
        public static void SetMiddleAnchor(Transform leftIcon, UnityEngine.UI.Text rightText, float midInterval = 20, float leftPadding = 0f)
        {
            DelaySetAnchorAsync(leftIcon as RectTransform, rightText, midInterval, leftPadding);
        }
        private static async void DelaySetAnchorAsync(RectTransform left, UnityEngine.UI.Text right, float midInterval, float leftPadding)
        {
            await new WaitForEndOfFrame();
            if (!left.gameObject.activeSelf || !right.gameObject.activeSelf) midInterval = 0;
            float leftSizeX = left.gameObject.activeSelf ? left.sizeDelta.x * left.localScale.x : 0;
            float rightSizeX = right.gameObject.activeSelf ? right.preferredWidth : 0;
            if (right.GetComponent<RectTransform>().sizeDelta.x < right.preferredWidth) rightSizeX = right.GetComponent<RectTransform>().sizeDelta.x;
            float totalSizeX = leftSizeX + rightSizeX + midInterval;
            left.localPosition = new Vector3(-totalSizeX / 2 + leftSizeX / 2 + leftPadding, left.localPosition.y);
            right.transform.localPosition = new Vector3(totalSizeX / 2 - rightSizeX / 2 + leftPadding, right.transform.localPosition.y);

        }
        /// <summary>
        /// 禁止、启用点击事件
        /// </summary>
        /// <param name="disable"></param>
        public static void DisableTouch(bool disable)
        {
            MainController.UIMgr.UiCanvas.GetComponent<UnityEngine.UI.GraphicRaycaster>().enabled = !disable;
        }

       

     

        /// <summary>
        /// 根据奖励类型获取图片
        /// </summary>
        /// <param name="rewardType"></param>
        public static Sprite GetRewardImage(string rewardType)
        {         
            return ResourceUtil.GetSprite(ConstDefine.AtlasCommon,rewardType);
        }


     

        public static float NotchHeight
        {
            get
            {
                if (Screen.height > Screen.safeArea.height)
                {
                    return Screen.height - Screen.safeArea.height;
                }
                return AndroidUtil.NotchHeight();
            }

        }


        /// <summary>
        /// 初始化物体的相对位置、旋转、缩放
        /// </summary>
        /// <param name="trans"></param>
        public static void InitTransformLocal(this Transform trans)
        {
            trans.localPosition = Vector3.zero;
            trans.localScale = Vector3.one;
            trans.localRotation = Quaternion.identity;
        }
    }
}