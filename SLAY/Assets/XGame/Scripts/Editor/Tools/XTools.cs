using UnityEngine;
using UnityEditor;

public class XTools
{
    [MenuItem("Tools/清除本地数据")]
    static void ClearUserDatas()
    {
        PlayerPrefs.DeleteAll();
    }

    [MenuItem("Tools/切换为A包")]
    static void SetToPackageA()
    {
        PlayerPrefs.SetInt("editor_abpack", 0);
    }
    [MenuItem("Tools/切换为B包")]
    static void SetToPackageB()
    {
        PlayerPrefs.SetInt("editor_abpack", 1);
    }
    [MenuItem("Tools/切换为A包", true)]
    static bool CheckPackageA()
    {
        return PlayerPrefs.GetInt("editor_abpack", 1) == 1;
    }
    [MenuItem("Tools/切换为B包", true)]
    static bool CheckPackageB()
    {
        return PlayerPrefs.GetInt("editor_abpack", 1) == 0;
    }
}
