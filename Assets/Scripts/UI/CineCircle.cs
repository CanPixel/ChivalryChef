using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//These are pop-up circles on the UI to notify the player of certain events that are near him
public class CineCircle : MonoBehaviour {
	public GameObject cameraPrefab;
	public Material cineMask;

	public const float margin = 8;
	public const float hideDistance = 10;
	
	private GameObject cineCam;
	[HideInInspector]
	public GameObject focalPoint;
	private GameObject content, arrow;

	public float rotateSpeed = 20;

	public float refreshTime = 10;
	private float lifetime;
	private float zoom, initialZoom;

	private static CineCircle self;

	protected Vector3 screenPos;
	protected Vector2 screenOffset;
	protected Text text;

	public float heightOffset = 22f;

	//The types of events that invoke a CineCam UI element
	public static EventType ATTACK = new EventType(new BorderColor("red"), "Clash!"),
													STOLENLOOT = new EventType(new BorderColor(""), "Loot snatched!");

	public class EventType {
		public BorderColor color;
		public string name;

		public EventType(BorderColor color, string name) {
			this.color = color;
			this.name = name;
		}
	}

	[System.Serializable]
	public class BorderColor {
		public string name;
		public Color color;

		public BorderColor(string name) {
			this.name = name;
		}
	}
	public BorderColor[] colors;

	private RawImage border;
	private Image arrowIMG;

	//Set starting values of a new CineCam
	public void Init(RenderTexture texture, GameObject focal, float offset, CineCircleManager.CineEventValues values) {
		content = transform.Find("Content").gameObject;
		arrow = transform.Find("Arrow").gameObject;
		text = GetComponentInChildren<Text>();
		screenPos = transform.localPosition;
		transform.localScale = Vector3.zero;
		var eventType = values.eventType;
		
		border = GetComponent<RawImage>();
		arrowIMG = arrow.GetComponent<Image>();

		focalPoint = focal;
		zoom = initialZoom = offset;
		cineCam = Instantiate(cameraPrefab);
		cineCam.GetComponent<Camera>().targetTexture = texture;
		content.GetComponent<RawImage>().texture = texture;
		cineCam.transform.SetParent(focal.transform);
		cineCam.transform.position = focalPoint.transform.position - new Vector3(0, -heightOffset, zoom);
		screenOffset = new Vector2(1, 0.2f) * (CineCircleManager.GetNotifyCount() - 1) * 120f;
		
		CineCircleManager.AddNotifyList(gameObject, values);

		if(eventType.name == "RAND") SetBorderColor(colors[Random.Range(0, colors.Length)].name);
		else SetBorderColor(eventType.color.name);

		text.text = eventType.name;
		var sample = eventType.color.color;
		text.color = sample;
	}

	protected void HideCircle(bool i) {
		border.enabled = !i;
		arrow.SetActive(!i);
		content.SetActive(!i);
		text.enabled = !i;
	}

