using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridUnit : MonoBehaviour
{
    public virtual UnitType unitType { get { return UnitType.Empty; } }
    public Cell cell;
}
