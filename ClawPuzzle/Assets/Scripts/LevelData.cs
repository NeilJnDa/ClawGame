using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Level Data", menuName = "ScriptableObjects/Level Data", order = 2)]
[InlineEditor]
public class LevelData : SerializedScriptableObject
{
    [OnValueChanged("CreateMatrix")]
    public GridSetting gridSetting;

    [SerializeField]
    [OnValueChanged("CopyAndRevert")]
    [TableMatrix(SquareCells = true, DrawElementMethod = "DrawEnumElement", HideRowIndices = true)]
    private CellType[,] revertedMatrix;

    //Exposed to other classes after reverted
    [HideInInspector]
    public CellType[,] initCellMatrix;


    [Button("Reset"), GUIColor(1,0,0)]
    private void Reset()
    {
        if(revertedMatrix == null) CreateMatrix();
        System.Array.Clear(revertedMatrix, 0, revertedMatrix.Length);
    }
    private void CopyAndRevert()
    {
        //Create new or Resize
        if(initCellMatrix == null && revertedMatrix != null 
            || initCellMatrix.GetLength(0) != gridSetting.length
            || initCellMatrix.GetLength(1) != gridSetting.width)
        {
            initCellMatrix = new CellType[gridSetting.length, gridSetting.width];
        }

        for (int i = 0; i < revertedMatrix.GetLength(0); ++i)
        {
            for (int j = 0; j < revertedMatrix.GetLength(1); ++j)
            {
                if (i < gridSetting.length && j < gridSetting.width)
                {
                    initCellMatrix[i, j] = revertedMatrix[i, revertedMatrix.GetLength(1) - 1 - j];
                }
            }
        }
    }
    private void CreateMatrix()
    {
        if(gridSetting!= null)
        {
            if(revertedMatrix == null)
                //create new
                revertedMatrix = new CellType[gridSetting.length, gridSetting.width];
            else
            {
                //copy and resize
                var newMatrix = new CellType[gridSetting.length, gridSetting.width];
                for(int i = 0; i < revertedMatrix.GetLength(0); ++i)
                {
                    for(int j = 0; j < revertedMatrix.GetLength(1); ++j)
                    {
                        if(i < gridSetting.length && j < gridSetting.width)
                        {
                            newMatrix[i, j] = revertedMatrix[i, j];
                        }
                    }
                }
                //C# have auto GC
                revertedMatrix = newMatrix;
            }
        }
    }
    private static CellType DrawEnumElement(Rect rect, CellType cellType)
    {
        //Padding
        rect.xMin += 1;
        rect.xMax -= 1;
        rect.yMin += 1;
        rect.yMax -= 1;
        Rect enumRect = new Rect(rect);
        enumRect.yMax -= enumRect.height * 0.8f;
        Rect previewRect = new Rect(rect);
        previewRect.yMin += previewRect.height * 0.2f;

        if (Event.current.type == EventType.MouseDown && previewRect.Contains(Event.current.mousePosition))
        {
            int enumNum = System.Enum.GetNames(typeof(CellType)).Length;
            cellType = (CellType)((((int)cellType) + 1 + enumNum) % enumNum);
            GUI.changed = true;  
            Event.current.Use();
        }

        var texture = UnityEditor.AssetPreview.GetAssetPreview(Resources.Load(cellType.ToString()));
        UnityEditor.EditorGUI.DrawPreviewTexture(previewRect, texture);
        cellType = (CellType)UnityEditor.EditorGUI.EnumPopup(enumRect, cellType);


        return cellType;
    }
}
