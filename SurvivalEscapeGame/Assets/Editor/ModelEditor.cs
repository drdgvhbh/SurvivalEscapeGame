using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Model))]
public class ModelEditor : Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		Model script = (Model)this.target;
	}
}
