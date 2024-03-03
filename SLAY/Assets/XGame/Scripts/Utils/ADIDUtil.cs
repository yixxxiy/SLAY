using UnityEngine;
using System.Runtime.InteropServices;
using AOT;
using System;
namespace XGame
{
    /// <summary>
    /// 唯一设备ID标识，
    /// </summary>
    public static class ADIDUtil
    {
        public static string ADID
        {
            get
            {
                var adid = LocalDataUtil.GetString("adidutil_adid", "");
                if (string.IsNullOrEmpty(adid))
                {
                    GetAdID();
                }
                return adid;
            }
            private set
            {
                LocalDataUtil.SetString("adidutil_adid", value);
            }

        }

        [RuntimeInitializeOnLoadMethodAttribute]
        static void Init()
        {
            var id = ADID;
        }

        static void GetAdID()
        {
#if UNITY_EDITOR
            ADID = "test";
#elif UNITY_ANDROID
            Application.RequestAdvertisingIdentifierAsync
            (
               (string advertisingId, bool trackingEnabled, string error) =>
               {
                   if (trackingEnabled)
                       ADID = advertisingId;
               }
            );
#elif UNITY_IOS
            OnGetADIDHandler handler = new OnGetADIDHandler(onGetADIDHandler);
            IntPtr handlerPointer = Marshal.GetFunctionPointerForDelegate(handler);
            requestADID(handlerPointer);
#endif
        }

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void requestADID(IntPtr getADIDHandler);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnGetADIDHandler(string resultString);
        [MonoPInvokeCallback(typeof(OnGetADIDHandler))]
        static void onGetADIDHandler(string resultStr)
        {
            ADID = resultStr;
        }
#endif
    }
}
