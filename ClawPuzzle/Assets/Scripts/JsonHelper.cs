using System.Collections;
using UnityEngine;
using System.IO;
using UnityEngine.Purchasing.MiniJSON;

// Note:
// UnityEngine.Purchasing.MiniJSON is a light json package, it can serialize Dictionary, but not suitable for ScriptableObject and GameObject;
// UnityEngine.JsonUtility can serialize ScriptableObject and GameObject;

public static class JsonHelper
{
    /// <summary>
    /// Path should be like "/XXXX"
    /// Note: Only run this in Editor!
    /// </summary>
    /// <param name="relativePath"></param>
    /// <param name="obj"></param>
    public static void SaveToJsonFile(string relativePath, object obj)
    {
        string fullPath = Application.dataPath + relativePath + ".json";

        //Unity JsonUtility
        string content = JsonUtility.ToJson(obj);

        //Unity In app purchasing minijson
        //string content = Json.Serialize(obj);

        Debug.Log("JsonHelper: Writing " + content);
        File.WriteAllText(fullPath, content);
    }
    /// <summary>
    /// Path should be like "/XXXX/XXX"  without .json or .txt
    /// </summary>
    /// <param name="relativePath"></param>
    /// <param name="name"></param>
    /// <param name="obj"></param>
    public static T LoadFromJsonFile<T>(string relativePath)
    {
        //var content = File.ReadAllText(Application.dataPath + relativePath+ ".json");
        var content = Resources.Load<TextAsset>(relativePath);

        //Unity JsonUtility
        //JsonUtility.FromJsonOverwrite(content, obj);
        T obj = JsonUtility.FromJson<T>(content.ToString());
        return obj;

        //Unity In app purchasing minijson
        //obj = Json.Deserialize(content) as Object;
    }
}