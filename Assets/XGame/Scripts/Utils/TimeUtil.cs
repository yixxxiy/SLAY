using System;
using UnityEngine;

namespace XGame
{
    
    public static class TimeUtil
    {
        public static string GetRemainingTimeMs(DateTime time)
        {
            var ms = GetNowTimeSpanMS(time);
            var h = ms / 3600000;
            var m = ms % 3600000 / 60000;
            var s = ms % 60000 / 1000;
            var ms2 = ms % 1000 / 10;
            return string.Format("{0:00}:{1:00}:{2:00}:{3:00}", h, m, s, ms2);
        }

        public static bool FloatEquals(float a, float b)
        {
            return Mathf.Abs(a - b) < 0.0001f;
        }
        /// <summary>
        /// a >= b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool FloatEqualsOrBeyond(float a, float b)
        {
            return a > b || Mathf.Abs(a - b) < 0.0001f;
        }
        /// <summary>
        /// 该时间是否已过期
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static bool HasGone(this DateTime time)
        {
            return time < DateTime.Now;
        }
        /// <summary>
        /// 获取当天的结束时间
        /// </summary>
        /// <returns></returns>
        public static DateTime TodayEndTime()
        {
            return Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("D").ToString()).AddSeconds(-1);
        }
        /// <summary>
        /// 获取指定时间与当前时间差值(秒)
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int GetNowTimeSpanSec(DateTime time)
        {
            TimeSpan ts = time.Subtract(DateTime.Now);
            int sec = Mathf.CeilToInt((float)ts.TotalSeconds);
            return Math.Max(sec, 0);
        }
        /// <summary>
        /// 获取指定时间与当前时间差值(毫秒)
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int GetNowTimeSpanMS(DateTime time)
        {
            TimeSpan ts = time.Subtract(DateTime.Now);
            int ms = Mathf.CeilToInt((float)ts.TotalMilliseconds);
            return Math.Max(ms, 0);
        }
        /// <summary>
        /// 获取当前时间减指定时间差值(秒)
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int GetGoneTimeSec(DateTime time)
        {
            TimeSpan ts = DateTime.Now.Subtract(time);
            int sec = Mathf.CeilToInt((float)ts.TotalSeconds);
            return Math.Max(sec, 0);
        }
        /// <summary>
        /// 统计玩游戏的总天数
        /// </summary>
        public static int GetTotalDaysSinceInstallGame()
        {
            return (int)(System.DateTime.Now - MainController.UserData.RegisterTime).TotalDays;
        }
        /// <summary>
        /// 获取剩余时间（UI显示用 格式00:00:00）
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string GetRemainingTime(DateTime time)
        {
            var second = GetNowTimeSpanSec(time);
            var h = second / 3600;
            var m = second % 3600 / 60;
            var s = second % 60;
            return string.Format("{0:00}:{1:00}:{2:00}", h, m, s);
        }

        public static string GetRemainingTime2(DateTime time)
        {
            var second = GetNowTimeSpanSec(time);
            var m = second % 3600 / 60;
            var s = second % 60;
            return string.Format("{0:00}:{1:00}",  m, s);
        }

        /// <summary>
        /// 获取剩余时间（UI显示用 格式00:00:00）
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string GetRemainingTimeWithD(DateTime time)
        {
            var second = GetNowTimeSpanSec(time);
            var d = second / 86400;
            var h = second % 86400 / 3600;
            var m = second % 3600 / 60;
            var s = second % 60;
            if (d > 0) return string.Format("{0}D {1:00}:{2:00}:{3:00}", d, h, m, s);
            return string.Format("{0:00}:{1:00}:{2:00}", h, m, s);
        }

        public static string GetRemainingTimeWithD2(DateTime time)
        {
            var second = GetNowTimeSpanSec(time);
            var d = second / 86400;
            var h = second % 86400 / 3600;
            var m = second % 3600 / 60;
            return string.Format("{0}d:{1:00}h:{2:00}m", d, h, m);
        }

        /// <summary>
        /// 是否是第二天
        /// </summary>
        /// <value></value>
        // public static bool IsSecondDay
        // {
        //     get
        //     {
        //         return isSecondDay;
        //     }
        // }
      
        /// <summary>
        /// 24小时当前剩余的秒数
        /// </summary>
        // public static double CurSurplusSecond
        // {
        //     get
        //     {
        //         if (times == null)
        //         {
        //             times = TimeManager.Instance.GetTimer(timekey);
        //         }
        //         return times.curSurplusSecond;
        //     }
        // }
        /// <summary>
        /// 结束这24小时的倒计时
        /// </summary>
        // public static void Over24Hours()
        // {
        //     if (times == null)
        //     {
        //         times = TimeManager.Instance.GetTimer(timekey);
        //     }
        //     times.curSurplusSecond = -1d;
        // }

     

        #region 24小时    
        // private static string timekey = "SetOne24HoursDowmTime";
        // private static bool isSecondDay = false;
        // /// <summary>
        // /// 是否是第一次来设置24的倒计时，已经设置过了就不会在设置
        // /// </summary>
        // private static bool IsFirstSetOne24HoursDowmTime
        // {
        //     get
        //     {
        //         return LocalDataUtil.GetBool("MainManager_IsFirstSetOne24HoursDowmTime", true);
        //     }
        //     set
        //     {
        //         LocalDataUtil.SetBool("MainManager_IsFirstSetOne24HoursDowmTime", value);
        //     }
        // }

        // /// <summary>
        // /// 统计玩游戏的总天数
        // /// </summary>
        // private static System.DateTime FirstPlayTime
        // {
        //     get
        //     {
        //         return System.DateTime.Parse(LocalDataUtil.GetString("MainManager_FirstPlayTime", System.DateTime.Now.ToString()));
        //     }
        //     set
        //     {
        //         LocalDataUtil.SetString("MainManager_FirstPlayTime", value.ToString());
        //     }
        // }

        public static bool IsSecondDay(System.DateTime d)
        {
            System.DateTime now = System.DateTime.Now;
            if (!(now.Year == d.Year && now.Month == d.Month && now.Day == d.Day))
            {
                return true;
            }
            return false;

        }
        /// <summary>
        /// 整个游戏只会在第一次运行的时候执行这窜代码
        /// </summary>
        // public static void SetOne24HoursDowmTime()
        // {
        //     if (IsFirstSetOne24HoursDowmTime)
        //     {
        //         IsFirstSetOne24HoursDowmTime = false;
        //         isSecondDay = true;
        //         FirstPlayTime = System.DateTime.Now;//第一次登录时的时间
        //         TimeManager.Instance.ADDTimer(timekey, 24);
        //     }
        // }


        #endregion




        
    }
}

