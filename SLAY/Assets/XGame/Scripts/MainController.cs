using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyExcel;
using MoreMountains.NiceVibrations;
using UnityEngine.SceneManagement;
namespace XGame
{

    /// <summary>
    /// 中心控制类
    /// </summary>
    public class MainController : MonoBehaviour
    {

        public static bool OpenHaptic
        {
            get
            {
                return LocalDataUtil.GetBool("haptic_open", true);
            }
            set
            {
                LocalDataUtil.SetBool("haptic_open", value);
            }
        }

        //静态表读取
        private static EEDataManager mEEDataManager = new EEDataManager();
        #region 一些常用的缩写方法
        //数据管理
        public static GameDataManager DataMgr
        {
            get
            {
                return GameDataManager.Instance;
            }
        }
        public static GameData UserData
        {
            get
            {
                return GameDataManager.Instance.Data;
            }
        }
        //读取静态表
        public static T GetRecord<T>(int key) where T : EERowData
        {
            return mEEDataManager.Get<T>(key);
        }
        public static T GetRecord<T>(string key) where T : EERowData
        {
            return mEEDataManager.Get<T>(key);
        }
        public static List<T> GetRecords<T>() where T : EERowData
        {
            return mEEDataManager.GetList<T>();
        }


        //UI管理
        public static UIManager UIMgr
        {
            get
            {
                return UIManager.Instance;
            }
        }



        public static bool NeedGamePause()
        {
            return MainController.UIMgr.HasShowing(UILayers.PopupLayer) || MainController.UIMgr.HasShowing(UILayers.MainLayer);
        }
        /// <summary>
        /// 显示UI
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="showCallBack"></param>
        /// <param name="delay"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ShowUI<T>(object msg = null, System.Action showCallBack = null, bool delay = true) where T : UIView
        {
            return UIManager.Instance.ShowUIView<T>(msg, showCallBack, delay);
        }
        /// <summary>
        /// 隐藏UI
        /// </summary>
        /// <param name="hideCallBack"></param>
        /// <typeparam name="T"></typeparam>
        public static void HideUI<T>(System.Action hideCallBack = null) where T : UIView
        {
            UIManager.Instance.HideUIView<T>(hideCallBack);
        }

        //读取多语言
        public static LanguageManager LanguageMgr
        {
            get
            {
                return LanguageManager.Instance;
            }
        }
        public static string GetString(string languegeKey, params string[] paramArray)
        {
            var str = LanguageManager.Instance.GetString(languegeKey);
            if (paramArray != null && paramArray.Length > 0) str = string.Format(str, paramArray);
            return str;
        }
        public static string GetString(LanguageEnum languageEnum, params string[] paramArray)
        {
            var str = LanguageManager.Instance.GetString(languageEnum);
            if (paramArray != null && paramArray.Length > 0) str = string.Format(str, paramArray);
            return str;
        }

        //声音管理
        public static AudioManager AudioMgr
        {
            get
            {
                return AudioManager.Instance;
            }
        }
        public static void PlaySound(string sound)
        {
            AudioManager.Instance.PlayOneShot(sound);
        }

        public static void PlayBtnOn()
        {
            PlaySound(AudioModel.Button);
        }

        public static void Haptic()
        {
            if (OpenHaptic) MMVibrationManager.Haptic(HapticTypes.Selection);
        }
        public static void Haptic2()
        {
            if (OpenHaptic) MMVibrationManager.Haptic(HapticTypes.Success);
        }




        /// <summary>
        /// 锁屏转圈
        /// </summary>
        /// <param name="waitTime">等待时间（秒）(小于0代表无限转圈)</param>
        /// <param name="cb">等待结束后回调</param>
        public static void Wait(float waitTime = -1, System.Action cb = null)
        {
            ShowUI<UI_ADMask>(new UI_ADMaskData() { WaitTime = waitTime, Cb = cb });
        }
        /// <summary>
        /// 强制结束转圈等待
        /// </summary>
        public static void StopWait()
        {
            HideUI<UI_ADMask>();
        }



        //Tips
        public static void ShowTips(string tip, float durationTime = 1f)
        {
            TipsManager.Instance.ShowTips(tip, durationTime);
        }

        #endregion






        void Awake()
        {

            DontDestroyOnLoad(gameObject);
            //设置帧率
            Application.targetFrameRate = 60;


        }
        void Start()
        {

            Init();
            //ShowUI<UI_Loading>();
        }



