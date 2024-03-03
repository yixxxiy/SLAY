
using System;
using System.Collections.Generic;

namespace XGame
{
    public class EventCenterManager : Singleton<EventCenterManager>
    {
        interface IRegisterations
        {

        }
        class Registerations<T> : IRegisterations
        {
            public event Action<T> OnReceives;
            public Dictionary<object, Action<T>> TheObjs = new Dictionary<object, Action<T>>();

            public void Do(T t)
            {
                OnReceives?.Invoke(t);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<Type, IRegisterations> mTypeEventDict = new Dictionary<Type, IRegisterations>();

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="onReceive"></param>
        /// <typeparam name="T"></typeparam>
        public static void Register<T>(object o, System.Action<T> onReceive)
        {
            EventCenterManager.Instance.RegisterEvent<T>(o, onReceive);
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        /// <param name="onReceive"></param>
        /// <typeparam name="T"></typeparam>
        public static void UnRegister<T>(object o)
        {
            EventCenterManager.Instance.UnRegisterEvent<T>(o);
        }

        /// <summary>
        /// 发送事件
        /// </summary>
        /// <param name="t"></param>
        /// <typeparam name="T"></typeparam>
        public static void Send<T>(T t)
        {
            EventCenterManager.Instance.SendEvent<T>(t);
        }

        /// <summary>
        /// 发送事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Send<T>() where T : new()
        {
            EventCenterManager.Instance.SendEvent<T>();
        }


        public void RegisterEvent<T>(object o, Action<T> onReceive)
        {
            var type = typeof(T);

            IRegisterations registerations = null;

            if (mTypeEventDict.TryGetValue(type, out registerations))
            {
                var reg = registerations as Registerations<T>;
                if (!reg.TheObjs.ContainsKey(o))
                {
                    reg.OnReceives += onReceive;
                    reg.TheObjs[o] = onReceive;
                }
            }
            else
            {
                var reg = new Registerations<T>();
                reg.OnReceives += onReceive;
                reg.TheObjs[o] = onReceive;
                mTypeEventDict.Add(type, reg);
            }
        }

        public void UnRegisterEvent<T>(object o)
        {
            var type = typeof(T);

            IRegisterations registerations = null;

            if (mTypeEventDict.TryGetValue(type, out registerations))
            {
                var reg = registerations as Registerations<T>;
                if (reg.TheObjs.ContainsKey(o))
                {
                    reg.OnReceives -= reg.TheObjs[o];
                    reg.TheObjs.Remove(o);
                }

            }
        }

        public void SendEvent<T>() where T : new()
        {
            var type = typeof(T);

            IRegisterations registerations = null;

            if (mTypeEventDict.TryGetValue(type, out registerations))
            {
                var reg = registerations as Registerations<T>;

                reg.Do(new T());
            }
        }

        public void SendEvent<T>(T e)
        {
            var type = typeof(T);

            IRegisterations registerations = null;

            if (mTypeEventDict.TryGetValue(type, out registerations))
            {
                var reg = registerations as Registerations<T>;
                reg.Do(e);
            }
        }

        public void Clear()
        {
            mTypeEventDict.Clear();
        }

    }


}