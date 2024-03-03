using System.Collections.Generic;
using UnityEngine;

namespace XGame
{
    [CreateAssetMenu(fileName = "LanguageData",order =2001)]
    public class LanguageData : ScriptableObject
    {
        public List<LanguageType> language = new List<LanguageType>();
    }

    [System.Serializable]
    public struct LanguageType
    {
        public string languageTypeName;
        public LanguageTypeEnum languageTypeEnum;
        public List<LanguageValue> allValue;
    }

    [System.Serializable]
    public struct LanguageValue
    {
        public string languageEnumName;
        public LanguageEnum languageEnum;
        public string value;
    }

}