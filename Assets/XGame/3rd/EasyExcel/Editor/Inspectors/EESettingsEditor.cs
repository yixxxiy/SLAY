using System;
using UnityEditor;
using UnityEngine;

namespace EasyExcel
{
	public class EESettingsEditor : EditorWindow
	{
		[MenuItem("Tools/EasyExcel/Settings", false, 400)]
		public static void OpenSettingsWindow()
		{
			try
			{
				if (EditorApplication.isCompiling)
				{
					EELog.Log("Waiting for Compiling completed.");
					return;
				}
				var window = GetWindowWithRect<EESettingsEditor>(new Rect(0, 0, 520, 640), 
					true, "EasyExcel Settings", true);
				window.Show();
			}
			catch (Exception e)
			{
				EELog.LogError(e.ToString());
			}
		}

		private const int nameLength = 120;
		private const int valueLength = 350;
		private const int nameHeight = 18;
		private const int helpLength = 475;
		
		private EESettings settings;
		private GUILayoutOption[] nameOptions;
		private GUILayoutOption[] valueOptions;

		private void Awake()
		{
			settings = EESettings.Current;
			nameOptions = new[] {GUILayout.Width(nameLength), GUILayout.Height(nameHeight)};
			valueOptions = new[] {GUILayout.Width(valueLength), GUILayout.Height(nameHeight)};
		}

