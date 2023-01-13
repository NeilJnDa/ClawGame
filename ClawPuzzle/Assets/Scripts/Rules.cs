using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public bool CheckEnterCell(Cell from, Cell to, Direction direction)
    {
        Debug.Log("Check " + from.currentGridUnit.name + " to " + to.name);

        //Glass Obstacles
        if (from.solidSurface[((int)direction)])
        {
            Debug.LogWarning("Checking: " +  from.currentGridUnit + " move " + direction + " failed, this cell has a solid surface");
            return false;
        }
        if (to.solidSurface[((int)DirectionUtility.RevertDirection(direction))])
        {
            Debug.LogWarning("Checking: " + from.currentGridUnit + " move " + direction + " failed, target cell has a solid surface");
            return false;
        }
        
        if (to.currentGridUnit != null)
        {
            Debug.LogWarning("Checking: " + from.currentGridUnit + " move " + direction + " failed, Rules not allowed since target is " + to.currentGridUnit.name);
            return false;
        }
        return true;
    }
    public bool CheckClawEnterCell(Cell from, Cell to, Direction direction)
    {
        Debug.Log("One step of iteration: Check claw if can enter" + " to " + to.name);

        //Glass Obstacles
        if (from.solidSurface[((int)direction)])
        {
            Debug.LogWarning("Checking: Claw move from " + from.name + " " + direction + " failed, this cell has a solid surface");
            return false;
        }
        if (to.solidSurface[((int)DirectionUtility.RevertDirection(direction))])
        {
            Debug.LogWarning("Checking: Claw move from " + from.name + " " + direction + " failed, target cell has a solid surface");
            return false;
        }

        //Claw rules
        if (to.currentGridUnit != null && to.currentGridUnit.catchable == false)
        {
            Debug.LogWarning("Checking: Claw move from " + from.name + " " + direction +" failed, Rules not allowed since target is " + to.currentGridUnit.name);
            return false;
        }
        return true;
    }
}
