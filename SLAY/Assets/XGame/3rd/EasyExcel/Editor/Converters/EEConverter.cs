using System;
using System.Collections.Generic;
using UnityEditor;

namespace EasyExcel
{
	/// <summary>
	///     Excel Converter
	/// </summary>
	public static partial class EEConverter
	{
		public const string excelPathKey = "EasyExcelExcelPath";
		public const string csChangedKey = "EasyExcelCSChanged";
		
		private static SheetData ToSheetData(EEWorksheet sheet)
		{
			var sheetData = new SheetData();
			for (var i = 0; i < sheet.RowCount; i++)
			{
				var rowData = new List<string>();
				for (var j = 0; j < sheet.ColumnCount; j++)
				{
					var cell = sheet.GetCell(i, j);
					rowData.Add(cell != null ? cell.value : "");
				}

				sheetData.Table.Add(rowData);
			}

			sheetData.RowCount = sheet.RowCount;
			sheetData.ColumnCount = sheet.ColumnCount;

			return sheetData;
		}
		
		private static SheetData ToSheetDataRemoveEmptyColumn(EEWorksheet sheet)
		{
			var validNameColumns = new List<int>();
			for (var column = 0; column < sheet.ColumnCount; column++)
			{
				var cellValue = sheet.GetCellValue(EESettings.Current.NameRowIndex, column);
				if (!string.IsNullOrEmpty(cellValue))
					validNameColumns.Add(column);
			}
			var validTypeColumns = new List<int>();
			for (var column = 0; column < validNameColumns.Count; column++)
			{
				var cellValue = sheet.GetCellValue(EESettings.Current.TypeRowIndex, validNameColumns[column]);
				if (EEColumnFieldParser.IsSupportedType(cellValue))
					validTypeColumns.Add(validNameColumns[column]);
			}
			
			var sheetData = new SheetData();
			for (var i = 0; i < sheet.RowCount; i++)
			{
				var rowData = new List<string>();
				foreach (var c in validTypeColumns)
				{
					var cell = sheet.GetCell(i, c);
					rowData.Add(cell != null ? cell.value : "");
				}

				sheetData.Table.Add(rowData);
			}

			sheetData.RowCount = sheet.RowCount;
			sheetData.ColumnCount = validTypeColumns.Count;

			return sheetData;
		}
		
		private static bool IsValidSheet(EEWorksheet sheet)
		{
			if (sheet == null || sheet.RowCount <= EESettings.Current.TypeRowIndex || sheet.ColumnCount < 1)
				return false;
			int validColumnCount = 0;
			for (int col = 0; col < sheet.ColumnCount; col++)
			{
				string varType = sheet.GetCellValue(EESettings.Current.TypeRowIndex, col);
				if (string.IsNullOrEmpty(varType) || varType.Equals(" ")  || varType.Equals("\r"))
					continue;
				if (EEColumnFieldParser.IsSupportedType(varType))
				{
					string varName = sheet.GetCellValue(EESettings.Current.NameRowIndex, col);
					if (!string.IsNullOrEmpty(varName))
						validColumnCount++;
				}
			}
			
			return validColumnCount > 0;
		}

		private static bool IsExcelFile(string filePath)
		{
			return EEUtility.IsExcelFileSupported(filePath);
		}

		private static bool isDisplayingProgress;
		
		private static void UpdateProgressBar(int progress, int progressMax, string desc)
		{
			var title = "EasyExcel importing...[" + progress + " / " + progressMax + "]";
			var value = progress / (float) progressMax;
			EditorUtility.DisplayProgressBar(title, desc, value);
			isDisplayingProgress = true;
		}

		private static void ClearProgressBar()
		{
			if (!isDisplayingProgress) return;
			try
			{
				EditorUtility.ClearProgressBar();
			}
			catch (Exception)
			{
				// ignored
			}

			isDisplayingProgress = false;
		}

		private class SheetData
		{
			private readonly List<List<string>> table = new List<List<string>>();
			public int ColumnCount;
			public int RowCount;

			public List<List<string>> Table => table;

			public string Get(int row, int column)
			{
				return table[row][column];
			}

			public void Set(int row, int column, string value)
			{
				table[row][column] = value;
			}
		}
	}
}