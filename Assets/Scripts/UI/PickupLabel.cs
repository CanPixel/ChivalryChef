using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupLabel : MonoBehaviour {
	private Vector2 basePos;

	public Text text, shadow;
	private GameObject obj;

	public GameObject[] consume;
	public Text consumeButton;
	private Color consumeButtonCol = new Color(1, 0.958f, 0.863f);

	void Update () {
		if(obj != null) basePos = Camera.main.WorldToScreenPoint(obj.transform.position + Vector3.up * 2);
		transform.position = new Vector3(basePos.x, basePos.y + Mathf.Sin(Time.time * 5) * 10, transform.position.z);
	}

	public void SetLabel(Ingredient.FoodValues values, GameObject obj, bool health) {
		this.obj = obj;
		shadow.text = "<color=#" + ColorUtility.ToHtmlStringRGB(values.color) +">" + values.ingredientName + "</color>";
		text.text = values.ingredientName;
		foreach(var i in consume) i.SetActive(true);
		consumeButton.color = (health) ? consumeButtonCol : Color.black;
		gameObject.SetActive(true);
	}

	public void SetLabel(KnightHead values, GameObject obj) {
		this.obj = obj;
		shadow.text = "Loot <color=#ffffff>" + values.playerName + "</color>";
		text.text = "Loot " + values.playerName;
		foreach(var i in consume) i.SetActive(false);
		gameObject.SetActive(true);
	}

	public void HideLabel() {
		gameObject.SetActive(false);
	}

	public GameObject GetObject() {
		return obj;
	}
}