		private void OnGUI()
		{
			EEGUIStyle.Ensure();
			
			if (settings == null)
			{
				EditorGUILayout.HelpBox("Cannot find EasyExcel settings file", MessageType.Error);
				return;
			}

			const float tipSpace = 5;

			GUILayout.Space(5);
			GUILayout.Label(settings.Lang == EELang.CN ? "EasyExcel设置" : "EasyExcel Settings", EEGUIStyle.largeLabel);
			GUILayout.Space(5);
			
			GUILayout.BeginHorizontal();
			GUILayout.Label(settings.Lang == EELang.CN ? "语言" : "Language", EEGUIStyle.label, GUILayout.Width(65));
			var langType = (EELang) EEGUIStyle.EnumPopup(settings.Lang, GUILayout.Width(100));
			if (langType != settings.Lang)
				settings.Lang = langType;
			GUILayout.Space(30);
			settings.ShowHelp = GUILayout.Toggle(settings.ShowHelp, settings.Lang == EELang.CN ? "显示帮助" : "Show Help", GUILayout.Width(80));
			GUILayout.Space(30);
			if (GUILayout.Button(settings.Lang == EELang.CN ? "重置全部" : "Reset All", GUILayout.Width(100), GUILayout.Height(20)))
			{
				if (EditorUtility.DisplayDialog("EasyExcel", "Are you sure to reset ALL EasyExcel settings?", "Yes", "Cancel"))
				{
					EESettings.Current.ResetAll();
					EditorUtility.SetDirty(EESettings.Current);
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(10);
			
			GUILayout.Space(5);
			if (settings.ShowHelp)
				GUILayout.Label(GetSettingFieldComment("nameRowIndex"), EEGUIStyle.helpBox, GUILayout.Width(helpLength));
			GUILayout.BeginHorizontal();
			GUILayout.Label(settings.Lang == EELang.CN ? "字段名称行" : "Row of Name", EEGUIStyle.label, nameOptions);
			settings.NameRowIndex = EditorGUILayout.IntField(settings.NameRowIndex, EEGUIStyle.textField, valueOptions);
			GUILayout.EndHorizontal();
			if (settings.ShowHelp)
				GUILayout.Space(tipSpace);
			
			GUILayout.Space(5);
			if (settings.ShowHelp)
				GUILayout.Label(GetSettingFieldComment("typeRowIndex"), EEGUIStyle.helpBox, GUILayout.Width(helpLength));
			GUILayout.BeginHorizontal();
			GUILayout.Label(settings.Lang == EELang.CN ? "字段类型行" : "Row of Type", EEGUIStyle.label, nameOptions);
			settings.TypeRowIndex = EditorGUILayout.IntField(settings.TypeRowIndex, EEGUIStyle.textField, valueOptions);
			GUILayout.EndHorizontal();
			if (settings.ShowHelp)
				GUILayout.Space(tipSpace);
			
			GUILayout.Space(5);
			if (settings.ShowHelp)
				GUILayout.Label(GetSettingFieldComment("dataStartIndex"), EEGUIStyle.helpBox, GUILayout.Width(helpLength));
			GUILayout.BeginHorizontal();
			GUILayout.Label(settings.Lang == EELang.CN ? "数据开始行" : "Row of Data", EEGUIStyle.label, nameOptions);
			settings.DataStartIndex = EditorGUILayout.IntField(settings.DataStartIndex, EEGUIStyle.textField, valueOptions);
			GUILayout.EndHorizontal();
			if (settings.ShowHelp)
				GUILayout.Space(tipSpace);
			
			GUILayout.Space(5);
			if (settings.ShowHelp)
				GUILayout.Label(GetSettingFieldComment("useFileNameAsNameSpace"), EEGUIStyle.helpBox, GUILayout.Width(helpLength));
			GUILayout.BeginHorizontal();
			settings.UseFileNameAsNameSpace = GUILayout.Toggle(settings.UseFileNameAsNameSpace, settings.Lang == EELang.CN ? "使用文件名作为命名空间" : "Use File Name As Name Space");
			GUILayout.EndHorizontal();
			if (settings.ShowHelp)
				GUILayout.Space(tipSpace);
			
			GUILayout.Space(5);
			if (settings.UseFileNameAsNameSpace)
			{
				if (settings.ShowHelp)
					GUILayout.Label(GetSettingFieldComment("nameSpacePrefix"), EEGUIStyle.helpBox, GUILayout.Width(helpLength));
				GUILayout.BeginHorizontal();
				GUILayout.Label(settings.Lang == EELang.CN ? "命名空间前缀" : "Name Space Prefix", EEGUIStyle.label, nameOptions);
				settings.NameSpacePrefix = EditorGUILayout.TextField(settings.NameSpacePrefix, EEGUIStyle.textField, valueOptions);
				GUILayout.EndHorizontal();
			}
			else
			{
				if (settings.ShowHelp)
					GUILayout.Label(GetSettingFieldComment("nameSpace"), EEGUIStyle.helpBox, GUILayout.Width(helpLength));
				GUILayout.BeginHorizontal();
				GUILayout.Label(settings.Lang == EELang.CN ? "命名空间" : "Name Space", EEGUIStyle.label, nameOptions);
				settings.NameSpace = EditorGUILayout.TextField(settings.NameSpace, EEGUIStyle.textField, valueOptions);
				GUILayout.EndHorizontal();
			}
			if (settings.ShowHelp)
				GUILayout.Space(tipSpace);
			
			GUILayout.Space(5);
			if (settings.ShowHelp)
				GUILayout.Label(GetSettingFieldComment("sheetDataPostfix"), EEGUIStyle.helpBox, GUILayout.Width(helpLength));
			GUILayout.BeginHorizontal();
			GUILayout.Label(settings.Lang == EELang.CN ? "页数据类名后缀" : "SheetData Postfix", EEGUIStyle.label, nameOptions);
			settings.SheetDataPostfix = EditorGUILayout.TextField(settings.SheetDataPostfix, EEGUIStyle.textField, valueOptions);
			GUILayout.EndHorizontal();
			if (settings.ShowHelp)
				GUILayout.Space(tipSpace);
			
			GUILayout.Space(5);
			if (settings.ShowHelp)
				GUILayout.Label(GetSettingFieldComment("rowDataPostfix"), EEGUIStyle.helpBox, GUILayout.Width(helpLength));
			GUILayout.BeginHorizontal();
			GUILayout.Label(settings.Lang == EELang.CN ? "行数据类名后缀" : "RowData Postfix", EEGUIStyle.label, nameOptions);
			settings.RowDataPostfix = EditorGUILayout.TextField(settings.RowDataPostfix, EEGUIStyle.textField, valueOptions);
			GUILayout.EndHorizontal();
			if (settings.ShowHelp)
				GUILayout.Space(tipSpace);
			
			GUILayout.Space(5);
			if (settings.ShowHelp)
				GUILayout.Label(GetSettingFieldComment("generatedAssetPath"), EEGUIStyle.helpBox, GUILayout.Width(helpLength));
			GUILayout.BeginHorizontal();
			GUILayout.Label(settings.Lang == EELang.CN ? "生成资源文件路径" : "AssetPath", EEGUIStyle.label, nameOptions);
			settings.GeneratedAssetPath = EditorGUILayout.TextField(settings.GeneratedAssetPath, EEGUIStyle.textField, valueOptions);
			GUILayout.EndHorizontal();
			if (settings.ShowHelp)
				GUILayout.Space(tipSpace);
			
			GUILayout.Space(5);
			if (settings.ShowHelp)
				GUILayout.Label(GetSettingFieldComment("generatedScriptPath"), EEGUIStyle.helpBox, GUILayout.Width(helpLength));
			GUILayout.BeginHorizontal();
			GUILayout.Label(settings.Lang == EELang.CN ? "生成C#文件路径" : "CSharpPath", EEGUIStyle.label, nameOptions);
			settings.GeneratedScriptPath = EditorGUILayout.TextField(settings.GeneratedScriptPath, EEGUIStyle.textField, valueOptions);
			GUILayout.EndHorizontal();
			if (settings.ShowHelp)
				GUILayout.Space(tipSpace);
			
			
		}

		private string GetSettingFieldComment(string fieldName)
		{
			return EEUtility.GetFieldComment(typeof(EESettings), fieldName);
		}
		
	}
}