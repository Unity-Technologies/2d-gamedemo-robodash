using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[CustomGridBrush(false, true, false, "Laser")]
public class LaserBrush : LayerObjectBrush<LaserBeamTurret>
{
    private List<LaserBeamTurret> m_Selection;
    public override bool alwaysCreateOnPaint { get { return true; } }
    
    public override void Paint(GridLayout grid, GameObject layer, Vector3Int position)
    {
        LaserBeamTurret current = activeObject;
        LaserBeamTurret next = activeObject != null ? activeObject.nextTurret : null;
        base.Paint(grid, layer, position);

        if (current != null)
        {
            current.nextTurret = activeObject;
            BrushUtility.SetDirty(current);
        }
        else if (activeObject != null)
        {
            activeObject.nextTurret = activeObject;
            BrushUtility.SetDirty(activeObject);
        }

        if (next != null)
        {
            activeObject.nextTurret = next;
            BrushUtility.SetDirty(activeObject);
        }

        EnsureFirstExists();
        EnsureLooping();
    }

    public void EnsureFirstExists()
    {
        if (activeObject != null)
        {
            if (activeObject.GetFirst() == null)
            {
                activeObject.firstTurret = true;
            }
        }
    }

    public void EnsureLooping()
    {
        if (activeObject != null)
        {
            LaserBeamTurret first = activeObject.GetFirst();
            LaserBeamTurret current = first.nextTurret;
            while (current != first && current != null)
            {
                if (current.nextTurret == null)
                {
                    current.nextTurret = first;
                    BrushUtility.SetDirty(current);
                    return;
                }
                current = current.nextTurret;
            }
        }
    }

    public override void Erase(GridLayout grid, GameObject layer, Vector3Int position)
    {
        LaserBeamTurret current = GetObject(grid, position);
        LaserBeamTurret previous = current != null ? current.GetPrevious() : null;
        LaserBeamTurret next = current != null ? current.nextTurret : null;

        base.Erase(grid, layer, position);

        if (previous != null)
        {
            previous.nextTurret = next;
            BrushUtility.SetDirty(previous);
        }
        EnsureFirstExists();
        EnsureLooping();
    }

    public override void Move(GridLayout grid, GameObject layer, BoundsInt from, BoundsInt to)
    {
        Vector3 fromWorld = grid.CellToWorld(from.min);
        Vector3 toWorld = grid.CellToWorld(to.min);
        Vector3 move = toWorld - fromWorld;
        foreach (var turret in m_Selection)
        {
            turret.transform.Translate(move);
        }
    }

    public override void Select(GridLayout grid, GameObject layer, BoundsInt position)
    {
        base.Select(grid, layer, position);

        if(m_Selection == null)
            m_Selection = new List<LaserBeamTurret>();

        m_Selection.Clear();

        foreach (var turret in allObjects)
        {
            if (position.Contains(grid.WorldToCell(turret.transform.position)))
            {
                m_Selection.Add(turret);
            }
        }
    }
}
