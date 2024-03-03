using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
namespace XGame
{
    public static class ExtendsUtil
    {
       
        /// <summary>
        /// 用于显示金币数量
        /// </summary>
        /// <param name="tokenNum"></param>
        /// <returns></returns>
        public static string GetTokenShowString(this int tokenNum)
        {
            string str = tokenNum.ToString();
            int length = str.Length;
            int pos = 0;
            for (int i = length - 1; i > 0; i--)
            {
                pos++;
                if (pos % 3 == 0)
                    str = str.Insert(i, ",");
            }
            return str;
        }


        /// <summary>
        /// 按钮添加点击
        /// </summary>
        /// <param name="self"></param>
        /// <param name="action"></param>
        public static void AddClickEvent(this Button button, UnityAction call)
        {         
            button.onClick.AddListener(call);
            button.onClick.AddListener(MainController.PlayBtnOn);
        }

        /// <summary>
        /// 延时显示某些按钮
        /// </summary>
        /// <param name="self"></param>
        /// <param name="time"></param>
        public static void SetCloseButtonDelay(this Button self, float time = 1f)
        {
            self.gameObject.SetActive(false);

            DOVirtual.DelayedCall(time, () =>
            {
                self.gameObject.SetActive(true);
            });

        }

        /// <summary>
        /// string转reward枚举
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Reward ToReward(this string str)
        {
            return (Reward)System.Enum.Parse(typeof(Reward), str);
        }


        /// <summary>
        /// string转指定类型枚举
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T ToEnumT<T>(this string str)
        {
            return (T)System.Enum.Parse(typeof(T), str);
        }

        /// <summary>
        /// 注册消息监听
        /// </summary>
        /// <param name="o"></param>
        /// <param name="onReceive"></param>
        /// <typeparam name="T"></typeparam>
		public static void RegisterEvent<T>(this object o, System.Action<T> onReceive)
        {
            EventCenterManager.Register<T>(o, onReceive);
        }
        /// <summary>
        /// 注销消息监听
        /// </summary>
        /// <param name="o"></param>
        /// <typeparam name="T"></typeparam>
		public static void UnRegisterEvent<T>(this object o)
        {
            EventCenterManager.UnRegister<T>(o);
        }

        /// <summary>
        /// 启动计时器
        /// </summary>
        /// <param name="o"></param>
        /// <param name="interval">间隔时间</param>
        /// <param name="times">重复次数。-1为无限循环</param>
        /// <param name="callback">回调</param>
        /// <param name="delay">延迟启动时间</param>
        /// <param name="endCallBack">结束回调</param>
        public static void StartTimer(this object o, float interval, int times, System.Action callback, float delay = 0, System.Action endCallBack = null)
        {
            TimerManager.Instance.StartTimer(o, interval, times, callback, delay, endCallBack);
        }
        /// <summary>
        /// 释放计时器
        /// </summary>
        /// <param name="o"></param>
        public static void ReleaseTimer(this object o)
        {
            TimerManager.Instance.ReleaseTimer(o);
        }

        /// <summary>
        /// 根据路径获取组件
        /// </summary>
        /// <param name="t"></param>
        /// <param name="path"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Find<T>(this Transform t, string path) where T : Component
        {
            return t.Find(path).GetComponent<T>();
        }


    }
}

