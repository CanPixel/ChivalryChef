using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using UnityEditor;
using System.IO;
using System.Threading.Tasks;

public class VoronoiGenerator : MonoBehaviour {
	public GameObject gamePlane;
	public GameObject gameMesh;
	public Minimap minimap;

	public GameObject[] enableAfterLoad;

	public Texture2D OcclusionNormals;

	public enum EditMode {
		MAP, MESH
	}
	public EditMode editMode;

	public bool generateOnStart = true;
	public bool randomWorld = true;
	public bool keepObjectsDisabled = true;

	[Header("Load Setting Profile")]
	public SettingProfile setting;
	
	[HideInInspector]
	public SettingProfile lastSettings;

	[HideInInspector]
	public bool worldSettingFoldout;

	[HideInInspector]
	public bool lastMesh = false;
	private Mesh grassMesh;

	public static int regionCount = 0;

	void OnValidate() {
		if(setting == null) return;

		foreach(var i in populations) {
			if(i.placementMin > i.placementMax) i.placementMin = i.placementMax;
			i.minColor = setting.baseColor.Evaluate(i.placementMin);
			i.maxColor = setting.baseColor.Evaluate(i.placementMax);
		}
	}

	[Header("Population Settings")]
	[Range(0, 0.8f)]
	public float grassMeshQuality = 0.5f;
	public Population[] populations;

	[System.Serializable]
	public class Population {
		public string name;
		public GameObject prefab;
		[Range(0, 1)]
		public float placementMin, placementMax, rarity;

		#if UNITY_EDITOR
		[ReadOnly] 
		#endif
		public Color minColor, maxColor;

		public bool unifiedMesh = true;

		public bool keep = false, enable = true;
		[HideInInspector]
		public GameObject generatedOBJ;

		public bool isDone = false;
	} 

	[Header("Debug Settings")]
	[Range(0f, 0.8f)]
	public float pointSize = 5;
	public bool showPoints;

	[System.Serializable]
	public class MapLayer {
		public Vector2[] points;
		[HideInInspector]
		public float[] heightMap;
		public Texture2D texture;

		public MapLayer(SettingProfile setting) {
			points = new Vector2[setting.regionCount];
			heightMap = new float[setting.mapSize.x * setting.mapSize.y];
			this.texture = new Texture2D(setting.mapSize.x, setting.mapSize.y);
		}
	}

	#region REFERENCES_FOR_GENERATION
	protected MapLayer map;
	protected float[] populationHeightMap;

	protected MeshRenderer meshRenderer;
	protected MeshFilter meshFilter;

	protected MeshRenderer mapRenderer;
	protected MeshFilter mapFilter;

	private Random.State oldRandom;
	#endregion

	private LoadingScreen loadingScreen;

	//Prevents worlds from generating that are too small, avoids glitchy terrain
	protected bool ValidMapSize() {
		return setting.mapSize.x >= 10;
	}	

	private bool IsGeneratingInGame = false;

	void Start() {
		IsGeneratingInGame = true;
		if(!generateOnStart) {
			FastBoot();
			return;
		}
		loadingScreen = GetComponent<LoadingScreen>();
		for(int i = 0; i < enableAfterLoad.Length; i++) if(!enableAfterLoad[i].activeInHierarchy && keepObjectsDisabled) enableAfterLoad[i] = null;
		foreach(var i in enableAfterLoad) if(i != null) i.SetActive(false);
		loadingScreen.Initiate();
		oldRandom = Random.state;
		regionCount++;
		if(gameMesh != null) Util.ClearChildren(gameMesh.transform);

		//Initialize all loading tasks
		GenIterateDirect(true);
		PopulateAll(true);
		foreach(var population in populations) population.keep = population.isDone = false;

		if(randomWorld) GenerateRandom();
		else GenerateIteratedMesh();
	}

	//For external editor use
	[ContextMenu("Load and Apply Texture")]
	public void FastBoot() {
		if(IsGeneratingInGame) GenerateTexture(false);
		if(map != null) gameMesh.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = map.texture;
		InitMiniMap();
		FinishLoad();
	}

	//Finish loading screen sequence
	public void FinishLoad() {
		if(generateOnStart) {
			gameMesh.SetActive(true);
			gamePlane.SetActive(false);
		}
		foreach(var i in enableAfterLoad) if(i != null) i.SetActive(true);
		var playerManager = GetComponent<PlayerManager>();
		playerManager.Begin();
		if(IsGeneratingInGame) Random.state = oldRandom;
		if(loadingScreen != null) loadingScreen.Finish();
	}

	public void GenerateAndPopulate() {
		var sw = new System.Diagnostics.Stopwatch();
		Util.ClearChildren(gameMesh.transform);
		foreach(var population in populations) population.keep = false;
		sw.Start();
		GenerateRandom();
		PopulateAll();
		sw.Stop();
		Debug.Log("Generation took " + sw.ElapsedMilliseconds + " ms.");
		gameMesh.SetActive(true);
		gamePlane.SetActive(false);
	}

