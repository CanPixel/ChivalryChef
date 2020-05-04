using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour {
	public TutorialNote note;
	public GameSettings settings;

	private static TutorialManager self; 

	public Tutorial[] tutorials;
	public bool resetOnPlay = true;

	[HideInInspector]
	public Tutorial currentTutorial;

	private float tutorialDelay = 0, finishDelay = 0;

	void Start() {
		self = this;
		note.gameObject.SetActive(false);

		if(resetOnPlay && self.settings.enableTutorial) foreach(var i in tutorials) i.finished = false;
	}

	void LateUpdate() {
		if(tutorialDelay > 0) tutorialDelay -= Time.deltaTime;
		if(finishDelay > 0) finishDelay -= Time.deltaTime;
	}

	public static void SetTutorial(Tutorial tutorial, Vector2 obj) {
		if(tutorial.finished || self.finishDelay > 0 || !self.settings.enableTutorial || self.tutorialDelay > 0 || (self.currentTutorial != null && self.currentTutorial.name.ToLower() == tutorial.name.ToLower())) return;
		self.currentTutorial = tutorial;
		self.note.SetNote(tutorial);
		self.tutorialDelay = 1f;
		SoundManager.PLAY_SOUND("musicalhit", 2f, 1.1f);
		SoundManager.PLAY_SOUND("Scroll", 1f, 0.9f);
	}
	public static bool SetTutorial(string name) {
		if(!self.settings.enableTutorial || self.finishDelay > 0 || self.currentTutorial != null || self.tutorialDelay > 0) return false;
		Tutorial tutorial = null;
		foreach(var tut in self.tutorials) {
			if(tut.tutorialName.ToLower().Trim() == name.ToLower().Trim()) {
				tutorial = tut;
				break;
			}
		}
		if(tutorial == null || tutorial.finished) return false;
		if(self.currentTutorial == null || (self.currentTutorial.tutorialName.ToLower().Trim() != tutorial.tutorialName.ToLower().Trim())) {
			SoundManager.PLAY_SOUND("musicalhit", 2f, 1.1f);
			SoundManager.PLAY_SOUND("Scroll", 1f, 0.9f);
			self.currentTutorial = tutorial;
		}
		self.tutorialDelay = 1f;
		self.note.SetNote(tutorial);
		return tutorial.finished;
	}

	public static void FinishTutorial(string name) {
		if(!self.settings.enableTutorial || self.currentTutorial == null) return;
		Tutorial tutorial = null;
		foreach(var tut in self.tutorials) {
			if(tut.tutorialName.ToLower().Trim() == name.ToLower().Trim()) {
				if(tut.name.ToLower().Trim() != self.currentTutorial.name.ToLower().Trim()) return;
				tutorial = tut;
				break;
			}
		}
		FinishTutorial(tutorial);
	}

	public static void FinishTutorial(Tutorial tutorial) {
		if(!tutorial.finished && (self.currentTutorial != null && self.currentTutorial.tutorialName.ToLower().Trim() == tutorial.tutorialName.ToLower().Trim())) {
			SoundManager.PLAY_SOUND("musicalhit", 1.25f, 1);
			SoundManager.PLAY_SOUND("fantastic", 1f, 1);
			SoundManager.PLAY_SOUND("triumph", 1, 0.8f);
			self.note.TriggerFinish();
			self.finishDelay = 1.5f;
		}
		tutorial.finished = true;
		self.currentTutorial = null;
	}
}
