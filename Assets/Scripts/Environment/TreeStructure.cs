using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GK;

[ExecuteInEditMode]
public class TreeStructure : Structure {
	[SerializeField]
	public enum TreeType {
		APPLETREE, BROCCAULI
	}
	
	public TreeType treeType;

	[Header("Leaves")]
	[Range(0.2f, 20f)]
	public float scaleMin;
	[Range(0.2f, 20f)]
	public float scaleMax;

	private float scale;
	[Range(1, 12)]
	public int AmountOfLeaves = 3;
	[Range(4, 150)]
	public int detailMin;
	[Range(4, 150)]
	public int detailMax;
	private int pointCount;
	public Material leaves;
	[Range(0, 20)]
	public float leafSpread = 3;
	public float Yoffset;

	public Vector3 shapeElongation;
	private MaterialPropertyBlock propBlock;

	private float windOffset;

	[Header("Trunk")]
	[Range(2, 24)]
	public int RadialSegments = 8;
	[Range(1, 10)]
	public int HeightSegments = 2;
	
	[Range(0.1f, 2f)]
	public float trunkRadius = 0.5f;
	[Range(0.1f, 5f)]
	public float trunkHeight = 1.0f;
	private int numVertexColumns, numVertexRows;
	[Range(0, 1)]
	public float trunkCrookedness;

	private GameObject rootLeaves;
	private MeshRenderer render, leafRender;

	private bool generated = false;

	public override bool ApplyOffsetToGround() {
		RaycastHit hit;
		if(Physics.Raycast(transform.position, Vector3.down, out hit, 2000, collisionMask) && hit.transform.tag == "World") {
			if(hit.transform != null) {
				transform.position = new Vector3(transform.position.x, hit.point.y + (scale / 2f) - Random.Range(0f, 1f), transform.position.z);
				return true;
			}
		}
		return false;
	}

	void Start() {
		windOffset = Random.Range(-1f, 1f);
		render = GetComponent<MeshRenderer>();
		propBlock = new MaterialPropertyBlock();
		Generate();
		if(rootLeaves != null) leafRender = rootLeaves.GetComponent<MeshRenderer>();
	}

	void Update() {
		if(propBlock == null || SoundManager.GetWindFrequency() < 0) return;
		render.GetPropertyBlock(propBlock);
		
		propBlock.SetFloat("_tree_sway_speed", windOffset + SoundManager.GetWindFrequency() * 1.2f);
		propBlock.SetFloat("_tree_sway_stutter", windOffset + SoundManager.GetWindFrequency() / 2f);
		propBlock.SetVector("_wind_dir", new Vector4(windOffset + SoundManager.GetWindFrequency() * 0.075f, windOffset - (SoundManager.GetWindFrequency() * 0.5f), 0.5f, 0.2f));
		propBlock.SetFloat("_wind_size", SoundManager.GetWindFrequency() * 20f);
		
		render.SetPropertyBlock(propBlock);
		leafRender.SetPropertyBlock(propBlock);
	}

	public override void Generate() {
		if(generated) return;
		base.Generate();

		Util.ClearChildren(transform);

		//TODO: Swaying animation
		//TODO: GRASS

		//Trunk
		var top = GenerateTrunk();

		//Leaves
		rootLeaves = new GameObject("Leaves");
		rootLeaves.transform.SetParent(transform);
		rootLeaves.transform.localPosition = Vector3.zero;
		rootLeaves.transform.localScale = Vector3.one;
		var leafFilter = rootLeaves.AddComponent<MeshFilter>();
		var leafRender = rootLeaves.AddComponent<MeshRenderer>();
		var leafCollide = rootLeaves.AddComponent<MeshCollider>();

		int amount = AmountOfLeaves;
		if(treeType == TreeType.BROCCAULI) amount = AmountOfLeaves * 2;

		var leafOBJ = new GameObject[amount];
		var leafMaterials = new Material[amount];
		var combine = new CombineInstance[amount];
		var leafColor = Color.HSVToRGB(Random.Range(0f, 1f), Random.Range(0.15f, 0.5f), 1f);
		for(int i = 0; i < amount; i++) {
			Mesh leaves;
			leafOBJ[i] = GenerateLeaves(i, top, leafColor, out leaves, out leafMaterials[i]);
			combine[i].mesh = leaves;
			combine[i].transform = Matrix4x4.TRS(Vector3.up * HeightSegments * trunkHeight + new Vector3(0, Yoffset, 0), Quaternion.identity, Vector3.one);
		}

		var leafMesh = new Mesh();
		leafMesh.CombineMeshes(combine, false);

		#if UNITY_EDITOR
		MeshUtility.Optimize(leafMesh);
		#endif
		
		leafFilter.sharedMesh = leafMesh;
		leafCollide.sharedMesh = leafMesh;
		leafRender.sharedMaterials = leafMaterials;

		foreach(var i in leafOBJ) DestroyImmediate(i);

		gameObject.name = treeType.ToString().ToLower();

		FinaliseMesh();
		generated = true;
	}