	//Spawn Rocks, Trees, Grass and misc. environmental assets
	//tasksOnly is needed to distinguish the loading screen process of generation with in-editor generation
	public void PopulateAll(bool tasksOnly = false) {
		foreach(var population in populations) {
			if(loadingScreen != null) loadingScreen.SetProgress("Adding " + population.name, tasksOnly);
			if(!tasksOnly) {
				if(!population.enable) {
					DestroyImmediate(population.generatedOBJ);
					continue;
				}
				if(population.keep) {
					population.isDone = true;
					continue;
				}
				else DestroyImmediate(population.generatedOBJ);
			}
			if(population.unifiedMesh) { //Checks if given asset (trees, rocks) needs a single slice of the terrain mesh to spawn. (crucial for grass generation)
				GameObject obj;
				GameObject[] slices = null, topSlice = null;
				MeshFilter filter = null;
				float top = 0;
				MeshRenderer render = null;
				if(!tasksOnly) {
					obj = Instantiate(population.prefab);
					obj.name = population.name;
					obj.transform.SetParent(gameMesh.transform);
					obj.transform.localPosition = Vector3.zero;
					obj.transform.localScale = Vector3.one;
					population.generatedOBJ = obj;

					render = obj.GetComponent<MeshRenderer>();
					filter = obj.GetComponent<MeshFilter>();
					render.sharedMaterial.SetFloat("rarity", population.rarity);

					var bottom = setting.curve.Evaluate(population.placementMin) * (setting.heightMultiplier / 5f);
					top = setting.curve.Evaluate(population.placementMax) * (setting.heightMultiplier / 3f);
					mapFilter = gameMesh.GetComponent<MeshFilter>();

					var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
					meshSimplifier.Initialize(mapFilter.sharedMesh);
					meshSimplifier.SimplifyMesh(grassMeshQuality);
					filter.sharedMesh = meshSimplifier.ToMesh();

					//Slices the mesh of the terrain
					slices = BLINDED_AM_ME.MeshCut.CutCopy(filter.gameObject, new Vector3(0, 1, 0) * bottom, Vector3.up, render.sharedMaterial);
				}
				if(loadingScreen != null) loadingScreen.SetProgress("Adding " + population.name, tasksOnly);
				
				if(!tasksOnly) {
					topSlice = BLINDED_AM_ME.MeshCut.CutCopy(slices[1].gameObject, new Vector3(0, 1, 0) * top, Vector3.up, render.sharedMaterial);
				
					foreach(var i in slices) {
						i.transform.SetParent(gameMesh.transform);
						i.transform.localScale = Vector3.one;
						DestroyImmediate(i.gameObject);
					}
					var mesh = topSlice[0].GetComponent<MeshFilter>().sharedMesh;
					mesh.RecalculateBounds();
					mesh.RecalculateTangents();
					mesh.RecalculateNormals();

					#if UNITY_EDITOR
					MeshUtility.Optimize(mesh);
					#endif

					filter.sharedMesh = grassMesh = mesh;
					foreach(var i in topSlice) {
						i.transform.SetParent(gameMesh.transform);
						i.transform.localScale = Vector3.one;
						DestroyImmediate(i.gameObject);
					}
				}
			} else { //Spawn Environmental assets randomly (rocks, trees) 
				if(tasksOnly) continue;
				var obj = new GameObject(population.name);
				obj.transform.SetParent(gameMesh.transform);
				obj.transform.localPosition = Vector3.zero;
				obj.transform.localScale = Vector3.one;
				population.generatedOBJ = obj;

				var heightMap = populationHeightMap;
				int count = 0;
				Random.InitState(System.Environment.TickCount);

				for(int i = 0; i < 100; i++) {
					if(count >= 40) break;
					int index = Random.Range(0, heightMap.Length);
					int x = index % setting.mapSize.x;
					int y = index / setting.mapSize.y;
					float val = Mathf.Abs(heightMap[index]);
					if(setting.clampHeights) heightMap[index] = Mathf.RoundToInt(heightMap[index]);
					if(setting.flipGradient) val = 1f - val;

					if(val < population.placementMin || val > population.placementMax) {
						i--;
						continue;
					}

					//Spawn based on rarity value
					if(population.rarity < Random.Range(0f, 1f)) {
						count++;
						var element = Instantiate(population.prefab);
						element.transform.SetParent(obj.transform);
						
						var globalHeight = val * setting.heightMultiplier;
						element.transform.localPosition = new Vector3(x, globalHeight, y);
						element.transform.localPosition -= new Vector3(setting.mapSize.x / 2f, 0, setting.mapSize.y / 2f);
						element.name = population.name;	
						element.GetComponent<Structure>().ApplyOffsetToGround();
					}
				}
			} 
			population.isDone = true;
		}
	}

