using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
[CustomGridBrush(false, true, false, "Level")]
public class LevelBrush : GridBrushBase
{
	public const string k_WallLayerName = "Walls";
	public const string k_FloorLayerName = "Floor";

	public TileBase m_Wall;
	public TileBase m_Floor;

	[NonSerialized]
	private static HashSet<Vector3Int> s_LevelCache;
	private static BoundsInt? s_LevelBoundsCache;

	public override void Paint(GridLayout grid, GameObject layer, Vector3Int position)
	{
		Tilemap walls = GetWall();
		Tilemap floor = GetFloor();

		if (walls != null && floor != null)
		{
			PaintInternal(position, floor, walls);
			TintTextureGenerator.RefreshTintmap(position);
		}
	}

	private void PaintInternal(Vector3Int position, Tilemap floor, Tilemap walls)
	{
		floor.SetTile(position, m_Floor);
		walls.SetTile(position, null);
		InitializeLevelCacheIfNecessary();
		RefreshLevelCache(position, floor, walls);
	}

	public override void Erase(GridLayout grid, GameObject layer, Vector3Int position)
	{
		Tilemap walls = GetWall();
		Tilemap floor = GetFloor();

		if (walls != null && floor != null)
		{
			EraseInternal(position, floor, walls);
			TintTextureGenerator.RefreshTintmap(position);
		}
	}

	private void EraseInternal(Vector3Int position, Tilemap floor, Tilemap walls)
	{
		floor.SetTile(position, null);
		walls.SetTile(position, m_Wall);
		InitializeLevelCacheIfNecessary();
		RefreshLevelCache(position, floor, walls);
	}

	public override void BoxFill(GridLayout grid, GameObject layer, BoundsInt position)
	{
		Tilemap walls = GetWall();
		Tilemap floor = GetFloor();

		if (walls != null && floor != null)
		{
			foreach (var pos in position.allPositionsWithin)
			{
				PaintInternal(pos, floor, walls);				
			}
			TintTextureGenerator.RefreshTintmap(position);
		}
	}

	public override void BoxErase(GridLayout grid, GameObject layer, BoundsInt position)
	{
		Tilemap walls = GetWall();
		Tilemap floor = GetFloor();

		if (walls != null && floor != null)
		{
			foreach (var pos in position.allPositionsWithin)
			{
				EraseInternal(pos, floor, walls);
			}
			TintTextureGenerator.RefreshTintmap(position);
		}
	}

	public static void ResetLevelCache()
	{
		if (s_LevelCache != null)
		{
			s_LevelCache.Clear();
			s_LevelCache = null;
			s_LevelBoundsCache = null;
		}
		InitializeLevelCacheIfNecessary();
	}
	
	public static void InitializeLevelCacheIfNecessary()
	{
		if (s_LevelCache == null)
		{
			s_LevelCache = new HashSet<Vector3Int>();
			Tilemap wall = GetWall();
			Tilemap floor = GetFloor();
			if (wall != null && floor != null)
			{
				foreach (var pos in wall.cellBounds.allPositionsWithin)
				{
					if (wall.GetTile(pos) == null && floor.GetTile(pos) != null)
					{
						s_LevelCache.Add(pos);
					}
				}
			}
		}
	}

	private static void RefreshLevelCache(Vector3Int position, Tilemap floor, Tilemap walls)
	{
		if (floor != null && walls != null)
		{
			if (walls.GetTile(position) == null && floor.GetTile(position) != null)
			{
				s_LevelCache.Add(position);
			} 
			else if (s_LevelCache.Contains(position))
			{
				s_LevelCache.Remove(position);
			}
			if (!GetLevelBounds().Contains(position))
				s_LevelBoundsCache = null;
		}
	}

	public static HashSet<Vector3Int> GetAllFloors()
	{
		return s_LevelCache;
	}

	public static BoundsInt GetLevelBounds()
	{
		if (!s_LevelBoundsCache.HasValue)
		{
			Vector3Int min = new Vector3Int(s_LevelCache.Min(p => p.x), s_LevelCache.Min(p => p.y), 0);
			Vector3Int max = new Vector3Int(s_LevelCache.Max(p => p.x) + 1, s_LevelCache.Max(p => p.y) + 1, 1);
			s_LevelBoundsCache = new BoundsInt(min, max - min);
		}
		return s_LevelBoundsCache.Value;
	}

	public static bool IsFloorFast(Vector3Int position)
	{
		return s_LevelCache.Contains(position);
	}

	public static bool IsFloor(Vector3Int position, Tilemap walls, Tilemap floor)
	{
		return floor.GetTile(position) != null && walls.GetTile(position) == null;
	}

	public static Tilemap GetWall()
	{
		GameObject go = GameObject.Find(k_WallLayerName);
		return go != null ? go.GetComponent<Tilemap>() : null;
	}

	public static Tilemap GetFloor()
	{
		GameObject go = GameObject.Find(k_FloorLayerName);
		return go != null ? go.GetComponent<Tilemap>() : null;
	}
}