	private GameObject GenerateLeaves(int index, Vector3 top, Color baseColor, out Mesh mesh, out Material mat) {
		scale = Random.Range(scaleMin, scaleMax);
		pointCount = Random.Range(detailMin, detailMax);

		var leafChild = new GameObject("Leaves");
		leafChild.transform.SetParent(transform);
		var leafFilter = leafChild.AddComponent<MeshFilter>();
		var leafRender = leafChild.AddComponent<MeshRenderer>();
		var leafMesh = new Mesh();

		switch(treeType) {
			case TreeType.APPLETREE: default: {
				if(index == 0) scale = scaleMax;
				Vector3[] points = new Vector3[pointCount];
				var spread = leafSpread * (scale / scaleMax) / 2;
				var randOffs = new Vector3(Random.Range(-1f, 1f) * spread, Random.Range(-1f, 1f) * spread, Random.Range(-1f, 1f) * spread) * (index == 0 ? 0 : 1);
				for(int i = 0; i < points.Length; i++) {
					points[i] = new Vector3(Random.Range(-(scale / 2) - shapeElongation.x, (scale / 2) + shapeElongation.x), Random.Range(-(scale / 2) - shapeElongation.y, (scale / 2) + shapeElongation.y), Random.Range(-(scale / 2) - shapeElongation.z, (scale / 2) + shapeElongation.z));
					points[i] += randOffs + top;
				}
				
				List<Vector3> verts = new List<Vector3>();
				List<Vector3> normals = new List<Vector3>();
				List<int> tris = new List<int>();
				new ConvexHullCalculator().GenerateHull(new List<Vector3>(points), flatShade, ref verts, ref tris, ref normals);
				leafMesh.vertices = verts.ToArray();
				leafMesh.triangles = tris.ToArray();
				leafMesh.normals = normals.ToArray();

				leafMesh.RecalculateBounds();
				leafMesh.RecalculateNormals();
				#if UNITY_EDITOR
				MeshUtility.Optimize(leafMesh);
				#endif

				leafRender.sharedMaterial = new Material(this.leaves);
				Color leafColor = baseColor * Random.Range(0.75f, 1.25f);
				leafRender.sharedMaterial.SetColor("_Color", leafColor);
				leafRender.sharedMaterial.SetColor("_RimColor", leafColor * 0.9f);
				leafChild.transform.localPosition = Vector3.up * HeightSegments * trunkHeight;
				leafChild.transform.localScale = Vector3.one;
				leafFilter.sharedMesh = leafMesh;
				mesh = leafMesh;
				mat = leafRender.sharedMaterial;
				break;}
			case TreeType.BROCCAULI: {
				if(index == 0) scale = scaleMax;
				float detail = ((float)(pointCount - 4) / detailMax) * 4f;
				var spread = leafSpread * (scale / 150f) / 2;
				//var size = scaleMax / scale;
				var point = new Vector3(0, Random.Range(-(scale / 2) - shapeElongation.y, (scale / 2) + shapeElongation.y), 0);
				point += top;
				point.y += (index / 2f); //* scaleMin;
				if(index == 0) point.x = point.z = 0;
				
				var height = (index * 2f) / (AmountOfLeaves * 2f);
				if(height > 1) {
					height -= 1;
					height = 1f - height;
				}
				var offset = new Vector3(Random.Range(0, 1f), 0, Random.Range(0, 1f)) * height * (spread * 100f) * (index > AmountOfLeaves ? -1f : 1f);
				point += offset;

				leafMesh = new PlanetSphere().Create(scale, (int)detail, point);
				leafMesh.RecalculateBounds();
				leafMesh.RecalculateNormals();

				#if UNITY_EDITOR
				MeshUtility.Optimize(leafMesh);
				#endif

				leafRender.sharedMaterial = new Material(this.leaves);
				var leafColor = baseColor * Random.Range(0.6f, 1.3f);
				leafRender.sharedMaterial.SetColor("_Color", leafColor);
				leafRender.sharedMaterial.SetColor("_RimColor", leafColor);
				leafChild.transform.localScale = Vector3.one * scale;
				leafFilter.sharedMesh = leafMesh;
				mesh = leafMesh;
				mat = leafRender.sharedMaterial;
				break; }
		}

		return leafChild;
	}