	//Populates the terrain with environmental assets (rocks, trees, grass)
	public IEnumerator Populate(Population population) {
		if(loadingScreen != null) loadingScreen.SetProgress("Adding " + population.name);
		if(!population.enable) {
			DestroyImmediate(population.generatedOBJ);
			yield return null;
		}
		if(population.keep) {
			population.isDone = true;
			yield return null;
		}
		else DestroyImmediate(population.generatedOBJ);

		if(population.unifiedMesh) {
			var obj = Instantiate(population.prefab);
			obj.name = population.name;
			obj.transform.SetParent(gameMesh.transform);
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localScale = Vector3.one;
			population.generatedOBJ = obj;

			var render = obj.GetComponent<MeshRenderer>();
			var filter = obj.GetComponent<MeshFilter>();
			render.sharedMaterial.SetFloat("rarity", population.rarity);
			var bottom = setting.curve.Evaluate(population.placementMin) * (setting.heightMultiplier / 5f);
			var top = setting.curve.Evaluate(population.placementMax) * (setting.heightMultiplier / 3f);
			mapFilter = gameMesh.GetComponent<MeshFilter>();
			render.enabled = false;

			var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
			meshSimplifier.Initialize(mapFilter.sharedMesh);
			meshSimplifier.SimplifyMesh(grassMeshQuality);
			filter.sharedMesh = meshSimplifier.ToMesh();

			var slices = BLINDED_AM_ME.MeshCut.CutCopy(filter.gameObject, new Vector3(0, 1, 0) * bottom, Vector3.up, render.sharedMaterial);
			if(loadingScreen != null) loadingScreen.SetProgress("Adding " + population.name);
			yield return new WaitForSeconds(1f);
			var topSlice = BLINDED_AM_ME.MeshCut.CutCopy(slices[1].gameObject, new Vector3(0, 1, 0) * top, Vector3.up, render.sharedMaterial);
			
			foreach(var i in slices) {
				i.transform.SetParent(gameMesh.transform);
				i.transform.localScale = Vector3.one;
				DestroyImmediate(i.gameObject);
			}
			var mesh = topSlice[0].GetComponent<MeshFilter>().sharedMesh;
			mesh.RecalculateBounds();
			mesh.RecalculateTangents();
			mesh.RecalculateNormals();
			
			#if UNITY_EDITOR
			MeshUtility.Optimize(mesh);
			#endif

			filter.sharedMesh = grassMesh = mesh;
			render.enabled = true;
			foreach(var i in topSlice) {
				i.transform.SetParent(gameMesh.transform);
				i.transform.localScale = Vector3.one;
				DestroyImmediate(i.gameObject);
			}
		} else {
			yield return new WaitForSeconds(1f);
			var obj = new GameObject(population.name);
			obj.transform.SetParent(gameMesh.transform);
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localScale = Vector3.one;
			population.generatedOBJ = obj;
			
			var heightMap = populationHeightMap;
			Random.InitState(System.Environment.TickCount);

			int count = 0, trial = 0;
			for(int i = 0; i < 100; i++) {
				if(count >= 40) break;

				int index = Random.Range(0, heightMap.Length);
				int x = index % setting.mapSize.x;
				int y = index / setting.mapSize.y;
				float val = Mathf.Abs(heightMap[index]);
				if(setting.clampHeights) heightMap[index] = Mathf.RoundToInt(heightMap[index]);
				if(setting.flipGradient) val = 1f - val;

				if(trial < 100 && (val < population.placementMin || val > population.placementMax)) {
					trial++;
					i--;
					continue;
				}

				if(population.rarity < Random.Range(0f, 1f)) {
					count++;
					var element = Instantiate(population.prefab);
					element.name = population.name;
					element.transform.SetParent(obj.transform);
					var globalHeight = val * setting.heightMultiplier;
					element.transform.localPosition = new Vector3(x, globalHeight, y);
					element.transform.localPosition -= new Vector3(setting.mapSize.x / 2f, 0, setting.mapSize.y / 2f);
					element.name = population.name;	
					element.GetComponent<Structure>().ApplyOffsetToGround();	
				}
			}
		}
		population.isDone = true;
	}

	//Start the generation sequence
	public void StartGenerate() {
		if(editMode == VoronoiGenerator.EditMode.MAP) {
			lastMesh = false;
			GenerateTexture();
		} else GenerateIteratedMesh();
	}

