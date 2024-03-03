
namespace EasyExcel
{
	/// <summary>
	/// Simple column types like int a, float b, string c...
	/// </summary>
	public class EEColumnFieldSystemType : EEColumnField
	{
		private readonly string fieldName;
		private readonly string fieldType;
		private readonly string propertyName;

		public EEColumnFieldSystemType(int columnIndex, string rawColumnName, string rawColumnType):
			base(columnIndex, rawColumnName, rawColumnType)
		{
			propertyName = isKeyField ? rawColumnName.Split(':')[0].Trim() : rawColumnName.Trim();
			fieldName = "_" + propertyName;
			fieldType = rawColumnType.Trim();
		}
		
		public override string GetDeclarationLines()
		{
			var stringBuilder = EEStringBuilderCache.Borrow();
			if (isKeyField)
				stringBuilder.Append("\t\t[EEKeyField]\n");
			stringBuilder.Append("\t\t[SerializeField]\n");
			stringBuilder.AppendFormat("\t\tprivate {0} {1};\n", fieldType, fieldName);
			stringBuilder.AppendFormat("\t\tpublic {0} {1} {{ get {{ return {2}; }} }}\n\n", fieldType, propertyName, fieldName);
			return EEStringBuilderCache.Return(stringBuilder);
		}
		
		public override string GetParseLines()
		{
			return "\t\t\tTryParse(sheet[row][column++], out " + fieldName + ");\n";
		}
	}

	/// <summary>
	/// Array types like int[] a, float[] b, string[] c...
	/// </summary>
	public class EEColumnFieldSystemTypeArray : EEColumnField
	{
		private readonly string fieldName;
		private readonly string fieldType;
		private readonly string propertyName;

		public EEColumnFieldSystemTypeArray(int columnIndex, string rawColumnName, string rawColumnType):
			base(columnIndex, rawColumnName, rawColumnType)
		{
			propertyName = rawColumnName.Trim();
			fieldName = "_" + propertyName;
			int startIndex = rawColumnType.IndexOf('[');
			fieldType = rawColumnType.Substring(0, startIndex).Trim();
		}
		
		public override string GetDeclarationLines()
		{
			var stringBuilder = EEStringBuilderCache.Borrow();
			stringBuilder.Append("\t\t[SerializeField]\n");
			stringBuilder.AppendFormat("\t\tprivate {0}[] {1};\n", fieldType, fieldName);
			stringBuilder.AppendFormat("\t\tpublic {0}[] {1} {{ get {{ return {2}; }} }}\n\n", fieldType, propertyName, fieldName);
			return EEStringBuilderCache.Return(stringBuilder);
		}

		public override string GetParseLines()
		{
			var stringBuilder = EEStringBuilderCache.Borrow();
			stringBuilder.Append("\t\t\tstring[] " + fieldName + "Array = sheet[row][column++].Split(\',\');" + "\n");
			stringBuilder.Append("\t\t\tint " + fieldName + "Count = " + fieldName + "Array.Length;" + "\n");
			stringBuilder.Append("\t\t\t" + fieldName + " = new " + fieldType + "[" + fieldName + "Count];\n");
			stringBuilder.Append("\t\t\tfor(int i = 0; i < " + fieldName + "Count; i++)\n");
			stringBuilder.Append("\t\t\t\tTryParse(" + fieldName + "Array[i], out " + fieldName + "[i]);\n");
			return EEStringBuilderCache.Return(stringBuilder);
		}
	}
	
	/// <summary>
	/// 基础类型的Dictionary
	/// 需要keyArray valueArray用来序列化
	/// </summary>
	public class EEColumnFieldSystemTypeDictionary : EEColumnField
	{
		private readonly string fieldName;
		private readonly string propertyName;
		private readonly string keyType;
		private readonly string valueType;

		public EEColumnFieldSystemTypeDictionary(int columnIndex, string rawColumnName, string rawColumnType):
			base(columnIndex, rawColumnName, rawColumnType)
		{
			propertyName = rawColumnName.Trim();
			fieldName = "_" + propertyName;
			int startIndex = rawColumnType.IndexOf('<');
			int sepIndex = rawColumnType.IndexOf(',');
			int endIndex = rawColumnType.IndexOf('>');
			keyType = rawColumnType.Substring(startIndex+1, sepIndex - startIndex - 1).Trim();
			valueType = rawColumnType.Substring(sepIndex + 1, endIndex - sepIndex - 1).Trim();
		}
		
