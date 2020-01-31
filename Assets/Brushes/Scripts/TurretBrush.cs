using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
[CustomGridBrush(false, true, false, "Turret")]
public class TurretBrush : LayerObjectBrush<BulletTurret>
{
    public TileBase m_Rail;
    private List<BulletTurret> m_Selected;
    
    public override void Paint(GridLayout grid, GameObject layer, Vector3Int position)
    {
        if (HasRail(position))
        {
            CreateObject(grid, position, m_Prefab);
            if (HasRail(position + Vector3Int.up))
                activeObject.m_StartDirection = BulletTurret.Facing.Up;
            else if (HasRail(position + Vector3Int.right))
                activeObject.m_StartDirection = BulletTurret.Facing.Right;
            else if (HasRail(position + Vector3Int.down))
                activeObject.m_StartDirection = BulletTurret.Facing.Down;
            else if (HasRail(position + Vector3Int.left))
                activeObject.m_StartDirection = BulletTurret.Facing.Left;
        }
        else
        {
            GetTilemap().SetTile(position, m_Rail);
        }
    }

    public override void Erase(GridLayout grid, GameObject layer, Vector3Int position)
    {
        if (GetObject(grid, position) != null)
            base.Erase(grid, layer, position);
        else
            GetTilemap().SetTile(position, null);
    }

    public override void Select(GridLayout grid, GameObject layer, BoundsInt position)
    {
        base.Select(grid, layer, position);

        if(m_Selected == null)
            m_Selected = new List<BulletTurret>();
        m_Selected.Clear();
        
        foreach (var turret in allObjects)
        {
            if (position.Contains(grid.WorldToCell(turret.transform.position)))
            {
                m_Selected.Add(turret);
            }
        }
    }

    public override void Move(GridLayout grid, GameObject layer, BoundsInt from, BoundsInt to)
    {
        Tilemap map = GetTilemap();
        List<TileBase> tiles = new List<TileBase>();
        foreach (var pos in from.allPositionsWithin)
        {
            tiles.Add(map.GetTile(pos));
            map.SetTile(pos, null);
        }
        int index = 0;
        foreach (var pos in to.allPositionsWithin)
        {
            map.SetTile(pos, tiles[index++]);
        }

        Vector3 worldFrom = grid.CellToWorld(from.min);
        Vector3 worldTo = grid.CellToWorld(to.min);
        
        foreach (var turret in m_Selected)
        {
            turret.transform.Translate(worldTo - worldFrom);
        }
    }

    private bool HasRail(Vector3Int position)
    {
        return GetTilemap().GetTile(position) == m_Rail;
    }

    private Tilemap GetTilemap()
    {
        Tilemap result = null;
        Transform layer = GetLayer();
        result = layer.GetComponent<Tilemap>();
        if (result == null)
            result = BrushUtility.AddTilemap(layer);
        
        return result;
    }
}
