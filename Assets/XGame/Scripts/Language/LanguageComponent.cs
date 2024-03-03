using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace XGame
{

    /// <summary>
    /// 设置多语言
    /// </summary>
    public class LanguageComponent : MonoBehaviour
    {
        private void Awake()
        {
            LanguageManager.Instance.OnLanguageChange += SetLanguage;
        }
        private void OnDestroy()
        {
            LanguageManager.Instance.OnLanguageChange -= SetLanguage;
        }
        private List<TextAndLanguageEnum> languageInfo = new List<TextAndLanguageEnum>();

        public void AddTextAndLanguageEnum(string textName, LanguageEnum languageEnum, params string[] param)
        {
            var child = transform.Find(textName);
            if (child == null)
            {
                Debug.LogError("没找到多语言Text:" + textName);
                return;
            }
            Text text = child.GetComponent<Text>();
            if (text == null)
            {
                Debug.LogError("不是Text:" + textName);
                return;
            }
            TextAndLanguageEnum t = new TextAndLanguageEnum() { text = text, languageEnum = languageEnum, paramArray = param };
            languageInfo.Add(t);
        }
        private void SetLanguage()
        {
            int count = languageInfo.Count;
            for (int i = 0; i < count; i++)
            {
                LanguageEnum languageEnum = languageInfo[i].languageEnum;
                var str = LanguageManager.Instance.GetString(languageEnum);
                if (languageInfo[i].paramArray != null && languageInfo[i].paramArray.Length > 0) str = string.Format(str, languageInfo[i].paramArray);
                languageInfo[i].text.text = str;
            }
        }
        private void Start()
        {
            SetLanguage();
        }

        [System.Serializable]
        public struct TextAndLanguageEnum
        {
            public Text text;
            public LanguageEnum languageEnum;
            public string[] paramArray;
        }
    }
}