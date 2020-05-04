using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

//Handles in-game settings and applies them to the local player 
public class GameSetting : MonoBehaviour {
	public string paramName;

	public Text valueText;
	private Player player;

	[SerializeField]
	private Slider slider;
	[SerializeField]
	private Toggle toggle;

	public GameMenu menu;

	void Start () {
		player = menu.player;
		if(slider != null) slider.onValueChanged.AddListener(ApplyVariable);
		if(toggle != null) toggle.onValueChanged.AddListener(ApplyToggle);
	}

	public void SetVariable(float lastValue) {
		if(player == null) player = menu.player;
		slider.value = lastValue;
		ApplyVariable(lastValue);
	}

	public void SetToggle(bool value) {
		if(player == null) player = menu.player;
		toggle.isOn = value;
		ApplyToggle(value);
	}

	public void ApplyVariable(float lastValue) {
		if(valueText != null) {
			if(slider.maxValue > 9) valueText.text = ((int)lastValue).ToString();
			else valueText.text = (Mathf.Round(lastValue * 100f) / 100f).ToString();
		}
		var propertyValues = menu.settingFile.playerSettings.GetType().GetFields();//typeof(Player.PlayerSettings).GetFields();
        for(int i = 0; i < propertyValues.Length; i++)
		if(propertyValues[i] != null && propertyValues[i].Name.ToLower() == paramName.ToLower()) {
			propertyValues[i].SetValue(menu.settingFile.playerSettings, lastValue);//player.playerSettings, lastValue);
			break;
		}
		if(player != null) player.OnPlayerSettingChanged();
	}
	public void ApplyToggle(bool value) {
		var propertyValues = menu.settingFile.playerSettings.GetType().GetFields();//typeof(Player.PlayerSettings).GetFields();
        for(int i = 0; i < propertyValues.Length; i++)
		if(propertyValues[i].Name.ToLower() == paramName.ToLower()) {
			propertyValues[i].SetValue(menu.settingFile.playerSettings, value);//player.playerSettings, value);
			break;
		}
		if(player != null) player.OnPlayerSettingChanged();
	}
}
