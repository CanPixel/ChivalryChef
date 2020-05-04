using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollButton : MonoBehaviour {
	private Button button;
	public RawImage left, right;

	private float center = 0;
	private Vector2 scrollTargetScale;
	private Vector2 leftTargetPos, rightTargetPos;

	void Awake () {
		Set();
	}

	public void Set() {
		button = GetComponentInChildren<Button>();
		scrollTargetScale = button.transform.localScale;

		leftTargetPos = left.transform.localPosition;
		rightTargetPos = right.transform.localPosition;

		button.transform.localScale = new Vector2(0, button.transform.localScale.y);

		left.transform.localPosition = new Vector2(center, left.transform.localPosition.y);
		right.transform.localPosition = new Vector2(center, right.transform.localPosition.y);
	}
	
	void Update () {
		float speed = Time.deltaTime * 4f;

		button.transform.localScale = Vector2.Lerp(button.transform.localScale, scrollTargetScale, speed);
		left.transform.localPosition = Vector2.Lerp(left.transform.localPosition, leftTargetPos, speed);
		right.transform.localPosition = Vector2.Lerp(right.transform.localPosition, rightTargetPos, speed);
	}
}
