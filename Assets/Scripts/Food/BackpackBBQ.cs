using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackpackBBQ : Backpack {
	public Renderer[] colorAffectedByHeat, glowyParts;
	public GameObject burnFoodParticles, cookFoodParticles;

	private Color[] baseMaterialHeatColors;

	public Light cookingLight;

	public bool IsCooking {
		get {return heatLevel > 0;}
	}
	public bool HeatedUp {
		get {return heat >= 5;}
	}

	private float baseTemperatureColor;

	void Start () {
		if(cookingLight != null) baseTemperatureColor = cookingLight.colorTemperature;
		foreach(var i in glowyParts) i.material.EnableKeyword("_EMISSION");
		
		baseMaterialHeatColors = new Color[colorAffectedByHeat.Length];
		for(int i = 0; i < baseMaterialHeatColors.Length; i++) baseMaterialHeatColors[i] = colorAffectedByHeat[i].material.color;

		BurnFood(false);
		Cook(false);
	}

	private float sizzleDelay = 0;
	
	void Update () {
		if(IsCooking) {
			if(sizzleDelay > 0) sizzleDelay -= Time.deltaTime;
			if(heat <= 0.1) SoundManager.PLAY_SOUND("Firecrackles", 0.6f, 1);
			else if(Random.Range(0, 20) < 4 && sizzleDelay <= 0) {
				SoundManager.PLAY_SOUND("sizzle", 0.75f, 1.4f);
				sizzleDelay = 1;
			}

			if(heat < heatLevel * 10f) heat += Time.deltaTime * 2f;

			cookingLight.intensity = Mathf.Lerp(cookingLight.intensity, heatLevel * 10f, Time.deltaTime);
			cookingLight.colorTemperature = Mathf.Lerp(cookingLight.colorTemperature, baseTemperatureColor - heatLevel * 1000f, Time.deltaTime);

			foreach(var i in glowyParts) {
				float red = heatLevel * 0.4f;
				i.material.SetColor("_EmissionColor", Color.Lerp(i.material.GetColor("_EmissionColor"), new Color(red, red / 2f, red / 2f), Time.deltaTime));
			}
			foreach(var i in colorAffectedByHeat) i.material.color = Color.Lerp(i.material.color, new Color(0.05f * heatLevel, 0, 0), (heat / (heatLevel * 10f)));
		} else CoolDown();
		
		Cook(IsCooking & HeatedUp);
	}

	//Lerps the emissive coloring of the backpack heater back to 'cool' temperatures
	protected void CoolDown() {
		heat = Mathf.Lerp(heat, 0, Time.deltaTime * 2f);
		for(int i = 0; i < colorAffectedByHeat.Length; i++) colorAffectedByHeat[i].material.color = Color.Lerp(colorAffectedByHeat[i].material.color, baseMaterialHeatColors[i], Time.deltaTime / 2f);
		foreach(var i in glowyParts) i.material.SetColor("_EmissionColor", Color.Lerp(i.material.GetColor("_EmissionColor"), new Color(0, 0, 0), Time.deltaTime / 2f));
	
		BurnFood(false);
	}

	public override void BurnFood(bool i) {
		if(burnFoodParticles != null) burnFoodParticles.SetActive(i);
	}

	public override void Cook(bool i) {
		if(cookFoodParticles != null) cookFoodParticles.SetActive(i);
	}
}

public abstract class Backpack : MonoBehaviour {
	public int heatLevel;
	public float heat;

	public abstract void BurnFood(bool i);
	public abstract void Cook(bool i);
}
