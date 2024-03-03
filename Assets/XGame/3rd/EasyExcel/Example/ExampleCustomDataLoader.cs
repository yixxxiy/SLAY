using System;
using EasyExcel;
using UnityEngine;

/// <summary>
/// Examples of customDataLoader
/// 演示自定义数据加载器（load from Resources or AssetBundle）
/// 示例数据表来自EasyExcel\Example\ExcelFiles
/// </summary>
public class ExampleCustomDataLoader : MonoBehaviour
{
	private EEDataManager _eeDataManager;
	
	private void Start()
	{
		// use ExampleDataLoaderResources as loader
		// 从Resources里加载数据
		_eeDataManager = new EEDataManager(new ExampleDataLoaderResources());
		
		// use ExampleDataLoaderAssetBundle as loader
		// 从AssetBundle里加载数据
		//_eeDataManager = new EEDataManager(new ExampleDataLoaderAssetBundle());
		
		// Set CustomDataLoader before load.
		// 先用自定义loader创建EEDataManager，后执行Load
		_eeDataManager.Load();
	}
	
	// Load from Resources.Load. 从Resources里加载。
	private class ExampleDataLoaderResources : IEEDataLoader
	{
		public EERowDataCollection Load(string sheetClassName)
		{
			var headName = sheetClassName;
			var filePath = EESettings.Current.GeneratedAssetPath.
				               Substring(EESettings.Current.GeneratedAssetPath.IndexOf("Resources/", StringComparison.Ordinal) + "Resources/".Length)
			               + headName;
			var collection = Resources.Load(filePath) as EERowDataCollection;
			return collection;
		}
	}
	
	// Load from AssetBundle. 从AssetBundle里加载。
	private class ExampleDataLoaderAssetBundle : IEEDataLoader
	{
		public EERowDataCollection Load(string sheetClassName)
		{
			// Your AssetBundle file path
			var bundlePath = Application.persistentDataPath + "/***Your AssetBundle File Name***";
			// Your AssetBundle file path
			var assetPath = "Assets/ExampleFolder/" + sheetClassName + ".asset";
			var bundle = AssetBundle.LoadFromFile(bundlePath);
			var collection =  bundle.LoadAsset(assetPath) as EERowDataCollection;
			return collection;
		}
	}


	#region 演示示例结果用，不必读。Just for showing the data. You do not have to read these
	
	private void OnGUI()
	{
		ExampleLoadData.gui(_eeDataManager);
	}
	
	#endregion
	
}