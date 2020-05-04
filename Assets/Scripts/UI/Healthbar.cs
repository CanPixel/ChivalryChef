using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour {
	public GameObject HP, Stamina; 
	public Image stamiFill, healthFill;

	[Header("Target")]
	public KnightMovement host;

	protected float currentHP, currentStami;

	void Update () {
		currentHP = (int)((host.health / host.maxHealth) * 100f);
		currentStami = (int)((host.stamina / host.maxStamina) * 100f);

		stamiFill.fillAmount = currentStami / 100f;
		healthFill.fillAmount = currentHP / 100f;
	}
}
