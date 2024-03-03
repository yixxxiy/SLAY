using System;
using UnityEditor;
using UnityEngine;

namespace EasyExcel
{
	public static class EditorDrawHelper
	{
		public static void DrawTableTab(EEWorkbook file, ref int selectIndex)
		{
			GUILayout.BeginHorizontal();
			{
				for (var i = 0; i < file.sheets.Count; i++)
					if (GUILayout.Toggle(selectIndex == i, file.sheets[i].name, EditorStyles.toolbarButton))
						selectIndex = i;
			}
			GUILayout.EndHorizontal();
		}

		public static void DrawTable(EEWorksheet sheet)
		{
			if (sheet == null) return;
			sheet.position = EditorGUILayout.BeginScrollView(sheet.position);
			for (var i = 0; i < sheet.RowCount; i++)
			{
				EditorGUILayout.BeginHorizontal();
				for (var j = 0; j < sheet.ColumnCount; j++) DrawCell(sheet, i, j);
				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.EndScrollView();
		}

		public static void DrawCell(EEWorksheet sheet, int row, int column)
		{
			var cell = sheet.GetCell(row, column);
			if (cell != null)
			{
				switch (cell.type)
				{
					case CellType.TextField:
						cell.value = EditorGUILayout.TextField(cell.value, EEGUIStyle.textFieldCell, GUILayout.MaxWidth(cell.width));
						break;
					case CellType.Label:
						EditorGUILayout.LabelField(cell.value, GUILayout.MaxWidth(cell.width));
						break;
					case CellType.Popup:
					{
						var selectIndex = cell.ValueSelected.IndexOf(cell.value);
						if (selectIndex < 0) selectIndex = 0;
						selectIndex = EditorGUILayout.Popup(selectIndex, cell.ValueSelected.ToArray(),
							GUILayout.MaxWidth(cell.width));
						cell.value = cell.ValueSelected[selectIndex];
						break;
					}
				}
			}
			else
			{
				var s = EditorGUILayout.TextField(sheet.GetCellValue(row, column));
				sheet.SetCellValue(row, column, s);
			}
		}

		public static void DrawButton(string title, Action onClick)
		{
			DrawButtonHorizontal(title, onClick);
		}

		public static void DrawButtonHorizontal(string title, Action onClick, bool horizontal = true)
		{
			if (horizontal) EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button(title))
				if (onClick != null && onClick.Target != null)
					onClick();
			if (horizontal) EditorGUILayout.EndHorizontal();
		}

		public static void DoHorizontal(Action callback)
		{
			EditorGUILayout.BeginHorizontal();
			if (callback != null && callback.Target != null)
				callback();
			EditorGUILayout.EndHorizontal();
		}
	}
}