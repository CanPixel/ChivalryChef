using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerMenu : MonoBehaviour {
    private bool show = false;

    void Start() {
        transform.GetChild(0).gameObject.SetActive(show);
    }

    public void ToggleMenu() {
        show = !show;
        transform.GetChild(0).gameObject.SetActive(show);
    }

    public void HideMenu() {
        show = false;
        transform.GetChild(0).gameObject.SetActive(show);
    }
}
