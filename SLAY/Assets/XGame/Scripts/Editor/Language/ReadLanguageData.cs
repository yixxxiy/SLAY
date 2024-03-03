using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Excel;
using System.Data;
using System.Text;

namespace XGame
{
    public class ReadLanguageData
    {
        /// <summary>
        /// Excal的路径
        /// </summary>
        private static string xlsxPath = Application.dataPath + "/XGame/Configs/LanguageData.xlsx";

        private static string languageTypeEnumPath = Application.dataPath + "/XGame/Scripts/Language//LanguageTypeEnum.cs";
        private static string languageEnumPath = Application.dataPath + "/XGame/Scripts/Language/LanguageEnum.cs";

        private static bool isNameSpace = true;
        private static string nameSpace = "XGame";

        private static string languageTypeEnumName = "LanguageTypeEnum";
        private static string languageEnumName = "LanguageEnum";

        private static string languageDataFilePath = Application.dataPath + "/XGame/Resources/Language";
        private static string languageDataPath = "Language/LanguageData";
        private static string languageData = Application.dataPath + "/XGame/Resources/Language/LanguageData.asset";

        public static bool HasReadInfo
        {
            get
            {
                return PlayerPrefs.GetInt(nameSpace + "_ReadMultipleLanguage", 0) == 1;
            }
            set
            {
                PlayerPrefs.SetInt(nameSpace + "_ReadMultipleLanguage", value ? 1 : 0);
            }
        }
        [MenuItem("LanguageTools/ReadLanguageData", false, 1)]
        private static void Read()
        {
            DataSet dataSet = ReadExcal(xlsxPath);
            if (dataSet == null)
            {
                Debug.LogError("Excal文件为空!");
                return;
            }

            DataTable dataTable = dataSet.Tables[0];

            ReadLanguageTypeEnum(dataTable);
            ReadLanguageEnum(dataSet);

            AssetDatabase.Refresh();
            if (!EditorApplication.isCompiling)
            {
                OnCompileComplete();
            }
            else
            {
                HasReadInfo = true;
            }
        }
        public static void OnCompileComplete()
        {
            HasReadInfo = false;

            //数据文件的读取与创建
            LanguageData data = Resources.Load<LanguageData>(languageDataPath);
            if (data == null)
            {
                if (!Directory.Exists(languageDataFilePath))
                {
                    Directory.CreateDirectory(languageDataFilePath);
                }
                data = ScriptableObject.CreateInstance<LanguageData>();
                AssetDatabase.CreateAsset(data, languageData);
            }

            data.language.Clear();

            DataSet dataSet = ReadExcal(xlsxPath);
            if (dataSet == null)
            {
                Debug.LogError("Excal文件为空!");
                return;
            }

            DataTable dataTable = dataSet.Tables[0];
            ReadLanguageInfo(dataSet, data);

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            LanguageManager.Instance.Init(EditorPrefs.GetString("language", "English").ToEnumT<LanguageTypeEnum>());
            EditorUtility.DisplayDialog("完成", "完成多语言读取", "确定");
            EditorPrefs.SetString("language_version", System.DateTime.Now.ToString("yyyyMMddHHmmss"));

        }

