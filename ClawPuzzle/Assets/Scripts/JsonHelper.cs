using System.Collections;
using UnityEngine;
using System.IO;
public static class JsonHelper
{
    /// <summary>
    /// Path should be like "/XXXX"
    /// </summary>
    /// <param name="path"></param>
    /// <param name="obj"></param>
    public static void SaveToFile(string path, Object obj)
    {
        string content = JsonUtility.ToJson(obj);
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
        JsonUtility.FromJsonOverwrite(content, obj);
    }
}