	//Save & export a randomly generated world into project files (only works in Editor)
	public void Export() {
		#if UNITY_EDITOR
		var obj = Instantiate(gameMesh, Vector3.zero, Quaternion.identity);
		obj.transform.localScale = Vector3.one * 0.5f;
		obj.name = "Exported World " + setting.seed;

		string baseDir = Application.dataPath;
		string directory = "/Worlds/" + setting.seed + "/";

		var mat = new Material(meshRenderer.sharedMaterial);
		var bytes = map.texture.EncodeToPNG();
		Directory.CreateDirectory(baseDir + directory);
		File.WriteAllBytes(baseDir + directory + "Texture.png", bytes);

		//Material + Texture
		AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(map.texture));
		AssetDatabase.ImportAsset(baseDir + directory + "Texture.png");
		AssetDatabase.Refresh();
		AssetDatabase.CreateAsset(mat, "Assets" + directory + "Material.mat");
		AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(mat));

		Util.ChangeRenderMode(ref mat, Util.BlendMode.Cutout);
		mat.SetFloat("_Cutoff", 0.774f);
		mat.SetFloat("_Metallic", 0.65f);
		mat.SetFloat("_Glossiness", 0.37f);
		mat.SetFloat("_OcclusionStrength", 1);
		mat.DisableKeyword("_EMISSION");
		mat.SetTexture("_OcclusionMap", OcclusionNormals);
		mat.SetTexture("_MainTex", AssetDatabase.LoadAssetAtPath("Assets" + directory + "Texture.png", typeof(Texture)) as Texture);

		//Mesh
		var tempMesh = (Mesh)Instantiate(gameMesh.GetComponent<MeshFilter>().sharedMesh);
		AssetDatabase.CreateAsset(tempMesh, "Assets" + directory + "Model.asset");

		//Grass Mesh
		var tempGrass = (Mesh)Instantiate(grassMesh);
		AssetDatabase.CreateAsset(tempGrass, "Assets" + directory + "GrassModel.asset");
		
		obj.transform.Find("Grass").GetComponent<MeshFilter>().sharedMesh = tempGrass;

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		Debug.Log("World exported to " + baseDir + directory);
		
		obj.GetComponent<MeshRenderer>().sharedMaterial = mat;
		obj.GetComponent<MeshFilter>().sharedMesh = obj.GetComponent<MeshCollider>().sharedMesh = tempMesh;
		
		//Forging Prefab
		//var prefab = PrefabUtility.CreateEmptyPrefab("Assets" + directory + setting.seed + ".prefab");
		//PrefabUtility.ReplacePrefab(obj, prefab);
		//var prefab = new GameObject("Assets" + directory + setting.seed);
		PrefabUtility.SaveAsPrefabAsset(obj, "Assets" + directory + setting.seed + ".prefab");

		gameMesh.SetActive(false);
		#endif
	}
	public void GenerateRandom() {
		Util.ClearChildren(gameMesh.transform);
		Random.InitState(System.DateTime.Now.Millisecond);
		setting.seed = Random.Range(0, 255655);
		setting.textureSeed = Random.Range(0, 255655);
		StartGenerate();
	}
	private void GenerateIteratedMesh() {
		if(IsGeneratingInGame) {
			SoundManager.SetOSTState(SoundManager.OSTEvent.LOADING);
			StartCoroutine(GenIterate());
		}
		else GenIterateDirect();
	}

	private void FinishLoadingSound(float pitch = -1) {
		if(pitch < 0) SoundManager.PLAY_UNIQUE_SOUND("PLOEP", 0.1f);
		else SoundManager.PLAY_SOUND("PLOEP", 0.3f, pitch);
	}

	//private bool genDone = false;
	private IEnumerator GenIterate() {
		var sw = new System.Diagnostics.Stopwatch();
		sw.Start();
		GenerateTexture(false);
		yield return new WaitForSeconds(0.5f);
		yield return new WaitUntil(() => map != null);
		StartCoroutine(GenerateMesh(false));
		yield return new WaitUntil(() => gameMesh.activeSelf);
		//Texture2D minimap;
		yield return new WaitUntil(() => InitMiniMap());
		sw.Stop();
		Debug.Log("Generation took " + sw.ElapsedMilliseconds + " ms.");
		if(IsGeneratingInGame) {
			for(int i = 0; i < populations.Length; i++) {
				StartCoroutine(Populate(populations[i]));
				yield return new WaitUntil(() => populations[i].isDone);
				FinishLoadingSound();
			}
		}
		yield return new WaitForSeconds(0.5f);
		FinishLoad();
		FinishLoadingSound(0.6f);
	}

	//Generates the model in one go. 
	//tasksOnly is used to distinguish the generation during the loading process with in-editor use of the generation
	private void GenIterateDirect(bool tasksOnly = false) {
		map = GenerateTexture(tasksOnly);

		populationHeightMap = new float[map.heightMap.Length];
		for(int i = 0; i < map.heightMap.Length; i++) populationHeightMap[i] = map.heightMap[i];

		if(tasksOnly) StartCoroutine(GenerateMesh(true));
		else GenerateMesh();
		InitMiniMap();
	}

	private Texture2D InitMiniMap() {
		if(this.map == null) this.map = GenerateTexture(false);
		var map = this.map.texture;
		map.filterMode = setting.filterMode;

		var cols = map.GetPixels();
		for(int i = 0; i < cols.Length; i++) {
			if(cols[i].r <= 0.1f && cols[i].g <= 0.1f && cols[i].b <= 0.1f) cols[i] = new Color(0, 0, 0, 0);
		}
		map.SetPixels(cols);
		map.Apply();
		if(minimap != null) minimap.LoadMinimap();
		
		return map;
	}

	public MapLayer GenerateMiniMap(int blur = -1) {
		map = new MapLayer(setting);
		var falloffLayer = GetFalloffmap();
		var heightLayer = GetHeightMap();
		var colorLayer = GetColorMap();
		map.points = falloffLayer.points;
		map.heightMap = falloffLayer.heightMap;
		populationHeightMap = new float[map.heightMap.Length];

		Color[] colors = new Color[setting.mapSize.x * setting.mapSize.y];
		for(int i = 0; i < map.heightMap.Length; i++) {
			int x = i % setting.mapSize.x;
			int y = i / setting.mapSize.x;
			var heightCol =  heightLayer.texture.GetPixel(x, y);
			var color = colorLayer.texture.GetPixel(x, y);

			var col = heightCol;
			if(setting.texturingType == SettingProfile.TexturingType.HEIGHTMAPGRADIENT || setting.texturingType == SettingProfile.TexturingType.BOTH) {
				float val = Mathf.Abs(falloffLayer.heightMap[i]);
				if(setting.clampHeights) falloffLayer.heightMap[i] = Mathf.RoundToInt(falloffLayer.heightMap[i]);
				if(setting.flipGradient) val = 1f - val;
				var gradientCol = setting.baseColor.Evaluate(val);
				col = gradientCol * setting.brightness;
			} 
			else if(setting.texturingType == SettingProfile.TexturingType.COLORMAP) {
				col = color;
				col *= setting.brightness * 2;
			}

			if(setting.transparancyLowest && (col / setting.brightness) == (setting.baseColor.Evaluate(0))) col = new Color(0, 0, 0, 0);

			if(setting.texturingType == SettingProfile.TexturingType.BOTH && col != new Color(0, 0, 0, 0)) {
				float val = Mathf.Abs(falloffLayer.heightMap[i]);
				if(setting.clampHeights) falloffLayer.heightMap[i] = Mathf.RoundToInt(falloffLayer.heightMap[i]);
				if(setting.flipGradient) val = 1f - val;

				Color mapColors;
				mapColors = color;
				mapColors *= setting.brightness * 2f;

				col = Color.Lerp(col, mapColors, setting.bothBlend);
				col = Color.Lerp(setting.baseColor.Evaluate(0), col, (1f - falloffLayer.heightMap[i]) * 4f);
			}
			float threshold = 0.9f;
			if(col.r >= threshold && col.r >= threshold && col.b >= threshold) col.a = 0;
			colors[i] = col;
		}

		if(blur < 0) map.texture = Util.BlurImage(GetImageFromColorArray(colors), setting.masterBlur);
		else if(blur > 0) map.texture = Util.BlurImage(GetImageFromColorArray(colors), blur);
		return map;
	}
	public MapLayer GenerateTexture(bool tasksOnly = false) {
		var map = new MapLayer(setting);
		GetGenerationType();

		meshRenderer = gamePlane.GetComponent<MeshRenderer>();
		meshFilter = gamePlane.GetComponent<MeshFilter>();

		meshFilter.sharedMesh.Clear();
		if(loadingScreen != null) loadingScreen.SetProgress("Painting world texture", tasksOnly);

		if(!tasksOnly) {
			if(setting.mapType == SettingProfile.MapType.COLORMAP) map = GetColorMap();
			else if(setting.mapType == SettingProfile.MapType.HEIGHTMAP) map = GetHeightMap();
			else if(setting.mapType == SettingProfile.MapType.FALLOFFMAP) map = GetFalloffmap();
			else if(setting.mapType == SettingProfile.MapType.COMBINE) map = GenerateMiniMap();

			map.texture.wrapMode = TextureWrapMode.Clamp;
			if(meshRenderer.sharedMaterial == null) meshRenderer.sharedMaterial = new Material(meshRenderer.material);
			meshRenderer.sharedMaterial.mainTexture = map.texture;

			gamePlane.SetActive(true);
			gameMesh.SetActive(false);
		}
		return map;
	}

	//Generates the 3D geometry of the procedural world (For loading screen)
	private IEnumerator GenerateMesh(bool tasksOnly = false) {
		if(gameMesh != null) {
			if(gameMesh.GetComponent<MeshFilter>().sharedMesh != null) gameMesh.GetComponent<MeshFilter>().sharedMesh.Clear();
			else gameMesh.GetComponent<MeshFilter>().sharedMesh = new Mesh();
		}
		for(int i = 0; i < setting.iterations; i++) {
			if(!tasksOnly) lastMesh = true;
			if(mapRenderer == null) mapRenderer = gameMesh.GetComponent<MeshRenderer>();
			if(mapFilter == null) mapFilter = gameMesh.GetComponent<MeshFilter>();
			if(!tasksOnly) meshRenderer.sharedMaterial.mainTexture = map.texture;

			GetGenerationType();

			if(!ValidMapSize() && !tasksOnly) yield return null;
			if(loadingScreen != null) loadingScreen.SetProgress("Creating traversable surfaces (" + (i + 1) + "/" + setting.iterations + ")", tasksOnly);

			if(!tasksOnly) yield return new WaitForSeconds(0.5f);
			
			Vector3[] verts = new Vector3[(setting.mapSize.x * setting.mapSize.y) / GetSimplification(setting)];
			Vector2[] uvs = new Vector2[verts.Length];
			int[] tris = new int[verts.Length - 2];
			if(setting.generationMethod != null && !tasksOnly) setting.generationMethod.CreateMesh(ref verts, ref uvs, ref tris, ref map);

			if(loadingScreen != null) loadingScreen.SetProgress("Shading landscapes (" + (i + 1) + "/" + setting.iterations + ")", tasksOnly);
			if(setting.flatShading && !tasksOnly) Util.FlatShading(ref verts, ref uvs, ref tris);

			if(!tasksOnly) {
				mapFilter.sharedMesh.vertices = verts;
				mapFilter.sharedMesh.uv = uvs;
				mapFilter.sharedMesh.triangles = tris;
				mapRenderer.sharedMaterial.mainTexture = map.texture;
				gameMesh.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
				gameMesh.GetComponent<MeshCollider>().sharedMesh = mapFilter.sharedMesh;
				
				mapFilter.sharedMesh.RecalculateNormals();

				CalculatePoints();
			}
			if(!tasksOnly) FinishLoadingSound();
		}

		gamePlane.SetActive(false);
		gameMesh.SetActive(true);
	}

	//Generates the 3D geometry of the procedural world (In-editor)
	private void GenerateMesh() {
		if(gameMesh.GetComponent<MeshFilter>().sharedMesh != null) gameMesh.GetComponent<MeshFilter>().sharedMesh.Clear();
		else gameMesh.GetComponent<MeshFilter>().sharedMesh = new Mesh();

		for(int i = 0; i < setting.iterations; i++) {
			lastMesh = true;
			if(mapRenderer == null) mapRenderer = gameMesh.GetComponent<MeshRenderer>();
			if(mapFilter == null) mapFilter = gameMesh.GetComponent<MeshFilter>();
			meshRenderer.sharedMaterial.mainTexture = map.texture;

			GetGenerationType();

			if(!ValidMapSize()) return;

			var mapVerts = new Vector3[(setting.mapSize.x * setting.mapSize.y) / GetSimplification(setting)];
			Vector2[] uvs = new Vector2[mapVerts.Length];
			int[] tris = new int[mapVerts.Length - 2];
			if(setting.generationMethod != null) setting.generationMethod.CreateMesh(ref mapVerts, ref uvs, ref tris, ref map);

			if(setting.flatShading) Util.FlatShading(ref mapVerts, ref uvs, ref tris);
			mapFilter.sharedMesh.vertices = mapVerts;
			mapFilter.sharedMesh.uv = uvs;
			mapFilter.sharedMesh.triangles = tris;
			mapRenderer.sharedMaterial.mainTexture = map.texture;
			gameMesh.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
			gameMesh.GetComponent<MeshCollider>().sharedMesh = mapFilter.sharedMesh;
			
			mapFilter.sharedMesh.RecalculateNormals();

			CalculatePoints();
		}

		gamePlane.SetActive(false);
		gameMesh.SetActive(true);
	}

	protected void GetGenerationType() {
		if(setting == null) return;
		setting.generationMethod = null;
		switch(setting.generationType) {
			default:
			case SettingProfile.GenerationType.LANDSCAPE:
				setting.generationMethod = new LandscapeGen(setting);
				break;
			case SettingProfile.GenerationType.ISLAND:
				setting.generationMethod = new IslandGen(setting);
				break;
			case SettingProfile.GenerationType.CITY: ///UNUSED 
				break;
			case SettingProfile.GenerationType.PLATFORMS: ///UNUSED
				break;
		}
	}

	//Voronoi Neighbor calculation for minimap texture
	protected void CalculatePoints() {
		for(int i = 0; i < map.points.Length; i++) {
			try {
				var point = map.points[i];
				int pX = (int)point.x;
				int pY = (int)point.y;
				var c1 = map.texture.GetPixel(pX + 1, pY);
				var c2 = map.texture.GetPixel(pX - 1, pY);
				var c3 = map.texture.GetPixel(pX, pY + 1);
				var c4 = map.texture.GetPixel(pX, pY - 1);
				Color[] neighbors = {c1, c2, c3, c4};
				Vector2[] neighPos = {new Vector2(pX + 1, pY), new Vector2(pX - 1, pY), new Vector2(pX, pY + 1), new Vector2(pX, pY - 1)};

				Color darkest = c1;
				for(int l = 0; l < neighbors.Length; l++) if(darkest.r > neighbors[l].r) {
					darkest = neighbors[l];
					map.points[i] = neighPos[l];
				}
			} catch(System.IndexOutOfRangeException) {}
		}
	}

	//Gets the color distribution of the voronoi diagram of this world
	public MapLayer GetColorMap() {
		Random.InitState(setting.seed);
		MapLayer map = new MapLayer(setting);
		var colors = new Color[setting.mapSize.x * setting.mapSize.y];
		//Place random points
		for(int i = 0; i < setting.regionCount; i++) {
			float randX = Random.Range(0f, setting.mapSize.x);
			float randY = Random.Range(0f, setting.mapSize.y);
			map.points[i] = new Vector2(randX, randY);
		}
		
		Random.InitState(setting.textureSeed);
		for(int i = 0; i < colors.Length; i++) colors[i] = Random.ColorHSV(0.2f, 1f, 1f ,1f, 0.2f, 1f);
		
		//Coloring
		Color[] pixelCols = new Color[setting.mapSize.x * setting.mapSize.y];
		float[] distances = new float[setting.mapSize.x * setting.mapSize.y];
		for(int x = 0; x < setting.mapSize.x; x++) 
		for(int y = 0; y < setting.mapSize.y; y++) {
			int index = y * setting.mapSize.x + x;
			distances[index] = GetDistance(new Vector2(x, y), map.points[GetClosestAreaIndex(new Vector2(x, y), map)]);
		}
		//Apply
		float maxDst = GetMaxDistance(distances);
		for(int i = 0; i < distances.Length; i++) {
			float colVal = distances[i] / maxDst * setting.elevation;
			map.heightMap[i] = colVal;

			int x = i % setting.mapSize.x;
			int y = i / setting.mapSize.x;
			pixelCols[i] = colors[GetClosestAreaIndex(new Vector2(x, y), map)];
		}
		map.texture = Util.BlurImage(GetImageFromColorArray(pixelCols), setting.colorMapBlur);
		map.texture.Apply();
		return map;
	}	
	public MapLayer GetHeightMap() {
		Random.InitState(setting.seed);
		map = new MapLayer(setting);
		map.heightMap = new float[setting.mapSize.x * setting.mapSize.y];
		//Place random points
		for(int i = 0; i < setting.regionCount; i++) {
			float randX = Random.Range(0f, setting.mapSize.x);
			float randY = Random.Range(0f, setting.mapSize.y);
			map.points[i] = new Vector2(randX, randY);
		}
		//Coloring
		Color[] pixelCols = new Color[setting.mapSize.x * setting.mapSize.y];
		float[] distances = new float[setting.mapSize.x * setting.mapSize.y];
		for(int x = 0; x < setting.mapSize.x; x++) 
		for(int y = 0; y < setting.mapSize.y; y++) {
			int index = y * setting.mapSize.x + x;
			distances[index] = GetDistance(new Vector2(x, y), map.points[GetClosestAreaIndex(new Vector2(x, y), map)]);
		}
		//Apply
		float maxDst = GetMaxDistance(distances);
		for(int i = 0; i < distances.Length; i++) {
			float colVal = distances[i] / maxDst * setting.elevation;
			map.heightMap[i] = colVal;
			pixelCols[i] = new Color(colVal, colVal, colVal);
		}
		map.texture = GetImageFromColorArray(pixelCols);
		return map;
	}
	public MapLayer GetFalloffmap() {
		MapLayer map = GetHeightMap();
		Color[] pixelCols = map.texture.GetPixels();
		if(!setting.usingFalloff) {
			map.texture = GetImageFromColorArray(pixelCols);
			return map;
		}
		Vector2 center = new Vector2(setting.mapSize.x / 2, setting.mapSize.y / 2);
		for(int i = 0; i < pixelCols.Length; i++) {
			pixelCols[i] = Color.white;
			int x = i % setting.mapSize.x;
			int y = i / setting.mapSize.x;

			float dist = Vector2.Distance(center, new Vector2(x, y)) / setting.mapSize.x * setting.falloffMapIntensity;
			float val = Mathf.Lerp(0, 1, dist * setting.fallSpread);
			val -= setting.fallOffset;
			
			map.heightMap[i] += val;
		}
		map.texture = GetImageFromColorArray(pixelCols);
		return map;
	}

	protected float GetMaxDistance(float[] dist) {
		float maxDst = float.MinValue;
		for(int i = 0; i < dist.Length; i++) {
			if(dist[i] > maxDst) maxDst = dist[i];
		}
		return maxDst;
	}
	protected int GetClosestAreaIndex(Vector2 pos, MapLayer map) {
		float closest = float.MaxValue;
		int index = 0;
		if(map.points == null) return -1;
		for(int i = 0; i < map.points.Length; i++) {
			if(GetDistance(pos, map.points[i]) < closest) {
				closest = GetDistance(pos, map.points[i]);
				index = i;
			}
		}
		return index;
	}
	protected Texture2D GetImageFromColorArray(Color[] col) {
		Texture2D texture = new Texture2D(setting.mapSize.x, setting.mapSize.y);
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.SetPixels(col);
		texture.Apply();
		return texture;
	}

	public float GetDistance(Vector2 a, Vector2 b) {
		setting.usingMinkowski = setting.distanceMethod == SettingProfile.DistanceMethod.MINKOWSKI;
		switch(setting.distanceMethod) {
			default:
			case SettingProfile.DistanceMethod.VECTOR:
				return Vector2.Distance(a, b);
			case SettingProfile.DistanceMethod.MINKOWSKI:
				return Util.MinkowskiDistance(a, b, setting.minkowskiP);
			case SettingProfile.DistanceMethod.MANHATTAN:
				return Util.ManhattanDistance(a, b);
		}
	}

	void OnDrawGizmos() {
		if(!showPoints || map.points == null || Application.isPlaying || setting == null) return;
		Gizmos.color = Color.red;
		foreach(Vector2 vec in map.points) {
			int ind = (int)(vec.y * setting.mapSize.x + vec.x);
			if(ind > map.heightMap.Length - 1) continue;
			try {
				float fact = setting.heightMultiplier * transform.localScale.y * setting.elevation;
				if(!lastMesh) fact = 0;
				Gizmos.DrawSphere(new Vector3((vec.x - setting.mapSize.x / 2) *  transform.localScale.x, ((map.heightMap[ind]) * fact), (vec.y - setting.mapSize.y / 2) *  transform.localScale.z) / 10, pointSize);
			} catch(System.IndexOutOfRangeException) {}
		}
	}

	public static int GetSimplification(SettingProfile settings) {
		return (settings.simplification > 0) ? settings.simplification + 1 : 1;
	}

