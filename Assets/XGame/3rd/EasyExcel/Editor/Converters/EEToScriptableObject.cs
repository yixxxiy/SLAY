using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EasyExcel
{
	/// <summary>
	///     Excel Converter
	/// </summary>
	public static partial class EEConverter
	{
		public static void GenerateScriptableObjects(string xlsxPath, string assetPath)
		{
			try
			{
				xlsxPath = xlsxPath.Replace("\\", "/");
				assetPath = assetPath.Replace("\\", "/");

				if (!Directory.Exists(xlsxPath))
				{
					EditorUtility.DisplayDialog("EasyExcel", "Xls/xlsx path doesn't exist.", "OK");
					return;
				}

				xlsxPath = xlsxPath.Replace("\\", "/");
				assetPath = assetPath.Replace("\\", "/");
				if (!assetPath.EndsWith("/"))
					assetPath += "/";
				if (Directory.Exists(assetPath))
					Directory.Delete(assetPath, true);
				Directory.CreateDirectory(assetPath);
				AssetDatabase.Refresh();

				var filePaths = Directory.GetFiles(xlsxPath);
				var count = 0;
				for (var i = 0; i < filePaths.Length; ++i)
				{
					var filePath = filePaths[i].Replace("\\", "/");
					if (!IsExcelFile(filePath)) continue;
					UpdateProgressBar(i, filePaths.Length, "");
					ToScriptableObject(filePath, assetPath);
					count++;
				}

				EELog.Log("Assets are generated successfully.");

				ClearProgressBar();
				AssetDatabase.Refresh();
				EELog.Log(string.Format("Import done. {0} sheets were imported.", count));
			}
			catch (Exception e)
			{
				EELog.LogError(e.ToString());
				ClearProgressBar();
				AssetDatabase.Refresh();
			}
		}
		
		private static void ToScriptableObject(string excelPath, string outputPath)
		{
			try
			{
				var book = EEWorkbook.Load(excelPath);
				if (book == null)
					return;
				foreach (var sheet in book.sheets)
				{
					if (sheet == null)
						continue;
					if (!IsValidSheet(sheet))
						continue;
					//var sheetData = ToSheetData(sheet);
					var sheetData = ToSheetDataRemoveEmptyColumn(sheet);
					ToScriptableObject(excelPath, sheet.name, outputPath, sheetData);
				}
			}
			catch (Exception e)
			{
				EELog.LogError(e.ToString());
				AssetDatabase.Refresh();
			}
		}

		private static void ToScriptableObject(string excelPath, string sheetName, string outputPath, SheetData sheetData)
		{
			try
			{
				string fileName = Path.GetFileName(excelPath);
				var sheetClassName = EESettings.Current.GetSheetClassName(fileName, sheetName);
				var asset = ScriptableObject.CreateInstance(sheetClassName);
				var dataCollect = asset as EERowDataCollection;
				if (dataCollect == null)
					return;
				
				dataCollect.ExcelFileName = fileName;
				dataCollect.ExcelSheetName = sheetName;
				var className = EESettings.Current.GetRowDataClassName(fileName, sheetName, true);
				var dataType = Type.GetType(className);
				if (dataType == null)
				{
					var assemblies = AppDomain.CurrentDomain.GetAssemblies();
					foreach (var assembly in assemblies)
					{
						dataType = assembly.GetType(className);
						if (dataType != null)
							break;
					}
				}
				if (dataType == null)
				{
					EELog.LogError(className + " not exist !");
					return;
				}

				var dataCtor = dataType.GetConstructor(new []{typeof(List<List<string>>), typeof(int), typeof(int)});
				if (dataCtor == null)
					return;
				var keySet = new HashSet<object>();
				for (var row = EESettings.Current.DataStartIndex; row < sheetData.RowCount; ++row)
				{
					for (var col = 0; col < sheetData.ColumnCount; ++col)
						sheetData.Set(row, col, sheetData.Get(row, col).Replace("\n", "\\n"));

					var inst = dataCtor.Invoke(new object[]{sheetData.Table, row, 0}) as EERowData;
					if (inst == null)
						continue;
					
					var key = inst.GetKeyFieldValue();
					if (key == null)
					{
						EELog.LogError("The value of key is null in sheet " + sheetName);
						continue;
					}
					
					if (key is int i && i == 0)
						continue;
					
					if (key is string s && string.IsNullOrEmpty(s))
						continue;
					
					if (!keySet.Contains(key))
					{
						dataCollect.AddData(inst);
						keySet.Add(key);
					}
					else
						EELog.LogError(string.Format("More than one rows have the same Key [{0}] in Sheet {1}", key, sheetName));
				}

				var keyField = EEUtility.GetRowDataKeyField(dataType);
				if (keyField != null)
					dataCollect.KeyFieldName = keyField.Name;

				var itemPath = outputPath + EESettings.Current.GetAssetFileName(fileName, sheetName);
				itemPath = itemPath.Substring(itemPath.IndexOf("Assets", StringComparison.Ordinal));
				AssetDatabase.CreateAsset(asset, itemPath);

				AssetDatabase.Refresh();
			}
			catch (Exception ex)
			{
				EELog.LogError(ex.ToString());
			}
		}

	}
}