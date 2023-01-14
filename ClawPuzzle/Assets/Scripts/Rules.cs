using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    // Start is called before the first frame update

    /// <summary>
    /// TODO: More Rules
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public bool CheckEnterCell(GridUnit gridUnit, Cell from, Cell to, Direction direction)
    {
        //We assume the target cell exists ("to" can not be null. This is already checked before this method)
        Debug.Log("Check " + gridUnit.name + " to " + to.name);

        //Glass Obstacles
        if (from.solidSurface[((int)direction)])
        {
            Debug.LogWarning("Checking: " + gridUnit + " move " + direction + " failed, this cell has a solid surface");
            return false;
        }
        if (to.solidSurface[((int)DirectionUtility.RevertDirection(direction))])
        {
            Debug.LogWarning("Checking: " + gridUnit + " move " + direction + " failed, target cell has a solid surface");
            return false;
        }

        //More rules about failing to enter
        switch (gridUnit.unitType)
        {
            case UnitType.Claw:
                {
                    //Claw rules

                    if(to.gridUnits.Exists(x => x.unitType == UnitType.Conveyor || x.unitType == UnitType.Ground || x.unitType == UnitType.Hole))
                    {
                        Debug.LogWarning("Checking: Claw move from " + from.name + " " + direction + " failed, Rules not allowed since there is an a ground/conveyor/hole");
                        return false;
                    }
                    else if (to.gridUnits.Count > 0 && to.gridUnits.Find(x => x.catchable == true) == null)
                    {
                        Debug.LogWarning("Checking: Claw move from " + from.name + " " + direction + " failed, Rules not allowed since all targets are not catchable.");
                        return false;
                    }
                    break;
                }


            default:
                {
                    if (to.gridUnits.Count != 0)
                    {
                        Debug.LogWarning("Checking: " + gridUnit + " move " + direction + " failed, Rules not allowed since target has " + to.gridUnits.Count + " units");
                        return false;
                    }
                    break;
                }

        }

        return true;
    }
}
