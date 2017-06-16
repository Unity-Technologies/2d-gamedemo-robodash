using UnityEngine;
using UnityEngine.Tilemaps;

[CustomGridBrush(true, false, false, "Tint")]
public class TintBrush : GridBrushBase
{
	public Color m_Color = Color.black;
	public const string k_TintColorPropertyName = "TintColor";
    public float m_Blend = 1f;

    TintTextureGenerator generator;

	public override void Paint(GridLayout grid, GameObject layer, Vector3Int position)
	{
        GridInformation info = BrushUtility.GetRootGridInformation(true);
		Color oldColor = info.GetPositionProperty(position, k_TintColorPropertyName, FindObjectOfType<TintTextureGenerator>().defaultColor);
        info.ErasePositionProperty(position, k_TintColorPropertyName);
        Color newColor = oldColor * (1 - m_Blend) + m_Color * m_Blend;
		info.SetPositionProperty(position, k_TintColorPropertyName, newColor);
        TintTextureGenerator.RefreshTintmap(position);
	}

	public override void Erase(GridLayout grid, GameObject layer, Vector3Int position)
	{
		GridInformation info = BrushUtility.GetRootGridInformation(true);
		info.ErasePositionProperty(position, k_TintColorPropertyName);
        if (generator == null) generator = FindObjectOfType<TintTextureGenerator>();
		TintTextureGenerator.RefreshTintmap(position);
    }

	public override void Pick(GridLayout grid, GameObject layer, BoundsInt position, Vector3Int pivot)
	{
		GridInformation info = BrushUtility.GetRootGridInformation(true);
		m_Color = info.GetPositionProperty(position.min, k_TintColorPropertyName, FindObjectOfType<TintTextureGenerator>().defaultColor);
	}
}
