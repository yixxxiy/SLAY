using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;

namespace EasyExcel
{
	/// <summary>
	///     Excel Converter
	/// </summary>
	public static partial class EEConverter
	{
		public static void GenerateCSharpFiles(string excelPath, string csPath)
		{
			try
			{
				excelPath = excelPath.Replace("\\", "/");
				csPath = csPath.Replace("\\", "/");

				if (!Directory.Exists(excelPath))
				{
					EditorUtility.DisplayDialog("EasyExcel", "Excel files path doesn't exist.", "OK");
					return;
				}

				if (!Directory.Exists(csPath))
				{
					Directory.CreateDirectory(csPath);
					Directory.CreateDirectory(csPath + "/Editor/");
				}

				var tmpPath = Environment.CurrentDirectory + "/EasyExcelTmp/";
				var tmpEditorPath = Environment.CurrentDirectory + "/EasyExcelTmp/Editor/";
				if (Directory.Exists(tmpPath))
					Directory.Delete(tmpPath, true);
				Directory.CreateDirectory(tmpPath);
				Directory.CreateDirectory(tmpEditorPath);

				excelPath = excelPath.Replace("\\", "/");
				csPath = csPath.Replace("\\", "/");
				if (!csPath.EndsWith("/"))
					csPath += "/";

				var csChanged = false;
				var filePaths = Directory.GetFiles(excelPath);
				for (var i = 0; i < filePaths.Length; ++i)
				{
					var excelFilePath = filePaths[i].Replace("\\", "/");
					if (i + 1 < filePaths.Length)
						UpdateProgressBar(i + 1, filePaths.Length, "");
					else
						ClearProgressBar();
					if (!IsExcelFile(excelFilePath))
						continue;
					string fileName = Path.GetFileName(excelFilePath);
					var newCsDict = ToCSharpArray(excelFilePath);
					foreach (var newCs in newCsDict)
					{
						var cSharpFileName = EESettings.Current.GetCSharpFileName(fileName, newCs.Key);
						var tmpCsFilePath = tmpPath + cSharpFileName;
						var csFilePath = csPath + cSharpFileName;
						var shouldWrite = true;
						if (File.Exists(csFilePath))
						{
							var oldCs = File.ReadAllText(csFilePath);
							shouldWrite = oldCs != newCs.Value;
						}

						if (!shouldWrite)
							continue;
						csChanged = true;
						File.WriteAllText(tmpCsFilePath, newCs.Value, Encoding.UTF8);
					}
					var newInspectorDict = ToCSharpInspectorArray(excelFilePath);
					foreach (var newCs in newInspectorDict)
					{
						var inspectorFileName = EESettings.Current.GetSheetInspectorFileName(fileName, newCs.Key);
						var tmpInspFilePath = tmpEditorPath + inspectorFileName;
						var csFilePath = csPath + "Editor/" + inspectorFileName;
						var shouldWrite = true;
						if (File.Exists(csFilePath))
						{
							var oldCs = File.ReadAllText(csFilePath);
							shouldWrite = oldCs != newCs.Value;
						}

						if (!shouldWrite)
							continue;
						csChanged = true;
						File.WriteAllText(tmpInspFilePath, newCs.Value, Encoding.UTF8);
					}
				}

				EEStringBuilderCache.Reset();
				
				if (csChanged)
				{
					EditorPrefs.SetBool(csChangedKey, true);
					var files = Directory.GetFiles(tmpPath);
					foreach (var s in files)
					{
						var p = s.Replace("\\", "/");
						File.Copy(s, csPath + p.Substring(p.LastIndexOf("/", StringComparison.Ordinal)), true);
					}
					files = Directory.GetFiles(tmpEditorPath);
					foreach (var s in files)
					{
						var p = s.Replace("\\", "/");
						File.Copy(s, csPath + "Editor/" + p.Substring(p.LastIndexOf("/", StringComparison.Ordinal)), true);
					}
					AssetDatabase.Refresh();
					EELog.Log("Scripts are generated, wait for generating assets...");
				}
				else
				{
					EELog.Log("No CSharp files changed, begin generating assets...");
					ClearProgressBar();
					var historyExcelPath = EditorPrefs.GetString(excelPathKey);
					if (!string.IsNullOrEmpty(historyExcelPath))
						GenerateScriptableObjects(historyExcelPath, Environment.CurrentDirectory + "/" + EESettings.Current.GeneratedAssetPath);
				}
				
				if (Directory.Exists(tmpPath))
					Directory.Delete(tmpPath, true);
			}
			catch (Exception e)
			{
				EELog.LogError(e.ToString());
				EditorPrefs.SetBool(csChangedKey, false);
				ClearProgressBar();
				AssetDatabase.Refresh();
			}
		}
		
