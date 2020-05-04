using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour {
	public GameObject loadingScreen; 

	private UILoad loadScreen;

	public void Initiate() {
		loadingScreen.SetActive(true);
		loadScreen = loadingScreen.GetComponent<UILoad>();
		load = true;
	}

	public bool IsLoading {
		get {return load;}
	}
	private bool load = false; 

	public void Finish() {
		load = false;
		loadingScreen.SetActive(false);
		loadScreen.Reset();
	}

	public void SetProgress(string txt = "", bool initTasks = false) {
		loadScreen.SetProgress(txt, initTasks);
	}
}
