using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[CustomGridBrush(false, true, false, "Sequence Turret")]
[CreateAssetMenu]
#endif
public class SequenceTurretBrush : LayerObjectBrush<SequenceTurret>
{
    private List<SequenceTurret> m_Selection;
 
    public float m_TickDelay;
    public int m_Tick;

    public override void Paint(GridLayout grid, GameObject layer, Vector3Int position)
    {
        bool hasActiveObject = activeObject != null;
        if (hasActiveObject)
        {
            if (activeObject.m_Targets == null)
            {
                activeObject.m_Targets = new List<Vector3Int>();
                activeObject.m_Ticks = new List<int>();
                activeObject.m_TickDelay = m_TickDelay;
            }
            Vector3Int localPosition = position - grid.WorldToCell(activeObject.transform.position);
            activeObject.m_Targets.Add(localPosition);
            activeObject.m_Ticks.Add(m_Tick);
            BrushUtility.SetDirty(activeObject);
        }

        base.Paint(grid, layer, position);

        if (!hasActiveObject)
        {
            activeObject.m_TickDelay = m_TickDelay;
            BrushUtility.SetDirty(activeObject);
        }
    }

    public override void Erase(GridLayout grid, GameObject layer, Vector3Int position)
    {
        if (activeObject != null && activeObject.m_Targets != null)
        {
            for (int i = activeObject.m_Targets.Count - 1; i >= 0; i--)
            {
                Vector3Int worldPosition = grid.WorldToCell(activeObject.transform.position) + activeObject.m_Targets[i];
                if (worldPosition == position)
                {
                    activeObject.m_Ticks.RemoveAt(i);
                    activeObject.m_Targets.RemoveAt(i);
                    BrushUtility.SetDirty(activeObject);
                    return;
                }
            }
        }
        base.Erase(grid, layer, position);
    }

    public override void Select(GridLayout grid, GameObject layer, BoundsInt position)
    {
        if (m_Selection == null)
            m_Selection = new List<SequenceTurret>();
        m_Selection.Clear();

        foreach (var turret in allObjects)
        {
            Vector3Int pos = grid.WorldToCell(turret.transform.position);
            if (position.Contains(pos))
            {
                m_Selection.Add(turret);
            }
        }
    }

    public override void Move(GridLayout grid, GameObject layer, BoundsInt from, BoundsInt to)
    {
        Vector3Int offset = to.min - from.min;
        foreach (var turret in m_Selection)
        {
            Vector3Int pos = grid.WorldToCell(turret.transform.position - offsetFromBottomLeft);
            Vector3 newPos = grid.CellToWorld(pos + offset);
            turret.transform.position = newPos + offsetFromBottomLeft;
        }
    }

    public override void Rotate(RotationDirection direction, Grid.CellLayout layout)
    {
        if (direction == RotationDirection.CounterClockwise)
        {
            m_Tick++;
        }
        else
        {
            m_Tick--;
            m_Tick = Mathf.Max(1, m_Tick);
        }
        
    }
}
