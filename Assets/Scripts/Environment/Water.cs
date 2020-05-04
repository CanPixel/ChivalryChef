using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ExecuteInEditMode]
public class Water : MonoBehaviour {
	public int dimension = 10;
	public Octave[] octaves;
	public float UVScale;

	protected MeshFilter filter;
	protected Mesh mesh;

	private float soundDelay;

	void Start () {
		mesh = new Mesh();
		mesh.name = gameObject.name;
		mesh.vertices = GenerateVerts();
		mesh.triangles = GenerateTris();
		mesh.uv = GenerateUV();
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();

		filter = gameObject.GetComponent<MeshFilter>();
		filter.mesh = mesh;
	}

	private Vector3[] GenerateVerts() {
		var verts = new Vector3[(dimension + 1) * (dimension + 1)];
		for(int x = 0; x < dimension; x++) for(int z = 0; z < dimension; z++) verts[GetIndex(x, z)] = new Vector3(x, 0, z);
		return verts;
	}

	private Vector2[] GenerateUV() {
		var uvs = new Vector2[mesh.vertices.Length];
		for(int x = 0; x <= dimension; x++)
		for(int z = 0; z <= dimension; z++) {
			var vec = new Vector2((x / UVScale) % 2, (z / UVScale) % 2);
			uvs[GetIndex(x, z)] = new Vector2(vec.x <= 1 ? vec.x : 2 - vec.x, vec.y <= 1 ? vec.y : 2 - vec.y);
		}
		return uvs;
	}

	private int[] GenerateTris() {
		var tris = new int[mesh.vertices.Length * 6];

		for(int x = 0; x < dimension; x++)
		for(int z = 0; z < dimension; z++) {
			tris[GetIndex(x, z) * 6 + 0] = GetIndex(x, z);
			tris[GetIndex(x, z) * 6 + 1] = GetIndex(x + 1, z + 1);
			tris[GetIndex(x, z) * 6 + 2] = GetIndex(x + 1, z);
			tris[GetIndex(x, z) * 6 + 3] = GetIndex(x, z);
			tris[GetIndex(x, z) * 6 + 4] = GetIndex(x, z + 1);
			tris[GetIndex(x, z) * 6 + 5] = GetIndex(x + 1, z + 1);
		}
		return tris;
	}

	private int GetIndex(float x, float z) {
		return (int)(x * (dimension + 1) + z);
	}
	
	void Update () { // [OPTIMIZE NEEDED]
		if(soundDelay > 0) soundDelay -= Time.deltaTime;
		var verts = mesh.vertices;
		for(int x = 0; x <= dimension; x++)
		for(int z = 0; z <= dimension; z++) {
			var y = 0f;
			for(int o = 0; o < octaves.Length; o++) {
				if(octaves[o].alternate) {
					var perl = Mathf.PerlinNoise((x * octaves[o].scale.x) / dimension, (z * octaves[o].scale.y) / dimension) * Mathf.PI * 2f;
					y += Mathf.Cos(perl + octaves[o].speed.magnitude * Time.time) * octaves[o].height;
				} else {
					var perl = Mathf.PerlinNoise((x * octaves[o].scale.x + Time.time * octaves[o].speed.x) / dimension, (z * octaves[o].scale.y + Time.time * octaves[o].speed.y) / dimension) - 0.5f;
					y += perl * octaves[o].height;
				}
			}
			
			verts[GetIndex(x, z)] = new Vector3(x, y, z);
		}
		mesh.vertices = verts;
		mesh.RecalculateNormals();
	}

	public float GetHeight(Vector3 position) {
		//local space scale factor and position
		var scale = new Vector3(1 / transform.lossyScale.x, 0, 1 / transform.lossyScale.z);
		var localPos = Vector3.Scale((position - transform.position), scale);

		//edge points
		var p1 = new Vector3(Mathf.Floor(localPos.x), 0, Mathf.Floor(localPos.z));
		var p2 = new Vector3(Mathf.Floor(localPos.x), 0, Mathf.Ceil(localPos.z));
		var p3 = new Vector3(Mathf.Ceil(localPos.x), 0, Mathf.Floor(localPos.z));
		var p4 = new Vector3(Mathf.Ceil(localPos.x), 0, Mathf.Ceil(localPos.z));

		//clamping
		p1.x = Mathf.Clamp(p1.x, 0, dimension);
		p1.z = Mathf.Clamp(p1.z, 0, dimension);
		p2.x = Mathf.Clamp(p2.x, 0, dimension);
		p2.z = Mathf.Clamp(p2.z, 0, dimension);
		p3.x = Mathf.Clamp(p3.x, 0, dimension);
		p3.z = Mathf.Clamp(p3.z, 0, dimension);
		p4.x = Mathf.Clamp(p4.x, 0, dimension);
		p4.z = Mathf.Clamp(p4.z, 0, dimension);

		//get max distance to one of the edges and take that to compute max - dist
		var max = Mathf.Max(Vector3.Distance(p1, localPos), Vector3.Distance(p2, localPos), Vector3.Distance(p3, localPos), Vector3.Distance(p4, localPos) + Mathf.Epsilon);
		var dist = (max - Vector3.Distance(p1, localPos))
						+ (max - Vector3.Distance(p2, localPos))
						+ (max - Vector3.Distance(p3, localPos))
						+ (max - Vector3.Distance(p4, localPos) + Mathf.Epsilon);
		//weighted sum
		var height = mesh.vertices[GetIndex(p1.x, p1.z)].y * (max - Vector3.Distance(p1, localPos))
							  + mesh.vertices[GetIndex(p2.x, p2.z)].y * (max - Vector3.Distance(p2, localPos))
							  + mesh.vertices[GetIndex(p3.x, p3.z)].y * (max - Vector3.Distance(p3, localPos))
							  + mesh.vertices[GetIndex(p4.x, p4.z)].y * (max - Vector3.Distance(p4, localPos));
		//scale
		return height * transform.lossyScale.y / dist;
	}

	[Serializable]
	public struct Octave {
		public Vector2 speed;
		public Vector2 scale;
		public float height;
		public bool alternate;
	}

	void OnTriggerStay(Collider col) {
		if(col.tag == "Knight") {
			var knight = col.gameObject.GetComponent<KnightMovement>();
			if(knight == null) return;
			knight.ToggleBoilParticles(true);
			knight.Hurt(5, gameObject, false);
			if(soundDelay <= 0) {
				SoundManager.PLAY_UNIQUE_SOUND_AT("Sizzle", col.transform.position, 1f, 0.1f, 0, 0.8f);
				soundDelay = 0.5f + UnityEngine.Random.Range(0f, 0.2f);
			}
		}
	}

	void OnTriggerExit(Collider col) {
		if(col.tag == "Knight") {
			var knight = col.gameObject.GetComponent<KnightMovement>();
			if(knight != null) knight.ToggleBoilParticles(false);
		}
	}
}