		private static Dictionary<string, string> ToCSharpArray(string excelPath)
		{
			var lst = new Dictionary<string, string>();
			var book = EEWorkbook.Load(excelPath);
			if (book == null)
				return lst;
			string fileName = Path.GetFileName(excelPath);
			foreach (var sheet in book.sheets)
			{
				if (sheet == null)
					continue;
				if (!IsValidSheet(sheet))
				{
					EELog.Log(string.Format("Skipped sheet {0} in file {1}.", sheet.name, fileName));
					continue;
				}
				var sheetData = ToSheetData(sheet);
				var csTxt = ToCSharp(sheetData, sheet.name, fileName);
				lst.Add(sheet.name, csTxt);
			}
			return lst;
		}

		private static string ToCSharp(SheetData sheetData, string sheetName, string fileName)
		{
			try
			{
				var rowDataClassName = EESettings.Current.GetRowDataClassName(fileName, sheetName);
				var sheetClassName = EESettings.Current.GetSheetClassName(fileName, sheetName);
				var csFile = new StringBuilder(2048);
				csFile.Append("//------------------------------------------------------------------------------\n");
				csFile.Append("// <auto-generated>\n");
				csFile.Append("//     This code was generated by EasyExcel.\n");
				csFile.Append("//     Runtime Version: " + EEConstant.Version + "\n");
				csFile.Append("//\n");
				csFile.Append("//     Changes to this file may cause incorrect behavior and will be lost if\n");
				csFile.Append("//     the code is regenerated.\n");
				csFile.Append("// </auto-generated>\n");
				csFile.Append("//------------------------------------------------------------------------------");
				csFile.Append("\nusing System;\nusing System.Collections.Generic;\nusing UnityEngine;\nusing EasyExcel;\n\n");
				csFile.Append(string.Format("namespace {0}\n", EESettings.Current.GetNameSpace(fileName)));
				csFile.Append("{\n");
				csFile.Append("\t[Serializable]\n");
				csFile.Append("\tpublic class " + rowDataClassName + " : EERowData\n");
				csFile.Append("\t{\n");

				bool keyFieldFound = false;
				var columnCount = sheetData.ColumnCount;
				var columnFields = new EEColumnField[columnCount];
				for (var col = 0; col < columnCount; col++)
				{
					var rawColumnName = sheetData.Get(EESettings.Current.NameRowIndex, col);
					var rawColumnType = sheetData.Get(EESettings.Current.TypeRowIndex, col);
					var ret = EEColumnFieldParser.Parse(col, rawColumnName, rawColumnType);
					columnFields[col] = ret;
					if (ret != null && ret.isKeyField)
						keyFieldFound = true;
				}
				
				if (!keyFieldFound)
					EELog.LogError("Cannot find Key column in sheet " + sheetName);
				
				for (var col = 0; col < columnCount; col++)
				{
					var columnField = columnFields[col];
					if (columnField == null)
						continue;
					csFile.Append(columnField.GetDeclarationLines());
				}
				
				csFile.AppendFormat("\n\t\tpublic {0}()\n", rowDataClassName);
				csFile.Append("\t\t{\n");
				csFile.Append("\t\t}\n");
				csFile.Append("\n#if UNITY_EDITOR\n");
				csFile.AppendFormat("\t\tpublic {0}(List<List<string>> sheet, int row, int column)\n", rowDataClassName);
				csFile.Append("\t\t{\n");
				
				for (var col = 0; col < columnCount; col++)
				{
					var columnField = columnFields[col];
					if (columnField == null)
						continue;
					csFile.Append(columnField.GetParseLines());
				}

				csFile.Append("\t\t}\n#endif\n");
				
				csFile.Append("\t\tpublic override void OnAfterSerialized()\n");
				csFile.Append("\t\t{\n");
				for (var col = 0; col < columnCount; col++)
				{
					var columnField = columnFields[col];
					if (columnField == null)
						continue;
					csFile.Append(columnField.GetAfterSerializedLines());
				}
				csFile.Append("\t\t}\n");
				csFile.Append("\t}\n\n");

				// EERowDataCollection class
				csFile.Append("\tpublic class " + sheetClassName + " : EERowDataCollection\n");
				csFile.Append("\t{\n");
				csFile.AppendFormat("\t\t[SerializeField]\n\t\tprivate List<{0}> elements = new List<{0}>();\n\n", rowDataClassName);

				csFile.AppendFormat("\t\tpublic override void AddData(EERowData data)\n\t\t{{\n\t\t\telements.Add(data as {0});\n\t\t}}\n\n", rowDataClassName);
				csFile.Append("\t\tpublic override int GetDataCount()\n\t\t{\n\t\t\treturn elements.Count;\n\t\t}\n\n");
				csFile.Append("\t\tpublic override EERowData GetData(int index)\n\t\t{\n\t\t\treturn elements[index];\n\t\t}\n\n");
				csFile.Append("\t\tpublic override void OnAfterSerialized()\n\t\t{\n");
				csFile.Append("\t\t\tforeach (var element in elements)\n");
				csFile.Append("\t\t\t\telement.OnAfterSerialized();\n");
				csFile.Append("\t\t}\n");

				csFile.Append("\t}\n");
				
				csFile.Append("}\n");

				return csFile.ToString();
			}
			catch (Exception ex)
			{
				EELog.LogError(ex.ToString());
			}

			return "";
		}
		
