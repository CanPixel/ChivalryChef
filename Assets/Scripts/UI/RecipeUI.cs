using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeUI : MonoBehaviour {
	public Image Photo, Ribbon, bgCategory;
	public Text text, category, cuisine;
	public Text textShadow, loadingText, loadingTextShadow;

	public Image[] ingredientIMG;

	void Start() {
		Activate(false);
	}
	
	public void Activate(bool i) {
		Photo.enabled = text.enabled = category.enabled = cuisine.enabled = textShadow.enabled = bgCategory.enabled = i;
		loadingText.enabled = loadingTextShadow.enabled = !i;
		foreach(var j in ingredientIMG) if(j != null) j.enabled = i;
	}	
}
