using System;
using UnityEngine;

namespace EasyExcel
{
	/// <summary>
	///     One row in an excel sheet.
	/// </summary>
	[Serializable]
	public abstract class EERowData
	{
		public object GetKeyFieldValue()
		{
			var keyField = EEUtility.GetRowDataKeyField(GetType());
			return keyField == null ? null : keyField.GetValue(this);
		}

		public abstract void OnAfterSerialized();
		
		protected bool TryParse(string raw, out string ret)
		{
			ret = raw;
			return true;
		}
		
		protected bool TryParse(string raw, out int ret)
		{
			return int.TryParse(raw, out ret);
		}
		
		protected bool TryParse(string raw, out float ret)
		{
			return float.TryParse(raw, out ret);
		}
		
		protected bool TryParse(string raw, out double ret)
		{
			return double.TryParse(raw, out ret);
		}
		
		protected bool TryParse(string raw, out bool ret)
		{
			return bool.TryParse(raw, out ret);
		}
		
		protected bool TryParse(string raw, out long ret)
		{
			return long.TryParse(raw, out ret);
		}
	}

	/// <summary>
	///     All RowData in an excel sheet
	/// </summary>
	public abstract class EERowDataCollection : ScriptableObject
	{
		public string ExcelFileName;
		public string ExcelSheetName;
		public string KeyFieldName;
		public abstract void AddData(EERowData data);
		public abstract int GetDataCount();
		public abstract EERowData GetData(int index);
		public abstract void OnAfterSerialized();
	}
	
	public static class EEConstant
	{
		public const string Version = "4.0";
	}

	/// <summary>
	/// 	Mark which field of class is key
	/// </summary>
	public class EEKeyFieldAttribute : Attribute
	{
		
	}
	
	[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
	public class EnumLabelAttribute : Attribute
	{
		private int[] order = new int[0];
		public readonly string label;

		public EnumLabelAttribute(string label)
		{
			this.label = label;
		}

		public EnumLabelAttribute(string label, params int[] order)
		{
			this.label = label;
			this.order = order;
		}
	}
	
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class EECommentAttribute : Attribute
	{
		public string content
		{
			get
			{
				return EESettings.Current.Lang == EELang.CN ? contentCN : contentEN;		
			}
		}

		private readonly string contentEN;
		private readonly string contentCN;
		
		public EECommentAttribute(string textEN, string textCN)
		{
			contentEN = textEN;
			contentCN = textCN;
		}
	}

	public enum EELang
	{
		[EnumLabel("English")]
		EN,
		[EnumLabel("中文")]
		CN
	}
	
}