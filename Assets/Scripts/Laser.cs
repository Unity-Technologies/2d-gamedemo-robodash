using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public List<Vector3Int> m_Waypoints;

    void OnDrawGizmosSelected()
    {
        Grid grid = GetComponentInParent<Grid>();
        if (grid != null && m_Waypoints.Count > 0)
        {
            Vector3Int laserPosition = grid.WorldToCell(transform.position);

            int count = m_Waypoints.Count;
            Gizmos.color = new Color(1f,0.5f,0.5f,0.8f);
            for (int index = 1; count > 1 && index <= count; index++)
            {
                Vector3 world1 = grid.GetCellCenterWorld(laserPosition + m_Waypoints[index - 1]);
                Vector3 world2 = grid.GetCellCenterWorld(laserPosition + m_Waypoints[index % count]);
                Gizmos.DrawLine(world1, world2);

            }

            Vector3 world = grid.GetCellCenterWorld(laserPosition + m_Waypoints[0]);
            Vector3 up = (grid.GetCellCenterWorld(laserPosition + m_Waypoints[0] + Vector3Int.up) - world) * .25f;
            Vector3 right = (grid.GetCellCenterWorld(laserPosition + m_Waypoints[0] + Vector3Int.right) - world) * .25f;
            Gizmos.DrawLine (world + up, world + right);
            Gizmos.DrawLine (world + right, world - up);
            Gizmos.DrawLine (world - up, world - right);
            Gizmos.DrawLine (world - right, world + up);
        }
    }
}
