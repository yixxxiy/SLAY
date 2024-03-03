using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace EasyExcel
{
	public class EEWorkbookEditor : EditorWindow
	{
		private const string WindowName = "Workbook Editor";
		private EEWorkbook _currentEeWorkbook;
		private EEWorksheet _currentEeWorksheet;
		private int selectIndex;

		//[MenuItem("Tools/EasyExcel/Workbook Editor", false, 9)]
		private static void MenuOpenWindow()
		{
			try
			{
				if (EditorApplication.isCompiling)
				{
					EELog.Log("Waiting for Compiling completed.");
					return;
				}
				var window = GetWindow<EEWorkbookEditor>(WindowName);
				window.Show();
			}
			catch (Exception e)
			{
				EELog.LogError(e.ToString());
			}
		}

		public static void OpenWithFile(string filePath)
		{
			try
			{
				if (string.IsNullOrEmpty(filePath))
					return;
				var fullPath = filePath;
				if (!Path.IsPathRooted(filePath))
				{
					if (filePath.StartsWith("Assets/"))
						filePath = filePath.Substring("Assets/".Length);
					fullPath = Path.Combine(Application.dataPath, filePath);
				}

				if (!File.Exists(fullPath))
					return;
				var workbook = EEWorkbook.Load(fullPath);
				var window = GetWindow<EEWorkbookEditor>(WindowName);
				window.Show();
				window.SetWorkbook(workbook);
			}
			catch (Exception e)
			{
				EELog.LogError(e.ToString());
			}
		}

		private void SetWorkbook(EEWorkbook eeWorkbook)
		{
			_currentEeWorkbook = eeWorkbook;
			//Worksheet.SetCellTypeByColumn(1, CellType.Label);
			//Worksheet.SetCellTypeByColumn(3, CellType.Popup, new List<string> {"1", "2"});
		}

		private void Awake()
		{
			EEGUIStyle.Ensure();
		}

		private void OnGUI()
		{
			EEGUIStyle.Ensure();
			try
			{
				if (_currentEeWorkbook != null)
				{
					EditorDrawHelper.DrawTableTab(_currentEeWorkbook, ref selectIndex);
					_currentEeWorksheet = _currentEeWorkbook.sheets[selectIndex];
					EditorDrawHelper.DrawTable(_currentEeWorksheet);
					DrawMenus();
				}
			}
			catch (Exception e)
			{
				EELog.LogError(e.ToString());
			}
		}

		private void DrawMenus()
		{
			EditorGUILayout.BeginHorizontal();
			EditorDrawHelper.DrawButton("Add", delegate
			{
				_currentEeWorksheet.RowCount++;
				SetWorkbook(_currentEeWorkbook);
			});

			EditorDrawHelper.DrawButton("Save", delegate
			{
				var path = Application.dataPath + "/Test/Test3.xlsx";
				_currentEeWorkbook.SaveToFile(path);
			});
			EditorGUILayout.EndHorizontal();
		}

		/*[OnOpenAsset(10)]
		private static bool OnOpenExcelFile(int instanceId, int line)
		{
			try
			{
				var asset = EditorUtility.InstanceIDToObject(instanceId);
				if (asset == null)
					return false;
					var path = AssetDatabase.GetAssetPath(asset);
				if (!EEUtility.IsExcelFileSupported(path))
					return false;
				OpenWithFile(path);
				return true;
			}
			catch (Exception e)
			{
				EELog.LogError(e.ToString());
			}

			return false;
		}*/
		
	}
}