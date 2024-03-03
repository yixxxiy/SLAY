
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
namespace XGame
{
    [RequireComponent(typeof(Text))]
    [ExecuteInEditMode]
    public class LanguageLabel : MonoBehaviour
    {
        [SerializeField]
        [TextArea]
        string mValue;
        [SerializeField]
        string[] mParams;

        Text mText;
        string mCurKey;
        LanguageTypeEnum mCurLanguage;


        string mLanguageManagerHashCode;

        void Awake()
        {
#if UNITY_EDITOR
            mLanguageManagerHashCode = EditorPrefs.GetString("language_version", "");
#endif
            mText = GetComponent<Text>();
            ResetLabel();
            LanguageManager.Instance.OnLanguageChange += ResetLabel;
        }
        void OnDestroy()
        {
            LanguageManager.Instance.OnLanguageChange -= ResetLabel;
        }
        public void UpdateValue(string v)
        {
            mValue = v;
            ResetLabel();
        }
        public void UpdateParams(params string[] param)
        {
            mParams = param;
            ResetLabel();
        }

        void ResetLabel()
        {
            mCurKey = mValue;
            string s = LanguageManager.Instance.GetString(mValue);
            if (mParams.Length > 0) s = string.Format(s, mParams);
            if (mText == null) return;
            mText.text = s;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(mText);
#endif
        }
#if UNITY_EDITOR
        LanguageLabel()
        {
            EditorApplication.update += EditorUpdate;
        }


        private void EditorUpdate()
        {
            if (mCurKey != mValue || mCurLanguage != LanguageManager.Instance.CurLanguage.languageTypeEnum || mLanguageManagerHashCode != EditorPrefs.GetString("language_version", ""))
            {
                mLanguageManagerHashCode = EditorPrefs.GetString("language_version", "");
                mCurLanguage = LanguageManager.Instance.CurLanguage.languageTypeEnum;
                ResetLabel();
            }

        }
#endif

    }
}


