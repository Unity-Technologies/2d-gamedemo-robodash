using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TurretBrush))]
public class TurretBrushEditor : LayerObjectBrushEditor<BulletTurret> 
{
    public override void OnPaintInspectorGUI()
    {
        if (BrushEditorUtility.SceneIsPrepared())
        {
            GUILayout.Space(5f);
            GUILayout.Label("Use paint tool to paint rails.");
            GUILayout.Label("Then paint again on the rails to place a turret.");
        }
        else
        {
            BrushEditorUtility.UnpreparedSceneInspector();
        }
    }

    public override void RegisterUndo(GameObject layer, GridBrushBase.Tool tool)
    {
        Undo.RegisterFullObjectHierarchyUndo(brush.GetLayer().gameObject, tool.ToString());
    }
}
