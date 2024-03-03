using System;
using System.Collections.Generic;

namespace EasyExcel
{
	public abstract class EEField
	{
		protected string name;
		protected string type;

		public string Name => name;
		
		public abstract string GetDeclaration();
	}

	public class EEField_Basic : EEField
	{
		public static EEField_Basic Parse(string _name, string _typeInfo)
		{
			return new EEField_Basic()
			{
				name = _name,
				type = _typeInfo
			};
		}

		public override string GetDeclaration()
		{
			var stringBuilder = EEStringBuilderCache.Borrow();
			stringBuilder.AppendFormat("public {0} {1};", type, name);
			return EEStringBuilderCache.Return(stringBuilder);
		}
	}
	
	/// <summary>
	/// 列名：Reward
	/// 类型：{int a, bool b, float c, string d,}
	/// 自动生成类：class Reward{...}
	/// 本列的定义为：Reward Reward;
	/// </summary>
	public class EEFieldCustom : EEField
	{
		private bool isArray;
		
		public string TypeName => name + "Class";
		public string FieldName => "_" + name;
		public string PropertyName => name;
		
		public static EEFieldCustom Parse(string _name, string _typeInfo)
		{
			/*if (!_typeInfo.StartsWith("{") || !_typeInfo.EndsWith("}"))
			{
				EELog.LogError($"Type format error of Column {_name}: {_typeInfo} should start with {{ and end with }}.");
				return null;
			}*/
			int endIndex = _typeInfo.IndexOf("}", StringComparison.Ordinal);
			string sub = _typeInfo.Substring(1, endIndex-1);
			string[] subs = sub.Split(',');
			var fields = new List<EEField>();
			foreach (var subone in subs)
			{
				string one = subone.Trim();
				int spaceIndex = one.IndexOf(" ", StringComparison.Ordinal);
				string _nm = one.Substring(spaceIndex).Trim();
				string _tp = one.Substring(0, spaceIndex);
				var field = EEFieldParser.TryParse(_nm, _tp);
				if (field != null)
					fields.Add(field);
				else
				{
					EELog.LogError($"Type format error of Column {_name}: {one}.");
					break;
				}
			}
			
			return new EEFieldCustom
			{
				name = _name,
				isArray = _typeInfo.EndsWith("[]"),
				fields = fields
			};
		}

		public List<EEField> fields;
		
		public override string GetDeclaration()
		{
			var stringBuilder = EEStringBuilderCache.Borrow();
			string arr = isArray ? "[]" : "";
			stringBuilder.Append("\t\t[SerializeField]\n");
			stringBuilder.AppendFormat("\t\tprivate {0}{1} {2};\n", TypeName, arr, FieldName);
			stringBuilder.AppendFormat("\t\tpublic {0}{1} {2} {{ get {{ return {3}; }} }}\n\n", TypeName, arr, PropertyName, FieldName);
			return EEStringBuilderCache.Return(stringBuilder);
		}
		
		public string GetDefinition()
		{
			var stringBuilder = EEStringBuilderCache.Borrow();
			stringBuilder.AppendFormat("\t\t[Serializable]\n");
			stringBuilder.AppendFormat("\t\tpublic class {0}\n\t\t{{\n", TypeName);
			foreach (var field in fields)
				stringBuilder.AppendFormat("\t\t\t{0}\n", field.GetDeclaration());
			stringBuilder.Append("\t\t}\n");
			return EEStringBuilderCache.Return(stringBuilder);
		}

		private static class EEFieldParser
		{
			private delegate EEField ParseFunc(string name, string type);
		
			private static readonly Dictionary<string, ParseFunc> _typedParses = new Dictionary<string, ParseFunc>
			{
				{EEType.INT, EEField_Basic.Parse},
				{EEType.FLOAT, EEField_Basic.Parse},
				{EEType.DOUBLE, EEField_Basic.Parse},
				{EEType.LONG, EEField_Basic.Parse},
				{EEType.BOOL, EEField_Basic.Parse},
				{EEType.STRING, EEField_Basic.Parse},
			};
		
			public static EEField TryParse(string name, string type)
			{
				type = type.Trim();
				if (!EEColumnFieldParser.IsSystemType(type)) return null;
				_typedParses.TryGetValue(type, out var func);
				return func?.Invoke(name, type);
			}
		}
	}

	
}