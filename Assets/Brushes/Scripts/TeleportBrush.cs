using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu]
[CustomGridBrush(false, true, false, "Swarm")]
public class TeleportBrush : LayerObjectBrush<Teleport>
{
    public override bool alwaysCreateOnPaint { get { return true; } }
    public GameObject m_SecondPrefab;
    private List<Teleport> m_Selection;

    public override void Paint(GridLayout grid, GameObject layer, Vector3Int position)
    {
        Teleport previous = activeObject;

        if (previous == null || previous.m_Target != null || BrushUtility.GetPrefabRoot(previous.transform) == m_SecondPrefab)
            CreateObject(grid, position, m_Prefab);
        else
            CreateObject(grid, position, m_SecondPrefab);

        if (previous != null && activeObject != null && previous.m_Target == null)
        {
            previous.m_Target = activeObject;
            activeObject.m_Target = previous;
            BrushUtility.SetDirty(previous);
            BrushUtility.SetDirty(activeObject);
        }
    }

    public override void Erase(GridLayout grid, GameObject layer, Vector3Int position)
    {
        Teleport other = null;
        if(GetObject(grid, position) != null)
            other = GetObject(grid, position).m_Target;
        base.Erase(grid, layer, position);

        if (other != null)
        {
            other.m_Target = null;
            BrushUtility.SetDirty(other);
        }
    }

    public override void Select(GridLayout grid, GameObject layer, BoundsInt position)
    {
        base.Select(grid, layer, position);

        if (m_Selection == null)
            m_Selection = new List<Teleport>();

        m_Selection.Clear();

        foreach (var turret in allObjects)
        {
            if (position.Contains(grid.WorldToCell(turret.transform.position)))
            {
                m_Selection.Add(turret);
            }
        }
    }

    public override void Move(GridLayout grid, GameObject layer, BoundsInt from, BoundsInt to)
    {
        Vector3 fromWorld = grid.CellToWorld(from.min);
        Vector3 toWorld = grid.CellToWorld(to.min);
        Vector3 move = toWorld - fromWorld;
        foreach (var teleport in m_Selection)
        {
            teleport.transform.Translate(move);
        }
    }
}
