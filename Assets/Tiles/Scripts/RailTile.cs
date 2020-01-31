using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class RailTile : TileBase
{
    public Sprite[] m_Sprites;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        int mask = (HasRail(position + Vector3Int.up, tilemap) ? 1 : 0)
                + (HasRail(position + Vector3Int.right, tilemap) ? 2 : 0)
                + (HasRail(position + Vector3Int.down, tilemap) ? 4 : 0)
                + (HasRail(position + Vector3Int.left, tilemap) ? 8 : 0);

        tileData.sprite = m_Sprites[mask];
        tileData.flags = TileFlags.LockTransform;
    }

    public bool HasRail(Vector3Int position, ITilemap tilemap)
    {
        return tilemap.GetTile(position) == this;
    }

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        foreach (var p in new BoundsInt(-1, -1, 0, 3, 3, 1).allPositionsWithin)
        {
            tilemap.RefreshTile(position + p);
        }
    }
}
