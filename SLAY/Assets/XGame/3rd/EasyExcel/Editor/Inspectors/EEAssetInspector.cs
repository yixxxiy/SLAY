using UnityEngine;
using UnityEditor;

namespace EasyExcel
{
	public abstract class EEAssetInspector : Editor
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
			
			var dataCollection = target as EERowDataCollection;
			if (dataCollection != null)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Box(Logo, EEGUIStyle.Box, GUILayout.Width(58), GUILayout.Height(36));
				EditorGUILayout.HelpBox(@"This file is generated from " + dataCollection.ExcelFileName, MessageType.Info);
				GUILayout.EndHorizontal();

				var prevGUIState = GUI.enabled;
				GUI.enabled = false;
				EditorGUILayout.LabelField("Element Count:  " + dataCollection.GetDataCount());
				base.OnInspectorGUI();
				GUI.enabled = prevGUIState;
			}
			else
			{
				base.OnInspectorGUI();
			}
		}
	}
}
