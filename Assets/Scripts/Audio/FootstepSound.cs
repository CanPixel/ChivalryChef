using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSound : MonoBehaviour {
	public string ENTITY = "Player";

	private float delay = 0, interval = 0;

	//Footstep material identification now works by world texture (Not the best method!!)
	//Maybe need World / BiomeInfo script per (non)-exported world, that holds heightmap + color information. (This will make biomes easier, and footsteps might work better through the heightMap)
	private Texture worldTexture;
	public bool left = true;

	void Start() {
		//worldTexture = GameObject.FindGameObjectWithTag("World").GetComponent<MeshRenderer>().sharedMaterial.mainTexture;
	}

	void Update() {
		if(delay > 0) delay -= Time.deltaTime; interval += Time.deltaTime;
	}

	void OnTriggerEnter(Collider col) {
		if(col.tag == "World" && delay <= 0) {
			string footStepMaterial = GetFootstepMaterial(col);
			SoundManager.PLAY_SOUND(ENTITY + footStepMaterial, transform.position, 0.025f + interval / 10f - (left ? 1f : 0.2f), Random.Range(0.6f, 1.6f));
			delay = 0.1f;
		}
	}

	private string GetFootstepMaterial(Collider col) {
	RaycastHit hit;
	if(Physics.Raycast(transform.position, transform.forward, out hit)) {
		//Debug.Log(hit.point);
	}
		return "Grass";
	}
}
