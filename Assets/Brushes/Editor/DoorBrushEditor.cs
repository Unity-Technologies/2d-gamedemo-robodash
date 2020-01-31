using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;

[CustomEditor(typeof(DoorBrush))]
public class DoorBrushEditor : LayerObjectBrushEditor<Door> 
{
    public new DoorBrush brush { get { return (target as DoorBrush); } }
    
    public void OnSceneGUI()
    {
        Grid grid = BrushUtility.GetRootGrid(false);
        if (grid != null)
        {
            if (brush.activeObject != null && brush.activeObject.m_Key != null)
            {
                Vector3Int keypos = grid.WorldToCell(brush.activeObject.m_Key.transform.position);
                Vector3Int doorpos = grid.WorldToCell(brush.activeObject.transform.position);
                Color color = brush.activeObject.m_Key.color;
                BrushEditorUtility.DrawLine(grid, keypos, doorpos, new Color(color.r, color.g, color.b, 0.5f));
            }
        }
    }

    public override void OnPaintInspectorGUI()
    {
        if (BrushEditorUtility.SceneIsPrepared())
        {
            brush.m_KeyColor = EditorGUILayout.ColorField("Key Color", brush.m_KeyColor);
            GUILayout.Space(5f);
            GUILayout.Label("Use this brush to place doors and keys.");
            GUILayout.Label("First paint the door and then the corresponding key.");
        }
        else
        {
            BrushEditorUtility.UnpreparedSceneInspector();
        }
    }
}
