using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class Structure : MonoBehaviour {
    public int seed;

    public LayerMask collisionMask;

	public bool flatShade, generateRandom;

	protected MeshFilter meshFilter;
	protected MeshCollider meshCollider;
	protected MeshRenderer meshRenderer;
	protected Mesh mesh;

	protected Vector3[] vertices;
	protected Vector2[] uvs;
	protected int[] tris;

	public Material material;

    public abstract bool ApplyOffsetToGround();

	public virtual void Generate() {
        if(!generateRandom) Random.InitState(seed);
		else {
            seed = Random.Range(0, 255655);
            Random.InitState(seed);
        }
		meshFilter = GetComponent<MeshFilter>();
		meshCollider = GetComponent<MeshCollider>();
		meshRenderer = GetComponent<MeshRenderer>();
        mesh = new Mesh();
	}

	protected virtual void FinaliseMesh() {
        vertices = mesh.vertices;
        uvs = mesh.uv;
        tris = mesh.triangles;

        if(flatShade) Util.FlatShading(ref vertices, ref uvs, ref tris);
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        meshFilter.sharedMesh = mesh;
        meshRenderer.sharedMaterial = material;
        meshCollider.sharedMesh = mesh;
	}
}


public class Polygon
{
    public List<int> m_Vertices;

    public Polygon(int a, int b, int c)
    {
        m_Vertices = new List<int>() { a, b, c };
    }
}

public class PlanetSphere {
    List<Polygon> m_Polygons = new List<Polygon>();
    List<Vector3> m_Vertices = new List<Vector3>();

    public Mesh Create(float radius, int recursion, Vector3 position) {
        float t = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;

        m_Vertices.Add(new Vector3(-1,  t,  0).normalized);
        m_Vertices.Add(new Vector3( 1,  t,  0).normalized);
        m_Vertices.Add(new Vector3(-1, -t,  0).normalized);
        m_Vertices.Add(new Vector3( 1, -t,  0).normalized);
        m_Vertices.Add(new Vector3( 0, -1,  t).normalized);
        m_Vertices.Add(new Vector3( 0,  1,  t).normalized);
        m_Vertices.Add(new Vector3( 0, -1, -t).normalized);
        m_Vertices.Add(new Vector3( 0,  1, -t).normalized);
        m_Vertices.Add(new Vector3( t,  0, -1).normalized);
        m_Vertices.Add(new Vector3( t,  0,  1).normalized);
        m_Vertices.Add(new Vector3(-t,  0, -1).normalized);
        m_Vertices.Add(new Vector3(-t,  0,  1).normalized);

        m_Polygons.Add(new Polygon( 0, 11,  5));
        m_Polygons.Add(new Polygon( 0,  5,  1));
        m_Polygons.Add(new Polygon( 0,  1,  7));
        m_Polygons.Add(new Polygon( 0,  7, 10));
        m_Polygons.Add(new Polygon( 0, 10, 11));
        m_Polygons.Add(new Polygon( 1,  5,  9));
        m_Polygons.Add(new Polygon( 5, 11,  4));
        m_Polygons.Add(new Polygon(11, 10,  2));
        m_Polygons.Add(new Polygon(10,  7,  6));
        m_Polygons.Add(new Polygon( 7,  1,  8));
        m_Polygons.Add(new Polygon( 3,  9,  4));
        m_Polygons.Add(new Polygon( 3,  4,  2));
        m_Polygons.Add(new Polygon( 3,  2,  6));
        m_Polygons.Add(new Polygon( 3,  6,  8));
        m_Polygons.Add(new Polygon( 3,  8,  9));
        m_Polygons.Add(new Polygon( 4,  9,  5));
        m_Polygons.Add(new Polygon( 2,  4, 11));
        m_Polygons.Add(new Polygon( 6,  2, 10));
        m_Polygons.Add(new Polygon( 8,  6,  7));
        m_Polygons.Add(new Polygon( 9,  8,  1));

        int rec = (recursion > 4) ? 4 : recursion;
        Subdivide(rec);

        Mesh mesh = new Mesh();
        int vertexCount = m_Polygons.Count * 3;
        int[] indices = new int[vertexCount];

        Vector3[] verts = new Vector3[vertexCount];
        Vector3[] normals = new Vector3[vertexCount];
        Color32[] colors = new Color32[vertexCount];

        Color32 green = new Color32(20,  255, 30, 255);
        Color32 brown = new Color32(220, 150, 70, 255);

        for(int i = 0; i < m_Polygons.Count; i++) {
            var poly = m_Polygons[i];
            indices[i * 3 + 0] = i * 3 + 0;
            indices[i * 3 + 1] = i * 3 + 1;
            indices[i * 3 + 2] = i * 3 + 2;

            verts[i * 3 + 0] = m_Vertices[poly.m_Vertices[0]] * radius + position;
            verts[i * 3 + 1] = m_Vertices[poly.m_Vertices[1]] * radius + position;
            verts[i * 3 + 2] = m_Vertices[poly.m_Vertices[2]] * radius + position;

            Color32 polyColor = Color32.Lerp(green, brown, Random.Range(0.0f, 1.0f)); 

            colors[i * 3 + 0] = polyColor;
            colors[i * 3 + 1] = polyColor;
            colors[i * 3 + 2] = polyColor;

            normals[i * 3 + 0] = m_Vertices[poly.m_Vertices[0]];
            normals[i * 3 + 1] = m_Vertices[poly.m_Vertices[1]];
            normals[i * 3 + 2] = m_Vertices[poly.m_Vertices[2]];
        }

        mesh.vertices = verts;
        mesh.normals = normals;
        mesh.colors32 = colors;
        mesh.SetTriangles(indices, 0);
        return mesh;
    }

