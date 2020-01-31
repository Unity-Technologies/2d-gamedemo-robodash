using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class LayerObjectBrush<T> : GridBrushBase
{
    public T activeObject { get { return BrushUtility.GetSelection() != null ? BrushUtility.GetSelection().GetComponent<T>() : default(T); } }
    public T[] allObjects { get { return BrushUtility.GetRootGrid(false) != null ? BrushUtility.GetRootGrid(false).GetComponentsInChildren<T>() : default(T[]); } }

    protected virtual Vector3 offsetFromBottomLeft { get { return m_PrefabOffset; } }
    public virtual bool alwaysCreateOnPaint { get { return false; } }
    public GameObject m_Prefab;
    public string m_LayerName;
    public Vector3 m_PrefabOffset;

    public override void Paint(GridLayout grid, GameObject layer, Vector3Int position)
    {
        if (m_Prefab == null)
        {
            Debug.LogError("Prefab is null. Brush paint operation cancelled.");
            return;
        }

        if (string.IsNullOrEmpty(m_LayerName))
        {
            Debug.LogError("Layer name is empty. Brush paint operation cancelled.");
            return;
        }

        if (activeObject == null || alwaysCreateOnPaint)
        {
            T obj = GetObject(grid, position);
            if (obj is Component)
            {
                BrushUtility.Select((obj as Component).gameObject);
            }
            else
            {
                CreateObject(grid, position, m_Prefab);
            }
        }
    }

    protected void CreateObject(GridLayout grid, Vector3Int position, GameObject prefab)
    {
        if (m_Prefab.GetComponent<T>() != null)
        {
            GameObject newObj = BrushUtility.Instantiate(prefab, grid.LocalToWorld(grid.CellToLocalInterpolated(position + offsetFromBottomLeft)), GetLayer());
            BrushUtility.Select(newObj);
        }
        else
        {
            Debug.LogError("Prefab " + m_Prefab.name + " doesn't contain component " + typeof(T) + ", brush paint operation cancelled.");
        }
    }

    public override void Erase(GridLayout grid, GameObject layer, Vector3Int position)
    {
        T obj = GetObject(grid, position);
        if (obj is Component)
        {
            BrushUtility.Destroy((obj as Component).gameObject);
            BrushUtility.Select(GetLayer().gameObject);
        }
    }

    public override void Pick(GridLayout grid, GameObject layer, BoundsInt position, Vector3Int pivot)
    {
        T obj = GetObject(grid, position.min);
        if (obj is Component)
        {
            BrushUtility.Select((obj as Component).gameObject);
        }
        else
        {
            BrushUtility.Select(GetLayer().gameObject);
        }
    }

    public T GetObject(GridLayout grid, Vector3Int position)
    {
        Transform parent = GetLayer();
        List<GameObject> children = new List<GameObject>();
        for(int i=0; i<parent.childCount; i++)
        {
            Vector3 p = parent.GetChild(i).position;
            if(grid.WorldToCell(p) == position)
                children.Add(parent.GetChild(i).gameObject);
        }
        return GetObject(children);
    }

    public T GetObject(List<GameObject> gameObjects)
    {
        foreach (var gameObject in gameObjects)
        {
            T obj = gameObject.GetComponent<T>();
            if (obj != null)
            {
                return obj;
            }
        }
        return default(T);
    }

    public Transform GetLayer()
    {
        Transform layer = BrushUtility.GetRootGrid(false).transform.Find(m_LayerName);
        if (layer == null)
        {
            GameObject newGameObject = new GameObject(m_LayerName);
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(newGameObject, "Create " + m_LayerName);
#endif
            layer = newGameObject.transform;
            layer.SetParent(BrushUtility.GetRootGrid(false).transform);
        }
        return layer;
    }

    public Vector3Int WorldToLocal(Grid grid, Vector3Int worldPosition)
    {
        return activeObject is Component ? worldPosition - grid.WorldToCell((activeObject as Component).transform.position) : default(Vector3Int);
    }

}
