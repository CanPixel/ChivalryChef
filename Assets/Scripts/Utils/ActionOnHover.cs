using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ActionOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	public UnityEvent OnEnter = new UnityEvent(), OnExit = new UnityEvent();

	public void OnPointerEnter(PointerEventData data) {
		OnEnter.Invoke();
	}
	public void OnPointerExit(PointerEventData data) {
		OnExit.Invoke();
	}
}
