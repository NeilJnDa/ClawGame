using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawLineIndicator : MonoBehaviour, ITurnUndo
{
    public Material red;
    public Material white;
    private LineRenderer lineRenderer;
    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }
    public void ChangeLineToRed()
    {
        lineRenderer.material = red;
    }
    public void ChangeLineToWhite()
    {
        lineRenderer.material = white;
    }


    private Stack<Material> materialHistory = new Stack<Material>();
    public void UndoOneStep()
    {
        lineRenderer.material = materialHistory.Pop();
    }

    public void ResetAll()
    {
        materialHistory.Push(lineRenderer.material);
        lineRenderer.material = white;
    }

    public void SaveToHistory()
    {
        materialHistory.Push(lineRenderer.material);
    }
}
