using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPusher : MonoBehaviour {
	[HideInInspector]
	public Vector3 push;
	public GameObject CameraPoint;

	private Vector3 pushTarget;

	private float bumpDelay = 0;

	void Update() {
		if(bumpDelay > 0) bumpDelay -= Time.deltaTime;
		else pushTarget = Vector3.zero;

		var colliders = Physics.OverlapSphere(transform.position, 1.4f);
		foreach(var i in colliders) {
			if(i.gameObject.tag == "World") {
				var distance = Vector3.Distance(i.transform.position, transform.position);
				pushTarget = new Vector3(0, distance / 2, 0);
				break;
			}
		}

		push = Vector3.Lerp(push, pushTarget, Time.deltaTime * 4f);
	}
	
	void OnCollisionEnter(Collision col) {
		if(col.gameObject.tag == "World") pushTarget += transform.forward;
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(transform.position, 1);
	}
}