        void OnDestroy()
        {

        }
        void Update()
        {


            if (Camera.main != null && UIManager.Instance.UiCanvas != null && Input.GetMouseButtonDown(0))
            {

                var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pos.z = 0;
                /// FlyRewardManager.Instance.ShowFlyBomb(pos, false);
            }
            if (UserData != null && TimeUtil.IsSecondDay(UserData.LastRefreshDataTime))
            {
                DataMgr.RefreshDayData();
            }
            //驱动计时器
            TimerManager.Instance.Update(Time.deltaTime);
        }
        #region 初始化各模块


        //初始化各个模块
        void Init()
        {
            //静态表初始化
            mEEDataManager.Load();
            //多语言初始化
            InitLanguage();


            //UI初始化
            UIManager.Instance.Init(transform);
            //音頻初始化
            AudioManager.Instance.Init();
            //读取信息
            GameDataManager.Instance.LoadData();
            //TipsManager初始化
            TipsManager.Instance.Init();

            //

        }
        void InitLanguage()
        {
            LanguageTypeEnum curlanguageType = LanguageTypeEnum.English;

            //获取本地语言
            SystemLanguage systemLanguage = Application.systemLanguage;
            //这里有一个逻辑是跟据获取到的语言来设置需要展示的语言
            switch (systemLanguage)
            {
                case SystemLanguage.Japanese:               //日语
                    curlanguageType = LanguageTypeEnum.Japanese;
                    break;
                case SystemLanguage.Korean:                 //韩语
                    curlanguageType = LanguageTypeEnum.Korean;
                    break;
                case SystemLanguage.Russian:                //俄罗斯语
                    curlanguageType = LanguageTypeEnum.Russian;
                    break;
                case SystemLanguage.Portuguese:             //葡萄牙语
                    curlanguageType = LanguageTypeEnum.Portuguese;
                    break;
                case SystemLanguage.German:                 //德语
                    curlanguageType = LanguageTypeEnum.German;
                    break;
                case SystemLanguage.French:                 //法语
                    curlanguageType = LanguageTypeEnum.French;
                    break;
                case SystemLanguage.ChineseTraditional:     //繁体中文
                    curlanguageType = LanguageTypeEnum.ChineseTraditional;
                    break;
                case SystemLanguage.Indonesian:             //印度尼西亚
                    curlanguageType = LanguageTypeEnum.Indonesian;
                    break;
                case SystemLanguage.Thai:             //泰语
                    curlanguageType = LanguageTypeEnum.Thai;
                    break;
                case SystemLanguage.Vietnamese:             //越南语
                    curlanguageType = LanguageTypeEnum.Vietnamese;
                    break;
                case SystemLanguage.Spanish:
                    curlanguageType = LanguageTypeEnum.Spanish;
                    break;
                default:                                    //英语
                    curlanguageType = LanguageTypeEnum.English;
                    break;
            }

            //--印尼语的本地化  墨西哥、老挝、菲律宾
            string languageString;
#if UNITY_EDITOR
            languageString = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
#elif UNITY_ANDROID
            languageString = AndroidUtil.GetLanguage();
#elif UNITY_IOS
            languageString = getPreferredLanguage();
#endif
            string wholeString = languageString;
            if (languageString == "lo")
            {
                curlanguageType = LanguageTypeEnum.Laothian;
            }
            else if (languageString == "fil")
            {
                curlanguageType = LanguageTypeEnum.Pilipinas;
            }
            else
            {
                languageString = languageString.Substring(0, languageString.IndexOf('-') <= 0 ? languageString.Length : languageString.IndexOf('-'));
                if (languageString == "hi")
                {
                    curlanguageType = LanguageTypeEnum.Hindi;
                }
            }

            LanguageManager.Instance.Init(curlanguageType);
        }
        #endregion
        /// <summary>
        /// 保存数据
        /// </summary>
        void SaveData()
        {
            //保存游戏数据
            GameDataManager.Instance.Save();
        }
        System.DateTime LeaveTime = System.DateTime.Now;
        private void OnApplicationFocus(bool focus)
        {
            if (!focus)
            {
                LeaveTime = System.DateTime.Now;
                SaveData();
            }
            else
            {
            }
        }
        private void OnApplicationQuit()
        {
            SaveData();
        }
        private void OnApplicationPause(bool pause)
        {
            if (pause) SaveData();
        }




    }
}