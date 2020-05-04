using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Tutorial : ScriptableObject {
	public string tutorialName;
	public string topText, middleText, bottomText;
	public Texture icon1, icon2;

	public bool finished = false;
}
