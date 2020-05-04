using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour {
	public GameObject bottom, map, iconPrefab;

	public Text title, titleShadow;

	private RawImage source;

	private bool show = true;

	public float rollDownSpeed = 1.5f;

	private Vector2 basePos;
	private Vector2 baseScale;

	private Vector2 baseBottomPos;
	private Vector2 baseBottomScale;

	public Camera playerCam;
	public float CamHeight = 2;
	private float BaseCam;

	private float turn = 0;

	private bool holdExtendView = false;
	private float holdExtendDelay = 0;

	public Tutorial extend, sprint;

	private float time = 0;

	void Start () {
		time = 0;
		BaseCam = CamHeight;
		basePos = transform.localPosition;
		baseScale = transform.localScale;
		baseBottomPos = bottom.transform.localPosition;
		baseBottomScale = bottom.transform.localScale;
	}

	public void LoadMinimap() {
		if(source == null) source = this.map.GetComponent<RawImage>();
		playerCam.enabled = true;
	}

	void Update () {
		if(playerCam != null) playerCam.orthographicSize = CamHeight;
		
		//Tutorial
		time += Time.deltaTime;
		if(time > 1 && !extend.finished && sprint.finished) TutorialManager.SetTutorial(extend, gameObject.transform.position + new Vector3(250, -100 , 0));

		if(RecipeSystem.IsShowing()) holdExtendView = false;

		if(Input.GetKey(KeyCode.M) && !RecipeSystem.IsShowing()) {
			holdExtendDelay += Time.deltaTime;
			holdExtendView = holdExtendDelay > 0.5f & show;
			if(holdExtendView) TutorialManager.FinishTutorial(extend);
		}
		if(Input.GetKeyUp(KeyCode.M) && !RecipeSystem.IsShowing()) {
			holdExtendDelay = 0;
			if(!holdExtendView) ToggleMap();
			holdExtendView = false;
		}

		title.text = "<color=" + (show ? "#dfdfa4" : "maroon") + ">M</color>ap";

		if(turn > 0) turn -= rollDownSpeed;
		else if(turn < 0) turn += rollDownSpeed;

		if(holdExtendView) {
			CamHeight = Mathf.Lerp(CamHeight, 70f, Time.deltaTime * 2f);
			transform.position = Vector3.Lerp(transform.position, new Vector3(Screen.width / 2f, Screen.height + 10 + GetWobble(), transform.localPosition.z), Time.deltaTime * 2f);
			transform.localScale = Vector3.Lerp(transform.localScale, 1.75f * new Vector3(baseScale.x + GetWobble(1, 0.02f) - 0.1f - (show ? -0.1f : 0.1f), baseScale.y + GetWobble(1, 0.02f) - 0.1f - (show ? -0.1f : 0.1f), 1), Time.deltaTime * 2f);
			transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, Mathf.LerpAngle(transform.localEulerAngles.z, turn, Time.deltaTime * 3.5f));
		} else {
			CamHeight = Mathf.Lerp(CamHeight, BaseCam, Time.deltaTime * 2f);
			transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(basePos.x, basePos.y + GetWobble(), transform.localPosition.z), Time.deltaTime * 2f);
			transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(baseScale.x + GetWobble(1, 0.02f) - 0.1f - (show ? -0.1f : 0.1f), baseScale.y + GetWobble(1, 0.02f) - 0.1f - (show ? -0.1f : 0.1f), 1), Time.deltaTime * 2f);
			transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, Mathf.LerpAngle(transform.localEulerAngles.z, turn, Time.deltaTime * 3.5f));
		}

		Vector3 targetScale, targetPos;
		if(show) {
			targetPos = baseBottomPos;
			targetScale = baseBottomScale;
		} else {
			targetPos = new Vector3(0, -160, 0);
			targetScale = new Vector3(1, 0.25f, 1);
		}
		bottom.transform.localPosition = Vector3.Lerp(bottom.transform.localPosition, targetPos, Time.deltaTime * 3);
		bottom.transform.localScale = Vector3.Lerp(bottom.transform.localScale, targetScale, Time.deltaTime * 5);
	}

	public void ToggleMap() {
		turn = 45 * (show ? -1 : 1);
		show = !show;
		SoundManager.PLAY_SOUND("Scroll", 0.8f, show ? 1.4f : 1.2f);
	}

	public void OpenMap(bool i) {
		show = i;
	}

	private float GetWobble(float speed = 2, float amp = 3) {
		return Mathf.Sin(Time.time * speed) * amp;
	}
}
