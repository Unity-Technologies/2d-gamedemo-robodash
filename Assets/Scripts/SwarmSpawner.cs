using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SwarmSpawner : MonoBehaviour {

    public List<GameObject> swarmPrefabs; //in order of size
    public void Restart()
    {
        GridInformation gi = FindObjectOfType<GridInformation>();
        Grid g = BrushUtility.GetRootGrid(false);

        if (g == null || gi == null) return;

        foreach (var pos in gi.GetAllPositions(SwarmBrush.k_SwarmDifficultyProperty))
        {
            float difficulty = gi.GetPositionProperty(pos, SwarmBrush.k_SwarmDifficultyProperty, 0f);

            if (Random.Range(0f, 1f) < difficulty)
            {
                GameObject prefab = swarmPrefabs[Random.Range(0, swarmPrefabs.Count)];
                Vector3 gCenter = g.GetCellCenterWorld(pos);
                gCenter.z = 0f;
                Instantiate(prefab, gCenter, Quaternion.identity);
            }
        }
    }
}
