using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CurvedText))]
public class CurvedTextEditor : Editor {

	public override void OnInspectorGUI() {
		DrawDefaultInspector();
	}
}
