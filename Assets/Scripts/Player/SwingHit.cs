using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwingHit : MonoBehaviour {
	public KnightMovement host;

	private float cutDelay = 0;

	void OnTriggerEnter(Collider col) {
		if(col.tag == "Knight" && host.meleeAction != KnightMovement.MELEE_ACTION.NONE && col.gameObject != host.gameObject) {
			var enemy = col.GetComponent<KnightMovement>();
			host.AttackKnight(enemy);
			enemy.TriggerBattle(host);
			host.TriggerBattle(enemy);
			SoundManager.PLAY_UNIQUE_SOUND_AT("knightHit", transform.position, 1f, 0.3f);
			SoundManager.PLAY_UNIQUE_SOUND_AT("SLASH", transform.position, 0.3f, 0.5f, 0.85f);
		}
		if(col.gameObject.tag == "Food" && host.meleeAction != KnightMovement.MELEE_ACTION.NONE) {
			TutorialManager.FinishTutorial("BattleHack");
			CutObject(col);
		}
	}

	void Update() {
		if(cutDelay >= 0) cutDelay -= Time.deltaTime;
	}

	protected void CutObject(Collider coll) {
		if(cutDelay > 0) return;
		var ing = coll.gameObject.GetComponentInParent<Ingredient>();
		if(ing.cutStage > 3) return;
		cutDelay = 3;
		ing.cutStage++;
		SpawnCutText(ing);

		SoundManager.PLAY_UNIQUE_SOUND_AT("ingredientHit", transform.position, 22f, 0.2f);
		SoundManager.PLAY_UNIQUE_SOUND_AT("SLASH", transform.position, 0.7f, 0.2f, 1f);

		var obj = Instantiate(Resources.Load("CHOMP") as GameObject);
		obj.GetComponent<ParticleSystemRenderer>().material.color = ing.values.cutColor;
		obj.transform.position = coll.transform.position;

		GameObject victim = coll.gameObject;
		victim.GetComponent<MeshCollider>().isTrigger = false;
		victim.GetComponent<Rigidbody>().useGravity = false;

		victim = coll.transform.GetComponent<MeshFilter>().gameObject;
		GameObject[] pieces = BLINDED_AM_ME.MeshCut.Cut(victim, host.transform.position, host.transform.right, ing.values.cutMaterial);

		foreach(var i in pieces) {
			i.transform.position += Vector3.up * 1;
			var rigid = i.GetComponent<Rigidbody>();
			if(!rigid) {
				var mC = i.AddComponent<MeshCollider>();
				mC.convex = true;
				rigid = i.AddComponent<Rigidbody>();
				mC.sharedMesh = i.GetComponent<MeshFilter>().mesh;
			}
			rigid.useGravity = true;
			rigid.AddForce(Vector3.up * 100);
		}
	}

	protected void SpawnCutText(Ingredient ing) {
		if(host.GetType() != typeof(Player)) return;
		var inv = (host.GetInventory() as InventoryBoard);
		var popup = Instantiate(inv.slashTextPrefab);
		popup.transform.SetParent(inv.canvasElement.transform.parent);
		popup.transform.SetSiblingIndex(popup.transform.parent.childCount - 2);
		popup.transform.localPosition = Vector3.zero;
		popup.name = "Slash Popup Text";
		popup.GetComponent<TrackObjectOnScreen>().Init(ing.gameObject);
		popup.transform.localScale = Vector3.one;
		
		var subText = popup.transform.GetChild(0).GetComponent<Text>();
		subText.text = "/" + ing.cutStage;
	}
}
