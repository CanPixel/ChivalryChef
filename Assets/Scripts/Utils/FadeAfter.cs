using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeAfter : MonoBehaviour {
	public float duration = 2;

	private float tick;

	private Text textComponent;

	public delegate void PostEvent();
	public PostEvent postEvent;

	void Start () {
		textComponent = transform.GetComponent<Text>();
	}
	
	void Update () {
		tick += Time.deltaTime;

		if(tick > duration) {
			var dest = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, 0);
			textComponent.color = Color.Lerp(textComponent.color, dest, Time.deltaTime * 2);
			if(tick > duration * 1.7f) {
				if(postEvent != null) postEvent.Invoke();
				Destroy(gameObject);
			}
		}
	}
}
