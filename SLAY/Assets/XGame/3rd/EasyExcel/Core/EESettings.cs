using System;
using System.IO;
using UnityEngine;

namespace EasyExcel
{
	/// <summary>
	/// EasyExcel setting
	/// </summary>
	//[CreateAssetMenu(fileName = "New EasyExcel Settings", menuName = "EasyExcel/Settings", order = 999)]
	public class EESettings : ScriptableObject
	{
		private const string EESettingsSavedFileName = "EasyExcelSettings";
		private const string EESettingsFileExtension = ".asset";
		private const string EESettingsSavedPath = "Assets/Resources/" + EESettingsSavedFileName + EESettingsFileExtension;
		
		[EEComment(
			"This row of an excel sheet is Name (starting from 0)",
			"指定这一行作为字段名（行号从0开始）")]
		[SerializeField]
		private int nameRowIndex;
		
		[EEComment(
			"This row of an excel sheet is Type (starting from 0)",
			"指定这一行作为字段类型（行号从0开始）")]
		[SerializeField]
		private int typeRowIndex;
		
		[EEComment(
			"This row of an excel sheet is where real data starts (starting from 0)",
			"指定这一行作为数据开始的行（行号从0开始）")]
		[SerializeField]
		private int dataStartIndex;
		
		[EEComment("This is where the generated ScriptableObject files will be",
			"生成的ScriptableObject文件的目标路径")]
		[SerializeField]
		private string generatedAssetPath;
		
		[EEComment("This is where the generated csharp files will be",
			"生成的C#文件的目标路径")]
		[SerializeField]
		private string generatedScriptPath;
		
		[EEComment(@"Postfix of generated sheet data classes",
			"生成的Excel页数据类名的前缀")]
		[SerializeField]
		private string sheetDataPostfix;
		
		[EEComment(@"Postfix of generated row data classes",
			"生成的Excel行数据类名的后缀")]
		[SerializeField]
		private string rowDataPostfix;

		[EEComment("The namespace of generated classes from excel files",
			"生成的C#类的命名空间")]
		[SerializeField]
		private string nameSpace;
		
		[EEComment("Use the name of the excel file as the namespace of generated classes from that excel file",
			"使用Excel文件名作为生成的C#类的命名空间（用于不同文件中包含相同数据页名）")]
		[SerializeField]
		private bool useFileNameAsNameSpace;
		
		[EEComment(@"Prefix of the namespace from the name of the excel file",
			"Excel文件名命名空间附加前缀（防止命名冲突）")]
		[SerializeField]
		private string nameSpacePrefix;

		[SerializeField]
		private EELang lang = EELang.EN;
		
		[NonSerialized] public bool ShowHelp = true;

		#region Properties

		private bool modified
		{
			set
			{
#if UNITY_EDITOR
				if (value)
					UnityEditor.EditorUtility.SetDirty(this);
#endif
			}
		}
		
		public int NameRowIndex
		{
			get { return nameRowIndex; }
			set
			{
				if (nameRowIndex == value)
					return;
				nameRowIndex = value;
				modified = true;
			}
		}

		public int TypeRowIndex
		{
			get { return typeRowIndex; }
			set
			{
				if (typeRowIndex == value)
					return;
				typeRowIndex = value;
				modified = true;
			}
		}
		
		public int DataStartIndex
		{
			get { return dataStartIndex; }
			set
			{
				if (dataStartIndex == value)
					return;
				dataStartIndex = value;
				modified = true;
			}
		}
		
		public string GeneratedAssetPath
		{
			get { return generatedAssetPath; }
			set
			{
				if (generatedAssetPath == value)
					return;
				generatedAssetPath = value;
				modified = true;
			}
		}
		
		public string GeneratedScriptPath
		{
			get { return generatedScriptPath; }
			set
			{
				if (generatedScriptPath == value)
					return;
				generatedScriptPath = value;
				modified = true;
			}
		}
		
		public string SheetDataPostfix
		{
			get { return sheetDataPostfix; }
			set
			{
				if (sheetDataPostfix == value)
					return;
				sheetDataPostfix = value;
				modified = true;
			}
		}
		
