using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ImageFX : MonoBehaviour {
	public Material FX;

	void OnRenderImage(RenderTexture src, RenderTexture dst) {
		if(FX != null) Graphics.Blit(src, dst, FX);
	}
}
