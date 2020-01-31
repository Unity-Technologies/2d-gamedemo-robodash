using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TeleportBrush))]
public class TeleportBrushEditor : LayerObjectBrushEditor<Teleport> 
{
    public void OnSceneGUI()
    {
        Grid grid = BrushUtility.GetRootGrid(false);
        if (grid == null)
            return;

        if (brush.activeObject != null)
        {
            BrushEditorUtility.DrawMarquee(grid, grid.WorldToCell(brush.activeObject.transform.position), new Color(0.5f, 0.5f, 1f));
        }

        Teleport[] allTeleports = brush.allObjects;
        BrushEditorUtility.BeginLines(Color.blue);
        foreach (var teleport in allTeleports)
        {
            if (teleport != null && teleport.m_Target != null)
            {
                Vector3Int from = grid.WorldToCell(teleport.transform.position);
                Vector3Int to = grid.WorldToCell(teleport.m_Target.transform.position);
                BrushEditorUtility.DrawLineBatched(grid, from, to);
            }
        }
        BrushEditorUtility.EndLines();
        
    }

    public override void OnPaintInspectorGUI()
    {
        if (BrushEditorUtility.SceneIsPrepared())
        {
            GUILayout.Space(5f);
            GUILayout.Label("Paint to place a blue teleport.");
            GUILayout.Label("Then paint again to place it's orange target.");
        }
        else
        {
            BrushEditorUtility.UnpreparedSceneInspector();
        }
    }
}
