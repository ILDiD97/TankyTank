using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BoardGenerator))]
public class CreateFloorCells : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        BoardGenerator myClass = (BoardGenerator)target;
        if (GUILayout.Button("Build"))
        {
            myClass.CreateLevel(true);
        }
        if (GUILayout.Button("Destroy"))
        {
            myClass.DestroyLevel(true);
        }
    }
}
