using System.Collections;
using UnityEngine;
using System.IO;
//using UnityEngine.Purchasing.MiniJSON;

// Note:
// UnityEngine.Purchasing.MiniJSON is a light json package, it can serialize Dictionary, but not suitable for ScriptableObject and GameObject;
// UnityEngine.JsonUtility can serialize ScriptableObject and GameObject;

public static class JsonHelper
{
    /// <summary>
    /// Path should be like "/XXXX"
    /// </summary>
    /// <param name="path"></param>
    /// <param name="obj"></param>
    public static void SaveToFile(string path, LevelData obj)
    {
        //Unity JsonUtility
        string content = JsonUtility.ToJson(obj);

        //Unity In app purchasing minijson
        //string content = Json.Serialize(obj);

        File.WriteAllText(Application.dataPath + path + "/" + obj.name + ".json", content);
    }
    /// <summary>
    /// Path should be like "/XXXX"
    /// </summary>
    /// <param name="path"></param>
    /// <param name="name"></param>
    /// <param name="obj"></param>
    public static void LoadFromFile(string path, string name, Object obj)
    {
        var content = File.ReadAllText(Application.dataPath + path + "/" + name + ".json");

        //Unity JsonUtility
        JsonUtility.FromJsonOverwrite(content, obj);


        //Unity In app purchasing minijson
        //obj = Json.Deserialize(content) as Object;
    }
}