		public override string GetDeclarationLines()
		{
			var stringBuilder = EEStringBuilderCache.Borrow();
			stringBuilder.Append("\t\t[SerializeField]\n");
			stringBuilder.AppendFormat("\t\tprivate {0}[] {1}Keys;\n", keyType, fieldName);
			stringBuilder.Append("\t\t[SerializeField]\n");
			stringBuilder.AppendFormat("\t\tprivate {0}[] {1}Values;\n", valueType, fieldName);
			stringBuilder.AppendFormat("\t\tprivate Dictionary<{0},{1}> {2} = new Dictionary<{0},{1}>();\n", keyType, valueType, fieldName);
			stringBuilder.AppendFormat("\t\tpublic Dictionary<{0},{1}> {2} {{ get {{ return {3}; }} }}\n\n", keyType, valueType, propertyName, fieldName);
			return EEStringBuilderCache.Return(stringBuilder);
		}

		public override string GetParseLines()
		{
			var stringBuilder = EEStringBuilderCache.Borrow();
			
			stringBuilder.AppendFormat("\t\t\tstring {0}RawData = sheet[row][column++];\n", fieldName);
			stringBuilder.AppendFormat("\t\t\tstring[] {0}Pairs = {0}RawData.Split(',');\n", fieldName);
			stringBuilder.AppendFormat("\t\t\tList<{1}> {0}KeysList = new List<{1}>();\n", fieldName, keyType);
			stringBuilder.AppendFormat("\t\t\tList<{1}> {0}ValuesList = new List<{1}>();\n", fieldName, valueType);
			stringBuilder.AppendFormat("\t\t\tfor (int i = 0; i < {0}Pairs.Length; ++i)\n", fieldName);
			stringBuilder.Append("\t\t\t{\n");
			stringBuilder.AppendFormat("\t\t\t\tstring {0}Pair = {0}Pairs[i];\n", fieldName);
			stringBuilder.AppendFormat("\t\t\t\tstring[] {0}Pairs2 = {0}Pair.Split(':');\n", fieldName);
			stringBuilder.AppendFormat("\t\t\t\tif ({0}Pairs2.Length < 2) continue;\n", fieldName);
			stringBuilder.AppendFormat("\t\t\t\t{0} {1}K;\n", keyType, fieldName);
			stringBuilder.AppendFormat("\t\t\t\tTryParse({0}Pairs2[0], out {0}K);\n", fieldName);
			stringBuilder.AppendFormat("\t\t\t\t{0} {1}V;\n", valueType, fieldName);
			stringBuilder.AppendFormat("\t\t\t\tTryParse({0}Pairs2[1], out {0}V);\n", fieldName);
			stringBuilder.AppendFormat("\t\t\t\t{0}KeysList.Add({0}K);\n", fieldName);
			stringBuilder.AppendFormat("\t\t\t\t{0}ValuesList.Add({0}V);\n", fieldName);
			stringBuilder.Append("\t\t\t}\n");
			stringBuilder.AppendFormat("\t\t\t{0}Keys = {0}KeysList.ToArray();\n", fieldName);
			stringBuilder.AppendFormat("\t\t\t{0}Values = {0}ValuesList.ToArray();\n", fieldName);
			
			return EEStringBuilderCache.Return(stringBuilder);
		}

		public override string GetAfterSerializedLines()
		{
			var stringBuilder = EEStringBuilderCache.Borrow();
			stringBuilder.AppendFormat("\t\t\tfor (int i = 0; i < {0}Keys.Length; ++i)\n", fieldName);
			stringBuilder.Append("\t\t\t{\n");
			stringBuilder.AppendFormat("\t\t\t\tvar k = {0}Keys[i];\n", fieldName);
			stringBuilder.AppendFormat("\t\t\t\tvar v = {0}Values[i];\n", fieldName);
			stringBuilder.AppendFormat("\t\t\t\tif ({0}.ContainsKey(k))\n", fieldName);
			stringBuilder.AppendFormat("\t\t\t\t\tEELog.LogError(\"Dictionary {0} already has the key \" + k + \".\");\n", fieldName);
			stringBuilder.Append("\t\t\t\telse\n");
			stringBuilder.AppendFormat("\t\t\t\t\t{0}.Add(k, v);\n", fieldName);
			stringBuilder.Append("\t\t\t}\n");
			return EEStringBuilderCache.Return(stringBuilder);
		}
	}
	
}