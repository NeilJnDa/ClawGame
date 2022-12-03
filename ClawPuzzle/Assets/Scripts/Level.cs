using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Level : MonoBehaviour
{
    private Grid3D grid;
    public LevelData levelData;
    
    #region Serialization
    [BoxGroup("Serialization")]
    [InfoBox("Both ScriptableObject and Json file will be modified")]
    [Button("Save Scene to Level Data")]
    private void SaveScenetoLevelData()
    {
        if (levelData == null)
        {
            Debug.LogError("Level data is null! You should create a Level Data Scriptable Object and assign it here");
            return;
        }
        if (levelData.gridSetting == null)
        {
            Debug.LogError("Grid setting is null! Update failed!");
            return;
        }
        Debug.Log("Saving Level Data to Scriptable File");
        var units = transform.GetComponentsInChildren<GridUnit>();
        levelData.initCellMatrix = new Unit[levelData.gridSetting.length * levelData.gridSetting.width * levelData.gridSetting.height];
        for(int i = 0; i < levelData.gridSetting.length; ++i)
        {
            for(int j = 0; j < levelData.gridSetting.width; ++j)
            {
                for(int k = 0; k < levelData.gridSetting.height; ++k)
                {
                    //Default
                    levelData.SetUnit(i, j, k, UnitType.Empty);
                }
            }
        }
        Debug.Log("Found " + units.Length + " units");
        try
        {
            foreach (var unit in units)
            {
                Vector3 distance = unit.transform.position - (this.transform.position + levelData.gridSetting.offset);
                Vector3Int posWorldSpace = Vector3Int.RoundToInt(distance / levelData.gridSetting.size);
                //Pos in Grid Space
                Vector3Int pos = new Vector3Int(posWorldSpace.x, posWorldSpace.z, posWorldSpace.y);
                levelData.SetUnit(pos, unit.unitType);
            }
        }
        catch
        {
            Debug.LogError(levelData.name + ": Serialize to ScriptableObject Failed");
        }

        Debug.Log("LevelData Serialized to " + levelData.name);

        try
        {
            JsonHelper.SaveToFile("/LevelData", levelData);
        }
        catch
        {
            Debug.LogError(levelData.name + ": Save to Json Failed");

        }
        Debug.Log(levelData.name + ": Saved to /LevelData as Json");
    }
    [BoxGroup("Serialization")]
    [Button("Update Scene from Level Data")]
    private void UpdateSceneFromLevelData()
    {
        if(levelData.initCellMatrix == null)
        {
            Debug.LogError("Init Cell Matrix is null! Updating Scene failed!");
            return;
        }
        Debug.Log("Load level from " + levelData.name);

        Initialize();
    }
    private void ParseJsonToScriptableObject()
    {
        try
        {
            JsonHelper.LoadFromFile("/LevelData", levelData.name, levelData);

        }
        catch
        {
            Debug.LogError("Parse Json failed");
        }
    }
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        Debug.Log("Delete " + transform.childCount + " children of " +  transform.name );

        while(transform.childCount > 0)
        {
            if (Application.isPlaying)
                Destroy(transform.GetChild(0).gameObject);
            else
                DestroyImmediate(transform.GetChild(0).gameObject);
        }
        
        ParseJsonToScriptableObject();
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
