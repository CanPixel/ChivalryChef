using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class GameSettings : ScriptableObject {
	public bool enableTutorial = true;

	public Player.PlayerSettings playerSettings;
}