    public void Subdivide(int recursions) {
        var midPointCache = new Dictionary<int, int>();

        for (int i = 0; i < recursions; i++)
        {
            var newPolys = new List<Polygon>();
            foreach (var poly in m_Polygons)
            {
                int a = poly.m_Vertices[0];
                int b = poly.m_Vertices[1];
                int c = poly.m_Vertices[2];

                // Use GetMidPointIndex to either create a
                // new vertex between two old vertices, or
                // find the one that was already created.

                int ab = GetMidPointIndex(midPointCache, a, b);
                int bc = GetMidPointIndex(midPointCache, b, c);
                int ca = GetMidPointIndex(midPointCache, c, a);

                // Create the four new polygons using our original
                // three vertices, and the three new midpoints.
                newPolys.Add(new Polygon(a, ab, ca));
                newPolys.Add(new Polygon(b, bc, ab));
                newPolys.Add(new Polygon(c, ca, bc));
                newPolys.Add(new Polygon(ab, bc, ca));
            }
            // Replace all our old polygons with the new set of
            // subdivided ones.
            m_Polygons = newPolys;
        }
    }
    public int GetMidPointIndex(Dictionary<int, int> cache, int indexA, int indexB) {
        // We create a key out of the two original indices
        // by storing the smaller index in the upper two bytes
        // of an integer, and the larger index in the lower two
        // bytes. By sorting them according to whichever is smaller
        // we ensure that this function returns the same result
        // whether you call
        // GetMidPointIndex(cache, 5, 9)
        // or...
        // GetMidPointIndex(cache, 9, 5)

        int smallerIndex = Mathf.Min(indexA, indexB);
        int greaterIndex = Mathf.Max(indexA, indexB);
        int key = (smallerIndex << 16) + greaterIndex;

        // If a midpoint is already defined, just return it.

        int ret;
        if (cache.TryGetValue(key, out ret))
            return ret;

        // If we're here, it's because a midpoint for these two
        // vertices hasn't been created yet. Let's do that now!

        Vector3 p1 = m_Vertices[indexA];
        Vector3 p2 = m_Vertices[indexB];
        Vector3 middle = Vector3.Lerp(p1, p2, 0.5f).normalized;

        ret = m_Vertices.Count;
        m_Vertices.Add(middle);

        // Add our new midpoint to the cache so we don't have
        // to do this again. =)

        cache.Add(key, ret);
        return ret;
    }
}

public static class DisplacedSphere
{
    private static Vector3[] directions = {
		Vector3.left, Vector3.back, Vector3.right, Vector3.forward
	};

