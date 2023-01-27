//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Sirenix.OdinInspector;

//namespace Deprecated
//{
//    //[CreateAssetMenu(fileName = "Level Data", menuName = "ScriptableObjects/Level Data", order = 2)]
//    [InlineEditor]
//    public class LevelData : SerializedScriptableObject
//    {
//        [OnValueChanged("CreateMatrix")]
//        public GridSetting gridSetting;

//        [SerializeField]
//        [OnValueChanged("CopyAndRevert")]
//        [TableMatrix(HorizontalTitle = "Ground Cell", SquareCells = true, DrawElementMethod = "DrawEnumElement", HideRowIndices = true)]
//        private UnitType[,] revertedMatrix;

//        //Exposed to other classes after reverted
//        [HideInInspector]
//        public UnitType[,] initCellMatrix;


//        [Button("Reset"), GUIColor(1, 0, 0)]
//        private void Reset()
//        {
//            if (revertedMatrix == null) CreateMatrix();
//            System.Array.Clear(revertedMatrix, 0, revertedMatrix.Length);
//        }
//        private void CopyAndRevert()
//        {
//            //Create new or Resize
//            if (initCellMatrix == null && revertedMatrix != null
//                || initCellMatrix.GetLength(0) != gridSetting.length
//                || initCellMatrix.GetLength(1) != gridSetting.width)
//            {
//                initCellMatrix = new UnitType[gridSetting.length, gridSetting.width];
//            }

//            for (int i = 0; i < revertedMatrix.GetLength(0); ++i)
//            {
//                for (int j = 0; j < revertedMatrix.GetLength(1); ++j)
//                {
//                    if (i < gridSetting.length && j < gridSetting.width)
//                    {
//                        initCellMatrix[i, j] = revertedMatrix[i, revertedMatrix.GetLength(1) - 1 - j];
//                    }
//                }
//            }
//        }
//        private void CreateMatrix()
//        {
//            if (revertedMatrix == null)
//                //create new
//                revertedMatrix = new UnitType[gridSetting.length, gridSetting.width];
//            else
//            {
//                //copy and resize
//                var newMatrix = new UnitType[gridSetting.length, gridSetting.width];
//                for (int i = 0; i < revertedMatrix.GetLength(0); ++i)
//                {
//                    for (int j = 0; j < revertedMatrix.GetLength(1); ++j)
//                    {
//                        if (i < gridSetting.length && j < gridSetting.width)
//                        {
//                            newMatrix[i, j] = revertedMatrix[i, j];
//                        }
//                    }
//                }
//                //C# have auto GC
//                revertedMatrix = newMatrix;
//            }
           
//        }
//        private static UnitType DrawEnumElement(Rect rect, UnitType cellType)
//        {
//            //Padding
//            rect.xMin += 1;
//            rect.xMax -= 1;
//            rect.yMin += 1;
//            rect.yMax -= 1;
//            Rect enumRect = new Rect(rect);
//            enumRect.yMax -= enumRect.height * 0.8f;
//            Rect previewRect = new Rect(rect);
//            previewRect.yMin += previewRect.height * 0.2f;

//            if (Event.current.type == EventType.MouseDown && previewRect.Contains(Event.current.mousePosition))
//            {
//                int enumNum = System.Enum.GetNames(typeof(UnitType)).Length;
//                cellType = (UnitType)((((int)cellType) + 1 + enumNum) % enumNum);
//                GUI.changed = true;
//                Event.current.Use();
//            }

//            var texture = UnityEditor.AssetPreview.GetAssetPreview(Resources.Load(cellType.ToString()));
//            UnityEditor.EditorGUI.DrawPreviewTexture(previewRect, texture);
//            cellType = (UnitType)UnityEditor.EditorGUI.EnumPopup(enumRect, cellType);
//            return cellType;
//        }
//    }

//}
///// <summary>
///// Deprecated 2D Level initial data
///// </summary>
