using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawLineIndicator : MonoBehaviour
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

}
