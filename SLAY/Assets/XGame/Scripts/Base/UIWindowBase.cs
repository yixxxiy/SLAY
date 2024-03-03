using System.Collections.Generic;
using UnityEngine;

namespace XGame
{
    /// <summary>
    /// UI面板以及UI的视图显示控制
    /// </summary>
    public abstract class UIWindowBase : UIView
    {
        public override UILayers Layer
        {
            get
            {
                return UILayers.PopupLayer;
            }
        }

        /// <summary>
        /// 面板的初始化
        /// </summary>
        public override void OnInit() { }

        /// <summary>
        /// 面板展示方法
        /// </summary>
        /// <param name="msg">万能参数</param>
        public override void OnShow(object obj) { }

        /// <summary>
        /// 面板的隐藏调用方法
        /// </summary>
        public override void OnHide() { }

        public override void DoShowAnim(System.Action cb)
        {
            AnimationUtil.ShowWindowAnim(transform, cb);
        }
        public override void DoHideAnim(System.Action cb)
        {
            AnimationUtil.HideWindowAnim(transform, cb);
        }

        public override void SetLanguage() { }

        //恢复展示
        public override void OnReShow() { }
    }

}