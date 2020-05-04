using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UICopyText : MonoBehaviour {
	public Text src;
	private Text host;

	void Start() {
		host = GetComponent<Text>();
	}

	void Update () {
		host.text = src.text;
	}
}
