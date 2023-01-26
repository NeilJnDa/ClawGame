using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public class RuleClass
{
    public UnitType unitType;
    public Dictionary<UnitType, bool> dict;

}
[CreateAssetMenu(fileName = "GridUnit Compatible Data", menuName = "ScriptableObjects/GridUnit Compatible Data", order = 2)]
[InlineEditor]
public class GridUnitCompatible : SerializedScriptableObject
{
    [TableList(AlwaysExpanded = true, DrawScrollView = false)]
    public List<RuleClass> rules = new List<RuleClass>();  
}