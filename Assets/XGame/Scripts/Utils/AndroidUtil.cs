using UnityEngine;
using System.Runtime.InteropServices;
using AOT;
using System;
namespace XGame
{
    public static class AndroidUtil
    {
        static AndroidJavaObject mCurrentActivity;
        static AndroidJavaObject CurrentActivity
        {
            get
            {
                if (mCurrentActivity == null)
                {
                    AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    mCurrentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                }

                return mCurrentActivity;
            }
        }
        public static string GetCountryCode()
        {
            var countryCode = "US";
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaObject locale = CurrentActivity.Call<AndroidJavaObject>("getResources").Call<AndroidJavaObject>("getConfiguration").Get<AndroidJavaObject>("locale");
            countryCode = locale.Call<string>("getCountry");
#endif
            return countryCode;
        }


        public static bool IsFBInstall()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            using (AndroidJavaObject packageManager = CurrentActivity.Call<AndroidJavaObject>("getPackageManager"))
            {
                AndroidJavaObject launchIntent = null;
                try
                {
                    launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", "com.facebook.katana");
                }
                catch
                {
                    return false;
                }
                return launchIntent != null;;
            }
#endif
            return true;
        }
        public static string GetLanguage()
        {
            var languageString = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaObject locale = CurrentActivity.Call<AndroidJavaObject>("getResources").Call<AndroidJavaObject>("getConfiguration").Get<AndroidJavaObject>("locale");
            languageString = locale.Call<string>("getLanguage");
#endif
            return languageString;
        }

        public static int GetAndroidVersion()
        {
            using (AndroidJavaClass a = new AndroidJavaClass("com.ttg.xlinkutil.Util"))
            {
                return a.CallStatic<int>("GetAndroidVersion");
            }
        }
        public static float NotchHeight()
        {
            if (GetAndroidVersion() < 28)
            {
                using (AndroidJavaClass a = new AndroidJavaClass("com.linxinfa.notchscreenfit.NotchScreenHelper"))
                {
                    return a.CallStatic<bool>("hasNotch") ? 60 : 0;
                }
            }
            return 0;
        }
        public static bool CheckVPN()
        {
            using (AndroidJavaClass a = new AndroidJavaClass("com.ttg.xlinkutil.Util"))
            {
                return a.CallStatic<bool>("CheckVPN");
            }
        }
    }
}
