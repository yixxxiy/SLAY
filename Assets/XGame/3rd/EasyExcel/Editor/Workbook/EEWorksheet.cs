using System;
using System.Collections.Generic;
using OfficeOpenXml;
using UnityEngine;

namespace EasyExcel
{
	public class EEWorksheet
	{
		public string name;
		
		private readonly Dictionary<int, Dictionary<int, EEWorksheetCell>> cells =
			new Dictionary<int, Dictionary<int, EEWorksheetCell>>();

		private int _columnCount;
		
		private int _rowCount;

		public int ColumnCount
		{
			get { return _columnCount; }
			set { _columnCount = value; }
		}

		public int RowCount
		{
			get { return _rowCount; }
			set { _rowCount = value; }
		}
		
		public Vector2 position;
		

		public EEWorksheet()
		{
			RowCount = 0;
			ColumnCount = 0;
		}

		public EEWorksheet(ExcelWorksheet sheet)
		{
			name = sheet.Name;
			if (sheet.Dimension != null)
			{
				RowCount = sheet.Dimension.Rows;
				ColumnCount = sheet.Dimension.Columns;
			}
			else//empty Sheet
			{
				RowCount = 0;
				ColumnCount = 0;
			}

			for (var row = 0; row < RowCount; row++)
			for (var column = 0; column < ColumnCount; column++)
			{
				var cellValue = sheet.Cells[row + 1, column + 1].Value;
				var value = cellValue == null ? "" : cellValue.ToString();
				SetCellValue(row, column, value);
			}
		}
		
		public void CopyTo(ExcelWorksheet sheet)
		{
			if (sheet == null) return;
			for (var row = 0; row < RowCount; row++)
				for (var column = 0; column < ColumnCount; column++)
					sheet.Cells[row + 1, column + 1].Value = GetCellValue(row, column);
		}

		/// <summary>
		/// Set cell's value
		/// </summary>
		/// <param name="row">Row of target cell, from 0 to RowCount</param>
		/// <param name="column">Column of target cell, from 0 to ColumnCount</param>
		/// <param name="value">Value of string to set</param>
		public EEWorksheetCell SetCellValue(int row, int column, string value)
		{
			if (row < 0 || column < 0)
				return null;

			if (RowCount < row)
				RowCount = row + 1;
			if (ColumnCount < column)
				ColumnCount = column + 1;

			Dictionary<int, EEWorksheetCell> targetRow;
			if (!cells.TryGetValue(row, out targetRow))
			{
				targetRow = new Dictionary<int, EEWorksheetCell>();
				cells.Add(row, targetRow);
			}

			EEWorksheetCell targetCell;
			if (!targetRow.TryGetValue(column, out targetCell))
			{
				targetCell = new EEWorksheetCell(row, column, value);
				targetRow.Add(column, targetCell);
			}

			return targetCell;
		}

		/// <summary>
		/// Get value from cell
		/// </summary>
		/// <param name="row">Row of target cell, from 0 to RowCount</param>
		/// <param name="column">Column of target, from 0 to ColumnCount</param>
		public string GetCellValue(int row, int column)
		{
			if (row < 0 || column < 0)
				return null;
			var cell = GetCell(row, column);
			return cell != null ? cell.value : null;
		}

		public EEWorksheetCell GetCell(int row, int column)
		{
			Dictionary<int, EEWorksheetCell> targetRow;
			if (cells.TryGetValue(row, out targetRow))
			{
				EEWorksheetCell targetCell;
				if (targetRow.TryGetValue(column, out targetCell))
					return targetCell;
			}
			
			return null;
		}

		/*public void SetCellTypeByRow(int row, CellType type)
		{
			for (var column = 0; column < columnCount; column++)
			{
				var cell = GetCell(row, column);
				if (cell != null) cell.type = type;
			}
		}

		public void SetCellTypeByColumn(int column, CellType type, List<string> values = null)
		{
			for (var row = 1; row <= rowCount; row++)
			{
				var cell = GetCell(row, column);
				if (cell == null) continue;
				cell.type = type;
				if (values != null) cell.ValueSelected = values;
			}
		}*/

		public void Dump()
		{
			try
			{
				var msg = "";
				for (var row = 0; row < RowCount; row++)
				{
					for (var column = 0; column < ColumnCount; column++)
						msg += string.Format("{0} ", GetCellValue(row, column));
					msg += "\n";
				}
				Debug.Log(msg);
			}
			catch (Exception e)
			{
				EELog.LogError(e.ToString());
			}
			
		}
	}
}