        /// <summary>
        /// 读取一个Excal文件
        /// </summary>
        /// <param name="xlsxPath"></param>
        /// <returns></returns>
        private static DataSet ReadExcal(string xlsxPath)
        {
            if (!File.Exists(xlsxPath))
            {
                Debug.LogError(xlsxPath + "下多语言Excal文件不存在");
                return null;
            }

            FileStream fs = new FileStream(xlsxPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(fs);
            DataSet dataSet = reader.AsDataSet();
            reader.Dispose();
            reader.Close();
            fs.Dispose();
            fs.Close();

            return dataSet;
        }
        /// <summary>
        /// 读取语言类型并设置为枚举
        /// </summary>
        /// <param name="dataTable">表</param>
        private static void ReadLanguageTypeEnum(DataTable dataTable)
        {
            int columnsCount = dataTable.Columns.Count;
            List<string> languageTypeEnumList = new List<string>();

            for (int i = 1; i < columnsCount; i++)
            {
                string value = dataTable.Rows[0][i].ToString();
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }
                if (languageTypeEnumList.Contains(value))
                {
                    Debug.LogError("相同语言类型" + value);
                    continue;
                }
                languageTypeEnumList.Add(value);
            }

            if (isNameSpace)
            {
                SetEnum(languageTypeEnumPath, languageTypeEnumList, languageTypeEnumName, nameSpace);
            }
            else
            {
                SetEnum(languageTypeEnumPath, languageTypeEnumList, languageTypeEnumName);
            }

            WriteChangeLanguage4Editor(languageTypeEnumList);
        }
        /// <summary>
        /// 读取语言枚举
        /// </summary>
        /// <param name="dataTable"></param>
        private static void ReadLanguageEnum(DataSet dataSet)
        {
            int tableNum = dataSet.Tables.Count;
            List<string> languageEnumList = new List<string>();
            for (int j = 0; j < tableNum; j++)
            {
                DataTable dataTable = dataSet.Tables[j];
                if (dataTable == null) continue;
                int rowCount = dataTable.Rows.Count;
                for (int i = 1; i < rowCount; i++)
                {
                    string value = dataTable.Rows[i][0].ToString();
                    if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                    {
                        continue;
                    }
                    if (languageEnumList.Contains(value))
                    {
                        Debug.LogError("相同语言" + value);
                        continue;
                    }
                    languageEnumList.Add(value);
                }

            }


            if (isNameSpace)
            {
                SetEnum(languageEnumPath, languageEnumList, languageEnumName, nameSpace);
            }
            else
            {
                SetEnum(languageEnumPath, languageEnumList, languageEnumName);
            }
        }
        /// <summary>
        /// 读取具体语言信息
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="data"></param>
        private static void ReadLanguageInfo(DataSet dataSet, LanguageData data)
        {
            int tableNum = dataSet.Tables.Count;//所有语言表

            for (int k = 0; k < tableNum; k++)
            {
                DataTable dataTable = dataSet.Tables[k];
                if (dataTable == null) continue;

                int rowCount = dataTable.Rows.Count;//行数
                int columnsCount = dataTable.Columns.Count;//列数
                for (int columnIndex = 1; columnIndex < columnsCount; columnIndex++)
                {
                    if (string.IsNullOrEmpty(dataTable.Rows[0][columnIndex].ToString())) continue;

                    if (data.language.Count >= (int)LanguageTypeEnum.LanguageTypeEnumCount)
                    {
                        for (int rowIndex = 1; rowIndex < rowCount; rowIndex++)
                        {
                            if (string.IsNullOrEmpty(dataTable.Rows[rowIndex][0].ToString())) continue;

                            LanguageEnum languageEnum = (LanguageEnum)System.Enum.Parse(typeof(LanguageEnum), dataTable.Rows[rowIndex][0].ToString());

                            LanguageValue languageValue = new LanguageValue()
                            {
                                languageEnumName = languageEnum.ToString(),
                                languageEnum = languageEnum,
                                value = dataTable.Rows[rowIndex][columnIndex].ToString()
                            };
                            data.language[columnIndex - 1].allValue.Add(languageValue);
                        }
                    }
                    else
                    {
                        LanguageTypeEnum languageTypeEnum = (LanguageTypeEnum)System.Enum.Parse(typeof(LanguageTypeEnum), dataTable.Rows[0][columnIndex].ToString());
                        LanguageType languageType = new LanguageType()
                        {
                            languageTypeName = languageTypeEnum.ToString(),
                            languageTypeEnum = languageTypeEnum,
                            allValue = new List<LanguageValue>()
                        };
                        for (int rowIndex = 1; rowIndex < rowCount; rowIndex++)
                        {
                            if (string.IsNullOrEmpty(dataTable.Rows[rowIndex][0].ToString())) continue;

                            LanguageEnum languageEnum = (LanguageEnum)System.Enum.Parse(typeof(LanguageEnum), dataTable.Rows[rowIndex][0].ToString());

                            LanguageValue languageValue = new LanguageValue()
                            {
                                languageEnumName = languageEnum.ToString(),
                                languageEnum = languageEnum,
                                value = dataTable.Rows[rowIndex][columnIndex].ToString()
                            };

                            languageType.allValue.Add(languageValue);
                        }
                        data.language.Add(languageType);
                    }
                }

            }
        }

