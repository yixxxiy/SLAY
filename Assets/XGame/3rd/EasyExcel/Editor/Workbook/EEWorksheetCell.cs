using System.Collections.Generic;

namespace EasyExcel
{
	public enum CellType
	{
		None = 0,
		TextField = 1,
		Label = 2,
		Popup = 3
	}

	public class EEWorksheetCell
	{
		public readonly int column;
		public readonly int row;
		public float width = 50f;
		public CellType type = CellType.TextField;
		public string value;
		public List<string> ValueSelected = new List<string>();

		public EEWorksheetCell(int row, int column, string value)
		{
			this.row = row;
			this.column = column;
			this.value = value;
		}
	}
}