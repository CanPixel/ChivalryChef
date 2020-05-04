using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraDeathSequence : MonoBehaviour {
	private Transform target;
	[HideInInspector]
	public Text deathText;

	private bool startSequence = false;
	private float sequenceTime = 0;

	private Vector3 startPos;

	public float speed = 2f;
	
	private float zoomOut = 0;

	private CameraShader deathShader;
	private const float fxCap = 0.8f;

	void Update () {
		if(startSequence) {
			sequenceTime += Time.deltaTime;
			
			var fx = deathShader.GetFloat("_Blend");
			if(fx >= fxCap) deathShader.SetFloat("_Blend", Mathf.Lerp(fx, Mathf.Sin(Time.time * 5f) * fxCap * 2f, Time.deltaTime * 0.2f));
			deathShader.SetFloat("_DitherFade", Mathf.Lerp(deathShader.GetFloat("_DitherFade"), 1f, Time.deltaTime * 0.1f));

			if(sequenceTime < Mathf.PI / speed) {
				transform.position = Vector3.Lerp(transform.position, startPos + new Vector3(0, Mathf.Sin(sequenceTime * speed) * 10f, 0), Time.deltaTime * 3f);
			} else {
				if(zoomOut < 4.0f) zoomOut += Time.deltaTime * 0.001f;

				var offset = new Vector3(target.position.x, target.position.y + 8, target.position.z + zoomOut);
				offset = Quaternion.AngleAxis((sequenceTime) * 5f * speed, Vector2.up) * offset;
				transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.deltaTime);
				transform.LookAt(target.transform);
			}
		}
	}

	public void StartSequence(GameObject tracker, GameObject deathSource, Player player) {
		deathText = player.deathText;
		deathShader = GetComponent<CameraShader>();
		startPos = transform.position;
		target = tracker.transform;
		startSequence = true;
		deathShader.enabled = true;
		deathShader.SetFloat("_Blend", 1);
		deathShader.SetFloat("_DitherFade", 1);
		if(deathSource != null && deathSource.tag == "Water") deathText.text = "Boiled!";
		else deathText.text = "Slashed!";
		deathText.gameObject.SetActive(true);
	}
}