        /// <summary>
        /// 写入一个枚举文件
        /// </summary>
        /// <param name="enumFilePath">文件路径</param>
        /// <param name="enumList">枚举列表</param>
        /// <param name="enumName">枚举名字</param>
        /// <param name="nameSpace">（可选）命名空间</param>
        private static void SetEnum(string enumFilePath, List<string> enumList, string enumName, string nameSpace = null)
        {
            StringBuilder areaString;
            if (nameSpace != null)
            {
                areaString = new StringBuilder("namespace " + nameSpace + "\n{\n\tpublic enum " + enumName + "\n\t{");
                string endContent = "\n\t}\n}";

                int count = enumList.Count;
                for (int i = 0; i < count; i++)
                {
                    areaString.Append("\n\t\t" + enumList[i] + ",");
                }

                areaString.Append("\n\t\t" + enumName + "Count");
                areaString.Append(endContent);
            }
            else
            {
                areaString = new StringBuilder("public enum " + enumName + "\n{");
                string endContent = "\n}";

                int count = enumList.Count;
                for (int i = 0; i < count; i++)
                {
                    areaString.Append("\n\t" + enumList[i] + ",");
                }

                areaString.Append("\n\t" + enumName + "Count");
                areaString.Append(endContent);
            }
            File.WriteAllText(enumFilePath, areaString.ToString());
        }
        private static void WriteChangeLanguage4Editor(List<string> enumList)
        {
            StringBuilder areaString;
            areaString = new StringBuilder("using UnityEditor;\n");
            areaString.Append("namespace " + nameSpace + "\n{\n\tpublic class ChangeLanguage4Editor\n\t{");
            string endContent = "\n\t}\n}";

            areaString.Append("\n\t\tstatic void ResetLanguage(string languageName)\n\t\t{");
            areaString.Append("\n\t\t\tEditorPrefs.SetString(\"language\", languageName);");
            areaString.Append("\n\t\t\tLanguageManager.Instance.Init(languageName.ToEnumT<LanguageTypeEnum>());\n\t\t}");

            int count = enumList.Count;
            for (int i = 0; i < count; i++)
            {
                areaString.Append($"\n\t\t[MenuItem(\"Languages/{enumList[i]}\", false, 8)]");
                areaString.Append("\n\t\tstatic void To" + enumList[i] + "()\n\t\t{");
                areaString.Append("\n\t\t\tResetLanguage(\"" + enumList[i] + "\");\n\t\t}");

                areaString.Append($"\n\t\t[MenuItem(\"Languages/{enumList[i]}\", true)]");
                areaString.Append("\n\t\tstatic bool Check" + enumList[i] + "Used()\n\t\t{");
                areaString.Append("\n\t\t\treturn EditorPrefs.GetString(\"language\")!= \"" + enumList[i] + "\";\n\t\t}");
            }
            areaString.Append(endContent);
            File.WriteAllText(Application.dataPath + "\\XGame\\Scripts\\Editor\\Language\\ChangeLanguage4Editor.cs", areaString.ToString());
        }

    }

    [InitializeOnLoad]
    public class UnityScriptCompiling : AssetPostprocessor
    {
        [UnityEditor.Callbacks.DidReloadScripts]
        public static void AllScriptsReloaded()
        {
            if (ReadLanguageData.HasReadInfo)
            {
                ReadLanguageData.OnCompileComplete();
            }
        }
    }
}