    public static Mesh Create(float radius, int recursionLevel) {
        if(recursionLevel < 0) recursionLevel = 0;
        else if(recursionLevel > 6) recursionLevel = 6;

        Vector3[] vertices = new Vector3[(recursionLevel + 1) * (recursionLevel + 1) * 4 - 
        (recursionLevel * 2 - 1) * 3];

        int[] triangles = new int[(1 << (recursionLevel * 2 + 3)) * 3];
        CreateOctahedron(vertices, triangles, recursionLevel);

        Vector3[] normals = new Vector3[vertices.Length];
        Normalize(vertices, normals);

        Vector2[] uv = new Vector2[vertices.Length];
        CreateUV(vertices, uv);

        Vector4[] tangents = new Vector4[vertices.Length];
        CreateTangents(vertices, tangents);

        if(radius != 1f) for(int i = 0; i < vertices.Length; i++) vertices[i] *= radius;

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.tangents = tangents;
        mesh.uv = uv;
        return mesh;
    }

    private static void CreateOctahedron(Vector3[] verts, int[] tris, int resolution) {
        int v = 0, vBottom = 0, t = 0;

        for(int i = 0; i < 4; i++) verts[v++] = Vector3.down;

        for(int i = 1; i <= resolution; i++) {
            float progress = (float)i / resolution;
            Vector3 from, to;
            verts[v++] = to = Vector3.Lerp(Vector3.down, Vector3.forward, progress);

            for(int d = 0; d < 4; d++) {
                from = to;
                to = Vector3.Lerp(Vector3.down, directions[d], progress);
                t = CreateLowerStrip(i, v, vBottom, t, tris);
                v = CreateVertexLine(from, to, i, v, verts);
                vBottom += i > 1 ? (i - 1) : 1;
            }
            vBottom = v - 1 - i * 4;
        }

        for(int i = resolution - 1; i >= 1; i--) {
            float progress = (float)i / resolution;
            Vector3 from, to;
            verts[v++] = to = Vector3.Lerp(Vector3.up, Vector3.forward, progress);

            for(int d = 0; d < 4; d++) {
                from = to;
                to = Vector3.Lerp(Vector3.up, directions[d], progress);
                t = CreateUpperStrip(i, v, vBottom, t, tris);
                v = CreateVertexLine(from, to, i, v, verts);
                vBottom += i + 1;
            }
            vBottom = v - 1 - i * 4;
        }

        for(int i = 0; i < 4; i++) {
            tris[t++] = vBottom;
            tris[t++] = v;
            tris[t++] = ++vBottom;
            verts[v++] = Vector3.up;
        }
    }

    private static void CreateTangents(Vector3[] verts, Vector4[] tangents) {
        for(int i = 0; i < verts.Length; i++) {
            var v = verts[i];
            v.y = 0f;
            v = v.normalized;
            Vector4 tangent;
            tangent.x = -v.z;
            tangent.y = 0f;
            tangent.z = v.x;
            tangent.w = -1f;
            tangents[i] = tangent;
        }
        tangents[verts.Length - 4] = tangents[0] = new Vector3(-1f, 0, -1f).normalized;
        tangents[verts.Length - 3] = tangents[1] = new Vector3(1f, 0, -1f).normalized;
        tangents[verts.Length - 2] = tangents[2] = new Vector3(1f, 0, 1f).normalized;
        tangents[verts.Length - 1] = tangents[3] = new Vector3(-1f, 0, 1f).normalized;
    }

    private static int CreateUpperStrip(int steps, int vTop, int vBottom, int t, int[] tris) {
        tris[t++] = vBottom;
        tris[t++] = vTop - 1;
        tris[t++] = ++vBottom;

        for(int i = 1; i <= steps; i++) {
            tris[t++] = vTop - 1;
            tris[t++] = vTop;
            tris[t++] = vBottom;

            tris[t++] = vBottom;
            tris[t++] = vTop++;
            tris[t++] = ++vBottom;
        }
        return t;
    }

    private static int CreateLowerStrip(int steps, int vTop, int vBottom, int t, int[] tris) {
        for(int i = 1; i < steps; i++) {
            tris[t++] = vBottom;
            tris[t++] = vTop - 1;
            tris[t++] = vTop;

            tris[t++] = vBottom++;
            tris[t++] = vTop++;
            tris[t++] = vBottom;
        }
        tris[t++] = vBottom;
        tris[t++] = vTop - 1;
        tris[t++] = vTop;
        return t;
    }

