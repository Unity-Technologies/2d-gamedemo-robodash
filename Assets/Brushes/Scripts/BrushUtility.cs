#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.Tilemaps;

public class BrushUtility 
{
    const string k_GridName = "Grid";
    
    public static Grid GetRootGrid(bool autoCreate)
    {
        Grid result = null;

        if (GetSelection() != null)
            result = GetSelection().GetComponentInParent<Grid>();

        if (result == null)
        {
            GameObject gridGameObject = GameObject.Find(k_GridName);
            if (gridGameObject != null && gridGameObject.GetComponent<Grid>() != null)
            {
                result = gridGameObject.GetComponent<Grid>();
            }
            else if (autoCreate)
            {
                gridGameObject = new GameObject(k_GridName);
                result = gridGameObject.AddComponent<Grid>();
            }
        }

        return result;
    }

    public static GridInformation GetRootGridInformation(bool autoCreate)
    {
        Grid grid = GetRootGrid(autoCreate);
        GridInformation info = grid.GetComponent<GridInformation>();
        if (info == null)
            info = grid.gameObject.AddComponent<GridInformation>();
        return info;
    }

    public static void Select(GameObject go)
    {
#if UNITY_EDITOR
        Selection.activeGameObject = go;
#endif
    }

    public static GameObject GetSelection()
    {
#if UNITY_EDITOR
        return Selection.activeGameObject;
#else
        return null;
#endif
    }

    public static void SetDirty(Object obj)
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(obj);
#endif
    }

    public static GameObject Instantiate(GameObject prefab, Vector3 position, Transform parent)
    {
#if UNITY_EDITOR
        GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        go.transform.position = position;
        go.transform.SetParent(parent);
        Undo.RegisterCreatedObjectUndo(go, "Create");
        return go;
#else
        return Instantiate(prefab, position, parent);
#endif
    }

    public static void Destroy(GameObject gameObject)
    {
#if UNITY_EDITOR
        Undo.DestroyObjectImmediate(gameObject);
#else
        Destroy(gameObject);
#endif
    }

    public static Tilemap AddTilemap(Transform layer)
    {
        Tilemap result = null;
        #if UNITY_EDITOR
                result = Undo.AddComponent<Tilemap>(layer.gameObject);
                TilemapRenderer r = Undo.AddComponent<TilemapRenderer>(layer.gameObject);
                r.sortingOrder = 0;
                r.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/tilemap lit.mat");
        #else
                result = layer.gameObject.AddComponent<Tilemap>();
                layer.gameObject.AddComponent<TilemapRenderer>();
        #endif
        return result;
    }

    public static GameObject GetPrefabRoot(Transform t)
    {
        #if UNITY_EDITOR
            return PrefabUtility.GetOutermostPrefabInstanceRoot(t.gameObject);
        #else
            return t.gameObject;
        #endif
    }
}

