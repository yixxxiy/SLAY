using System;
using UnityEngine;

namespace EasyExcel
{
	public interface IEEDataLoader
	{
		EERowDataCollection Load(string sheetClassName);
	}
	
	/// <summary>
	/// Load generated data by Resources.Load.
	/// </summary>
	public class EEDataLoaderResources : IEEDataLoader
	{
		public EERowDataCollection Load(string sheetClassName)
		{
			var headName = sheetClassName;
			var filePath = EESettings.Current.GeneratedAssetPath.
				               Substring(EESettings.Current.GeneratedAssetPath.IndexOf("Resources/", StringComparison.Ordinal) + "Resources/".Length)
			               + headName;
			var table = Resources.Load(filePath) as EERowDataCollection;
			return table;
		}
	}
}