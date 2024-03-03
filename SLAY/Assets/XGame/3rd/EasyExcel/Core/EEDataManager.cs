using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace EasyExcel
{
	using RowDataDictInt = Dictionary<int, EERowData>;
	using RowDataDictStr = Dictionary<string, EERowData>;
	
	public class EEDataManager
	{
		private readonly IEEDataLoader dataLoader;
		private readonly Dictionary<Type, RowDataDictInt> dataCollectionDictInt = new Dictionary<Type, RowDataDictInt>();
		private readonly Dictionary<Type, RowDataDictStr> dataCollectionDictStr = new Dictionary<Type, RowDataDictStr>();
		
		public EEDataManager(IEEDataLoader _dataLoader = null)
		{
			dataLoader = _dataLoader ?? new EEDataLoaderResources();
		}
		
		#region Find with key
		
		public T Get<T>(int key) where T : EERowData
		{
			return Get(key, typeof(T)) as T;
		}
		
		public T Get<T>(string key) where T : EERowData
		{
			return Get(key, typeof(T)) as T;
		}

		public EERowData Get(int key, Type type)
		{
			RowDataDictInt soDic;
			dataCollectionDictInt.TryGetValue(type, out soDic);
			if (soDic == null) return null;
			EERowData data;
			soDic.TryGetValue(key, out data);
			return data;
		}
		
		public EERowData Get(string key, Type type)
		{
			RowDataDictStr soDic;
			dataCollectionDictStr.TryGetValue(type, out soDic);
			if (soDic == null) return null;
			EERowData data;
			soDic.TryGetValue(key, out data);
			return data;
		}

		public List<T> GetList<T>() where T : EERowData
		{
			RowDataDictInt dictInt;
			dataCollectionDictInt.TryGetValue(typeof(T), out dictInt);
			if (dictInt != null)
			{
				var list = new List<T>();
				foreach (var data in dictInt)
					list.Add((T) data.Value);
				return list;
			}
			RowDataDictStr dictStr;
			dataCollectionDictStr.TryGetValue(typeof(T), out dictStr);
			if (dictStr != null)
			{
				var list = new List<T>();
				foreach (var data in dictStr)
					list.Add((T) data.Value);
				return list;
			}
			return null;
		}

		public List<EERowData> GetList(Type type)
		{
			RowDataDictInt dictInt;
			dataCollectionDictInt.TryGetValue(type, out dictInt);
			if (dictInt != null)
				return dictInt.Values.ToList();
			RowDataDictStr dictStr;
			dataCollectionDictStr.TryGetValue(type, out dictStr);
			if (dictStr != null)
				return dictStr.Values.ToList();
			return null;
		}
		
		#endregion

		#region Load Assets
		
		public void Load()
		{
#if UNITY_EDITOR
			if (!EESettings.Current.GeneratedAssetPath.Contains("/Resources/"))
			{
				UnityEditor.EditorUtility.DisplayDialog("EasyExcel",
					string.Format(
						"AssetPath of EasyExcel Settings MUST be in Resources folder.\nCurrent is {0}.",
						EESettings.Current.GeneratedAssetPath), "OK");
				return;
			}
#endif
			dataCollectionDictInt.Clear();
			dataCollectionDictStr.Clear();

			var assembly = EEUtility.GetSourceAssembly();
			var types = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(EERowDataCollection)));
			foreach (var dataCollectionType in types)
				ParseOneDataCollection(assembly, dataCollectionType);
			
			EELog.Log(string.Format("{0} tables loaded.", dataCollectionDictInt.Count + dataCollectionDictStr.Count));
		}

		private void ParseOneDataCollection(Assembly assembly, Type dataCollectionType)
		{
			try
			{
				var sheetClassName = dataCollectionType.Name;//GetSheetName(dataCollectionType);
				var collection = dataLoader.Load(sheetClassName);
				if (collection == null)
				{
					EELog.LogError("EEDataManager: Load asset error, sheet name " + sheetClassName);
					return;
				}
				collection.OnAfterSerialized();
				var rowDataType = GetRowDataClassType(assembly, collection.ExcelFileName, dataCollectionType);
				var keyField = EEUtility.GetRowDataKeyField(rowDataType);
				if (keyField == null)
				{
					EELog.LogError("EEDataManager: Cannot find Key field in sheet " + sheetClassName);
					return;
				}

				var keyType = keyField.FieldType;
				if (keyType == typeof(int))
				{
					var dataDict = new RowDataDictInt();
					for (var i = 0; i < collection.GetDataCount(); ++i)
					{
						var data = collection.GetData(i);
						int key = (int) keyField.GetValue(data);
						dataDict.Add(key, data);
					}
					
					dataCollectionDictInt.Add(rowDataType, dataDict);
				}
				else if (keyType == typeof(string))
				{
					var dataDict = new RowDataDictStr();
					for (var i = 0; i < collection.GetDataCount(); ++i)
					{
						var data = collection.GetData(i);
						string key = (string) keyField.GetValue(data);
						dataDict.Add(key, data);
					}

					dataCollectionDictStr.Add(rowDataType, dataDict);
				}
				else
				{
					EELog.LogError(string.Format("Load {0} failed. There is no valid Key field in ", dataCollectionType.Name));
				}
			}
			catch (Exception e)
			{
				EELog.LogError(e.ToString());
			}
		}

		private static Type GetRowDataClassType(Assembly assembly, string excelFileName, Type sheetClassType)
		{
			var excelName = Path.GetFileNameWithoutExtension(excelFileName);
			var sheetName = GetSheetName(sheetClassType);
			var type = assembly.GetType(EESettings.Current.GetRowDataClassName(excelName, sheetName, true));
			return type;
		}

		private static string GetSheetName(Type sheetClassType)
		{
			return EESettings.Current.GetSheetName(sheetClassType);
		}

		#endregion
	}
}