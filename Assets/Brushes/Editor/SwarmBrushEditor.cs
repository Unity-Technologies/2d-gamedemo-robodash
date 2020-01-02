using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(SwarmBrush))]
public class SwarmBrushEditor : GridBrushEditorBase
{
    public SwarmBrush brush { get { return (target as SwarmBrush); } }

    public void OnSceneGUI()
    {
        Grid grid = BrushUtility.GetRootGrid(true);
        GridInformation info = BrushUtility.GetRootGridInformation(false);
        if (info != null)
        {
            foreach (var pos in info.GetAllPositions(SwarmBrush.k_SwarmDifficultyProperty))
            {
                float difficulty = info.GetPositionProperty(pos, SwarmBrush.k_SwarmDifficultyProperty, 0f);
                Color col = Color.Lerp(new Color(0f, 1f, 0f, 0.3f), new Color(1f, 0f, 0f, 0.3f), difficulty);
                BrushEditorUtility.DrawQuad(grid, pos, col);
            }
        }
    }

    public override void OnPaintInspectorGUI()
    {
        if (BrushEditorUtility.SceneIsPrepared())
        {
            GUILayout.BeginHorizontal();
            brush.m_Difficulty = EditorGUILayout.Slider("Difficulty", brush.m_Difficulty, 0f, 1f);
            GUILayout.EndHorizontal();
            GUILayout.Space(5f);
            GUILayout.Label("Paint areas where you want minions spawning.");
            GUILayout.Label("Adjust difficulty slider for more and tougher minions.");
        }
        else
        {
            BrushEditorUtility.UnpreparedSceneInspector();
        }
    }

    public override void RegisterUndo(GameObject layer, GridBrushBase.Tool tool)
    {
        GridInformation info = BrushUtility.GetRootGridInformation(false);
        if (info != null)
        {
            Undo.RegisterFullObjectHierarchyUndo(info, tool.ToString());
        }
    }
}
