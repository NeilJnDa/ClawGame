using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

public class Level : MonoBehaviour
{
    //[InfoBox("@\"Init Cell Matrix Length: \" + (levelData != null && levelData.initCellMatrix != null ? levelData.initCellMatrix.Length : 0)")]
    public string levelName;
    public string displayName;
    private Grid3D grid;
    private LevelData levelData;

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
        levelData.displayName = displayName;
        Debug.Log("Saving Level Data to Json");
        try
        {

            var cells = transform.GetComponentsInChildren<Cell>();

            bool[] defaultSolidSurfaceSetting = new bool[6] { false, false, false, false, false, false, };
            foreach (var cell in cells)
            {
                cell.UpdatePosition(this.transform, gridSetting);
                if (cell.solidSurface.All(x => !x)) continue;

                //Not Default setting, then save this to levelData
                levelData.SetCellSpace(cell.i, cell.j, cell.k, cell.solidSurface);
            }

            var units = transform.GetComponentsInChildren<GridUnit>();
            foreach (var unit in units)
            {
                //Debug.Log("saving " + unit.name);
                Vector3 distance = unit.transform.position - (this.transform.position + levelData.gridSetting.offset);
                Vector3Int posWorldSpace = Vector3Int.RoundToInt(distance / levelData.gridSetting.size);
                //Pos in Grid Space
                //Calculate the pos again, in case it dose not have a proper hierarchy. (Correct child of correct cell)
                Vector3Int pos = new Vector3Int(posWorldSpace.x, posWorldSpace.z, posWorldSpace.y);
                levelData.SetGridUnit(pos.x, pos.y, pos.z, unit.unitType, unit.setting);
            }
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
            levelData = JsonHelper.LoadFromFile<LevelData>("/LevelData", levelName);
        }
        catch (Exception e)
        {
            Debug.LogError("Parse Json failed: " + e);
        }
    }
    [BoxGroup("Serialization")]
    [Button("Select Json File")]
    private void SelectJsonFile()
    {
        string fullPath = "Assets/LevelData/" + levelName + ".json";
    #if UNITY_EDITOR
        UnityEditor.Selection.activeObject = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(fullPath);
    #endif 
    }
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        Initialize();
    }

    public void Initialize()
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
        displayName = levelData.displayName;
        gridSetting = levelData.gridSetting;
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
