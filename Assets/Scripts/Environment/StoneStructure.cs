using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GK;

[ExecuteInEditMode]
public class StoneStructure : Structure {
	[Range(0.1f, 100f)]
	public float scaleMin, scaleMax;

	private float scale;

	[Range(4, 150)]
	public int detailMin, detailMax;
	private int pointCount;

	public Vector3 shapeElongation;

	public bool canStandUp = false;

	private bool generated = false;

	public override bool ApplyOffsetToGround() {
		RaycastHit hit;
		if(Physics.Raycast(transform.position, Vector3.down, out hit, 2500, collisionMask) && hit.transform.tag == "World") {
			if(hit.transform != null) {
				transform.position = new Vector3(transform.position.x, hit.point.y + (transform.lossyScale.x / 2f) - (scale / 2f), transform.position.z);
				return true;
			}
		}
		return false;
	}

	//void Update() {
		//if(generated) ApplyOffsetToGround();
	//}

	void Start() {
		Generate();
	}

	public override void Generate() {
		if(generated) return;
		base.Generate();
		Random.InitState(System.DateTime.Now.Millisecond);

		scale = Random.Range(scaleMin, scaleMax);
		pointCount = Random.Range(detailMax, detailMax);
		Vector3[] points = new Vector3[pointCount];
		for(int i = 0; i < points.Length; i++) {
			points[i] = new Vector3(Random.Range(-(scale / 2) - shapeElongation.x, (scale / 2) + shapeElongation.x), Random.Range(-(scale / 2) - shapeElongation.y, (scale / 2) + shapeElongation.y), Random.Range(-(scale / 2) - shapeElongation.z, (scale / 2) + shapeElongation.z));
		}

		List<Vector3> verts = new List<Vector3>();
		List<Vector3> normals= new List<Vector3>();
		List<int> tris = new List<int>();
		new ConvexHullCalculator().GenerateHull(new List<Vector3>(points), flatShade, ref verts, ref tris, ref normals);

		mesh.vertices = verts.ToArray();
		mesh.triangles = tris.ToArray();
		mesh.normals = normals.ToArray();

		FinaliseMesh();

		GetComponent<MeshFilter>().sharedMesh = mesh;

		transform.localRotation = Quaternion.Euler(canStandUp ? (Random.Range(0, 4) * 90) : 0, Random.Range(0, 360), Random.Range(0, 360));
		generated = true;
	}
}