using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
	public string CURRENT_VERSION = "";

	public Dropdown tutorialDropdown;
	public Text version;
	public GameSettings settings;

	void OnValidate() {
		version.text = CURRENT_VERSION;
	}

	void Start () {
		tutorialDropdown.value = settings.enableTutorial ? 0 : 1;
		Application.runInBackground = true;
	}

	public void LoadWorld(string world) {
		StartCoroutine(LoadAsync(world));
	}

	IEnumerator LoadAsync(string world) {
		yield return new WaitForSeconds(1f);
		yield return SceneManager.LoadSceneAsync(world);

		DontDestroyOnLoad(gameObject);
		yield return SceneManager.UnloadSceneAsync(0);
		Destroy(gameObject);
	}

	public void SetTutorialSetting() {
		settings.enableTutorial = tutorialDropdown.value == 0 ? true : false;
	}

	public void QuitGame() {
		Application.Quit();
	}
}