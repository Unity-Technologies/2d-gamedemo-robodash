using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;

[CustomEditor(typeof(DefaultBrushReplacement))]
public class DefaultBrushReplacementEditor : GridBrushEditor 
{
    public override void OnPaintInspectorGUI()
    {
        GUILayout.Space(5f);
        GUILayout.Label("This is built-in default brush.");
        GUILayout.Label("It is generic brush for painting tiles and game objects.");
        GUILayout.Space(5f);
        GUILayout.Label("You'll also find other brushes from the dropdown.");
        GUILayout.Label("They are custom made for this game.");
        if (!BrushEditorUtility.SceneIsPrepared())
            BrushEditorUtility.UnpreparedSceneInspector();
    }
}
