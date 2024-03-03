using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace XGame
{
    public class AssetsImportMyTools : AssetPostprocessor
    {
        /// <summary>
        /// 所有的资源的导入，删除，移动，都会调用此方法
        /// </summary>
        /// <param name="importedAssets">导入的资源</param>
        /// <param name="deletedAssets">删除的资源</param>
        /// <param name="movedAssets">移动资源到新位置</param>
        /// <param name="movedFromAssetPaths">被移动资源原来的位置</param>
        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string str in importedAssets)
            {
                if (str.Contains("AppConfig.json") || str.Contains("AppConfigTest.json"))
                {
                    //ConfigAppWindow.ConfigApp(ConfigAppWindow.IsTestMode);

                    Debug.Log("刷新配置完成");
                    break;
                }
            }
        }
    }
}
