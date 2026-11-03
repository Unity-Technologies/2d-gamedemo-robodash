using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class WallTile : RuleTile
{
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);

        // Random rules list each sprite as many times as its weight. Pick from that list per cell position
        // instead of with Perlin noise, so walls keep the variants they had before the switch to a Rule Tile.
        Matrix4x4 transform = Matrix4x4.identity;
        foreach (var rule in m_TilingRules)
        {
            if (!RuleMatches(rule, position, tilemap, ref transform))
                continue;

            if (rule.m_Output == TilingRuleOutput.OutputSprite.Random && rule.m_Sprites.Length > 0)
            {
                Random.InitState(position.GetHashCode());
                int total = rule.m_Sprites.Length;
                int random = Mathf.FloatToHalf(Random.value * total);
                tileData.sprite = rule.m_Sprites[Mathf.Clamp(random % total, 0, total - 1)];
            }
            break;
        }
        tileData.flags = TileFlags.LockAll;
    }

    // Rules are matched without rotation or mirroring. A neighbour counts as a wall when it holds this tile
    // or has no tile on the Floor tilemap, so the empty area around a level behaves like solid wall.
    public override bool RuleMatches(TilingRule rule, Vector3Int position, ITilemap tilemap, ref Matrix4x4 transform)
    {
        Tilemap floor = GetFloor(tilemap);
        int count = Mathf.Min(rule.m_Neighbors.Count, rule.m_NeighborPositions.Count);
        for (int i = 0; i < count; i++)
        {
            bool wall = HasWall(position + rule.m_NeighborPositions[i], tilemap, floor);
            switch (rule.m_Neighbors[i])
            {
                case TilingRuleOutput.Neighbor.This: if (!wall) return false; break;
                case TilingRuleOutput.Neighbor.NotThis: if (wall) return false; break;
            }
        }
        transform = Matrix4x4.identity;
        return true;
    }

    public bool HasWall(Vector3Int position, ITilemap tilemap, Tilemap floor)
    {
        return tilemap.GetTile(position) == this || floor != null && floor.GetTile(position) == null;
    }

    static Tilemap GetFloor(ITilemap tilemap)
    {
        Transform root = tilemap.GetComponent<Tilemap>().transform.parent;
        if (root != null)
        {
            Transform floorGo = root.Find(LevelBrush.k_FloorLayerName);
            if (floorGo != null)
                return floorGo.GetComponent<Tilemap>();
        }
        return null;
    }
}
