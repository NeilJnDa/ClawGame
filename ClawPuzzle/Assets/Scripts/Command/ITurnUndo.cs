using System.Collections;
using UnityEngine;

/// <summary>
/// All objects that needs to undo according to turn should inherit this interface
/// TurnManager will manage turn undo system, and call methods of each interface to register or undo or reset 
/// </summary>
public interface ITurnUndo
{
    public void UndoOneStep();

    public void ResetAll();

    public void NextStep();

}