
using System;

namespace EasyExcel
{
	/// <summary>
	/// Custom Type
	/// Format is {int a, float b, string c}
	/// </summary>
	public class EEColumnFieldCustomType : EEColumnField
	{
		private readonly string fieldName;
		private readonly string fieldType;
		private readonly string propertyName;

		private readonly EEFieldCustom custom;

		public EEColumnFieldCustomType(int columnIndex, string rawColumnName, string rawColumnType):
			base(columnIndex, rawColumnName, rawColumnType)
		{
			propertyName = rawColumnName.Trim();
			fieldName = "_" + propertyName;
			custom = EEFieldCustom.Parse(rawColumnName, rawColumnType);
		}
		
		public override string GetDeclarationLines()
		{
			if (custom == null) return null;
			var stringBuilder = EEStringBuilderCache.Borrow();
			stringBuilder.Append(custom.GetDefinition());
			stringBuilder.Append(custom.GetDeclaration());
			return EEStringBuilderCache.Return(stringBuilder);
		}

		public override string GetParseLines()
		{
			if (custom == null) return null;
			var stringBuilder = EEStringBuilderCache.Borrow();
			stringBuilder.AppendFormat("\t\t\t{0} = new {1}();\n", custom.FieldName, custom.TypeName);
			stringBuilder.AppendFormat("\t\t\tstring raw{0} = sheet[row][column++];\n", custom.PropertyName);
			stringBuilder.AppendFormat("\t\t\tstring[] subs{0} = raw{0}.Split(',');\n", custom.PropertyName);
			stringBuilder.AppendFormat("\t\t\tfor (int i = 0; i < subs{0}.Length; ++i)\n", custom.PropertyName);
			stringBuilder.Append("\t\t\t{\n");
			stringBuilder.AppendFormat("\t\t\t\tvar strValue = subs{0}[i];\n", custom.PropertyName);
			for (int i = 0; i < custom.fields.Count; ++i)
			{
				var field = custom.fields[i];
				stringBuilder.AppendFormat("\t\t\t\t{0}if (i == {1})\n", (i == 0 ? "" : "else "), i);
				stringBuilder.AppendFormat("\t\t\t\t\tTryParse(strValue, out {0}.{1});\n", custom.PropertyName, field.Name);
			}
			stringBuilder.Append("\t\t\t}\n");
			
			return EEStringBuilderCache.Return(stringBuilder);
		}

	}
	
	public class EEColumnFieldCustomTypeArray : EEColumnField
	{
		private readonly string fieldName;
		private readonly string fieldType;
		private readonly string propertyName;

		private readonly EEFieldCustom custom;

		public EEColumnFieldCustomTypeArray(int columnIndex, string rawColumnName, string rawColumnType):
			base(columnIndex, rawColumnName, rawColumnType)
		{
			propertyName = rawColumnName.Trim();
			fieldName = "_" + propertyName;
			custom = EEFieldCustom.Parse(rawColumnName, rawColumnType);
		}
		
		public override string GetDeclarationLines()
		{
			if (custom == null) return null;
			var stringBuilder = EEStringBuilderCache.Borrow();
			stringBuilder.Append(custom.GetDefinition());
			stringBuilder.Append(custom.GetDeclaration());
			return EEStringBuilderCache.Return(stringBuilder);
		}

