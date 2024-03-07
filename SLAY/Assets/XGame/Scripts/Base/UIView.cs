using System.Collections.Generic;
using UnityEngine;

namespace XGame
{
    /// <summary>
    /// UI面板以及UI的视图显示控制
    /// </summary>
    public abstract class UIView : MonoBehaviour
    {
        public virtual UILayers Layer
        {
            get
            {
                return mLayer;
            }
        }
        //是否只能存在一个
        public virtual bool IsSingle
        {
            get
            {
                return false;
            }
        }

        [Tooltip("UI的层级")]
        public UILayers mLayer = UILayers.DefaultLayer;

        /// <summary>
        /// 面板的初始化
        /// </summary>
        public abstract void OnInit();

        /// <summary>
        /// 面板展示方法
        /// </summary>
        /// <param name="msg">万能参数</param>
        public abstract void OnShow(object obj);

        /// <summary>
        /// 面板的隐藏调用方法
        /// </summary>
        public abstract void OnHide();

        public virtual void DoShowAnim(System.Action cb)
        {
            cb?.Invoke();
        }
        public virtual void DoHideAnim(System.Action cb)
        {
            cb?.Invoke();
        }

        public virtual void SetLanguage() { }

        //恢复展示
        public virtual void OnReShow() { }


        /// <summary>
        /// 关闭自己
        /// </summary>
        protected void CloseSelf(bool destroy = false)
        {
            UIManager.Instance.HideUIView(this, null, destroy);
        }
        /// <summary>
        /// 关闭自己
        /// </summary>
        protected void CloseSelfImmediate(bool destroy = false)
        {
            UIManager.Instance.HideUIViewImmediate(this, destroy);
        }
    }

    /// <summary>
    /// UI面板的层级
    /// </summary>
    public enum UILayers
    {
        BackgroundLayer = 0,
        DefaultLayer = 10,
        NormalLayer = 20,
        MainLayer = 30,
        MaskLayer = 40,
        PopupLayer = 50,
        RewardLayer = 60,
        PromptLayer = 70,
        LoadingLayer = 80,
    }

    public class DelayUI
    {
        public System.Type type;
        public System.Action showCallBack;
        public object msg;

        public DelayUI(System.Type type, System.Action showCallBack, object msg)
        {
            this.type = type;
            this.showCallBack = showCallBack;
            this.msg = msg;
        }
    }
}