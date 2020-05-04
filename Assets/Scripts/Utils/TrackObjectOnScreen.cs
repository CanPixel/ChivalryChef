using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackObjectOnScreen : MonoBehaviour {
	private GameObject trackingObj;

	public void Init (GameObject trackObj) {
		trackingObj = trackObj;
		var target = Camera.main.WorldToViewportPoint(trackingObj.transform.position);
		transform.localPosition = new Vector3(target.x, target.y, target.z);
	}
	
	void Update () {
		var target = Camera.main.WorldToViewportPoint(trackingObj.transform.position);
		transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(target.x, target.y, target.z), Time.deltaTime);
	}
}
