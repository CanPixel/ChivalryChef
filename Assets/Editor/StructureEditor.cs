using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Structure), true)]
public class StructureEditor : Editor {
	Editor settingsEditor;	

	public override void OnInspectorGUI() {
		var tar = target as Structure;
		
		DrawDefaultInspector();

		EditorGUILayout.Space();
		
		if(GUILayout.Button("Generate")) tar.Generate();
	}
}