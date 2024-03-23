using System.Collections.Generic;
using UnityEngine;

namespace XGame
{
    /// <summary>
    /// UI管理者，管理着所有UI面板
    /// </summary>
    public class UIManager : Singleton<UIManager>
    {
        public Transform UiCanvas { get; private set; }

        public Transform GetLayerRoot(UILayers layer)
        {
            return mLayerParents[layer];
        }

        /// <summary>
        /// 场景切换后需要设置新场景的Canvas，为UI的父物体
        /// </summary>
        /// <param name="uiCanvas"></param>
        public void Init(Transform uiCanvas)
        {
            UiCanvas = uiCanvas;

            RectTransform Root = new GameObject("BackgroundLayerParent", typeof(RectTransform)).transform as RectTransform;
            mLayerParents[UILayers.BackgroundLayer] = Root.transform;
            Root = new GameObject("TempBgLayerParent", typeof(RectTransform)).transform as RectTransform;
            mLayerParents[UILayers.TempBgLayer] = Root.transform;
            Root = new GameObject("DefaultLayerParent", typeof(RectTransform)).transform as RectTransform;
            mLayerParents[UILayers.DefaultLayer] = Root.transform;
            Root = new GameObject("NormalLayerParent", typeof(RectTransform)).transform as RectTransform;
            mLayerParents[UILayers.NormalLayer] = Root.transform;
            Root = new GameObject("MainLayerParent", typeof(RectTransform)).transform as RectTransform;
            mLayerParents[UILayers.MainLayer] = Root.transform;
            Root = new GameObject("MaskLayerParent", typeof(RectTransform)).transform as RectTransform;
            mLayerParents[UILayers.MaskLayer] = Root.transform;
            Root = new GameObject("PopupLayerParent", typeof(RectTransform)).transform as RectTransform;
            mLayerParents[UILayers.PopupLayer] = Root.transform;
            Root = new GameObject("RewardLayerParent", typeof(RectTransform)).transform as RectTransform;
            mLayerParents[UILayers.RewardLayer] = Root.transform;
            Root = new GameObject("PromptLayerParent", typeof(RectTransform)).transform as RectTransform;
            mLayerParents[UILayers.PromptLayer] = Root.transform;
            Root = new GameObject("LoadingLayerParent", typeof(RectTransform)).transform as RectTransform;
            mLayerParents[UILayers.LoadingLayer] = Root.transform;

            foreach (var layer in mLayerParents.Values)
            {
                var root = layer.GetComponent<RectTransform>();
                root.SetParent(uiCanvas);
                root.localScale = Vector3.one;
                root.anchorMin = new Vector2(0, UIUtil.NotchHeight / Screen.height);
                root.anchorMax = Vector2.one;
                root.sizeDelta = Vector2.zero;
                root.localPosition = new Vector3(0, -UIUtil.NotchHeight * 1920 / Screen.height / 2f, 0);
            }

            //初始化飞金币动画管理器
            FlyRewardManager.Instance.Init(mLayerParents[UILayers.RewardLayer], mLayerParents[UILayers.NormalLayer]);
        }
        //所有面板的父物体
        private Dictionary<UILayers, Transform> mLayerParents = new Dictionary<UILayers, Transform>();

        private Dictionary<UILayers, Stack<UIView>> mCurLayers = new Dictionary<UILayers, Stack<UIView>>();

        private Dictionary<string, UIView> mUiViewDic = new Dictionary<string, UIView>();
        private Dictionary<string, UIView> mPreUiViewDic = new Dictionary<string, UIView>();
        //等待展示的窗口（只用在Pop中）
        private Queue<DelayUI> mDelayUIs = new Queue<DelayUI>();

        System.Type GetTypeByName(string typeName)
        {
            System.Type type = null;
            System.Reflection.Assembly[] assemblyArray = System.AppDomain.CurrentDomain.GetAssemblies();
            int assemblyArrayLength = assemblyArray.Length;
            for (int i = 0; i < assemblyArrayLength; ++i)
            {
                type = assemblyArray[i].GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }

            for (int i = 0; (i < assemblyArrayLength); ++i)
            {
                System.Type[] typeArray = assemblyArray[i].GetTypes();
                int typeArrayLength = typeArray.Length;
                for (int j = 0; j < typeArrayLength; ++j)
                {
                    if (typeArray[j].Name.Equals(typeName))
                    {
                        return typeArray[j];
                    }
                }
            }
            if (type == null)
            {
                LogUtil.Err("未找到类型：" + typeName);
            }
            return type;
        }