		public string RowDataPostfix
		{
			get { return rowDataPostfix; }
			set
			{
				if (rowDataPostfix == value)
					return;
				rowDataPostfix = value;
				modified = true;
			}
		}

		public string NameSpace
		{
			get { return nameSpace; }
			set
			{
				if (nameSpace == value)
					return;
				nameSpace = value;
				modified = true;
			}
		}
		
		public string NameSpacePrefix
		{
			get { return nameSpacePrefix; }
			set
			{
				if (nameSpacePrefix == value)
					return;
				nameSpacePrefix = value;
				modified = true;
			}
		}

		public bool UseFileNameAsNameSpace
		{
			get { return useFileNameAsNameSpace; }
			set
			{
				if (useFileNameAsNameSpace == value)
					return;
				useFileNameAsNameSpace = value;
				modified = true;
			}
		}
		
		public EELang Lang
		{
			get { return lang; }
			set
			{
				if (lang == value)
					return;
				lang = value;
				modified = true;
			}
		}
		
		#endregion

		public string GetNameSpace(string fileName)
		{
			return UseFileNameAsNameSpace ? NameSpacePrefix + Path.GetFileNameWithoutExtension(fileName) : NameSpace;
		}
		
		public void ResetAll()
		{
			NameRowIndex = 1;
			TypeRowIndex = 2;
			DataStartIndex = 3;
			GeneratedAssetPath = "Assets/Resources/EasyExcelGeneratedAsset/";
			GeneratedScriptPath = "Assets/EasyExcel/Example/AutoGenCode/";
			SheetDataPostfix = "_Sheet";
			RowDataPostfix = "";
			NameSpace = "EasyExcelGenerated";
			NameSpacePrefix = "EasyExcel_";
			UseFileNameAsNameSpace = false;
		}

		#region Sigleton

		private static EESettings _current;
		
		public static EESettings Current
		{
			get
			{
				if (_current != null) return _current;
				_current = Resources.Load<EESettings>(EESettingsSavedFileName);
				if (_current != null) return _current;
				_current = CreateInstance<EESettings>();
				_current.ResetAll();
#if UNITY_EDITOR
				var resourcesPath = Application.dataPath + "/Resources";
				if (!Directory.Exists(resourcesPath))
				{
					Directory.CreateDirectory(resourcesPath);
					UnityEditor.AssetDatabase.Refresh();
				}

				UnityEditor.AssetDatabase.CreateAsset(_current, EESettingsSavedPath);
#endif
				return _current;
			}
		}

		#endregion

		public string GetRowDataClassName(string fileName, string sheetName, bool includeNameSpace = false)
		{
			return (includeNameSpace? GetNameSpace(fileName) + "." : null) + sheetName + RowDataPostfix;
		}

		public string GetSheetClassName(string fileName, string sheetName)
		{
			return Path.GetFileNameWithoutExtension(fileName) + "_" + sheetName + SheetDataPostfix;
		}
		
		public string GetSheetInspectorClassName(string fileName, string sheetName)
		{
			return Path.GetFileNameWithoutExtension(fileName) + "_" + sheetName + "Inspector";
		}

		public string GetAssetFileName(string fileName, string sheetName)
		{
			return Path.GetFileNameWithoutExtension(fileName) + "_" + sheetName + SheetDataPostfix + EESettingsFileExtension;
		}
		
		public string GetCSharpFileName(string fileName, string sheetName)
		{
			// The file name must not differ from the sheet class name
			return GetSheetClassName(fileName, sheetName) + ".cs";
		}
		
		public string GetSheetInspectorFileName(string fileName, string sheetName)
		{
			return GetSheetInspectorClassName(fileName, sheetName) + ".cs";
		}

		public string GetSheetName(Type sheetClassType)
		{
			string fullName = sheetClassType.Name;
			string[] parts = fullName.Split('.');
			string lastPart = parts[parts.Length - 1];
			lastPart = lastPart.Substring(lastPart.IndexOf('_') + 1);
			return string.IsNullOrEmpty(SheetDataPostfix) ? 
				lastPart : 
				lastPart.Substring(0, lastPart.IndexOf(SheetDataPostfix, StringComparison.Ordinal));
		}
	}
}