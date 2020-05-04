using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OvenDial : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler {
	private Image image;

	private HeaterCook cooker;
	public Player playerKnight;

	public GameObject indicatorDial;

	private float startAngle = 0;
	private Quaternion newRot;

	private GameObject dialTip;

	[System.Serializable]
	public class RotationClamp {
		[HideInInspector]
		public string name;
		public bool clamp;
		public Vector2 exclusionRange;
	}
	public RotationClamp[] clamps = new RotationClamp[3];
	void OnValidate() {
		for(int i = 0; i < clamps.Length; i++) {
			switch(i) {
				case 0:
					clamps[i].name = "X";
					break;
				case 1:
					clamps[i].name = "Y";
					break;
				default:
				case 2:
					clamps[i].name = "Z";
					break;
			}
		}
	}

	[System.Serializable]
	public class HeatClass {
		public int heatLevel;
		public Collider2D collider;
		private Image image;
		private bool selected = false;

		public void Init() {
			image = collider.GetComponent<Image>();
		}

		public void Tick(OvenDial dial, Vector3 point) {
			if(collider != null) {
				if(collider.OverlapPoint(point)) {
					dial.currentHeat = heatLevel;
					image.color = Color.Lerp(image.color, new Color(0.7f, 0.7f, 0.7f), Time.deltaTime * 4f);
					if(!selected) {
						collider.transform.SetAsLastSibling();
						collider.transform.SetSiblingIndex(collider.transform.GetSiblingIndex() - 1);
					}
					selected = true;
				}
				else {
					image.color = Color.Lerp(image.color, new Color(1f, 1f, 1f), Time.deltaTime * 4f);
					selected = false;
				}
			}
		}
	}
	public HeatClass[] heatClasses;
	public int currentHeat = 0;

	private bool isHovering = false, isDragging = false;

	void Start () {
		cooker = playerKnight.GetComponent<HeaterCook>();
		image = GetComponent<Image>();
		dialTip = transform.GetChild(0).gameObject;
		foreach(var i in heatClasses) i.Init();
	}

	public void InputDown() {
		var screenPos = transform.position;
		var vector = Input.mousePosition - screenPos;
		startAngle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
		startAngle -= Mathf.Atan2(transform.right.y, transform.right.x) * Mathf.Rad2Deg;

		SoundManager.PLAY_SOUND("ovenSelect", 1f, 1.8f);
	}

	//Holding Left Mouse / Dragging
	public void InputHeld() {
		if(playerKnight.GetInventory().IsDraggingItem || GameMenu.MenuOn) return;

		if(Input.GetMouseButtonDown(0)) InputDown();
		var screenPos = transform.position;
		var vector = Input.mousePosition - screenPos;
		float angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
		newRot = Quaternion.AngleAxis(angle - startAngle, transform.forward);
		
		foreach(var clamp in clamps) {
			if(clamp.clamp) {
				switch(clamp.name) {
					default:
					case "X":
						break;
					case "Y":
						break;
					case "Z":
						var value = newRot.eulerAngles.z;
						if(value > clamp.exclusionRange.x && value < clamp.exclusionRange.y) value = clamp.exclusionRange.x;
						else if(value < clamp.exclusionRange.y && value > clamp.exclusionRange.x) value = clamp.exclusionRange.y;
						newRot.eulerAngles = new Vector3(newRot.eulerAngles.x, newRot.eulerAngles.y, value);
						break;
				}
			}
		}
	}

	public void OnPointerExit(PointerEventData data) {
		if(playerKnight.GetInventory().IsDraggingItem || GameMenu.MenuOn) return;
		if(isHovering) SoundManager.PLAY_SOUND("ovenSelect", 0.8f, 0.8f);
		isHovering = false;
		(playerKnight.GetInventory() as InventoryBoard).SetCursorClick(false);
	}
	public void OnPointerEnter(PointerEventData data) {
		if(playerKnight.GetInventory().IsDraggingItem || GameMenu.MenuOn) return;
		(playerKnight.GetInventory() as InventoryBoard).SetCursorClick(true);
		isHovering = true;
		SoundManager.PLAY_SOUND("ovenSelect", 0.8f, 1.35f);
	}
	
	void Update() {
		indicatorDial.transform.rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z);

		foreach(var i in heatClasses) i.Tick(this, dialTip.transform.position);

		transform.localScale = Vector3.Lerp(transform.localScale, (isDragging) ? Vector3.one * 1f : Vector3.one * 0.85f, Time.deltaTime * 6f);

		if(isHovering || isDragging) {
			if(Input.GetMouseButton(0)) {
				isDragging = true;
				Colorize(0.7f, 8f);
				InputHeld();
			} else {
				Colorize(0.45f, 6f);
				isDragging = false;
			}
		} else {
			Decolorize();
			isDragging = false;
		}
		transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime * 3f);

		if(cooker.heater.heatLevel != currentHeat) SoundManager.PLAY_SOUND("ovenClick", 1, 1f + (currentHeat / 10f));
		cooker.heater.heatLevel = currentHeat;
	}

	//Coloring effect on mouse hover
	private void Colorize(float i, float speed = 4f) {
		image.color = Color.Lerp(image.color, new Color(i, i, i), Time.deltaTime * speed);
	}

	//When not hovering over the dial with the mouse
	private void Decolorize() {
		image.color = Color.Lerp(image.color, Color.black, Time.deltaTime * 4f);
	}
}
