using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TintTextureGenerator))]
public class TintTextureGeneratorEditor : Editor 
{
	private TintTextureGenerator generator { get { return target as TintTextureGenerator; } }
	public override void OnInspectorGUI()
	{
		EditorGUI.BeginChangeCheck();
		base.OnInspectorGUI();
		if (GUILayout.Button("Refresh") || EditorGUI.EndChangeCheck())
		{
			LevelBrush.ResetLevelCache();
			TintTextureGenerator.RefreshTintmap();
		}
	}
}
