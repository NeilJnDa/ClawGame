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
        if (to.currentGridUnit != null && to.currentGridUnit.unitType != UnitType.Empty)
        {
            Debug.LogWarning(from.currentGridUnit + " move " + direction + " failed, Rules not allowed since target is " + to.currentGridUnit.name);
            return false;
        }
        if (from.solidSurface[((int)direction)])
        {
            Debug.LogWarning(from.currentGridUnit + " move " + direction + " failed, this cell has a solid surface");
            return false;
        }
        if (to.solidSurface[((int)DirectionUtility.RevertDirection(direction))])
        {
            Debug.LogWarning(from.currentGridUnit + " move " + direction + " failed, target cell has a solid surface");
            return false;
        }

        return true;
    }
}
