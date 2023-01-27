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
    private void Update()
    {
        Vector3 endPos = new Vector3(0, this.transform.root.position.y - this.transform.position.y, 0);
        lineRenderer.SetPosition(1, endPos);
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