	void Update () {
		lifetime += Time.deltaTime;
		if(focalPoint == null) {
			DestroyCircle();
			return;
		}

		//Visibility
		var dist = Vector3.Distance(Camera.main.transform.position, focalPoint.transform.position);
		if(dist < hideDistance) HideCircle(true);
		else HideCircle(false);
		dist = Mathf.Clamp(dist, -50, 50);

		var focalOnScreen = Camera.main.WorldToScreenPoint(focalPoint.transform.position + Vector3.up * 7);
		//Onscreen
		if(focalOnScreen.z < 0.001f) {
			focalOnScreen.y = (Screen.height - focalOnScreen.y) * 0.1f;
			focalOnScreen.x = Screen.width - focalOnScreen.x;
		}
		focalOnScreen.x = Mathf.Clamp(focalOnScreen.x, Screen.width / margin, Screen.width / margin * (margin - 1));
		focalOnScreen.y = Mathf.Clamp(focalOnScreen.y, Screen.height / margin, Screen.height / margin * (margin - 1));
		screenPos = focalOnScreen;

		//Arrow
		var viewPos = Camera.main.WorldToViewportPoint(focalPoint.transform.position);
		viewPos.x -= 0.5f;
		viewPos.y -= 0.5f;
		viewPos.z = 0;
		float fAngle = -Mathf.Atan2(viewPos.x, viewPos.y) * Mathf.Rad2Deg;
		if(focalOnScreen.z >= 0.001f) fAngle = 180;
		else fAngle -= 180;
		arrow.transform.localEulerAngles = new Vector3(0f, 0f, fAngle);

		float rotRange = 7.5f;
		transform.localRotation = Quaternion.Euler(Mathf.Sin(Time.time * 2) * rotRange, Mathf.Cos(Time.time * 2) * rotRange, Mathf.Sin(1f - Time.time) * rotRange / 2f);

		//Follow focal target
		zoom = Mathf.Lerp(zoom, initialZoom + 10, Time.deltaTime * 2);
		cineCam.transform.position = Vector3.Lerp(cineCam.transform.position, focalPoint.transform.position - new Vector3(0, -heightOffset, zoom), Time.deltaTime * 2);
		cineCam.transform.LookAt(focalPoint.transform, Vector3.up);
		
		float scal = Mathf.Clamp((Mathf.Sin(Time.time * 3) + 1f) / 100f, 0, 0.5f);
		if(lifetime > refreshTime) {
			transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * 8f);
			if(lifetime > refreshTime * 1.25f) DestroyCircle();
		} else {
			float distOffs = 0;
			var normDist =  Mathf.Clamp(dist / 80f, 0, 1f);
			if(focalOnScreen.z < 0.001f) normDist = 0.8f;
			else distOffs += normDist * 100f;

			var targetScale = (new Vector3(1f + scal, 1f + scal, 1f)) * (normDist);
			transform.localScale = Vector3.Lerp(transform.localScale, targetScale * (Player.host.isZoom ? 0.1f : 1), Time.deltaTime * 4.5f);
			transform.position = new Vector2(screenPos.x, screenPos.y + Mathf.Sin(Time.time * 2f) * 5f + distOffs + normDist * 10f);
		} 
		transform.position += new Vector3(screenOffset.x, screenOffset.y, 0);
	}

	protected void DestroyCircle() {
		CineCircleManager.RemoveNotifyFromList(gameObject);
		CineCircleManager.cineCamCount--;
		Destroy(cineCam);
		Destroy(gameObject);
	}

	protected void SetBorderColor(string coli) {
		for(int i = 0; i < colors.Length; i++) {
			string col = coli.ToLower();
			if(colors[i].name.ToLower() == col) {
				arrowIMG.color = new Color(colors[i].color.r, colors[i].color.g, colors[i].color.b, 0.15f);
				border.color = colors[i].color;
				return;
			}
		}
	}

	public void SetAlpha(float i) {
		var arr = arrow.GetComponent<Image>();
		var cont = content.GetComponent<RawImage>();
		border.color = new Color(border.color.r, border.color.g, border.color.b, i);
		arr.color = new Color(arr.color.r, arr.color.g, arr.color.b, i);
		text.color = new Color(text.color.r, text.color.g, text.color.b, i);
		cont.color = new Color(cont.color.r, cont.color.g, cont.color.b, i);
	}
	public void LerpAlpha(float i, float time) {
		var arr = arrow.GetComponent<Image>();
		var cont = content.GetComponent<RawImage>();
		float currentA = border.color.a;
		float lerp = Mathf.Lerp(currentA, i, Time.deltaTime * time);
		border.color = new Color(border.color.r, border.color.g, border.color.b, lerp);
		arr.color = new Color(arr.color.r, arr.color.g, arr.color.b, lerp);
		text.color = new Color(text.color.r, text.color.g, text.color.b, lerp);
		cont.color = new Color(cont.color.r, cont.color.g, cont.color.b, lerp);
	}
}
