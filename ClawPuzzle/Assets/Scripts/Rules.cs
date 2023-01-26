using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

[System.Serializable]
[InlineEditor]
public class Rules : MonoBehaviour
{
    #region Singleton
    public static Rules Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<Rules>();
            }
            return _instance;
        }
    }
    private static Rules _instance;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    #endregion

    public GridUnitCompatible ruleData;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gridUnitFrom"></param>
    /// <param name="gridUnitTo"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="direction"></param>
    /// <param name="ignorePushable">Ignore Pushable Objects</param>
    /// <returns></returns>
    public bool CheckCompatible(GridUnit gridUnitFrom, GridUnit gridUnitTo, Cell from, Cell to, Direction direction, bool ignorePushable = false)
    {

        if (!CheckSolidSurface(gridUnitFrom, from, to, direction))
        {
            return false;
        }
        Debug.Log("Check if " + gridUnitFrom.name + " is compatible with " + gridUnitTo.name);
        if (ruleData == null || ruleData.rules == null)
        {
            Debug.LogError("No Rule Data!");
            return false;
        }
        var ruleClass = ruleData.rules.Find(x => x.unitType == gridUnitFrom.unitType);
        if (ruleClass == null)
        {
            Debug.LogError("Can not find " + gridUnitFrom.name + " in rule data");
            return false;
        }
        if (ruleClass.dict.Count == 0)
        {
            Debug.LogError("Rule Dict of " + ruleClass + " is empty!");
            return false;
        }
        if (!ruleClass.dict.ContainsKey(gridUnitTo.unitType))
        {
            Debug.LogWarning("Can not find the rule between " + gridUnitFrom.unitType + " and " + gridUnitTo);
        }
        if (ignorePushable && gridUnitTo.pushable) return true;

        return ruleClass.dict[gridUnitTo.unitType];
        
    }
    public bool CheckEnterCell(GridUnit gridUnit, Cell from, Cell to, Direction direction, bool isClawToCatch)
    {
        if (!CheckSolidSurface(gridUnit, from, to, direction))
        {
            return false;
        }
        Debug.Log("Check " + gridUnit.name + " to " + to.name);
        if (ruleData == null || ruleData.rules == null)
        {
            Debug.LogError("No Rule Data!");
            return false;
        }
        var ruleClass = ruleData.rules.Find(x => x.unitType == gridUnit.unitType);
        if (ruleClass == null)
        {
            Debug.LogError("Can not find " + gridUnit.name + " in rule data");
            return false;
        }
        if (ruleClass.dict.Count == 0)
        {
            Debug.LogError("Rule Dict of " + ruleClass + " is empty!");
            return false;
        }

        bool success = true;
        foreach (var unit in to.gridUnits)
        {
            if (isClawToCatch && unit.pushable) continue;
            if (!ruleClass.dict.ContainsKey(unit.unitType))
            {
                Debug.LogWarning("Can not find the rule between " + gridUnit.unitType + " and " + unit.unitType);
            }
            else
            {
                success = success && ruleClass.dict[unit.unitType];
            }
        }
        return success;
    }
        
    public bool CheckSolidSurface(GridUnit gridUnit, Cell from, Cell to, Direction direction)
    {
        //Glass Obstacles
        if (from.solidSurface[((int)direction)])
        {
            Debug.LogWarning("Checking: " + gridUnit + " move from " + from.name + " to " + direction + " failed, this cell has a solid surface");
            return false;
        }
        if (to.solidSurface[((int)DirectionUtility.RevertDirection(direction))])
        {
            Debug.LogWarning("Checking: " + gridUnit + " move from " + from.name + " to " + direction + " failed, target cell has a solid surface");
            return false;
        }
        return true;
    }
}
