using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CurvedText : Text {
	public float radius = 0.5f;
	public float wrapAngle = 360.0f;
	public float scaleFactor = 100.0f;

	private float circumference {
		get {
			if(_radius != radius || _scaleFactor != scaleFactor) {
				_circumference = 2.0f * Mathf.PI * radius * scaleFactor;
				_radius = radius;
				_scaleFactor = scaleFactor;
			}
			return _circumference;
		}
	}
	private float _radius = -1;
	private float _scaleFactor = -1, _circumference = 1;

	#if UNITY_EDITOR
	protected override void OnValidate() {
		base.OnValidate();
		if(radius <= 0.0f) radius = 0.001f;
		if(scaleFactor <= 0.0f) scaleFactor = 0.001f;
	}
	#endif

	protected override void OnPopulateMesh(VertexHelper vh) {
		base.OnPopulateMesh(vh);
		List<UIVertex> stream = new List<UIVertex>();
		vh.GetUIVertexStream(stream);

		for(int i = 0; i < stream.Count; i++) {
			UIVertex v = stream[i];
			float percentCircumference = v.position.x / circumference;
			Vector3 offset = Quaternion.Euler(0, 0, -percentCircumference * 360f) * Vector3.up;
			v.position = offset * radius * scaleFactor + offset * v.position.y;
			v.position += Vector3.down * radius * scaleFactor;
			stream[i] = v;
		}

		vh.AddUIVertexTriangleStream(stream);
	}

	private static void DrawMultiLine() {

	}

	void Update () {
		if(radius <= 0.0f) radius = 0.001f;
		if(scaleFactor <= 0.0f) scaleFactor = 0.001f;
		rectTransform.sizeDelta = new Vector2(circumference * wrapAngle / 360f, rectTransform.sizeDelta.y);
	}
}
