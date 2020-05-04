using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraIgnoreFog : MonoBehaviour {
	private void OnPreRender() {
		RenderSettings.fog = false;
	}

	private void OnPostRender() {
		RenderSettings.fog = true;
	}
}
