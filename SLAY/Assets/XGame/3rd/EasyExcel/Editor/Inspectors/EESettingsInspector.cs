using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace EasyExcel
{
	[CustomEditor(typeof(EESettings))]
	public class EESettingsInspector : Editor
	{
		private static Texture2D _logo;

		private static Texture2D Logo
		{
			get
			{
				if (_logo == null)
					_logo = Resources.Load<Texture2D>("logo");
				return _logo;
			}
		}
		
		public override void OnInspectorGUI()
		{
			EEGUIStyle.Ensure();
			
			GUILayout.BeginHorizontal();
			GUILayout.Box(Logo, EEGUIStyle.Box, GUILayout.Width(58), GUILayout.Height(36));
			EditorGUILayout.HelpBox(@"To modify this, open the settings window.", MessageType.Info);
			GUILayout.EndHorizontal();
			EditorGUILayout.Separator();
			
			var prevGUIState = GUI.enabled;
			GUI.enabled = false;
			base.OnInspectorGUI();
			GUI.enabled = prevGUIState;
			
			EditorGUILayout.Separator();
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Edit", GUILayout.Width(100), GUILayout.Height(20)))
				EESettingsEditor.OpenSettingsWindow();
			GUILayout.Space(50);
			if (GUILayout.Button("Reset", GUILayout.Width(100), GUILayout.Height(20)))
			{
				if (EditorUtility.DisplayDialog("EasyExcel", "Are you sure to reset it?", "Yes", "Cancel"))
				{
					EESettings.Current.ResetAll();
					EditorUtility.SetDirty(EESettings.Current);
				}
			}
			GUILayout.EndHorizontal();
		}
		
		[OnOpenAsset(10)]
		private static bool OnOpenExcelFile(int instanceId, int line)
		{
			try
			{
				var asset = EditorUtility.InstanceIDToObject(instanceId) as EESettings;
				if (asset == null)
					return false;
				EESettingsEditor.OpenSettingsWindow();
				return true;
			}
			catch (Exception e)
			{
				EELog.LogError(e.ToString());
			}

			return false;
		}
		
	}
}