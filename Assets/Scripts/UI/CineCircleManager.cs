using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//SINGLETON to locally manage all instances of CineCams on player's UI 
public class CineCircleManager : MonoBehaviour {
	public GameObject cineCirclePrefab;
	private static CineCircleManager self;

	public GameObject screenshotNotePrefab;

	public const int cineCamMax = 3;
	public static int cineCamCount = 0;

	private List<GameObject> cineCircleNotifies = new List<GameObject>();
	private Dictionary<GameObject, CineEventValues> cineCircleValues = new Dictionary<GameObject, CineEventValues>();

	public class CineEventValues {
		public GameObject[] obj;
		public CineCircle.EventType eventType; 
		public GameObject focalPoint;

		public CineEventValues SetValues(GameObject[] obj, CineCircle.EventType eventType) {
			this.focalPoint = obj[0];
			this.obj = obj;
			this.eventType = eventType;
			return this;
		}
	}

	private static float markerDelay = 0;

	void Start () {
		self = this;
	}
	
	void Update () {
		if(Player.host == null) return;

		if(Input.GetKeyDown(KeyCode.F10)) Util.CreateScreenCap(this, NotifyScreencap);

		if(markerDelay > 0) markerDelay -= Time.deltaTime;
		if(cineCamCount < 0) cineCamCount = 0;

		//Order active cam pop-ups by how close the event is to the player 
		cineCircleNotifies = cineCircleNotifies.OrderBy(x => Vector3.Distance(Player.host.transform.position, x.transform.position)).ToList();
		for(int i = 0; i < cineCircleNotifies.Count; i++) cineCircleNotifies[i].transform.SetSiblingIndex(i);

		foreach(var i in cineCircleNotifies) {
			var circ = i.GetComponent<CineCircle>(); 
			circ.LerpAlpha((Player.IsInventory) ? 0.15f : 0.8f, 2);
		}
	}

	protected void NotifyScreencap() {
		SoundManager.PLAY_SOUND("PLOEP");
		var obj = Instantiate(screenshotNotePrefab);
		obj.transform.SetParent(GameObject.FindWithTag("Canvas").transform, false);
	}

	//Spawns a new CineCam popup
	public static void NotifyCinematic(float zoom, CineCircle.EventType eventType, params GameObject[] focalObjects) {
		if(cineCamCount >= cineCamMax || markerDelay > 0 || self == null || Player.host == null) return;
		if(!Player.host.playerPrefs.playerSettings.showMarkers) return;
		var focalPoint = focalObjects[0];

		if(eventType == CineCircle.ATTACK) {
			foreach(KeyValuePair<GameObject, CineEventValues> value in self.cineCircleValues) {
				if(value.Value.focalPoint == focalObjects[0]) return;
			}
		}

		var cineCam = Instantiate(self.cineCirclePrefab, new Vector2(Screen.width / 2, Screen.height / 3), Quaternion.identity);
		cineCam.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform);
		cineCam.transform.SetSiblingIndex(0);

		var texture = new RenderTexture(512, 512, 16, RenderTextureFormat.ARGB32);
		var cineCircle = cineCam.GetComponent<CineCircle>();
		cineCircle.Init(texture, focalPoint, zoom, new CineEventValues().SetValues(focalObjects, eventType));

		cineCamCount++;
		markerDelay = 1;
	}

	public static void AddNotifyList(GameObject obj, CineEventValues eventValues) {
		self.cineCircleNotifies.Add(obj);
		self.cineCircleValues.Add(obj, eventValues);
	}
	public static void RemoveNotifyFromList(GameObject obj) {
		self.cineCircleNotifies.Remove(obj);
		self.cineCircleValues.Remove(obj);
	}
	public static int GetNotifyCount() {
		return self.cineCircleNotifies.Count;
	}
}
