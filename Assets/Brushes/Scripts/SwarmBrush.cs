using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
[CustomGridBrush(false, true, false, "Swarm")]
public class SwarmBrush : GridBrushBase
{
    public const string k_SwarmDifficultyProperty = "SwarmDifficulty";
    public float m_Difficulty;

    public override void Paint(GridLayout grid, GameObject layer, Vector3Int position)
    {
        GridInformation info = BrushUtility.GetRootGridInformation(true);
        info.ErasePositionProperty(position, k_SwarmDifficultyProperty);
        info.SetPositionProperty(position, k_SwarmDifficultyProperty, m_Difficulty);
    }

    public override void Erase(GridLayout grid, GameObject layer, Vector3Int position)
    {
        GridInformation info = BrushUtility.GetRootGridInformation(true);
        info.ErasePositionProperty(position, k_SwarmDifficultyProperty);
    }

    public override void Pick(GridLayout grid, GameObject layer, BoundsInt position, Vector3Int pivot)
    {
        GridInformation info = BrushUtility.GetRootGridInformation(true);
        m_Difficulty = info.GetPositionProperty(position.min, k_SwarmDifficultyProperty, 0f);
    }
}
