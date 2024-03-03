using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;

namespace EasyExcel
{
	public static class EEMenu
	{
		[MenuItem(@"Tools/EasyExcel/Import", false, 100)]
		public static void ImportFolder()
		{
			var historyExcelPath = EditorPrefs.GetString(EEConverter.excelPathKey);
			if (string.IsNullOrEmpty(historyExcelPath) || !Directory.Exists(historyExcelPath))
			{
				var fallbackDir = Environment.CurrentDirectory + "/Assets/EasyExcel/Example/ExcelFiles";
				historyExcelPath = Directory.Exists(fallbackDir) ? fallbackDir : Environment.CurrentDirectory;
			}

			var excelPath = EditorUtility.OpenFolderPanel("Select the folder of excel files", historyExcelPath, "");
			if (string.IsNullOrEmpty(excelPath))
				return;
			
			EditorPrefs.SetString(EEConverter.excelPathKey, excelPath);
			EEConverter.GenerateCSharpFiles(excelPath, Environment.CurrentDirectory + "/" + EESettings.Current.GeneratedScriptPath);
		}

		[MenuItem(@"Tools/EasyExcel/Clean", false, 101)]
		public static void Clean()
		{
			EditorPrefs.SetBool(EEConverter.csChangedKey, false);

			DeleteCSFolder();
			DeleteScriptableObjectFolder();

			AssetDatabase.Refresh();
		}

		[DidReloadScripts]
		private static void OnScriptsReloaded()
		{
			if (!EditorPrefs.GetBool(EEConverter.csChangedKey, false)) return;
			EditorPrefs.SetBool(EEConverter.csChangedKey, false);
			var historyExcelPath = EditorPrefs.GetString(EEConverter.excelPathKey);
			if (string.IsNullOrEmpty(historyExcelPath)) return;
			EELog.Log("Scripts are reloaded, start generating assets...");
			EEConverter.GenerateScriptableObjects(historyExcelPath, Environment.CurrentDirectory + "/" + EESettings.Current.GeneratedAssetPath);
		}

		private static void DeleteCSFolder()
		{
			if (Directory.Exists(EESettings.Current.GeneratedScriptPath))
				Directory.Delete(EESettings.Current.GeneratedScriptPath, true);

			string csMeta = null;
			if (EESettings.Current.GeneratedScriptPath.EndsWith("/") || EESettings.Current.GeneratedScriptPath.EndsWith("\\"))
				csMeta = EESettings.Current.GeneratedScriptPath.Substring(0, EESettings.Current.GeneratedScriptPath.Length - 1) + ".meta";
			if (!string.IsNullOrEmpty(csMeta) && File.Exists(csMeta))
				File.Delete(csMeta);
		}

		private static void DeleteScriptableObjectFolder()
		{
			if (Directory.Exists(EESettings.Current.GeneratedAssetPath))
				Directory.Delete(EESettings.Current.GeneratedAssetPath, true);

			string asMeta = null;
			if (EESettings.Current.GeneratedAssetPath.EndsWith("/") || EESettings.Current.GeneratedAssetPath.EndsWith("\\"))
				asMeta = EESettings.Current.GeneratedAssetPath.Substring(0, EESettings.Current.GeneratedAssetPath.Length - 1) + ".meta";
			if (!string.IsNullOrEmpty(asMeta) && File.Exists(asMeta))
				File.Delete(asMeta);
		}
		
	}
}