using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(TintBrush))]
public class TintBrushEditor : GridBrushEditorBase
{
    public TintBrush brush { get { return target as TintBrush; } }

    public override void OnPaintInspectorGUI()
    {
        if (BrushEditorUtility.SceneIsPrepared())
        {
            brush.m_Color = EditorGUILayout.ColorField("Color", brush.m_Color);
            brush.m_Blend = EditorGUILayout.Slider("Blend", brush.m_Blend, 0f, 1f);
            GUILayout.Space(5f);
            GUILayout.Label("Use this brush to tint areas with a color.");
            GUILayout.Space(5f);
        }
        else
        {
            BrushEditorUtility.UnpreparedSceneInspector();
        }
    }
}