    private static int CreateVertexLine(Vector3 from, Vector3 to, int steps, int v, Vector3[] verts) {
        for(int i = 1; i <= steps; i++) verts[v++] = Vector3.Lerp(from, to, (float)i / steps);
        return v;
    }

    private static void Normalize(Vector3[] verts, Vector3[] normals) {
        for(int i = 0; i < verts.Length; i++) normals[i] = verts[i] = verts[i].normalized;
    }

    private static void CreateUV(Vector3[] verts, Vector2[] uv) {
        float previousX = 1f;
        for(int i = 0; i < verts.Length; i++) {
            Vector3 v = verts[i];

            if(v.x == previousX) uv[i - 1].x = 1f;
            previousX = v.x;

            Vector2 texCoords;
            texCoords.x = Mathf.Atan2(v.x, v.z) / (-2f * Mathf.PI);

            if(texCoords.x < 0f) texCoords.x += 1f;
            texCoords.y = Mathf.Asin(v.y) / Mathf.PI + 0.5f;
            uv[i] = texCoords;
        }
        uv[verts.Length - 4].x = uv[0].x = 0.125f;
        uv[verts.Length - 3].x = uv[1].x = 0.375f;
        uv[verts.Length - 2].x = uv[2].x = 0.625f;
        uv[verts.Length - 1].x = uv[3].x = 0.875f;
    }
}


public static class IcoSphere
{
    private struct TriangleIndices
    {
        public int v1;
        public int v2;
        public int v3;
 
        public TriangleIndices(int v1, int v2, int v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }
    }
 
    // return index of point in the middle of p1 and p2
    private static int getMiddlePoint(int p1, int p2, ref List<Vector3> vertices, ref Dictionary<long, int> cache, float radius)
    {
        // first check if we have it already
        bool firstIsSmaller = p1 < p2;
        long smallerIndex = firstIsSmaller ? p1 : p2;
        long greaterIndex = firstIsSmaller ? p2 : p1;
        long key = (smallerIndex << 32) + greaterIndex;
 
        int ret;
        if (cache.TryGetValue(key, out ret))
        {
            return ret;
        }
 
        // not in cache, calculate it
        Vector3 point1 = vertices[p1];
        Vector3 point2 = vertices[p2];
        Vector3 middle = new Vector3
		(
            (point1.x + point2.x) / 2f, 
            (point1.y + point2.y) / 2f, 
            (point1.z + point2.z) / 2f
		);
 
        // add vertex makes sure point is on unit sphere
		int i = vertices.Count;
        vertices.Add( middle.normalized * radius ); 
 
        // store it, return index
        cache.Add(key, i);
 
        return i;
    }
 