		public override string GetParseLines()
		{
			if (custom == null) return null;
			var stringBuilder = EEStringBuilderCache.Borrow();
			
			stringBuilder.AppendFormat("\t\t\tstring raw{0} = sheet[row][column++];\n", custom.PropertyName);
			stringBuilder.AppendFormat("\t\t\tstring[] subs{0}_0 = raw{0}.Split(';');\n", custom.PropertyName);
			stringBuilder.AppendFormat("\t\t\t{0} = new {1}[subs{2}_0.Length];\n", custom.FieldName, custom.TypeName, custom.PropertyName);
			stringBuilder.AppendFormat("\t\t\tfor (int j = 0; j < subs{0}_0.Length; ++j)\n", custom.PropertyName);
			stringBuilder.Append("\t\t\t{\n");
			stringBuilder.AppendFormat("\t\t\t\tvar {0}one = new {1}();\n", custom.FieldName, custom.TypeName);
			stringBuilder.AppendFormat("\t\t\t\t{0}[j] = {0}one;\n", custom.FieldName);
			
			stringBuilder.AppendFormat("\t\t\t\tstring[] subs{0}_1 = subs{0}_0[j].Split(',');\n", custom.PropertyName);
			stringBuilder.AppendFormat("\t\t\t\tfor (int i = 0; i < subs{0}_1.Length; ++i)\n", custom.PropertyName);
			stringBuilder.Append("\t\t\t\t{\n");
			stringBuilder.AppendFormat("\t\t\t\t\tvar strValue = subs{0}_1[i];\n", custom.PropertyName);
			for (int i = 0; i < custom.fields.Count; ++i)
			{
				var field = custom.fields[i];
				stringBuilder.AppendFormat("\t\t\t\t\t{0}if (i == {1})\n", (i == 0 ? "" : "else "), i);
				stringBuilder.AppendFormat("\t\t\t\t\t\tTryParse(strValue, out {0}one.{1});\n", custom.FieldName, field.Name);
			}
			stringBuilder.Append("\t\t\t\t}\n");
			
			stringBuilder.Append("\t\t\t}\n");
			
			return EEStringBuilderCache.Return(stringBuilder);
		}
		
	}
	
	/// <summary>
	/// 自定义类型的Dictionary
	/// 需要keyArray valueArray用来序列化
	/// </summary>
	public class EEColumnFieldCustomTypeDictionary : EEColumnField
	{
		private readonly string fieldName;
		private readonly string propertyName;
		private readonly string keyType;
		private readonly string valueType;
		
		private readonly string fieldType;
		private readonly EEFieldCustom custom;

		public EEColumnFieldCustomTypeDictionary(int columnIndex, string rawColumnName, string rawColumnType):
			base(columnIndex, rawColumnName, rawColumnType)
		{
			propertyName = rawColumnName.Trim();
			fieldName = "_" + propertyName;
			int startIndex = rawColumnType.IndexOf('<');
			int sepIndex = rawColumnType.IndexOf(',');
			int endIndex = rawColumnType.IndexOf('>');
			keyType = rawColumnType.Substring(startIndex+1, sepIndex - startIndex - 1).Trim();
			valueType = rawColumnType.Substring(sepIndex + 1, endIndex - sepIndex - 1).Trim();
			// ??截断后解析
			int startCustom = rawColumnType.IndexOf("{", StringComparison.Ordinal);
			int endCustom = rawColumnType.IndexOf("}", StringComparison.Ordinal);
			string customType = rawColumnType.Substring(startCustom, endCustom - startCustom + 1);
			custom = EEFieldCustom.Parse(rawColumnName, customType);
			valueType = custom.TypeName;
		}
		
		public override string GetDeclarationLines()
		{
			var stringBuilder = EEStringBuilderCache.Borrow();
			stringBuilder.AppendFormat("{0}", custom.GetDefinition());
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
			stringBuilder.AppendFormat("\t\t\tstring[] {0}Pairs = {0}RawData.Split(';');\n", fieldName);
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

			stringBuilder.AppendFormat("\t\t\t\t{0}V = new {1}();\n", fieldName, custom.TypeName);
			stringBuilder.AppendFormat("\t\t\t\tstring raw{0} = {0}Pairs2[1];\n", fieldName);
			stringBuilder.AppendFormat("\t\t\t\tstring[] subs{0} = raw{1}.Split(',');\n", custom.PropertyName, fieldName);
			stringBuilder.AppendFormat("\t\t\t\tfor (int j = 0; j < subs{0}.Length; ++j)\n", custom.PropertyName);
			stringBuilder.Append("\t\t\t\t{\n");
			stringBuilder.AppendFormat("\t\t\t\t\tvar strValue = subs{0}[j];\n", custom.PropertyName);
			for (int i = 0; i < custom.fields.Count; ++i)
			{
				var field = custom.fields[i];
				stringBuilder.AppendFormat("\t\t\t\t\t{0}if (j == {1})\n", (i == 0 ? "" : "else "), i);
				stringBuilder.AppendFormat("\t\t\t\t\t\tTryParse(strValue, out {0}V.{1});\n", fieldName, field.Name);
			}
			stringBuilder.Append("\t\t\t\t}\n");
			
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