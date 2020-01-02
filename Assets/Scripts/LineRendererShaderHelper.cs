using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class LineRendererShaderHelper : MonoBehaviour {

    public int orderInLayer;

    LineRenderer lr;
    MaterialPropertyBlock block;
    void Update() {
        if (lr == null) lr = GetComponent<LineRenderer>();
        if (block == null) { block = new MaterialPropertyBlock(); lr.GetPropertyBlock(block); }

        block.SetFloat("_LineLength", GetLineRendererLength(lr));
        lr.SetPropertyBlock(block);
        lr.sortingOrder = orderInLayer;
    }

    static float GetLineRendererLength(LineRenderer lineRenderer) {
        Vector3[] positions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(positions);
        float length = 0f;
        for (int i = 0; i < positions.Length - 1; i++) {
            length += Vector3.Distance(positions[i + 1], positions[i]);
        }

        return length;
    }
}
