using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(SequenceTurretBrush))]
public class SequenceTurretBrushEditor : LayerObjectBrushEditor<SequenceTurret> 
{
    public new SequenceTurretBrush brush { get { return (target as SequenceTurretBrush); } }
    public void OnSceneGUI()
    {
        Grid grid = BrushUtility.GetRootGrid(false);
        if (grid != null)
        {
            if (brush.activeObject != null)
            {
                Vector3Int worldTurret = grid.WorldToCell(brush.activeObject.transform.position);
                for (int i = 0; i < brush.activeObject.m_Targets.Count; i++)
                {
                    Vector3Int localPos = brush.activeObject.m_Targets[i];
                    Vector3Int worldPos = worldTurret + localPos;
                    int tick = brush.activeObject.m_Ticks[i];
                    Handles.Label(grid.CellToWorld(worldPos + Vector3Int.up), " " + tick.ToString());
                    BrushEditorUtility.DrawLine(grid, worldPos, grid.WorldToCell(brush.activeObject.transform.position), new Color(1f, 0f, 1f, 0.6f));
                    BrushEditorUtility.DrawQuad(grid, worldPos, new Color(1f, 0f, 1f, 0.4f));
                }
                BrushEditorUtility.DrawMarquee(grid, worldTurret, new Color(1f, 0f, 1f, 0.6f));
                Vector3 world = grid.CellToWorld(worldTurret);
                Handles.Label(world, " " + brush.activeObject.m_TickDelay.ToString("F"));
            }
        }
    }

    public override void OnPaintSceneGUI(GridLayout grid, GameObject layer, BoundsInt position, GridBrushBase.Tool tool, bool executing)
    {
        base.OnPaintSceneGUI(grid, layer, position, tool, executing);
        if (brush.activeObject != null)
        {
            if (tool == GridBrushBase.Tool.Box || tool == GridBrushBase.Tool.Paint)
            {
                Handles.Label(grid.CellToWorld(position.min + Vector3Int.up), " " + brush.m_Tick.ToString());
            }
        }
    }

    public override void OnPaintInspectorGUI()
    {
        if (BrushEditorUtility.SceneIsPrepared())
        {
            brush.m_Tick = Mathf.Max(EditorGUILayout.IntField("Tick", brush.m_Tick), 1);
            brush.m_TickDelay = Mathf.Max(EditorGUILayout.FloatField("Tick Length", brush.m_TickDelay), 0.1f);
            GUILayout.Space(5f);
            GUILayout.Label("Use paint tool to place sequence turrets.");
            GUILayout.Space(5f);
            GUILayout.Label("Then paint again to place it's target(s).");
            GUILayout.Label("Set different ticks to the targets to create sequences.");
            GUILayout.Label("Use picking tool to select existing turret.");
            GUILayout.Space(5f);
            GUILayout.Label("Hotkeys . and , to rotate the tick number.");
        }
        else
        {
            BrushEditorUtility.UnpreparedSceneInspector();
        }
    }
}
