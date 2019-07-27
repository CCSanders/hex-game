using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapCamera))]
public class MapCameraEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MapCamera camera = (MapCamera)target;
        if(GUILayout.Button("Reset Camera"))
        {
            camera.ResetCamera();
        }
    }
}
