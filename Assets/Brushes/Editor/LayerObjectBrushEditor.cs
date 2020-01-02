using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;

[CustomEditor(typeof(LayerObjectBrush<>))]
public class LayerObjectBrushEditor<T> : GridBrushEditorBase
{
    protected LayerObjectBrush<T> brush { get { return (target as LayerObjectBrush<T>); }}

    public override void OnPaintInspectorGUI()
    {
        if (BrushEditorUtility.SceneIsPrepared())
            base.OnPaintInspectorGUI();
        else
            BrushEditorUtility.UnpreparedSceneInspector();
    }

    public override void RegisterUndo(GameObject layer, GridBrushBase.Tool tool)
    {
        Undo.RegisterFullObjectHierarchyUndo(brush.GetLayer().gameObject, tool.ToString());
    }

    public override GameObject[] validTargets
    {
        get
        {
            Grid grid = FindObjectOfType<Grid>();
            return grid != null ? new GameObject[] { grid.gameObject } : null;
        }
    }
}
