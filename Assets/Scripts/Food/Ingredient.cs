using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour {
	public GameObject iconPrefab;
	public Camera iconCamera;

	[System.Serializable]
	public class FoodValues {
		public string ingredientName;
		public Color color, cutColor;
		public int tier;
		public Vector2 sizeFactor = Vector2.one;

		[HideInInspector]
		public Material cutMaterial;
	}
	public FoodValues values;
	public KnightMovement owner;

	[Range(0, 1)]
	public float cookingResistance; //TODO: IMPLEMENT COOK RESISTANCE
 
	[HideInInspector]
	public float cookedness = 0;

	//The cutting stage of the ingredient [0-3]
	[HideInInspector] public int cutStage = 0;

	public bool alternativeUVMap = false;
	[ConditionalHide("alternativeUVMap", 1)]
	public Texture uvMap;

	private MeshRenderer[] foodParts;

	void OnValidate() {
		values.sizeFactor = new Vector2(Mathf.RoundToInt(values.sizeFactor.x), Mathf.RoundToInt(values.sizeFactor.y));
	}

	void Start() {
		if(alternativeUVMap) transform.GetComponentInChildren<MeshRenderer>().material.SetTexture("_Diffuse", uvMap);
		values.cutMaterial = new Material(Shader.Find("Diffuse"));
		values.cutMaterial.color = values.cutColor;

		if(iconPrefab != null) {
			var icon = Instantiate(iconPrefab);
			icon.name = iconPrefab.name;
			icon.transform.SetParent(transform);
			icon.transform.localPosition = Vector3.zero;
			var sprite = icon.GetComponent<SpriteRenderer>();
			var col = GetComponent<Ingredient>().values.color;
			sprite.color = new Color(col.r, col.g, col.b, 1);
		} 
	}

	public void Cook(float heat, float level) {
		foodParts = GetComponentsInChildren<MeshRenderer>();

		cookedness += (level * heat / 100f) * Time.deltaTime;
		if(cookedness > 100) cookedness = 100;
		if(cookedness > 50) {
			foreach(var i in foodParts) {
				foreach(var mat in i.materials) mat.color = Color.Lerp(mat.color, Color.black, Time.deltaTime * (cookedness / 200f));
			}
		}
	}
}
