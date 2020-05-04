using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapIcon : MonoBehaviour {
	private Vector3 previousPos;

	private float direction = 0;

	private Transform minimapCam;

	void Start() {
		if(GameObject.FindGameObjectWithTag("MinimapCamera") != null) minimapCam = GameObject.FindGameObjectWithTag("MinimapCamera").transform;
		if(minimapCam == null) return;
		transform.rotation = Quaternion.Euler(minimapCam.eulerAngles);
		direction = 0;
	}

	void Update () {
		if(minimapCam == null) return;
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(minimapCam.eulerAngles) * Quaternion.AngleAxis(direction * 180f, new Vector3(0, -1, 0)), Time.deltaTime * 4f);

		if(transform.position.z < previousPos.z) direction = -1;
		else direction = 0;

		previousPos = transform.position;
	}
}
