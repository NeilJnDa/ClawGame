using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Level : MonoBehaviour
{
    //[InfoBox("@\"Init Cell Matrix Length: \" + (levelData != null && levelData.initCellMatrix != null ? levelData.initCellMatrix.Length : 0)")]
    public string levelName;
    private Grid3D grid;
    private LevelData levelData;
    [ReadOnly]
    public TextAsset levelDataJson;

    //Default value of gridSetting
    public GridSetting gridSetting = new GridSetting { length = 8, width = 6, height = 5, offset = new Vector3(0.5f, 0.5f, 0.5f), size = 1, spacing = 0 };

    #region Serialization
    [BoxGroup("Serialization")]
    [InfoBox("The json file will be modified instantly")]
    [Button("Save Scene to Json")]
    private void SaveScenetoJson()
    {
        //Create a new levelData every time. Unity GC will deal with the old one
        levelData = new LevelData(levelName, gridSetting);
        
        Debug.Log("Saving Level Data to Json");
        var units = transform.GetComponentsInChildren<GridUnit>();
        for (int i = 0; i < levelData.gridSetting.length; ++i)
        {
            for (int j = 0; j < levelData.gridSetting.width; ++j)
            {
                for (int k = 0; k < levelData.gridSetting.height; ++k)
                {
                    //Default
                    levelData.SetUnit(i, j, k, UnitType.Empty, null);
                }
            }
        }
        Debug.Log("Found " + units.Length + " units");
        try
        {
            foreach (var unit in units)
            {
                //Debug.Log("saving " + unit.name);
                Vector3 distance = unit.transform.position - (this.transform.position + levelData.gridSetting.offset);
                Vector3Int posWorldSpace = Vector3Int.RoundToInt(distance / levelData.gridSetting.size);
                //Pos in Grid Space
                Vector3Int pos = new Vector3Int(posWorldSpace.x, posWorldSpace.z, posWorldSpace.y);
                levelData.SetUnit(pos, unit.unitType, unit.setting);
            }
            Debug.Log("Level Data Created");
        }
        catch (Exception e)
        {
            Debug.LogError("Initialize Level Data Failed. Error: " + e);
        }


        try
        {
            JsonHelper.SaveToFile("/LevelData", levelName, levelData);
        }
        catch
        {
            Debug.LogError(levelName + ": Save to Json Failed");
        } 
        Debug.Log(levelName + ": level data saved to /LevelData as Json");
    }
    [BoxGroup("Serialization")]
    [Button("Update Scene from Json")]
    private void UpdateSceneFromJson()
    {
        Debug.Log("Load level from " + levelName);
        Initialize();
    }
    private void ParseJsonToLevelData()
    {
        try
        {
            JsonHelper.LoadFromFile("/LevelData", levelName, levelData);
        }
        catch
        {
            Debug.LogError("Parse Json failed");
        }
    }
    [BoxGroup("Serialization")]
    [Button("Select Json File")]
    private void SelectJsonFile()
    {
        string fullPath = "Assets/LevelData/" + levelName + ".json";
        UnityEditor.Selection.activeObject = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(fullPath);
    }
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        Debug.Log("Delete " + transform.childCount + " children of " + transform.name);

        
        if (Application.isPlaying)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
        else
        {
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }


        ParseJsonToLevelData();
        grid = new Grid3D(transform, levelData);
        Debug.Log("New cells created"); 
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(this.transform.position, Vector3.one / 2f);
    }


}