public struct MapData {
	public readonly float[,] heightMap;
	public readonly Color[] colorMap;

	public MapData(float[,] heightMap, Color[] colorMap = null) {
		this.heightMap = heightMap;
		this.colorMap = colorMap;
	}
}

public class MeshData {
	Vector3[] vertices;
	int[] triangles;
	Vector2[] uvs;
	Vector3[] bakedNormals;

	Vector3[] borderVertices;
	int[] borderTriangles;

	int triangleIndex;
	int borderIndex;

	bool flatShading;

	public MeshData(int verticesPerLine, bool flatShading) {
		this.flatShading = flatShading;

		vertices = new Vector3[verticesPerLine * verticesPerLine];
		triangles = new int[(verticesPerLine - 1) * (verticesPerLine - 1) * 6];
		uvs = new Vector2[verticesPerLine * verticesPerLine];

		borderVertices = new Vector3[verticesPerLine * 4 + 4];
		borderTriangles = new int[24 * verticesPerLine];
	}

	public void AddVertex(Vector3 vertexPos, Vector2 uv, int vertexIndex) {
		if(vertexIndex < 0) {
			borderVertices[-vertexIndex - 1] = vertexPos;

		} else {
			vertices[vertexIndex] = vertexPos;
			uvs[vertexIndex] = uv;
		}
	}

