using UnityEngine;
using UnityEngine.Tilemaps;
[ExecuteInEditMode]
public class TintTextureGenerator : MonoBehaviour
{
	public Color shadowColor;
	public Color defaultColor;

	Texture2D tintTexture;
	Color32[] colorArray;
    
	GridInformation gi;
	private Tilemap floor;
	private Tilemap wall;

    void Start() 
	{
        Refresh();
    }

	public bool BeginUpdate()
	{
		return InitializeCachedReferences();
	}

	public void UpdateColorBatched(Vector3Int position, Color c)
	{
		colorArray[WorldToIndex(position)] = c;
	}

	public void UpdateColorSingle(Vector3Int position, Color c)
	{
		Vector3Int texpos = WorldToTexture(position);
		tintTexture.SetPixel(texpos.x, texpos.y, c);
	}

	public void EndBatchedUpdate()
	{
		tintTexture.SetPixels32(colorArray);
		tintTexture.Apply();
	}

	public void EndSingleUpdate()
	{
		tintTexture.Apply();
	}

	void Refresh(BoundsInt position)
	{
		if (!BeginUpdate())
			return;

		BoundsInt extruded = new BoundsInt(position.min + new Vector3Int(-2, -2, 0), position.size + new Vector3Int(4, 4, 0));
		
		foreach (var cell in extruded.allPositionsWithin)
		{
			int index = WorldToIndex(cell);
			if (colorArray.Length <= index || index < 0)
			{
				Debug.LogWarning("Painting outside texture limits");
				return;
			}
			UpdateColorSingle(cell, GetColor(cell));
		}
		EndSingleUpdate();

		Shader.SetGlobalTexture("_TintMap", tintTexture);
	}

	Color GetColor(Vector3Int position)
	{
		Color c = defaultColor;
		if(WantsTint(position))
			c = gi.GetPositionProperty(position, "TintColor", defaultColor);

		int shadowStrength = ShadowStrength(position);
		if (shadowStrength == 1)
			c *= Color.Lerp(shadowColor, Color.white, 0.5f);
		else if (shadowStrength == 2)
			c *= shadowColor;

		if (!LevelBrush.IsFloorFast(position))
			c = Color.Lerp(c, Color.black, DistanceToLevel(position) / 10);

		return c;
	}

	float DistanceToLevel(Vector3Int position)
	{
		int result = int.MaxValue;
		foreach (var pos in new BoundsInt(position + new Vector3Int(-2, -2, 0), new Vector3Int(5, 5, 1)).allPositionsWithin)
		{
			if (LevelBrush.IsFloorFast(pos))//, wall, floor))
			{
				result = Mathf.Min((pos - position).sqrMagnitude, result);
			}
		}
		return result - 2;
	}

    void Refresh()
    {
	    if (!BeginUpdate()) return;

	    foreach (var pos in LevelBrush.GetWall().cellBounds.allPositionsWithin)
	    {
			colorArray[WorldToIndex(pos)] = GetColor(pos);
	    }	

        EndBatchedUpdate();

        Shader.SetGlobalTexture("_TintMap", tintTexture);
    }

	int ShadowStrength(Vector3Int position)
	{
		int result = 0;
		if (floor.GetTile(position) != null && wall.GetTile(position) == null)
		{
			if (wall.GetTile(position + Vector3Int.left) != null || wall.GetTile(position + Vector3Int.up) != null)
				result++;
			if (wall.GetTile(position + Vector3Int.left + Vector3Int.up) != null)
				result++;
		}
		return result;
	}

	bool WantsTint(Vector3Int position)
	{
		return floor.GetTile(position) != null && wall.GetTile(position) == null ||
			wall.GetTile(position) != null && wall.GetTile(position + Vector3Int.down) == null && floor.GetTile(position + Vector3Int.down) != null;
	}

	Vector3Int WorldToTexture(Vector3Int world)
	{
		return new Vector3Int(world.x + tintTexture.width / 2, world.y + tintTexture.height / 2, 0);
	}

	Vector3Int TextureToWorld(Vector3Int texpos)
	{
		return new Vector3Int(texpos.x - tintTexture.width / 2, texpos.y - tintTexture.height / 2, 0);
	}

	int WorldToIndex(Vector3Int world)
	{
		Vector3Int texpos = WorldToTexture(world);
		return texpos.y*tintTexture.width + texpos.x%tintTexture.width;
	}

	Vector3Int IndexToWorld(int index)
	{
		Vector3Int texpos = new Vector3Int(index % tintTexture.width, index / tintTexture.width, 0);
		return TextureToWorld(texpos);
	}

	bool InitializeCachedReferences()
	{
		if (floor == null)
			floor = LevelBrush.GetFloor();
		if (wall == null)
			wall = LevelBrush.GetWall();
		if (gi == null) 
			gi = GetComponentInParent<GridInformation>();
		
		if (tintTexture == null)
		{
			tintTexture = new Texture2D(256, 256, TextureFormat.ARGB32, false);
			tintTexture.wrapMode = TextureWrapMode.Clamp;
			tintTexture.filterMode = FilterMode.Bilinear;
		}
		colorArray = new Color32[tintTexture.width * tintTexture.height];

		LevelBrush.InitializeLevelCacheIfNecessary();

		return floor && wall & gi & tintTexture;
	}

	public static void RefreshTintmap()
	{
		TintTextureGenerator instance = FindObjectOfType<TintTextureGenerator>();
		if(instance != null)
			instance.Refresh();
	}

	public static void RefreshTintmap(Vector3Int position)
	{
		TintTextureGenerator instance = FindObjectOfType<TintTextureGenerator>();
		if (instance != null)
			instance.Refresh(new BoundsInt(position, Vector3Int.one));
	}

	public static void RefreshTintmap(BoundsInt position)
	{
		TintTextureGenerator instance = FindObjectOfType<TintTextureGenerator>();
		if (instance != null)
			instance.Refresh(position);
	}
}
