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
        
        //}
        /// <summary>
        /// TODO: More Rules
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
    //    public bool CheckEnterCell(GridUnit gridUnit, Cell from, Cell to, Direction direction, bool ignorePushable = false, bool isClawToCatch = false)
    //{
    //    //We assume the target cell exists ("to" can not be null. This is already checked before this method)
    //    Debug.Log("Check " + gridUnit.name + " to " + to.name);

    //    if (!CheckSolidSurface(gridUnit, from, to, direction))
    //    {
    //        return false;
    //    }

    //    //Claw Try To catch
    //    if (isClawToCatch)
    //    {
    //        if (to.gridUnits.Exists(x => x.pushable == false))
    //        {
    //            Debug.LogWarning("Checking: " + gridUnit.name + " (Claw) move and catch from " + from.name + " " + direction + " failed, Rules not allowed since there is an a ground/conveyor/hole (Not catchable)");
    //            return false;
    //        }
    //    }

    //    //Other grid units
    //    else if (ignorePushable)
    //    {
    //        if(to.gridUnits.Exists(x=> x.pushable == false))
    //        {
    //            Debug.LogWarning("Checking: " + gridUnit + " move from " + from.name + " to " + direction + " failed, Rules not allowed since target has unPushable units");
    //            return false;
    //        }
    //    }
    //    else
    //    {
    //        if (to.gridUnits.Count != 0)
    //        {
    //            Debug.LogWarning("Checking: " + gridUnit + " move from " + from.name + " to " + direction + " failed, Rules not allowed since target has " + to.gridUnits.Count + " units");
    //            return false;
    //        }
    //    }

    //    return true;
    //}
    private bool CheckSolidSurface(GridUnit gridUnit, Cell from, Cell to, Direction direction)
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
