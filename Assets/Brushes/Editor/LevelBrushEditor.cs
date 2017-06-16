using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(LevelBrush))]
public class LevelBrushEditor : GridBrushEditorBase
{
	public void OnEnable()
	{
		Undo.undoRedoPerformed += UndoRedoPerformed;
	}

	public void OnDisable()
	{
		Undo.undoRedoPerformed -= UndoRedoPerformed;
	}

	private void UndoRedoPerformed()
	{
		LevelBrush.ResetLevelCache();
		TintTextureGenerator.RefreshTintmap();
	}

	public override void OnPaintInspectorGUI()
	{
		GUILayout.Space(5f);
		GUILayout.Label("This brush is for painting the floorplan.");
		GUILayout.Space(5f);
		
		if (!BrushEditorUtility.SceneIsPrepared())
			BrushEditorUtility.UnpreparedSceneInspector();
	}

	public override void OnPaintSceneGUI(GridLayout grid, GameObject layer, BoundsInt position, GridBrushBase.Tool tool, bool executing)
	{
		Tilemap floor = LevelBrush.GetFloor();
		Tilemap walls = LevelBrush.GetWall();

		if (floor != null && walls != null)
		{
			LevelBrush.InitializeLevelCacheIfNecessary();
			HashSet<Vector3Int> level = LevelBrush.GetAllFloors();
			
			BrushEditorUtility.BeginQuads(new Color(1f, 0f, 0f, 0.5f));
			foreach (Vector3Int pos in LevelBrush.GetLevelBounds().allPositionsWithin)
			{
				if (walls.GetTile(pos) != null)
				{
					int mask = 0;
					if (!level.Contains(pos + Vector3Int.up))
						mask += 1;
					if (!level.Contains(pos + Vector3Int.right))
						mask += 2;
					if (!level.Contains(pos + Vector3Int.down))
						mask += 4;
					if (!level.Contains(pos + Vector3Int.left))
						mask += 8;

					if (mask == 5 || mask == 10 || mask == 1 || mask == 2 || mask == 4 || mask == 8 || mask == 0)
						BrushEditorUtility.DrawQuadBatched(grid, pos);
				}
			}
			BrushEditorUtility.EndQuads();
		}

		base.OnPaintSceneGUI(grid, layer, position, tool, executing);
	}

	public override void OnToolActivated(GridBrushBase.Tool tool)
	{
		LevelBrush.ResetLevelCache();
	}

	public override void RegisterUndo(GameObject layer, GridBrushBase.Tool tool)
	{
		Undo.RegisterCompleteObjectUndo(LevelBrush.GetWall(), "Paint");
		Undo.RegisterCompleteObjectUndo(LevelBrush.GetFloor(), "Paint");
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
