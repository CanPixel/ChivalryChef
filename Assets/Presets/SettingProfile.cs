using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SettingProfile : ScriptableObject {

	#if UNITY_EDITOR
	public event System.Action OnValuesUpdated;
	#endif

	const int MAX_MAP_SIZE = 700;

	public enum MapType {
		COLORMAP, HEIGHTMAP, FALLOFFMAP, COMBINE
	}
	public enum GenerationType {
		CITY, LANDSCAPE, PLATFORMS, ISLAND
	}
	public enum DistanceMethod {
		VECTOR, MANHATTAN, MINKOWSKI
	}

	public enum TexturingType {
		HEIGHTMAPGRADIENT, COLORMAP, BOTH
	}

	public bool autoUpdate = true;

	[HideInInspector]
	public Vector2Int mapSize;
	public int MapSize = 100;
	[Space(5)]
	public int seed;

	public AnimationCurve curve;

	public DistanceMethod distanceMethod;
	[Range(1, 100)]
	public int regionCount = 100;

	[Range(0, 2)]
	public float elevation = 1;

	[Range(1, 10)]
	public int iterations = 1;

	[HideInInspector]
	public bool usingMinkowski;

	public float heightMultiplier = 2;
	public GenerationType generationType;
	[ConditionalHide("usingMinkowski", 1)]
	public float minkowskiP = 2;

	public bool usingFalloff;
	public float falloffMapIntensity = 1;

	[Range(0, 10)]
	public int simplification;

	public bool clampHeights;

	public GenerationMethod generationMethod;

	[Header("Texture")]
	public FilterMode filterMode;
	public int textureSeed = 0;
	public MapType mapType;
	public TexturingType texturingType;
	[Range(0, 1)]
	public float bothBlend;
	[Range(0, 1)]
	public float fallOffset = 0.5f;
	[Range(0, 5)]
	public float fallSpread = 0.5f;
	[Range(0, 5)]
	public float brightness = 0.5f;
	public Gradient baseColor;
	public bool flipGradient;
	[Range(0, 20)]
	public int colorMapBlur;
	[Range(0, 10)]
	public int masterBlur;
	public bool flatShading;
	public bool transparancyLowest;

	#if UNITY_EDITOR
	protected virtual void OnValidate() {
		UnityEditor.EditorApplication.update -= NotifyUpdatedValues;
		UnityEditor.EditorApplication.update += NotifyUpdatedValues;
		if(falloffMapIntensity < 0) falloffMapIntensity = 0;
		if(minkowskiP > 10) minkowskiP = 10;
		if(minkowskiP < 0) minkowskiP = 0;

		if(MapSize <= 9) MapSize = 10;
		if(MapSize > MAX_MAP_SIZE) MapSize = MAX_MAP_SIZE;
		mapSize = new Vector2Int(MapSize, MapSize);
	}

	public void NotifyUpdatedValues() {
		UnityEditor.EditorApplication.update -= NotifyUpdatedValues;
		if(OnValuesUpdated != null) OnValuesUpdated();
	}
	#endif
}
