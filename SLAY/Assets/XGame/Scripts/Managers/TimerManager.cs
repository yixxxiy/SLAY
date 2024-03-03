using System;
using System.Collections.Generic;
using UnityEngine;
namespace XGame
{
    public class TimerManager : Singleton<TimerManager>
    {
        Dictionary<object, Timer> _timers = new Dictionary<object, Timer>();
        event Action<float> _onUpdate;
        /// <summary>
        /// 开启一个计时器
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="interval"></param>
        /// <param name="times"></param>
        /// <param name="callback"></param>
        /// <param name="delay"></param>
        public void StartTimer(object owner, float interval, int times, Action callback, float delay = 0)
        {
            StartTimer(owner, interval, times, callback, delay, null, null);
        }
        public void StartTimer(object owner, float interval, int times, Action callback, float delay, Action onCompleted = null, Action onRelease = null)
        {
            Timer t = null;
            if (!_timers.ContainsKey(owner))
            {
                t = new Timer();
                _timers.Add(owner, t);
                _onUpdate += t.Update;
            }
            else
            {
                t = _timers[owner];
            }
            t.Start(interval, times, callback, delay, onCompleted, onRelease);

        }

        public void ReleaseAll()
        {
            foreach (var t in _timers)
            {
                _onUpdate -= t.Value.Update;
                t.Value.Release();
            }
            _timers.Clear();
        }

        //需要驱动此函数
        public void Update(float dt)
        {
            _onUpdate?.Invoke(dt);
        }

        public void PauseTimer(object owner)
        {
            Timer timer;
            if (_timers.TryGetValue(owner, out timer))
            {
                timer.Pause();
            }
        }
        public void ResumeTimer(object owner)
        {
            Timer timer;
            if (_timers.TryGetValue(owner, out timer))
            {
                timer.Resume();
            }
        }

        public void ReleaseTimer(object owner)
        {
            if (!_timers.ContainsKey(owner)) return;
            _timers[owner].Release();
            _onUpdate -= _timers[owner].Update;
            _timers.Remove(owner);
        }
    }
    public class Timer
    {
        private enum TState
        {
            Invalid,
            WaitDelay,
            Running,
            Pause,

        }

        Action _onCompleted; //结束时回调
        Action _onRelease;  //销毁回调
        TState _curState;

        float _interval;  //时间间隔
        float _delay; //延迟时间
        Action _callback;  //回调函数
        int _times;       //循环次数 <=0为无限循环

        float _curDur;    //当前已经过时间
        int _doTimes;   //已执行过的次数

        object _owner;


        /// <summary>
        /// 开始计时器
        /// </summary>
        public void Start(float interval, int times, Action callback, float delay, Action onCompleted, Action onRelease)
        {
            Reset();

            _interval = interval;
            _times = times;
            _callback = callback;
            _delay = delay;
            _onCompleted = onCompleted;
            _onRelease = onRelease;

            if (delay > 0)
            {
                _curState = TState.WaitDelay;
            }
            else
            {
                //直接执行一次callback
                DoCallback();
                _curState = TState.Running;
            }


            CheckEnd();
        }
        /// <summary>
        /// 停止计时器，并不会销毁
        /// </summary>
        public void Stop()
        {
            _curState = TState.Invalid;
        }
        /// <summary>
        /// 暂停计时器
        /// </summary>
        public void Pause()
        {
            if (TState.Invalid == _curState) return;
            _curState = TState.Pause;
        }
        /// <summary>
        /// 恢复计时器，只能从暂停状态恢复
        /// </summary>
        public void Resume()
        {
            if (_curState != TState.Pause) return;
            _curState = TState.Running;
        }
        /// <summary>
        /// 释放计时器
        /// </summary>
        public void Release()
        {
            _onRelease?.Invoke();
        }
        public void Update(float dt)
        {
            if (_curState == TState.Pause || _curState == TState.Invalid) return;
            _curDur += dt;
            //大于延迟时间，开始执行
            if (_curState == TState.WaitDelay && TimeUtil.FloatEqualsOrBeyond(_curDur, _delay))
            {
                _curState = TState.Running;
                _curDur -= _delay;
                DoCallback();
                CheckEnd();
            }

            if (_curState == TState.Running && TimeUtil.FloatEqualsOrBeyond(_curDur, _interval))
            {
                _curDur -= _interval;
                DoCallback();
                CheckEnd();
            }

        }

        void DoCallback()
        {
            _callback?.Invoke();
            _doTimes++;
        }

        bool CheckEnd()
        {
            if (_times > 0 && _doTimes >= _times)
            {
                _onCompleted?.Invoke();
                _curState = TState.Invalid;
                return true;
            }
            return false;
        }

        void Reset()
        {
            _curDur = 0;
            _doTimes = 0;
        }

    }
}
