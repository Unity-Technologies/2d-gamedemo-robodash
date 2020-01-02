using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
public class BrushEditorUtility 
{
    const string k_CameraName = "Cameras";
    private static Material s_GizmoMaterial;

    private static void InitializeMaterial(Color color)
    {
        if (s_GizmoMaterial == null)
            s_GizmoMaterial = new Material(Shader.Find("Unlit/GizmoShader"));

        s_GizmoMaterial.color = color;
        s_GizmoMaterial.SetPass(0);
    }

    public static void DrawQuad(GridLayout grid, Vector3Int position, Color color)
    {
        BeginQuads(color);
        DrawQuadBatched(grid, position);
        EndQuads();
    }

    public static void DrawMarquee(GridLayout grid, Vector3Int position, Color color)
    {
        BeginMarquee(color);
        DrawMarqueeBatched(grid, position);
        EndMarquee();
    }

    public static void DrawLine(GridLayout grid, Vector3Int from, Vector3Int to, Color color)
    {
        BeginLines(color);
        DrawLineBatched(grid, from, to);
        EndLines();
    }
    public static void BeginLines(Color color)
    {
        InitializeMaterial(color);
        GL.PushMatrix();
        GL.Begin(GL.LINES);
    }

    public static void BeginMarquee(Color color)
    {
        BeginLines(color);
    }

    public static void BeginQuads(Color color)
    {
        InitializeMaterial(color);
        GL.PushMatrix();
        GL.Begin(GL.QUADS);
    }

    public static void EndQuads()
    {
        GL.End();
        GL.PopMatrix();        
    }

    public static void EndLines()
    {
        GL.End();
        GL.PopMatrix();
    }

    public static void EndMarquee()
    {
        EndLines();
    }


    public static void DrawLineBatched(GridLayout grid, Vector3Int from, Vector3Int to)
    {
        GL.Vertex(grid.GetComponent<Grid>().GetCellCenterWorld(from));
        GL.Vertex(grid.GetComponent<Grid>().GetCellCenterWorld(to));
    }

    public static void DrawSplitLineBatched(GridLayout grid, Vector3Int from, Vector3Int direction)
    {
        if (direction == Vector3Int.up)
        {
            GL.Vertex(grid.CellToWorld(from + new Vector3Int(0, 1, 0)));
            GL.Vertex(grid.CellToWorld(from + new Vector3Int(1, 1, 0)));
        }
        else if (direction == Vector3Int.right)
        {
            GL.Vertex(grid.CellToWorld(from + new Vector3Int(1, 1, 0)));
            GL.Vertex(grid.CellToWorld(from + new Vector3Int(1, 0, 0)));
        }
        else if (direction == Vector3Int.down)
        {
            GL.Vertex(grid.CellToWorld(from + new Vector3Int(0, 0, 0)));
            GL.Vertex(grid.CellToWorld(from + new Vector3Int(1, 0, 0)));
        }
        else if (direction == Vector3Int.left)
        {
            GL.Vertex(grid.CellToWorld(from + new Vector3Int(0, 1, 0)));
            GL.Vertex(grid.CellToWorld(from + new Vector3Int(0, 0, 0)));
        }
    }

    public static void DrawMarqueeBatched(GridLayout grid, Vector3Int position)
    {
        GL.Vertex(grid.CellToWorld(position));
        GL.Vertex(grid.CellToWorld(position + Vector3Int.up));

        GL.Vertex(grid.CellToWorld(position + Vector3Int.up));
        GL.Vertex(grid.CellToWorld(position + Vector3Int.up + Vector3Int.right));

        GL.Vertex(grid.CellToWorld(position + Vector3Int.up + Vector3Int.right));
        GL.Vertex(grid.CellToWorld(position + Vector3Int.right));

        GL.Vertex(grid.CellToWorld(position + Vector3Int.right));
        GL.Vertex(grid.CellToWorld(position));
    }

    public static void DrawQuadBatched(GridLayout grid, Vector3Int position)
    {
        GL.Vertex(grid.CellToWorld(position));
        GL.Vertex(grid.CellToWorld(position + Vector3Int.up));
        GL.Vertex(grid.CellToWorld(position + Vector3Int.up + Vector3Int.right));
        GL.Vertex(grid.CellToWorld(position + Vector3Int.right));
    }

