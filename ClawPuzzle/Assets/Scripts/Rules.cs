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
    public bool CheckEnterCell(GridUnit gridUnit, Cell from, Cell to, Direction direction, bool ignorePushable = false, bool isClawToCatch = false)
    {
        //We assume the target cell exists ("to" can not be null. This is already checked before this method)
        Debug.Log("Check " + gridUnit.name + " to " + to.name);

        if (!CheckSolidSurface(gridUnit, from, to, direction))
        {
            return false;
        }

        //Claw Try To catch
        if (isClawToCatch)
        {
            if (to.gridUnits.Exists(x => x.catchable == false))
            {
                Debug.LogWarning("Checking: " + gridUnit.name + " (Claw) move and catch from " + from.name + " " + direction + " failed, Rules not allowed since there is an a ground/conveyor/hole (Not catchable)");
                return false;
            }
        }

        //Other grid units
        else if (ignorePushable)
        {
            if(to.gridUnits.Exists(x=> x.pushable == false))
            {
                Debug.LogWarning("Checking: " + gridUnit + " move from " + from.name + " to " + direction + " failed, Rules not allowed since target has unPushable units");
                return false;
            }
        }
        else
        {
            if (to.gridUnits.Count != 0)
            {
                Debug.LogWarning("Checking: " + gridUnit + " move from " + from.name + " to " + direction + " failed, Rules not allowed since target has " + to.gridUnits.Count + " units");
                return false;
            }
        }

        return true;
    }
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