	public void AddTriangle(int a, int b, int c) {
		if(a < 0 || b < 0 || c < 0) {
			borderTriangles[borderIndex] = a;
			borderTriangles[borderIndex + 1] = b;
			borderTriangles[borderIndex + 2] = c;
			borderIndex += 3;
		} else {
			triangles[triangleIndex] = a;
			triangles[triangleIndex + 1] = b;
			triangles[triangleIndex + 2] = c;
			triangleIndex += 3;
		}
	}

	Vector3[] CalculateNormals() {
		Vector3[] vertexNormals = new Vector3[vertices.Length];
		int triangleCount = triangles.Length / 3;
		for(int i = 0; i < triangleCount; i++) {
			int normalTriangleIndex = i * 3;
			int vertexIndexA = triangles[normalTriangleIndex];
			int vertexIndexB = triangles[normalTriangleIndex + 1];
			int vertexIndexC = triangles[normalTriangleIndex + 2];

			Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
			vertexNormals[vertexIndexA] += triangleNormal;
			vertexNormals[vertexIndexB] += triangleNormal;
			vertexNormals[vertexIndexC] += triangleNormal;
		}

		int borderTriangleCount = borderTriangles.Length / 3;
		for(int i = 0; i < borderTriangleCount; i++) {
			int normalTriangleIndex = i * 3;
			int vertexIndexA = borderTriangles[normalTriangleIndex];
			int vertexIndexB = borderTriangles[normalTriangleIndex + 1];
			int vertexIndexC = borderTriangles[normalTriangleIndex + 2];

			Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
			if(vertexIndexA >= 0) vertexNormals[vertexIndexA] += triangleNormal;
			if(vertexIndexB >= 0) vertexNormals[vertexIndexB] += triangleNormal;
			if(vertexIndexC >= 0) vertexNormals[vertexIndexC] += triangleNormal;
		}

		for(int i = 0; i < vertexNormals.Length; i++) vertexNormals[i].Normalize();
		return vertexNormals;
	}

	Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC) {
		Vector3 pointA = (indexA < 0) ? borderVertices[-indexA - 1] : vertices[indexA];
		Vector3 pointB = (indexB < 0) ? borderVertices[-indexB - 1] : vertices[indexB];
		Vector3 pointC = (indexC < 0) ? borderVertices[-indexC - 1] : vertices[indexC];

		Vector3 sideAB = pointB - pointA;
		Vector3 sideAC = pointC - pointA;
		return Vector3.Cross(sideAB, sideAC).normalized;
	}

	public void Shade() {
		if(flatShading) FlatShading();
		else BakeNormals();
	}

	void BakeNormals() {
		bakedNormals = CalculateNormals();
	}

	void FlatShading() {
		Vector3[] flatShadedVertices = new Vector3[triangles.Length];
		Vector2[] flatShadedUvs = new Vector2[triangles.Length];

		for(int i = 0; i < triangles.Length; i++) {
			flatShadedVertices[i] = vertices[triangles[i]];
			flatShadedUvs[i] = uvs[triangles[i]];
			triangles[i] = i;
		}

		vertices = flatShadedVertices;
		uvs = flatShadedUvs;
	}

	public Mesh CreateMesh() {
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		if(flatShading) mesh.RecalculateNormals();
		else mesh.normals = bakedNormals;
		return mesh;
	}
}
}