    public static void UnpreparedSceneInspector()
    {
        GUILayout.Space(5f);
        GUILayout.Label("This scene is not yet ready for level editing.");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Initialize Scene"))
        {
            PrepareScene();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    public static bool SceneIsPrepared()
    {
        bool prepared = false;
        prepared = GameObject.Find(k_CameraName) != null;
        prepared &= BrushUtility.GetRootGrid(false);
        return prepared;
    }
    
    public static void PrepareScene()
    {
        GameObject rig = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cameras.prefab");
        GameObject walls = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Walls.prefab");
        GameObject floor = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Floor.prefab");
        GameObject hero = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Hero.prefab");
        GameObject gameState = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Game state manager.prefab");
        GameObject canvas = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Canvas stack.prefab");
        GameObject start = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Level Start.prefab");
        GameObject goal = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Level Goal.prefab");
        GameObject swarmSpawner = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Swarm Spawner.prefab");
        GameObject tintTextureGenerator = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Tint Texture Generator.prefab");

        if (rig != null && walls != null && floor != null && hero != null && gameState != null && canvas != null && start != null && goal != null && swarmSpawner != null)
        {
            RenderSettings.ambientLight = Color.white;
            foreach (var cam in Object.FindObjectsOfType<Camera>())
            {
                Object.DestroyImmediate(cam.gameObject, false);
            }
            Grid grid = BrushUtility.GetRootGrid(true);

            PrefabUtility.InstantiatePrefab(rig);
            GameObject wallsGo = PrefabUtility.InstantiatePrefab(walls) as GameObject;
            PrefabUtility.UnpackPrefabInstance(wallsGo, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            wallsGo.transform.SetParent(grid.transform);
            GameObject floorGo = PrefabUtility.InstantiatePrefab(floor) as GameObject;
            PrefabUtility.UnpackPrefabInstance(floorGo, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            floorGo.transform.SetParent(grid.transform);
            PrefabUtility.InstantiatePrefab(gameState);
            PrefabUtility.InstantiatePrefab(swarmSpawner);
            GameObject canvasObject = PrefabUtility.InstantiatePrefab(canvas) as GameObject;
            canvasObject.GetComponentInChildren<UIRenderTextureCamera>().UpdateCamera();

            GameObject startGo = PrefabUtility.InstantiatePrefab(start) as GameObject;
            startGo.transform.SetParent(grid.transform);
            startGo.transform.position = grid.GetCellCenterWorld(new Vector3Int(-6, 1, 0));

            GameObject goalGo = PrefabUtility.InstantiatePrefab(goal) as GameObject;
            goalGo.transform.SetParent(grid.transform);
            goalGo.transform.position = grid.GetCellCenterWorld(new Vector3Int(5, -2, 0));

            GameObject swarmSpawnerGo = PrefabUtility.InstantiatePrefab(swarmSpawner) as GameObject;
            swarmSpawnerGo.transform.SetParent(grid.transform);

            grid.gameObject.AddComponent<GridInformation>();
    
            GameObject tintTextureGo = PrefabUtility.InstantiatePrefab(tintTextureGenerator) as GameObject;
            tintTextureGo.transform.SetParent(grid.transform);

            LevelBrush.ResetLevelCache();
            TintTextureGenerator.RefreshTintmap();
        }
        else
        {
            Debug.LogWarning("Some prefabs for initializing the scene are missing.");
        }
    }

    public static void AutoSelectGrid()
    {
        if (Selection.activeGameObject == null || Selection.activeGameObject.GetComponentInParent<Grid>() == null)
        {
            Grid grid = BrushUtility.GetRootGrid(false);
            if(grid)
                Selection.activeTransform = grid.transform;
        }
    }

    public static void AutoSelectLayer(string name)
    {
        Transform transform = Selection.activeTransform;
        if (transform != null)
        {
            while (transform.parent != null)
            {
                if (transform.name == name)
                {
                    return;
                }
                transform = transform.parent;
            }
        }

        AutoSelectGrid();
    }
}
#endif
