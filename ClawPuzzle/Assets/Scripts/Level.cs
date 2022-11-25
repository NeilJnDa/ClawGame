using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    private Grid2D grid;
    public LevelData levelData;
    // Start is called before the first frame update
    void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        grid = new Grid2D(levelData, transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