public class LandscapeGen : GenerationMethod {
	private SettingProfile settings;

	public LandscapeGen(SettingProfile settings) {
		this.settings = settings;
	}

	public void CreateMesh(ref Vector3[] verts, ref Vector2[] uv, ref int[] tris, ref VoronoiGenerator.MapLayer map) {
		var curve = settings.curve;
		var mapSize = settings.mapSize;
		var heightMultiplier = settings.heightMultiplier;
		List<Vector3> vertsi = new List<Vector3>();
		List<int> trisi = new List<int>();
		for(int x = 0; x < mapSize.x; x++)
		for(int y = 0; y < mapSize.y; y++) {
			Vector3 center =  new Vector3(mapSize.x / 2, 0, mapSize.y / 2);
			map.heightMap[y * mapSize.x + x] = 1f - curve.Evaluate(map.heightMap[y * mapSize.x + x]);
			vertsi.Add(new Vector3(x, map.heightMap[y * mapSize.x + x] * heightMultiplier, y) - center);
			if(x == 0 || y == 0) continue;
			trisi.Add(mapSize.x * x + y);
			trisi.Add(mapSize.x * x + y - 1);
			trisi.Add(mapSize.x * (x - 1) + y - 1);
			trisi.Add(mapSize.x * (x - 1) + y - 1);
			trisi.Add(mapSize.x * (x - 1) + y);
			trisi.Add(mapSize.x * x + y);
		}
		var vertArr = vertsi.ToArray();
		Vector2[] uvs = new Vector2[vertArr.Length];
		for(var i = 0; i < uvs.Length; i++) {
			Vector2 size = new Vector2(mapSize.x, mapSize.y);
			var scaled = new Vector2(vertArr[i].x, vertArr[i].z);
			scaled += Vector2.one * (size / 2);
			uvs[i] = scaled / size;
		}
		verts = vertArr;
		uv = uvs;
		tris = trisi.ToArray();
	}

