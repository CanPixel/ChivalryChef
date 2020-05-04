using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlayerManager : MonoBehaviour {
	public Material[] knightQualityMaterials;

	public List<GameObject> players = new List<GameObject>();

	private int prevLevel;

	private static PlayerManager self;

	void OnValidate() {
		UpdateKnightMaterial();
	}

	void Start () {
		self = this;
		UpdateKnightMaterial();
	}

	public void Begin() {
		Start();
	}

	//For certain graphics settings other materials are assigned to all knight characters
	private void UpdateKnightMaterial() {
		players.Clear();
		var list = GameObject.FindGameObjectsWithTag("Knight");
		foreach(var knight in list) players.Add(knight);
		
		int level = QualitySettings.GetQualityLevel();
		foreach(var knight in players) {
			var render = knight.GetComponentInChildren<SkinnedMeshRenderer>();
			var mats = new Material[]{knightQualityMaterials[level]};
			render.materials = mats;
		}

		prevLevel = level;
	}
	
	void Update () {
		if(prevLevel != QualitySettings.GetQualityLevel()) UpdateKnightMaterial();
	}

	public static List<GameObject> GetPlayers() {
		return self.players;
	}

	public static GameObject GetMainPlayer() {
		foreach(var knight in self.players) if(knight.GetComponent<KnightMovement>().GetType() == typeof(Player)) return knight;
		return null;
	}

	//Gets all the AI knight characters
	public static List<GameObject> GetAiKnights() {
		var newList = new List<GameObject>();
		foreach(var knight in self.players) if(knight.GetComponent<KnightMovement>().GetType() != typeof(Player)) newList.Add(knight);
		return newList;
	}
}
