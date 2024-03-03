using UnityEngine;
using System.Collections.Generic;

namespace XGame
{
    public class LanguageManager : Singleton<LanguageManager>
    {
#if UNITY_EDITOR
        public LanguageManager()
        {
            if (string.IsNullOrEmpty(UnityEditor.EditorPrefs.GetString("language")))
            {
                UnityEditor.EditorPrefs.SetString("language", LanguageTypeEnum.English.ToString());
            }
            var name = UnityEditor.EditorPrefs.GetString("language");
            Init(name.ToEnumT<LanguageTypeEnum>());


        }
#endif
        string mCountryCode;
        public string CountryCode
        {
            set
            {
                mCountryCode = value;
                LocalDataUtil.SetString("C", mCountryCode);

            }
            get
            {
                return mCountryCode;
            }
        }

        public string AdCountryCode { get; set; }
#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern string getPreferredLanguage();
#endif
        private const string LanguageDataPath = "Language/LanguageData";

        private Dictionary<string, string> dataDic = new Dictionary<string, string>();
        private LanguageType curLanguage;
        public LanguageType CurLanguage
        {
            get
            {
                return curLanguage;
            }
        }

        private LanguageData languageData;

        public event System.Action OnLanguageChange;


        /// <summary>
        /// 初始化当前语言
        /// </summary>
        /// <param name="LanguageTypeEnum"></param>
        public void Init(LanguageTypeEnum languageType)
        {
            mCountryCode = LocalDataUtil.GetString("country_code", "");
            if (string.IsNullOrEmpty(CountryCode))
            {
                CountryCode = AndroidUtil.GetCountryCode();
            }
            if (languageData == null) languageData = ResourceUtil.Load<LanguageData>(LanguageDataPath);
            curLanguage = GetLanguage(languageType);
            CacheData();
            OnLanguageChange?.Invoke();
        }


        private LanguageType GetLanguage(LanguageTypeEnum languageTypeEnum)
        {
            int count = languageData.language.Count;
            for (int i = 0; i < count; i++)
            {
                if (languageData.language[i].languageTypeEnum == languageTypeEnum)
                {
                    return languageData.language[i];
                }
            }
            if (count > 0) return languageData.language[0];
            Debug.LogError("没有任何语言");
            return default;
        }
        public void ChangeCurLanguage(LanguageTypeEnum languageTypeEnum)
        {
            curLanguage = GetLanguage(languageTypeEnum);
            CacheData();
            OnLanguageChange?.Invoke();
        }
        /// <summary>
        /// 缓存多语言数据
        /// </summary>
        private void CacheData()
        {
            dataDic.Clear();
            foreach (var data in curLanguage.allValue)
            {
                dataDic[data.languageEnum.ToString()] = data.value;
            }
        }

        public string GetString(string language)
        {
            var str = string.Empty;
            if (dataDic.TryGetValue(language, out str))
            {
                return str;
            }
            Debug.LogError("没有此枚举信息" + language);
            return language + "不存在";
        }

        /// <summary>
        /// 跟据语言枚举来获得对应的语言信息
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public string GetString(LanguageEnum language)
        {
            return GetString(language.ToString());
        }

 

    }
}