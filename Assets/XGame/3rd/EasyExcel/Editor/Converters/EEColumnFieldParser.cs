using System;

namespace EasyExcel
{
	public static class EEType
	{
		public const string INT = "int";
		public const string FLOAT = "float";
		public const string DOUBLE = "double";
		public const string LONG = "long";
		public const string STRING = "string";
		public const string BOOL = "bool";
	}
	
	public static class EEColumnFieldParser
	{
		private const string KEY_FLAG = ":key";
		private static readonly string[] systemTypes = { EEType.INT, EEType.FLOAT, EEType.DOUBLE, EEType.LONG, EEType.STRING, EEType.BOOL }; 
		
		public static EEColumnField Parse(int columnIndex, string rawColumnName, string rawColumnType)
		{
			try
			{
				rawColumnType = rawColumnType.Trim();
				if (IsSystemType(rawColumnType))
					return new EEColumnFieldSystemType(columnIndex, rawColumnName, rawColumnType);
				if (IsSystemTypeArray(rawColumnType))
					return new EEColumnFieldSystemTypeArray(columnIndex, rawColumnName, rawColumnType);
				if (IsSystemTypeDictionary(rawColumnType))
					return new EEColumnFieldSystemTypeDictionary(columnIndex, rawColumnName, rawColumnType);
				if (IsCustomType(rawColumnType))
					return new EEColumnFieldCustomType(columnIndex, rawColumnName, rawColumnType);
				if (IsCustomTypeArray(rawColumnType))
					return new EEColumnFieldCustomTypeArray(columnIndex, rawColumnName, rawColumnType);
				if (IsCustomTypeDictionary(rawColumnType))
					return new EEColumnFieldCustomTypeDictionary(columnIndex, rawColumnName, rawColumnType);
				if (!string.IsNullOrEmpty(rawColumnName) && !string.IsNullOrEmpty(rawColumnType))
					EELog.LogError($"Failed to parse column \"{rawColumnName}\" with type \"{rawColumnType}\".");
			}
			catch (Exception e)
			{
				EELog.LogError(e.ToString());
			}

			return null;
		}
		
		public static bool IsSupportedType(string rawColumnType)
		{
			if (string.IsNullOrEmpty(rawColumnType)) return false;
			
			var lowerRawType = rawColumnType.ToLower().Trim();
			
			if (IsSystemType(lowerRawType)) return true;
			if (IsSystemTypeArray(lowerRawType)) return true;
			if (IsSystemTypeDictionary(lowerRawType)) return true;
			if (IsCustomType(lowerRawType)) return true;
			if (IsCustomTypeArray(lowerRawType)) return true;
			if (IsCustomTypeDictionary(lowerRawType)) return true;
			
			return false;
		}

		public static bool IsKeyColumn(string rawColumnName, string rawColumnType)
		{
			string lowerTrimmedName = rawColumnName.ToLower().Trim();
			if (!lowerTrimmedName.EndsWith(KEY_FLAG))
				return false;
			if (!rawColumnType.Equals("int") && !rawColumnType.Equals("string"))
			{
				EELog.LogError(
					$"Only columns with type int or string can be key column, but {rawColumnName}'s type is {rawColumnType}.");
				return false;
			}
			return true;
		}
		
		public static bool IsSystemType(string lowerRawType)
		{
			foreach (var type in systemTypes)
				if (lowerRawType.Equals(type))
					return true;
			return false;
		}

		private static bool IsSystemTypeArray(string lowerRawType)
		{
			if (!lowerRawType.EndsWith("[]"))
				return false;
			int startIndex = lowerRawType.IndexOf('[');
			string typeName = lowerRawType.Substring(0, startIndex);
			if (string.IsNullOrEmpty(typeName))
				return false;
			foreach (var type in systemTypes)
				if (typeName.Equals(type))
					return true;
			return false;
		}

		// Format is <key-type,value-type>
		private static bool IsSystemTypeDictionary(string lowerRawType)
		{
			if (!lowerRawType.StartsWith("<") || !lowerRawType.EndsWith(">") 
				|| lowerRawType.Contains("{") || lowerRawType.Contains("}"))
				return false;
			int startIndex = lowerRawType.IndexOf('<');
			int sepIndex = lowerRawType.IndexOf(',');
			int endIndex = lowerRawType.IndexOf('>');
			return startIndex == 0 && sepIndex > 0 && endIndex > sepIndex;
		}
		
		// Format is {int a, float b, string c}
		private static bool IsCustomType(string lowerRawType)
		{
			/*if (lowerRawType.Contains("[") || lowerRawType.Contains("]"))
				return false;
			int startIndex = lowerRawType.IndexOf('{');
			int endIndex = lowerRawType.IndexOf('}');
			return startIndex == 0 && endIndex > startIndex;*/
			return lowerRawType.StartsWith("{") && lowerRawType.EndsWith("}");
		}

		// Format is {int a, float b, string c}[]
		private static bool IsCustomTypeArray(string lowerRawType)
		{
			/*if (!lowerRawType.EndsWith("[]"))
				return false;
			int startIndex = lowerRawType.IndexOf('{');
			int endIndex = lowerRawType.IndexOf('}');
			return startIndex == 0 && endIndex > startIndex;*/
			return lowerRawType.StartsWith("{") && lowerRawType.EndsWith("}[]");
		}
		
		// Format is <key-type,{int a, float b, string c}>
		private static bool IsCustomTypeDictionary(string lowerRawType)
		{
			if (!lowerRawType.StartsWith("<") || !lowerRawType.EndsWith("}>"))
				return false;
			return lowerRawType.Contains(",{") || lowerRawType.Contains(", {");
		}
	}
	
}