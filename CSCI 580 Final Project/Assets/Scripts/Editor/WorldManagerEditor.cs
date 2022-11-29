using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WorldManager))]
public class WorldManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        WorldManager worldManager = (WorldManager)target;
        if (DrawDefaultInspector())
        {
            if (worldManager.AutoUpdate)
            {
                //worldManager.DrawMapInEditor();
            }
        }
        if (GUILayout.Button("Generate"))
        {
            //worldManager.DrawMapInEditor();
        }

    }
}