using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothMouseLook : MonoBehaviour {
    Vector2 _mouseAbsolute;
    Vector2 _smoothMouse;

    [HideInInspector]
    public GameObject CameraOBJ;
    public GameObject LocalCameraOBJ, CameraPoint;

    public bool freeze;
    public Vector2 viewRange = new Vector2(360, 35);

	private Player.PlayerSettings playerSettings;
    private Vector2 sensitivity;
    public float smoothing;

    public Vector2 rotationOffset = new Vector2(20, 0);

    private Transform neck;

    private Vector2 hurtOffset;
    private CameraPusher pusher;

	void Start() {
        neck = GetComponent<Player>().neck;
        pusher = Camera.main.GetComponent<CameraPusher>();
		playerSettings = GetComponent<Player>().playerPrefs.playerSettings;
		smoothing = playerSettings.smoothing;
	}
 
    void Update() {
        if(CameraOBJ == null && Camera.main != null) CameraOBJ = Camera.main.gameObject;
        if(pusher == null && CameraOBJ != null) pusher = CameraOBJ.GetComponent<CameraPusher>();

		sensitivity = new Vector2(playerSettings.xSensitivity, playerSettings.ySensitivity);
        hurtOffset = Vector3.Lerp(hurtOffset, Vector3.zero, Time.deltaTime * 3);

        if (!freeze) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;

        if(freeze) return;

        // Allow the script to clamp based on a desired target value.
        var targetOrientation = Quaternion.Euler(rotationOffset);
 
        // Get raw mouse input for a cleaner reading on more sensitive mice.
        var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
 
        // Scale input against the sensitivity setting and multiply that against the smoothing value.
        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing, sensitivity.y * smoothing));
 
        // Interpolate mouse movement over time to apply smoothing delta.
        _smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1f / smoothing);
        _smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1f / smoothing);
 
        // Find the absolute mouse movement value from point zero.
        _mouseAbsolute += _smoothMouse;
 
        // Clamp and apply the local x value first, so as not to be affected by world transforms.
        if (viewRange.x < 360) _mouseAbsolute.x = Mathf.Clamp(_mouseAbsolute.x, -viewRange.x * 0.5f + viewRange.x, viewRange.x * 0.5f + viewRange.x);
 
        // Then clamp and apply the global y value.
        if (viewRange.y < 360)  _mouseAbsolute.y = Mathf.Clamp(_mouseAbsolute.y, -viewRange.y * 0.5f + viewRange.y, viewRange.y * 0.5f + viewRange.y);
 
        transform.localRotation = Quaternion.AngleAxis(0, targetOrientation * Vector3.right) * targetOrientation;

		var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, transform.InverseTransformDirection(Vector3.up));
		transform.localRotation *= yRotation * Quaternion.Euler(0, hurtOffset.x, hurtOffset.y);
    }

    void LateUpdate() {
        var targetOrientation = Quaternion.Euler(rotationOffset);
        if(neck != null) neck.localRotation *= Quaternion.AngleAxis(-_mouseAbsolute.y * 2 + 50, new Vector3(1, 0, 0));

        UpdateCameraType();
        if(playerSettings.staticCamera) Camera.main.transform.localRotation = Quaternion.AngleAxis(-_mouseAbsolute.y, targetOrientation * Vector3.right) * targetOrientation;
        else {
            if(pusher == null) return;
            var push = pusher.push;
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, CameraPoint.transform.position + push, Time.deltaTime * 4f);
            Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, CameraPoint.transform.rotation * Quaternion.Euler(-_mouseAbsolute.y + 25, 0, 0), Time.deltaTime * 6f);
        }
    }

    public void UpdateCameraType() {
        if(playerSettings == null || CameraOBJ == null || LocalCameraOBJ == null) return;

        if(playerSettings.staticCamera) {
            if(!CameraOBJ.activeSelf && LocalCameraOBJ.activeSelf) return;
            CameraOBJ.SetActive(false);
            LocalCameraOBJ.SetActive(true);
        } else {
            if(CameraOBJ.activeSelf && !LocalCameraOBJ.activeSelf) return;
            LocalCameraOBJ.SetActive(false);
            CameraOBJ.SetActive(true);
        }
    }

    public void HurtShake(Vector3 offset) {
        hurtOffset = offset;
    }
}
