using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshBuilder))]
public class MeshBuilderEditor : Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		MeshBuilder mb = (MeshBuilder)this.target;
		if (GUILayout.Button("Rebuild GameGrid")) {
			mb.BuildMesh();
		}

	}
}
