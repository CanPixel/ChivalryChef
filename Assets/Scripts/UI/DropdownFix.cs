using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownFix : MonoBehaviour {

	void Start () {
		GetComponent<Dropdown>().onValueChanged.AddListener(value => DeselectList());
	}

	public void DeselectList() {
		var go = transform.Find("Dropdown List");
		if(go != null) Destroy(go.gameObject);
	}
}
