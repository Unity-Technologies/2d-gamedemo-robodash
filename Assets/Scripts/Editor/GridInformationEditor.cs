using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor
{
    [CustomEditor(typeof(GridInformation))]
    internal class GridInformationEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Do nothing for inspector
        }
    }
}