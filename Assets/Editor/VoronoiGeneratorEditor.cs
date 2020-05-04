using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VoronoiGenerator))]
public class VoronoiGeneratorEditor : Editor {
	Editor settingsEditor;

	public override void OnInspectorGUI() {
		var tar = target as VoronoiGenerator;

		DrawDefaultInspector();

		if(tar.setting == null) return;

		if(GUILayout.Button("Generate")) {
			if(tar.randomWorld) tar.GenerateRandom();
			else tar.StartGenerate();
		}
		if(GUILayout.Button("Populate")) tar.PopulateAll();
		if(GUILayout.Button("Randomize & Populate")) tar.GenerateAndPopulate();
		if(GUILayout.Button("Export World")) tar.Export();

		EditorGUILayout.Space();

		DrawSettingsEditor(tar.setting, tar.StartGenerate, ref tar.worldSettingFoldout, ref settingsEditor, tar.setting.autoUpdate);
		if(GUILayout.Button("Generate")) tar.StartGenerate();
	}

	void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor, bool autoUpdate) {
		if(settings != null) {
			foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
			using (var check = new EditorGUI.ChangeCheckScope()) {
				if(foldout) {
					CreateCachedEditor(settings, null, ref editor);
					editor.OnInspectorGUI();
					if(check.changed && autoUpdate) {
						if(onSettingsUpdated != null) onSettingsUpdated.Invoke();
					}
				}
			}
		}
	}
}