        void PreLoadUIPanel(System.Type type)
        {
            if (type == null) return;
            var prefabName = type.Name;
            GameObject UIPrefab = ResourceUtil.Instantiate<GameObject>("Prefabs/UIPanelPrefabs/" + prefabName);
            if (UIPrefab == null)
            {
                LogUtil.Err("未找到预设：Prefabs/UIPanelPrefabs/" + prefabName);
                return;
            }

            UIView uiView = UIPrefab.GetComponent(type) as UIView;
            if (uiView == null) uiView = UIPrefab.AddComponent(type) as UIView;
            Transform uiType = mLayerParents[uiView.Layer];
            if (uiType == null)
            {
                LogUtil.Err("将UICanvas脚本挂载到Canvas下");
                return;
            }
            uiView.transform.SetParent(uiType);
            RectTransform rt = uiView.transform as RectTransform;
            rt.localPosition = Vector3.zero;
            rt.anchoredPosition = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.offsetMin = Vector2.zero;
            UIPrefab.name = prefabName;
            UIPrefab.transform.localScale = Vector3.one;
            mPreUiViewDic.Add(type.Name, uiView);
            uiView.gameObject.SetActive(false);
        }

        /// <summary>
        /// 实例化出UI面板
        /// </summary>
        /// <param name="prefabName">UI面板名字</param>
        /// <returns>实例化出的面板引用</returns>
        private UIView InstantiateUIPanel(System.Type type)
        {
            var prefabName = type.Name;
            GameObject UIPrefab = ResourceUtil.Instantiate<GameObject>("Prefabs/UIPanelPrefabs/" + prefabName);
            if (UIPrefab == null) return null;

            Transform uiType = null;
            UIView uiView = UIPrefab.GetComponent(type) as UIView;
            if (uiView == null) uiView = UIPrefab.AddComponent(type) as UIView;
            uiView.OnInit();
            uiView.SetLanguage();
            UILayers uiLayer = uiView.Layer;
            uiType = mLayerParents[uiLayer];
            if (uiType == null)
            {
                LogUtil.Err("将UICanvas脚本挂载到Canvas下");
                return null;
            }


            UIPrefab.transform.SetParent(uiType);
            UIPrefab.name = prefabName;
            UIPrefab.transform.localScale = Vector3.one;
            RectTransform rt = UIPrefab.transform as RectTransform;
            rt.localPosition = Vector3.zero;
            rt.anchoredPosition = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.offsetMin = Vector2.zero;
            return uiView;
        }
        /// <summary>
        /// 显示面板
        /// </summary>
        /// <param name="name"></param>
        /// <param name="showCallBack"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public T ShowUIView<T>(object msg = null, System.Action showCallBack = null, bool delay = true) where T : UIView
        {
            return ShowUIView(typeof(T), msg, showCallBack, delay) as T;
        }
        
        
        /// <summary>
        /// 用于检测type类型的UI是否显示中，仅适用于单例对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool isShowing<T>()
        {
            string key = typeof(T).Name;
            UIView uiView = null;
            if (mUiViewDic.ContainsKey(key))
            {
                uiView = mUiViewDic[key];
                return uiView.gameObject.activeSelf;
            }
            return false;
        }
        
