using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialNote : MonoBehaviour {
	public RawImage icon1, icon2, tutorialShield, okSign, checkMark;
	public Text topText, middleText, bottomText;

	public Texture locked, filled;

	private Tutorial current;

	private bool finishAnim = false;
	private float fadeTick = 0;
	private bool show = false;

	public void SetNote(Tutorial tutorial) {
		if(current != null && current.name == tutorial.name) return;
		current = tutorial;
		gameObject.SetActive(true);
		show = true;
		transform.gameObject.SetActive(true);
		icon1.texture = tutorial.icon1;
		icon2.texture = tutorial.icon2;
		topText.text = tutorial.topText;
		middleText.text = tutorial.middleText;
		bottomText.text = tutorial.bottomText;
		transform.parent.localPosition = new Vector3(0, 0, 0);
		transform.parent.localScale = Vector3.zero;
		fadeTick = 0;
		finishAnim = false;
	}

	void LateUpdate() {
		if(current != null && current.finished) {
			tutorialShield.texture = filled;
			checkMark.enabled = okSign.enabled = true;
		}
		else {
			tutorialShield.texture = locked;
			checkMark.enabled = okSign.enabled = false;
		}

		if(current != null && current.finished && !finishAnim) {
			transform.parent.localScale = Vector3.one * 1.2f;
			finishAnim = true;
		}

		if(show) {
			if(finishAnim && fadeTick > 2) transform.parent.localScale = Vector3.Lerp(transform.parent.localScale, Vector3.zero, Time.deltaTime * 2f);
			else transform.parent.localScale = Vector3.Lerp(transform.parent.localScale, Vector3.one, Time.deltaTime * 2f);
		}

		if(fadeTick > 0) fadeTick += Time.deltaTime;
		if(fadeTick > 3.5f) {
			current = null;
			fadeTick = 0;
			finishAnim = false;
			show = false;
		}
	}
	
	public void TriggerFinish() {
		fadeTick = 0.1f;
	}
}
