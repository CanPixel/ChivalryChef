using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingDirectionHandler : MonoBehaviour {
	public KnightMovement host;
	public BoxCollider swordCollider;

	private Vector3 initialSize;
	private Vector3 initialKnightSize;

	void Start() {
		initialSize = swordCollider.size;
		initialKnightSize = transform.localScale;
	}

	//Sets the direction where the player is about to swing (from left to right, right to left) 
	protected void ApplySwingDirection() {
		transform.localScale = new Vector3(host.swingDirection * 1.2f, 2, 2);
		swordCollider.size = initialSize * 3;
		((Player)host).TriggerSwingZoom();
	}

	void Update() {
		transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(transform.localScale.x, initialKnightSize.y, initialKnightSize.z), Time.deltaTime * 4f);
	}

	protected void ResetSwingDirection() {
		transform.localScale = new Vector3(initialKnightSize.x, transform.localScale.y, transform.localScale.z);
		if(host.GetType() == typeof(Player)) {
			var player = ((Player)host);
			player.CancelSwing();
			player.mouseLook.freeze = false;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.lockState = CursorLockMode.None;
		}
		swordCollider.size = initialSize;
	}

	//Marks the ending of the animation for the simple ''quick hack''
	protected void EndHack() {
		swordCollider.size = initialSize;
		host.meleeAction = KnightMovement.MELEE_ACTION.NONE;
		if(host.GetType() == typeof(Player)) ((Player)host).CancelSwing();
	}

	//Heavy, directional swing attack
	protected void Swing() {
		host.meleeAction = KnightMovement.MELEE_ACTION.SWING;
		TutorialManager.FinishTutorial("BattleSwing");
	}

	//Quick forward slash/hack
	protected void Hack() {
		swordCollider.size = initialSize;
		host.meleeAction = KnightMovement.MELEE_ACTION.HACK;

		if(host.GetType() == typeof(Player)) ((Player)host).TriggerSwingZoom();
	}
}