	private Vector3 GenerateTrunk() {
		mesh = new Mesh();

		int numColumns = RadialSegments + 1;
		int numRows = HeightSegments + 1;

		int numVerts = numColumns * numRows;
		//int numNormals = numVerts;
		int numUVs = numVerts;
		int numSideFaces = RadialSegments * HeightSegments * 2;
		int numCapFaces = RadialSegments - 2;

		Vector3[] verts = new Vector3[numVerts];
		var uvs = new Vector2[numUVs];
		var faces = new int[(numSideFaces + numCapFaces * 2) * 3];
		
		var step = Mathf.PI * 2 / RadialSegments;

		float lastExtrude = 0;
		var extrude = trunkRadius;
		var displace = new Vector3(0, 0, 0);
		var lastDisplace = Vector3.zero;
		for(int i = 0; i < numRows; i++) {
			if(i > 1) {
				extrude = Mathf.Clamp(lastExtrude + Random.Range(-1f, 1f), i / 3f, trunkRadius + 1);
				lastExtrude = extrude;

				float xOffs = Random.Range(-1f, 1f), zOffs = Random.Range(-1f, 1f);
				lastDisplace = displace;
				displace = lastDisplace + new Vector3(xOffs, xOffs * zOffs, zOffs);
			} else if(i == 0) extrude *= 2f;

			for(int j = 0; j < numColumns; j++) {
				float angle = j * step;
				if(j == numColumns - 1) angle = 0;

				verts[i * numColumns + j] = new Vector3(extrude * Mathf.Cos(angle) * (i == 0 ? 2 : 1), i * trunkHeight, extrude * Mathf.Sin(angle) * (i == 0 ? 2 : 1)) + displace * trunkCrookedness;
				uvs[i * numColumns + j] = new Vector2(j * 1 / trunkRadius, i * 1 / Vector3.up.y);

				if(i != 0 && j < numColumns - 1) {
					int index = numCapFaces * 3 + (i - 1) * RadialSegments * 6 + j * 6;

					faces[index] = i * numColumns + j;
					faces[index + 1] = i * numColumns + j + 1;
					faces[index + 2] = (i - 1) * numColumns + j;

					faces[index + 3] = (i - 1) * numColumns + j;
					faces[index + 4] = i * numColumns + j + 1;
					faces[index + 5] = (i - 1) * numColumns + j + 1;
				}
			}
		}

		int firstIndex = 0, midIndex = 0, lastIndex = 0;
		int topCapOffs = numVerts - numColumns;

		for(int i = 0; i < numCapFaces; i++) {
			int bottomIndex = i * 3;
			int topIndex = (numCapFaces + numSideFaces) * 3 + i * 3;

			if(i == 0) {
				firstIndex = 1;
				midIndex = 0;
				lastIndex = numColumns - 2;
			} else {
				midIndex = lastIndex;
				lastIndex = lastIndex - 1;
			}

			faces[bottomIndex] = lastIndex;
			faces[bottomIndex + 1] = midIndex;
			faces[bottomIndex + 2] = firstIndex;

			faces[topIndex] = topCapOffs + firstIndex;
			faces[topIndex + 1] = topCapOffs + midIndex;
			faces[topIndex + 2] = topCapOffs + lastIndex;
		}

		mesh.vertices = verts;
		mesh.uv = uvs;
		mesh.triangles = faces;

		#if UNITY_EDITOR
		MeshUtility.Optimize(mesh);
		#endif

		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		return displace;
	}
}