using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using System.Text;
using System.IO;

#if UNITY_EDITOR
 public class ReadOnlyAttribute : PropertyAttribute
 {
 
 }
 
 [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
 public class ReadOnlyDrawer : PropertyDrawer
 {
     public override float GetPropertyHeight(SerializedProperty property,
                                             GUIContent label)
     {
         return EditorGUI.GetPropertyHeight(property, label, true);
     }
 
     public override void OnGUI(Rect position,
                                SerializedProperty property,
                                GUIContent label)
     {
         GUI.enabled = false;
         EditorGUI.PropertyField(position, property, label, true);
         GUI.enabled = true;
     }
 }
 #endif

public static class Util {
    public static string SingularizeWord(string str) {
        string final = str;
        if(str.EndsWith("ies")) final = str.Substring(0, str.Length - 3) + "y";
        else if(str.EndsWith("oes")) final = str.Substring(0, str.Length - 2);
        else if(str.EndsWith("s")) final = str.Substring(0, str.Length - 1);

        return final;
    }

    [System.Serializable]
    public class MeshData {
        public Vector3[] vertices;
        public Vector2[] uvs;
        public int[] tris;

        public MeshData(Vector3[] verts, Vector2[] uv, int[] tri) {
            vertices = verts;
            uvs = uv;
            tris = tri;
        }
    }

    public static bool ColorInRange(Color val, Color min, Color max) {
        bool isRedGood =  val.r >= min.r && val.r <= max.r;
        bool isGreenGood = val.g >= min.g && val.g <= max.g;
        bool isBlueGood = val.b >= min.b && val.b <= max.b;
        return isRedGood && isGreenGood && isBlueGood;
    }

    public static GameObject FindChildByTag(Transform parent, string tag) {
        for(int i = 0; i < parent.childCount; i++) {
            var child = parent.GetChild(i).gameObject;
            if(child.tag == tag) return child;
        }
        return null;
    }

    public static void ClearChildren(Transform parent) {
        for(int i = parent.childCount - 1; i >= 0; i--) GameObject.DestroyImmediate(parent.GetChild(i).gameObject);
    }

    #if UNITY_EDITOR
    [MenuItem("Tools/Clear Console %#c")]
    #endif
    public static void ClearConsole() {
        var logEntries = System.Type.GetType("UnityEditor.LogEntries,UnityEditor.dll");
        var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        clearMethod.Invoke(null, null);
    }

    public static int ConvertToBase2(int num) {
        if(num >= 2) return ConvertToBase2(num / 2) * 10 + (num % 2);
        else return num;
    }

    public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t) {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;
        var mid = Vector3.Lerp(start, end, t);
        return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
    }

    public static Vector2 Parabola(Vector2 start, Vector2 end, float height, float t) {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;
        var mid = Vector2.Lerp(start, end, t);
        return new Vector2(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t));
    }

    public static bool Approximately(float a, float b, float threshold = 0.05f) {
        float max = Mathf.Max(a, b);
        float min = Mathf.Min(a, b);
        return Mathf.Abs(max - min) <= threshold;
    }

    public static void HiResScreenshot(string name, int res) {
        ScreenCapture.CaptureScreenshot(name, res);
    }

    public static float ManhattanDistance(Vector2 p1, Vector2 p2) {
        return Mathf.Abs(p1.x - p2.x) + Mathf.Abs(p1.y - p2.y);
    }

    public static float MinkowskiDistance(Vector2 p1, Vector2 p2, float p) {
        return Mathf.Pow(Mathf.Pow(Mathf.Abs(p1.x - p2.x), p) + Mathf.Pow(Mathf.Abs(p1.y - p2.y), p), 1 / p);
    }

	public static void FlatShading(ref Vector3[] vertices, ref Vector2[] uvs, ref int[] triangles) {
		Vector3[] flatShadedVertices = new Vector3[triangles.Length];
		Vector2[] flatShadedUvs = new Vector2[triangles.Length];
        if(triangles.Length < 1 || uvs.Length < triangles[triangles.Length - 1]) return;
		for(int i = 0; i < triangles.Length; i++) {
            if(triangles[i] > vertices.Length - 1 || triangles[i] > uvs.Length - 1 || i > flatShadedUvs.Length) continue;
			flatShadedVertices[i] = vertices[triangles[i]];
			flatShadedUvs[i] = uvs[triangles[i]];
			triangles[i] = i;
		}
		vertices = flatShadedVertices;
		uvs = flatShadedUvs;
	}

    public static MeshData FlatShading(Vector3[] vertices, Vector2[] uvs, int[] triangles) {
		Vector3[] flatShadedVertices = new Vector3[triangles.Length];
		Vector2[] flatShadedUvs = new Vector2[triangles.Length];
        int[] tris = new int[triangles.Length];
		for(int i = 0; i < triangles.Length; i++) {
			flatShadedVertices[i] = vertices[triangles[i]];
			flatShadedUvs[i] = uvs[triangles[i]];
			tris[i] = i;
		}
        return new MeshData(flatShadedVertices, flatShadedUvs, tris);
	}

    public static Texture2D BlurImage(Texture2D img, int blur) {
        Texture2D blurred = new Texture2D(img.width, img.height, TextureFormat.ARGB32, false);
        Color[] colors = new Color[img.width * img.height];
        for(int x = 0; x < img.width; x++)
            for(int y = 0; y < img.height; y++) {
                float avgR = 0, avgG = 0, avgB = 0, avgA = 1;
                int blurPixelCount = 0;

                for(int xx = x; (xx < x + blur && xx < img.width); xx++)
                    for(int yy = y; (yy < y + blur && yy < img.height); yy++) {
                        var pixel = img.GetPixel(xx, yy);
                        avgR += pixel.r;
                        avgG += pixel.g;
                        avgB += pixel.b;
                        avgA += pixel.a;
                        blurPixelCount++;
                    }
                avgR /= blurPixelCount;
                avgG /= blurPixelCount;
                avgB /= blurPixelCount;
                avgA /= blurPixelCount;

                for(int xx = x; xx < x + blur && xx < img.width; xx++) 
                    for(int yy = y; yy < y + blur && yy < img.height; yy++) {
                        colors[yy * img.width + xx] = new Color(avgR, avgG, avgB, avgA);
                    } 
            }
        blurred.SetPixels(colors);
        blurred.filterMode = FilterMode.Point;
        blurred.Apply();
        return blurred;
    }

    public static int Fibonacci(int n) {
        return n < 2 ? n : Fibonacci(n - 1) + Fibonacci(n - 2);
    }

     public enum BlendMode
     {
         Opaque,
         Cutout,
         Fade,
         Transparent
     }
 
     public static void ChangeRenderMode(ref Material standardShaderMaterial, BlendMode blendMode) {
        switch (blendMode) {
            case BlendMode.Opaque:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                standardShaderMaterial.SetInt("_ZWrite", 1);
                standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.SetOverrideTag("RenderType", "Opaque");
                standardShaderMaterial.SetFloat("_Mode", 0); 
                standardShaderMaterial.renderQueue = -1;
                break;
            case BlendMode.Cutout:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                standardShaderMaterial.SetInt("_ZWrite", 1);
                standardShaderMaterial.EnableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.SetOverrideTag("RenderType", "Cutout");
                standardShaderMaterial.SetFloat("_Mode", 1); 
                standardShaderMaterial.renderQueue = 2450;
                break;
            case BlendMode.Fade:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                standardShaderMaterial.SetInt("_ZWrite", 0);
                standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.EnableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.SetOverrideTag("RenderType", "Fade");
                standardShaderMaterial.SetFloat("_Mode", 2); 
                standardShaderMaterial.renderQueue = 3000;
                break;
            case BlendMode.Transparent:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                standardShaderMaterial.SetInt("_ZWrite", 0);
                standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.SetOverrideTag("RenderType", "Transparent");
                standardShaderMaterial.SetFloat("_Mode", 3); 
                standardShaderMaterial.renderQueue = 3000;
                break;
        }
     }

    public static T CopyComponent<T>(T original, GameObject destination) where T : Component {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields) field.SetValue(copy, field.GetValue(original));
        return copy as T;
    }

    public delegate void ScreenshotNotify();
    public static void CreateScreenCap(MonoBehaviour host, ScreenshotNotify myCallback) {
        host.StartCoroutine(Capture(myCallback));
    }

    private static IEnumerator Capture(ScreenshotNotify myCallback) {
        yield return new WaitForEndOfFrame();
        ScreenCapture.CaptureScreenshot("Chivalry Image " + System.DateTime.Now.Month + "-" + System.DateTime.Now.Day + "-" + System.DateTime.Now.Year + "-" + System.DateTime.Now.Hour + "-" + System.DateTime.Now.Minute + "-" + System.DateTime.Now.Millisecond + ".png");
        yield return new WaitForEndOfFrame();
        myCallback();
    }
}