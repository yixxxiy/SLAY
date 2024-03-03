using System;
using UnityEditor;
using UnityEngine;

namespace EasyExcel
{
	internal class EEAboutWindow : EditorWindow
	{
		[MenuItem(@"Tools/EasyExcel/About", false, 500)]
		private static void OpenAboutWindow()
		{
			try
			{
				if (EditorApplication.isCompiling)
				{
					EELog.Log("Waiting for Compiling completed.");
					return;
				}
				var window = GetWindowWithRect<EEAboutWindow>(new Rect(0, 0, 480, 320), true, "About EasyExcel", true);
				window.Show();
			}
			catch (Exception e)
			{
				EELog.LogError(e.ToString());
			}
		}

		private Texture2D logo;
		
		private void Awake()
		{
			logo = Resources.Load<Texture2D>("logo");
		}

		private void OnGUI()
		{
			EEGUIStyle.Ensure();
			
			GUILayout.Space(10);
			GUILayout.Box(logo, EEGUIStyle.Box, GUILayout.Width(200), GUILayout.Height(124));
			GUILayout.Label("EasyExcel", EEGUIStyle.largeLabel);
			
			GUILayout.BeginHorizontal();
			GUILayout.Space(20);
			GUILayout.Label("Version " + EEConstant.Version, EEGUIStyle.label);
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			GUILayout.Space(20);
			GUILayout.Label("(c) 2018-2019 Locke. All rights reserved.", EEGUIStyle.label);
			GUILayout.EndHorizontal();
			
			GUILayout.Space(10);
			
			GUILayout.Label("Support", EEGUIStyle.boldLabel);
			
			GUILayout.BeginHorizontal();
			GUILayout.Space(20);
			if (GUILayout.Button("Asset Store", EEGUIStyle.link))
				Application.OpenURL("http://u3d.as/WsS");
			GUILayout.EndHorizontal();
			
			GUILayout.Space(5);
			
			GUILayout.BeginHorizontal();
			GUILayout.Space(20);
			if (GUILayout.Button("locke.indienova.com", EEGUIStyle.link))
				Application.OpenURL("https://locke.indienova.com/");
			GUILayout.EndHorizontal();
			
			GUILayout.Space(5);
			
			GUILayout.BeginHorizontal();
			GUILayout.Space(20);
			if (GUILayout.Button("Email 1534921818@qq.com", EEGUIStyle.link))
				Application.OpenURL("mailto:1534921818@qq.com");
			GUILayout.EndHorizontal();
		}
		
	}
}