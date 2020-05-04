using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MimicParentProperties : MonoBehaviour {
	public bool mimicPosition, mimicRotation, mimicScale, mimicUIColor, mimicAlpha;

	private MaskableGraphic parentHost;
	private MaskableGraphic current;

	public float offset = 0;

	void Start () {
		parentHost = transform.parent.GetComponent<Image>();
		current = GetComponent<Image>();
		if(current == null) current = GetComponent<RawImage>();
		if(parentHost == null) parentHost = transform.parent.GetComponent<RawImage>();
	}
	
	void Update () {
		if(mimicPosition) transform.position = parentHost.transform.position;
		if(mimicRotation) transform.rotation = parentHost.transform.rotation;
		if(mimicScale) transform.localScale = parentHost.transform.localScale;
		if(mimicUIColor) current.color = parentHost.color;
		if(mimicAlpha) current.color = new Color(current.color.r, current.color.g, current.color.b, parentHost.color.a + offset);
	}
}
