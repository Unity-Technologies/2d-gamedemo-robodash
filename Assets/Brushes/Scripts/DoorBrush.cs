using UnityEngine;

[CreateAssetMenu]
[CustomGridBrush(false, true, false, "Door")]
public class DoorBrush : LayerObjectBrush<Door>
{
    public GameObject m_KeyPrefab;
    public Color m_KeyColor;

    public override void Paint(GridLayout grid, GameObject layer, Vector3Int position)
    {
        if (activeObject != null)
        {
            if (activeObject.m_Key == null)
            {
                GameObject newKey = BrushUtility.Instantiate(m_KeyPrefab, grid.LocalToWorld(grid.CellToLocalInterpolated(position + offsetFromBottomLeft)), GetLayer());
                newKey.GetComponent<Doorkey>().m_Door = activeObject;
                BrushUtility.SetDirty(newKey);

                activeObject.m_Key = newKey.GetComponent<Doorkey>();
                newKey.GetComponent<Doorkey>().SetColor(m_KeyColor);
                activeObject.SetColor(m_KeyColor);
                BrushUtility.SetDirty(activeObject);
            }
            else
            {
                BrushUtility.Select(BrushUtility.GetRootGrid(false).gameObject);
            }
        }
        base.Paint(grid, layer, position);
        if (activeObject)
        {
            activeObject.SetColor(m_KeyColor);
        }
    }

    public override void Erase(GridLayout grid, GameObject layer, Vector3Int position)
    {
        foreach (var door in allObjects)
        {
            if (grid.WorldToCell(door.transform.position) == position)
            {
                DestroyDoor(door);
                BrushUtility.Select(BrushUtility.GetRootGrid(false).gameObject);
                return;
            }
            if (door.m_Key != null && grid.WorldToCell(door.m_Key.transform.position) == position)
            {
                DestroyImmediate(door.m_Key.gameObject);
                door.m_Key = null;
                BrushUtility.SetDirty(door);
            }
        }
    }

    private void DestroyDoor(Door door)
    {
        if(door.m_Key != null)
            DestroyImmediate(door.m_Key.gameObject);
        DestroyImmediate(door.gameObject);
    }
}
