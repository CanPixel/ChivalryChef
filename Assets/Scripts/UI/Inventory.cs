using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Inventory : MonoBehaviour {
	public abstract int itemLimit {get;}

	public bool showing = true;

	protected KnightMovement inventoryHolder;

	public abstract void AddItemToInventory(Ingredient ing);
	
	public virtual bool IsDraggingItem {
		get {return false;}
	}
}
