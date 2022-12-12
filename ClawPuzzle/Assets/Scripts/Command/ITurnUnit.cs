using System.Collections;
using UnityEngine;


public interface ITurnUnit
{
    public void UndoOneStep();

    public void ResetAll();

    public void NextStep();

}