using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RecipeSystem))]
public class RecipeSystemEditor : Editor {
	public override void OnInspectorGUI() {
		var tar = target as RecipeSystem;
		if(GUILayout.Button("Fetch Recipes")) tar.Load();
		EditorGUILayout.Space();
		DrawDefaultInspector();
	}
}
