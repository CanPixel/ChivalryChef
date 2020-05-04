using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActionAfter : MonoBehaviour {
	public float duration = 2;

	private float tick;
	public UnityEvent action;

	void Update () {
		tick += Time.deltaTime;

		if(tick > duration) {
			if(tick > duration * 1.7f) {
				action.Invoke();
				Destroy(this);
			}
		}
	}
}