		private static Dictionary<string, string> ToCSharpInspectorArray(string excelPath)
		{
			var lst = new Dictionary<string, string>();
			var book = EEWorkbook.Load(excelPath);
			if (book == null)
				return lst;
			string fileName = Path.GetFileName(excelPath);
			foreach (var sheet in book.sheets)
			{
				if (sheet == null)
					continue;
				if (!IsValidSheet(sheet))
					continue;
				var csTxt = ToCSharpInspector(sheet.name, fileName);
				lst.Add(sheet.name, csTxt);
			}
			return lst;
		}
		
		private static string ToCSharpInspector(string sheetName, string fileName)
		{
			try
			{
				var inspectorClassName = EESettings.Current.GetSheetInspectorClassName(fileName, sheetName);
				var csFile = new StringBuilder(1024);
				csFile.Append("//------------------------------------------------------------------------------\n");
				csFile.Append("// <auto-generated>\n");
				csFile.Append("//     This code was generated by EasyExcel.\n");
				csFile.Append("//     Runtime Version: " + EEConstant.Version + "\n");
				csFile.Append("//\n");
				csFile.Append("//     Changes to this file may cause incorrect behavior and will be lost if\n");
				csFile.Append("//     the code is regenerated.\n");
				csFile.Append("// </auto-generated>\n");
				csFile.Append("//------------------------------------------------------------------------------");

				csFile.Append("\nusing UnityEditor;\nusing EasyExcel;\n\n");
				csFile.Append(string.Format("namespace {0}\n", EESettings.Current.GetNameSpace(fileName)));
				csFile.Append("{\n");
				csFile.Append(string.Format("\t[CustomEditor(typeof({0}))]\n", EESettings.Current.GetSheetClassName(fileName, sheetName) /*sheetName, EESettings.Current.SheetDataPostfix*/));
				csFile.Append("\tpublic class " + inspectorClassName + " : EEAssetInspector\n");
				csFile.Append("\t{\n");
				csFile.Append("\t}\n");
				csFile.Append("}\n");
				
				return csFile.ToString();
			}
			catch (Exception ex)
			{
				EELog.LogError(ex.ToString());
			}

			return "";
		}
	}
}