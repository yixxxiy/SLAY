using System;
using System.Collections.Generic;
using System.Linq;
using EasyExcel;
using UnityEngine;

/// <summary>
/// 这个例子展示了如何加载、查找数据（load and find）
/// 示例数据表来自EasyExcel\Example\ExcelFiles
/// </summary>
public class ExampleLoadData : MonoBehaviour
{
	private readonly EEDataManager _eeDataManager = new EEDataManager();

	private void Awake()
	{
		// Load all data sheets like this when initializing your game
		// 载入全部导入的数据
		_eeDataManager.Load();
		
		// 查找数据的示例
		FindDataExample();
	}

	private void FindDataExample()
	{
		// ------------Uncomment below after importing example files------------
		// ------------取消下面的注释需要先运行完Tools/EasyExcel/Import导入完示例文件！------------
		/*
		 
		// Get EasyExcelGenerated.KeyColumn with string-type key
		// You can specify a column in a sheet as key with Your_Column_Name:Key.
		var keyColumn = _eeDataManager.Get<EasyExcelGenerated.KeyColumn>("Brigand");
		Debug.Log(keyColumn.Description);
		
		// Get EasyExcelGenerated.MultiSheet01 with int-type key
		var multiSheet01 = _eeDataManager.Get<EasyExcelGenerated.MultiSheet01>(1001);
		Debug.Log(multiSheet01.Description);

		// Get EasyExcelGenerated.KeyColumn list
		List<EasyExcelGenerated.KeyColumn> list = _eeDataManager.GetList<EasyExcelGenerated.KeyColumn>();
		foreach (var data in list)
		{
			Debug.Log(data.Icon);
		}
		
		*/
	}

	#region 演示示例结果用，不必读。Just for showing the data. You do not have to read these

	private void OnGUI()
	{
		gui(_eeDataManager);
	}

	// Just for test show, you do not have to know the details below.
	public static void gui(EEDataManager eeDataManager)
	{
		var index = 0;
		var labelBottom = 0;
		index++;
		GUI.Label(new Rect(10, labelBottom + index * 40, 800, 40), "API examples:");
		index++;
		GUI.Label(new Rect(30, labelBottom + index * 40, 800, 40), "1.Load all data:\n    <color=#569CD6>EEDataManager.Load();</color>");
		index++;
		GUI.Label(new Rect(30, labelBottom + index * 40, 800, 40),
			"2.Find a XXXData by id (int or string):\n    <color=#569CD6>EEDataManager.Get<XXXData>(id);</color>");
		index++;
		GUI.Label(new Rect(30, labelBottom + index * 40, 800, 40),
			"3.Find XXXData list:\n    <color=#569CD6>EEDataManager.GetList<XXXData>();</color>");

		index++;
		var assembly = EEUtility.GetSourceAssembly();
		var types = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(EERowDataCollection)));
		var sheetClassTypes = types as Type[] ?? types.ToArray();
		if (!sheetClassTypes.Any())
		{
			labelBottom = index * 40;
			GUI.Label(new Rect(10, labelBottom, 800, 100),
				"<color=#FF5555>No data loaded, you need to import excel files first by Tools->EasyExcel->Import.</color>");
		}
		else
		{
			labelBottom = index * 40 + 20;
			GUI.Label(new Rect(10, labelBottom, 800, 30), string.Format("Loaded <color=#569CD6>{0}</color> sheets:", sheetClassTypes.Count()));
			labelBottom += 30;
			var typesIndex = 0;
			foreach (var sheetClassType in sheetClassTypes)
			{
				var collectClassName = sheetClassType.FullName;
				/*var headNameRaw =
					collectClassName.Substring(0, collectClassName.IndexOf(EESettings.Current.SheetDataPostfix, StringComparison.Ordinal));
				var headParts = headNameRaw.Split('.');*/
				var headParts = collectClassName.Split('.');
				var fileName = headParts.Length == 1 ? null : headParts[0].Substring(EESettings.Current.NameSpacePrefix.Length);
				var sheetClassName = headParts.Length == 1 ? headParts[0] : headParts[1];
				var sheetName = EESettings.Current.GetSheetName(sheetClassType);
				var rowDataClassName = EESettings.Current.GetRowDataClassName(fileName, sheetName, true);
				var rowType = assembly.GetType(rowDataClassName);
				var dic = eeDataManager.GetList(rowType);
				var rowDataClassNameShort = EESettings.Current.GetRowDataClassName(fileName, sheetName, false);
				GUI.Label(new Rect(30, labelBottom + typesIndex * 20, 380, 20), string.Format("Sheet Class: <color=#569CD6>{0}</color>", sheetClassName));
				GUI.Label(new Rect(410, labelBottom + typesIndex * 20, 250, 20), string.Format("RowData Class: <color=#569CD6>{0}</color>", rowDataClassNameShort));
				GUI.Label(new Rect(660, labelBottom + typesIndex * 20, 200, 20), string.Format("Rows: <color=#569CD6>{0}</color>", dic != null ? dic.Count.ToString() : "empty"));
				typesIndex++;
			}
		}
	}

	#endregion
	
}