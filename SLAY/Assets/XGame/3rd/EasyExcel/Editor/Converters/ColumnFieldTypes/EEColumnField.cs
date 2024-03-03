
namespace EasyExcel
{
	/// <summary>
	/// Abstract Column field
	/// </summary>
	public abstract class EEColumnField
	{
		protected readonly int rawColumnIndex;
		protected readonly string rawFiledName;
		protected readonly string rawFieldType;
		public readonly bool isKeyField;
		
		protected EEColumnField(int columnIndex, string rawColumnName, string rawColumnType)
		{
			rawColumnIndex = columnIndex;
			rawFiledName = rawColumnName;
			rawFieldType = rawColumnType;
			isKeyField = EEColumnFieldParser.IsKeyColumn(rawColumnName, rawColumnType);
		}
		
		public abstract string GetDeclarationLines();

		public abstract string GetParseLines();

		public virtual string GetAfterSerializedLines()
		{
			return null;
		}
	}

}