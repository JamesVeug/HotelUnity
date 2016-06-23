using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(TileMap))]
public class TileMapInspector : Editor {

    //float value = .5f;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        /*EditorGUILayout.BeginVertical();
        value = EditorGUILayout.Slider(value, 0, 2.0f, null);
        EditorGUILayout.EndVertical();*/

        if (GUILayout.Button("Regenerate"))
        {
            TileMap tileMap = (TileMap)target;
            tileMap.BuildMesh();
        }
    }
}
