using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace EasyExcel
{
	public static class EELog
	{
		public static void Log(string message)
		{
			Debug.Log("[EasyExcel] " + message);
		}
		
		public static void LogWarning(string message)
		{
			Debug.LogWarning("[EasyExcel] " + message);
		}

		public static void LogError(string message)
		{
			Debug.LogError("[EasyExcel] " + message);
		}
	}
	
	public static class EEUtility
	{
		public static bool IsExcelFileSupported(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
				return false;
			var fileName = Path.GetFileName(filePath);
			if (fileName.Contains("~$"))// avoid temporary files
				return false;
			var lower = Path.GetExtension(filePath).ToLower();
			return lower == ".xlsx" || lower == ".xls" || lower == ".xlsm";
		}
		
		public static string GetFieldComment(Type classType, string fieldName)
		{
			try
			{
				var fld = classType.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				return fld.GetCustomAttributes(typeof(EECommentAttribute), true)[0] is EECommentAttribute comment ? comment.content : null;
			}
			catch
			{
				// ignored
			}

			return null;
		}

		public static FieldInfo GetRowDataKeyField(Type rowDataType)
		{
			var fields = rowDataType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			var keyField = (from fieldInfo in fields let attrs = fieldInfo.GetCustomAttributes(typeof(EEKeyFieldAttribute), false) 
				where attrs.Length > 0 select fieldInfo).FirstOrDefault();
			return keyField;
		}

		public static Assembly GetSourceAssembly()
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var assembly in assemblies)
			{
				var types = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(EERowDataCollection)));
				var dataCollectionTypes = types as Type[] ?? types.ToArray();
				if (dataCollectionTypes.Any())
					return assembly;
			}

			return typeof(EERowDataCollection).Assembly;
		}
	}
}