    public static Mesh Create(float radius, int recursionLevel) {
        Mesh mesh = new Mesh();
 
        List<Vector3> vertList = new List<Vector3>();
        Dictionary<long, int> middlePointIndexCache = new Dictionary<long, int>();
 
        // create 12 vertices of a icosahedron
        float t = (1f + Mathf.Sqrt(5f)) / 2f;
 
        vertList.Add(new Vector3(-1f,  t,  0f).normalized * radius);
        vertList.Add(new Vector3( 1f,  t,  0f).normalized * radius);
        vertList.Add(new Vector3(-1f, -t,  0f).normalized * radius);
        vertList.Add(new Vector3( 1f, -t,  0f).normalized * radius);
 
        vertList.Add(new Vector3( 0f, -1f,  t).normalized * radius);
        vertList.Add(new Vector3( 0f,  1f,  t).normalized * radius);
        vertList.Add(new Vector3( 0f, -1f, -t).normalized * radius);
        vertList.Add(new Vector3( 0f,  1f, -t).normalized * radius);
 
        vertList.Add(new Vector3( t,  0f, -1f).normalized * radius);
        vertList.Add(new Vector3( t,  0f,  1f).normalized * radius);
        vertList.Add(new Vector3(-t,  0f, -1f).normalized * radius);
        vertList.Add(new Vector3(-t,  0f,  1f).normalized * radius);
 
 
        // create 20 triangles of the icosahedron
        List<TriangleIndices> faces = new List<TriangleIndices>();
 
        // 5 faces around point 0
        faces.Add(new TriangleIndices(0, 11, 5));
        faces.Add(new TriangleIndices(0, 5, 1));
        faces.Add(new TriangleIndices(0, 1, 7));
        faces.Add(new TriangleIndices(0, 7, 10));
        faces.Add(new TriangleIndices(0, 10, 11));
 
        // 5 adjacent faces 
        faces.Add(new TriangleIndices(1, 5, 9));
        faces.Add(new TriangleIndices(5, 11, 4));
        faces.Add(new TriangleIndices(11, 10, 2));
        faces.Add(new TriangleIndices(10, 7, 6));
        faces.Add(new TriangleIndices(7, 1, 8));
 
        // 5 faces around point 3
        faces.Add(new TriangleIndices(3, 9, 4));
        faces.Add(new TriangleIndices(3, 4, 2));
        faces.Add(new TriangleIndices(3, 2, 6));
        faces.Add(new TriangleIndices(3, 6, 8));
        faces.Add(new TriangleIndices(3, 8, 9));
 
        // 5 adjacent faces 
        faces.Add(new TriangleIndices(4, 9, 5));
        faces.Add(new TriangleIndices(2, 4, 11));
        faces.Add(new TriangleIndices(6, 2, 10));
        faces.Add(new TriangleIndices(8, 6, 7));
        faces.Add(new TriangleIndices(9, 8, 1));
 
 
        // refine triangles
        for (int i = 0; i < recursionLevel; i++)
        {
            List<TriangleIndices> faces2 = new List<TriangleIndices>();
            foreach (var tri in faces)
            {
                // replace triangle by 4 triangles
                int a = getMiddlePoint(tri.v1, tri.v2, ref vertList, ref middlePointIndexCache, radius);
                int b = getMiddlePoint(tri.v2, tri.v3, ref vertList, ref middlePointIndexCache, radius);
                int c = getMiddlePoint(tri.v3, tri.v1, ref vertList, ref middlePointIndexCache, radius);
 
                faces2.Add(new TriangleIndices(tri.v1, a, c));
                faces2.Add(new TriangleIndices(tri.v2, b, a));
                faces2.Add(new TriangleIndices(tri.v3, c, b));
                faces2.Add(new TriangleIndices(a, b, c));
            }
            faces = faces2;
        }
        mesh.vertices = vertList.ToArray();
 
        List< int > triList = new List<int>();
        for( int i = 0; i < faces.Count; i++ )
        {
            triList.Add( faces[i].v1 );
            triList.Add( faces[i].v2 );
            triList.Add( faces[i].v3 );
        }		
        mesh.triangles = triList.ToArray();

        var nVerts = mesh.vertices;
        Vector2[] UVs = new Vector2[nVerts.Length];

        for(var i = 0; i < nVerts.Length; i++) {
            var unitVec = nVerts[i].normalized;
            Vector2 ICOuv = new Vector2(0, 0);
            ICOuv.x = (Mathf.Atan2(unitVec.x, unitVec.z) + Mathf.PI) / Mathf.PI / 2;
            ICOuv.y = (Mathf.Acos(unitVec.y) + Mathf.PI) / Mathf.PI - 1;
            UVs[i] = new Vector2(ICOuv.x, ICOuv.y);
        }
        mesh.uv = UVs;
 
        Vector3[] normales = new Vector3[vertList.Count];
        for( int i = 0; i < normales.Length; i++ ) normales[i] = vertList[i].normalized;
        
        Vector3[] flatShadedVertices = new Vector3[mesh.triangles.Length];
		Vector2[] flatShadedUvs = new Vector2[mesh.triangles.Length];
		for(int i = 0; i < mesh.triangles.Length; i++) {
			flatShadedVertices[i] = mesh.vertices[mesh.triangles[i]];
			flatShadedUvs[i] = mesh.uv[mesh.triangles[i]];
			mesh.triangles[i] = i;
		}
		mesh.vertices = flatShadedVertices;
		mesh.uv = flatShadedUvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        #if UNITY_EDITOR
        MeshUtility.Optimize(mesh);
        #endif
        return mesh;
    }
}
