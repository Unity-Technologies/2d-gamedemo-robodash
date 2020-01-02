using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(LaserBrush))]
public class LaserBrushEditor : LayerObjectBrushEditor<LaserBeamTurret>
{
    public override void OnPaintInspectorGUI()
    {
        if (BrushEditorUtility.SceneIsPrepared())
        {
            GUILayout.Space(5f);
            GUILayout.Label("Use paint tool to place a new laser tower.");
            GUILayout.Label("The new tower will connect to currently selected one.");
            GUILayout.Space(5f);
            GUILayout.Label("Select an existing laser via picking tool.");
        }
        else
        {
            BrushEditorUtility.UnpreparedSceneInspector();
        }
    }
}

