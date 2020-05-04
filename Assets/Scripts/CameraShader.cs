using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649

[ExecuteInEditMode]
public class CameraShader : MonoBehaviour {
	[SerializeField]
	private Material material;

	public Vector3 clampRotation = new Vector3(0, 0, 0);

	void Update() {
		if(clampRotation != Vector3.zero) transform.rotation = Quaternion.Euler(clampRotation);
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest) {
		if(material != null) Graphics.Blit(src, dest, material);
	}

	public void SetFloat(string name, float val) {
		material.SetFloat(name, val);
	}

	public float GetFloat(string name) {
		return material.GetFloat(name);
	}
}
