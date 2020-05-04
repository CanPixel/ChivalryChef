using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour {
	public Player player;
	public ScrollButton exitButton;

	public bool OnMainMenu = false;

	public GameSettings settingFile;

	private bool showMenu = false;
	public static bool MenuOn = false;

	public object lastValue;

	public GameSetting[] settings;

	public void Toggle() {
		showMenu = !showMenu;
		if(!showMenu) SoundManager.PLAY_SOUND("Scroll", 1, 1.5f);
		for(int i = 0; i < transform.childCount; i++) transform.GetChild(i).gameObject.SetActive(showMenu);
		MenuOn = showMenu;
	}

	public void HideMenu() {
		showMenu = MenuOn = false;
		for(int i = 0; i < transform.childCount; i++) transform.GetChild(i).gameObject.SetActive(showMenu);
	}

	void Start () {
		for(int i = 0; i < transform.childCount; i++) transform.GetChild(i).gameObject.SetActive(showMenu);
		ApplyBaseSettings();
	}
	
	void Update () {
		if(OnMainMenu) return;
		if(Input.GetKeyDown(KeyCode.Escape) && !RecipeSystem.IsShowing()) {
			if(showMenu) exitButton.Set();
			else SoundManager.PLAY_SOUND("Scroll", 1, 1.5f);
			
			showMenu = !showMenu;
			for(int i = 0; i < transform.childCount; i++) transform.GetChild(i).gameObject.SetActive(showMenu);
			MenuOn = showMenu;
			if(player != null) player.mouseLook.enabled = !showMenu;
			if(showMenu) Cursor.lockState = CursorLockMode.None;
			else {
				Cursor.visible = false;
				if(player != null && player.controller != null) player.controller.enabled = true;
			}
		}
		ApplyBaseSettings();
	}

	protected void ApplyBaseSettings() {
		Dictionary<string, object> currentSettings = new Dictionary<string, object>();
		var propertyValues = settingFile.playerSettings.GetType().GetFields();
        for(int i = 0; i < propertyValues.Length; i++) currentSettings.Add(propertyValues[i].Name.ToLower(), propertyValues[i].GetValue(settingFile.playerSettings));

		foreach(var setting in settings) {
			var paramName = setting.paramName.ToLower();
			if(!currentSettings.ContainsKey(paramName)) continue;
			if(currentSettings[paramName].GetType() == typeof(bool)) {
				setting.SetToggle((bool)currentSettings[paramName]);
				continue;
			}
			if(currentSettings[paramName].GetType() == typeof(float)) {
				setting.SetVariable((float)currentSettings[paramName]);
				continue;
			}
		}
	}

	public void ReturnToMainMenu() {
		SceneManager.LoadSceneAsync(0);
		Destroy(player.gameObject);
	}
}