	public void CreateMap(ref float[] heightMap, ref Color[] colorMap, Vector2Int mapSize) {}
}

public class IslandGen : GenerationMethod {
	private SettingProfile settings;

	public IslandGen(SettingProfile settings) {
		this.settings = settings;
	}

	public void CreateMesh(ref Vector3[] verts, ref Vector2[] uv, ref int[] tris, ref VoronoiGenerator.MapLayer map) {
		var curve = settings.curve;
		var mapSize = settings.mapSize;
		var heightMultiplier = settings.heightMultiplier;
		List<Vector3> vertsi = new List<Vector3>();
		List<int> trisi = new List<int>();

		int simple = VoronoiGenerator.GetSimplification(settings);
		mapSize = new Vector2Int(settings.mapSize.x / simple, settings.mapSize.y / simple);
		for(int x = 0; x < mapSize.x; x++)
		for(int y = 0; y < mapSize.y; y++) {
			int xx = x * simple;
			int yy = y * simple;
			Vector3 center =  new Vector3(mapSize.x / 2, 0, mapSize.y / 2);
			map.heightMap[yy * settings.mapSize.x + xx] = 1f - curve.Evaluate(map.heightMap[yy * settings.mapSize.x + xx]);
			vertsi.Add(new Vector3(x * simple, map.heightMap[(yy * settings.mapSize.x + xx)] * heightMultiplier, y * simple) - center * simple);
			if(x == 0 || y == 0) continue;
			trisi.Add(mapSize.x * x + y);
			trisi.Add(mapSize.x * x + y - 1);
			trisi.Add(mapSize.x * (x - 1) + y - 1);
			trisi.Add(mapSize.x * (x - 1) + y - 1);
			trisi.Add(mapSize.x * (x - 1) + y);
			trisi.Add(mapSize.x * x + y);
		}
		var vertArr = vertsi.ToArray();
		Vector2[] uvs = new Vector2[vertArr.Length];
		for(var i = 0; i < uvs.Length; i++) {
			Vector2 size = new Vector2(mapSize.x, mapSize.y);
			var scaled = new Vector2(vertArr[i].x, vertArr[i].z) / simple;
			scaled += Vector2.one * (size / 2);
			uvs[i] = scaled / size;
		}
		verts = vertArr;
		uv = uvs;
		tris = trisi.ToArray();
	}

	public void CreateMap(ref float[] heightMap, ref Color[] colorMap, Vector2Int mapSize) {}
}

public interface GenerationMethod {
	void CreateMesh(ref Vector3[] v, ref Vector2[] uv, ref int[] i, ref VoronoiGenerator.MapLayer layer);
	void CreateMap(ref float[] heightMap, ref Color[] pixelCols, Vector2Int mapSize);
}