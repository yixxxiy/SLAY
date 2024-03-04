using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SaveManager
{
    private static string savePath
    {
        get {
            return Path.Combine(Application.persistentDataPath, "saves");
        }
    }

    public static void Test()
    {
        Debug.Log(Application.persistentDataPath);

        if (Directory.Exists(savePath))
        {
            Debug.Log("存在");
        }
        else
        {
            Debug.Log("不存在");
            Directory.CreateDirectory(savePath);
        }

    }

    public static void SaveGame()
    {
        List<string> jsonList = new List<string>();
        //Todo:存档逻辑

        SaveByJson("gameSave.json", jsonList);
    }

    public static void LoadGame()
    {
        var jsonList = LoadByJson<List<string>>("gameSave.json");
        //Todo:读档逻辑
    }

    public static void SaveByJson(string fileName, object data)
    {
        var json = JsonUtility.ToJson(data);
        var path = Path.Combine(savePath, fileName);

        //Todo: 异常处理
        File.WriteAllText(path, json);
    }

    public static T LoadByJson<T>(string fileName)
    {
        var path = Path.Combine(savePath, fileName);

        //Todo: 异常处理
        var json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<T>(json);
        return data;
    }
}