        public UIView ShowUIView(System.Type type, object msg = null, System.Action showCallBack = null, bool delay = true)
        {
            UIView uiView = null;
            var key = type.Name;
            if (!mUiViewDic.TryGetValue(key, out uiView))
            {
                if (mPreUiViewDic.ContainsKey(key))
                {
                    uiView = mPreUiViewDic[key];
                    uiView.OnInit();
                    uiView.SetLanguage();
                }
                else
                {
                    uiView = InstantiateUIPanel(type);
                    if (uiView == null)
                    {
                        LogUtil.Err("加载失败" + key);
                        return null;
                    }
                }
                mUiViewDic.Add(key, uiView);
                uiView.gameObject.SetActive(false);
            }

            if (uiView.IsSingle)
            {
                if (mCurLayers.ContainsKey(uiView.Layer) && mCurLayers[uiView.Layer].Count > 0 && mCurLayers[uiView.Layer].Peek() == uiView) return null;
                else
                {
                    foreach (var ui in mDelayUIs)
                    {
                        if (ui.type == uiView.GetType())
                        {
                            return null;
                        }
                    }
                }
            }



            if (delay && uiView.Layer == UILayers.PopupLayer)
            {
                if (mDelayUIs.Count > 0)
                {
                    mDelayUIs.Enqueue(new DelayUI(type, showCallBack, msg));
                    return null;
                }
                else
                {
                    foreach (KeyValuePair<string, UIView> kvp in mUiViewDic)
                    {
                        if (uiView.Layer == UILayers.PopupLayer && uiView.Layer == kvp.Value.Layer && kvp.Value.gameObject.activeInHierarchy)
                        {
                            mDelayUIs.Enqueue(new DelayUI(type, showCallBack, msg));

                            return null;
                        }
                    }
                }
            }

            //设置当前弹板
            if (!mCurLayers.ContainsKey(uiView.Layer)) mCurLayers[uiView.Layer] = new Stack<UIView>();

            if (mCurLayers[uiView.Layer].Count > 0)
            {
                var curView = mCurLayers[uiView.Layer].Peek();
                curView.gameObject.SetActive(false);
            }
            mCurLayers[uiView.Layer].Push(uiView);

            uiView.gameObject.SetActive(true);
            uiView.OnShow(msg);
            uiView.DoShowAnim(() =>
            {
                // if (uiView.Layer != UILayers.MainLayer) ADUtil.ShowBanner("展示banner:" + key);
                // else ADUtil.HideBanner();
                showCallBack?.Invoke();
            });

            return uiView;
        }
        public void HideUIView<T>(System.Action hideCallBack = null) where T : UIView
        {
            HideUIView(typeof(T), hideCallBack);
        }
        public void HideUIView(System.Type type, System.Action hideCallBack = null)
        {
            HideUIView(type.Name, hideCallBack);
        }
        public void HideUIViewImmediate(UIView uiView, bool destroy = false)
        {

            if (mCurLayers[uiView.Layer].Count > 0)
            {
                mCurLayers[uiView.Layer].Pop();
            }
            bool isSameUI = false;
            if (mCurLayers[uiView.Layer].Count > 0)
            {

                var layer = mCurLayers[uiView.Layer].Peek();
                layer.gameObject.SetActive(true);
                layer.OnReShow();
                if (uiView.name == layer.name)
                {
                    AnimationUtil.ShowWindowAnim(layer.transform, null);
                    isSameUI = true;
                }
            }
            else if (uiView.Layer == UILayers.PopupLayer && mDelayUIs.Count > 0)
            {
                var panel = mDelayUIs.Dequeue();
                ShowUIView(panel.type, panel.msg, delay: false);

                if (uiView.name == panel.type.Name) isSameUI = true;
            }
            uiView.OnHide();

            if (isSameUI) return;
            uiView.gameObject.SetActive(false);



            if (destroy)
            {
                RemoveUI(uiView);
                GameObject.Destroy(uiView.gameObject);
            }

        }

        /// <summary>
        /// 隐藏面板
        /// </summary>
        /// <param name="uiView"></param>
        /// <param name="hideCallBack"></param>
        public void HideUIView(string uiView, System.Action hideCallBack = null)
        {
            HideUIView(FindView(uiView), hideCallBack);
        }

        /// <summary>
        /// 隐藏面板
        /// </summary>
        /// <param name="uiView"></param>
        /// <param name="hideCallBack"></param>
        public void HideUIView(UIView uiView, System.Action hideCallBack = null, bool destroy = false)
        {
            if (mCurLayers[uiView.Layer].Count > 0)
            {
                mCurLayers[uiView.Layer].Pop();
            }
            uiView.DoHideAnim(() =>
            {
                hideCallBack?.Invoke();
                bool isSameUI = false;
                if (mCurLayers[uiView.Layer].Count > 0)
                {

                    var layer = mCurLayers[uiView.Layer].Peek();

                    layer.gameObject.SetActive(true);
                    layer.OnReShow();
                    if (uiView.name == layer.name)
                    {
                        AnimationUtil.ShowWindowAnim(layer.transform, null);
                        isSameUI = true;
                    }

                }
                else if (uiView.Layer == UILayers.PopupLayer && mDelayUIs.Count > 0)
                {
                    var panel = mDelayUIs.Dequeue();
                    ShowUIView(panel.type, panel.msg, delay: false);

                    if (uiView.name == panel.type.Name) isSameUI = true;

                }

                uiView.OnHide();

                if (isSameUI) return;

                uiView.gameObject.SetActive(false);

                if (destroy)
                {
                    RemoveUI(uiView);
                    GameObject.Destroy(uiView.gameObject);
                }
            });

        }

        public UIView FindView(string name)
        {
            UIView uiView = null;
            if (!mUiViewDic.TryGetValue(name, out uiView))
            {
                LogUtil.Err("输入错误没有找到" + name);
                return null;
            }
            return uiView;
        }

        public void Clear()
        {
            mUiViewDic.Clear();
            mCurLayers.Clear();
            mPreUiViewDic.Clear();
            mLayerParents.Clear();
            mDelayUIs.Clear();
        }

        /// <summary>
        /// 该层级是否有正在显示的ui
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public bool HasShowing(UILayers layer)
        {
            for (int i = 0; i < mLayerParents[layer].childCount; ++i)
            {
                var ui = mLayerParents[layer].GetChild(i).GetComponent<UIView>();
                if (ui != null && ui.gameObject.activeSelf)
                {
                    return true;
                }
            }
            return false;
        }

        public void RemoveUI(UIView view)
        {
            mUiViewDic.Remove(view.name);
        }
    }
}