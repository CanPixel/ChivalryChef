using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

//THE inventory system of the player
public class InventoryBoard : Inventory {
	#region REFERENCES
	[Header("References")]
	public GameObject canvasElement;
	public GameObject cellPrefab;
	public GameObject popupTextPrefab, slashTextPrefab;
	public GameObject ovenDial;
	public Image ovenIndicator;
	public GameObject slashIconPrefab;
	public Sprite MarkedItemEye;
	public Texture2D[] cursors;

	public OvenDial ovenScr;
	private Image ovenImage;
	public Image[] ovenIMG;

	public GameObject cookednessBarPrefab;
	public Gradient cookingGradient;
	#endregion

	private Backpack heater;

	[Header("Variables")]
	public Vector2Int gridSize;
	[Range(0, 5)]
	public float cellSize;

	//A looooot of dictionaries to keep track of certain item positions within the inventory, and how many spaces they occupy
	public Dictionary<Vector2Int, Ingredient> inv = new Dictionary<Vector2Int, Ingredient>();
	public Dictionary<Vector2Int, GameObject> cells = new Dictionary<Vector2Int, GameObject>();
	public Dictionary<Ingredient, GameObject> itemCells = new Dictionary<Ingredient, GameObject>();
	private List<InventoryCell> sharedCells = new List<InventoryCell>();

	[Header("Database")]
	public List<Ingredient> ingredients = new List<Ingredient>();
	public Dictionary<RawImage, Ingredient> thumbnails = new Dictionary<RawImage, Ingredient>();
	
	public Dictionary<Ingredient, GameObject> ingredientToLabel  = new Dictionary<Ingredient, GameObject>();

	public int maxTrackingItems = 3;
	public const int MAX_POPUPCOUNT = 5;
	protected int popups = 0;

	public override int itemLimit {
		get {return gridSize.x * gridSize.y;}
	}

	private GameObject lastSelectedCell;
	private GameObject selectedCell = null;
	[HideInInspector]
	public Ingredient draggingItem;
	public override bool IsDraggingItem {
		get {return draggingItem != null;}
	}

	private BoxCollider2D colliderBox;

	//In-game, at all times, the player can double-click on max 3 items to keep them in the QUICK-HUD, and not lose track of potentially burning the food
	private Dictionary<GameObject, Ingredient> trackingItems = new Dictionary<GameObject, Ingredient>();
	private Dictionary<Ingredient, GameObject> trackersByIngredient = new Dictionary<Ingredient, GameObject>();
	[HideInInspector]
	public List<GameObject> trackIngredients = new List<GameObject>();

	void Start () {
		ovenImage = ovenScr.GetComponent<Image>();
		colliderBox = canvasElement.GetComponent<BoxCollider2D>();
		heater = GetComponent<HeaterCook>().heater;
		inventoryHolder = transform.GetComponent<KnightMovement>();
		InitInventory();
	}
	
	void Update () {
		if(sharedCells.Count > 0) sharedCells.Clear();
		
		//Cook Ingredients
		foreach(KeyValuePair<Vector2Int, Ingredient> ingredient in inv) if(ingredient.Value != null) {
			ingredient.Value.Cook(heater.heat, heater.heatLevel);
			if(ingredient.Value.cookedness >= 80 && heater.heatLevel > 0 && heater.heat > 40) heater.BurnFood(true);
		}

		//ToggleInventory
		if(Input.GetKeyUp(KeyCode.Q) && !GameMenu.MenuOn) {
			showing = !showing;
			if(!showing) ovenIndicator.transform.position = ovenDial.transform.position;
		}

		ovenImage.enabled = showing;
		foreach(var i in ovenIMG) i.enabled = showing;

		//QuickHUD food trackers
		foreach(var i in trackIngredients) 
			if(i != null) {
				foreach(Transform cook in i.transform) {
					if(cook.tag == "CookingBar" && trackingItems.ContainsKey(i) && ingredientToLabel.ContainsKey(trackingItems[i])) {
						cook.GetComponent<Image>().color = ingredientToLabel[trackingItems[i]].GetComponent<ItemCell>().GetCookingColor();
						break;
					}
				}
			}

		//QuickHUD oven dial indicator
		if(showing) {
			ovenIndicator.color = Color.Lerp(ovenIndicator.color, new Color(1f, 1f, 1f, 0f), Time.deltaTime * 4f);
			ovenIndicator.transform.position = Vector2.Lerp(ovenIndicator.transform.position, ovenDial.transform.position, Time.deltaTime * 3f);
			ovenIndicator.transform.localScale = Vector3.Lerp(ovenIndicator.transform.localScale, ovenDial.transform.localScale, Time.deltaTime * 4f);
		
			for(int i = 0; i < trackIngredients.Count; i++) if(trackIngredients[i] != null) {
				var col = trackIngredients[i].GetComponent<MaskableGraphic>().color;
				trackIngredients[i].GetComponent<MaskableGraphic>().color = new Color(col.r, col.g, col.b, 0.55f);
				trackIngredients[i].transform.localScale = Vector3.one;
				trackIngredients[i].transform.localPosition = Vector3.Lerp(trackIngredients[i].transform.localPosition, new Vector3(i * 125 - ((trackingItems.Count - 1) * 125) / 2, -275, 0), Time.deltaTime * 4f);
			}
		}
		else {
			ovenIndicator.color = Color.Lerp(ovenIndicator.color, new Color(1f, 1f, 1f, 0.3f), Time.deltaTime * 3f);
			ovenIndicator.transform.position = Vector2.Lerp(ovenIndicator.transform.position, new Vector2(Screen.width / 8 * 7 + 20, Screen.height / 7) + new Vector2(Mathf.Sin(Time.time * 4f) / 2f, Mathf.Cos(Time.time * 4f) / 2f), Time.deltaTime * 6f);
			ovenIndicator.transform.localScale = Vector3.Lerp(ovenIndicator.transform.localScale, new Vector3(0.2f, 0.2f, 1), Time.deltaTime * 4f);

			for(int i = 0; i < trackIngredients.Count; i++) if(trackIngredients[i] != null) {
				var col = trackIngredients[i].GetComponent<MaskableGraphic>().color;
				trackIngredients[i].GetComponent<MaskableGraphic>().color = new Color(col.r, col.g, col.b, 0.75f);
				trackIngredients[i].transform.localScale = Vector3.one * 1.5f;
				trackIngredients[i].transform.localPosition = Vector3.Lerp(trackIngredients[i].transform.localPosition, new Vector3(i * 250 - ((trackingItems.Count - 1) * 250) / 2, -275, 0), Time.deltaTime);
			}
		}

		canvasElement.SetActive(showing);
		Cursor.visible = Player.IsInventory = showing | GameMenu.MenuOn;

		//Bigger items slot dragging selection - visual feedback
		if(IsDraggingItem && selectedCell != null && (draggingItem.values.sizeFactor.x > 1 || draggingItem.values.sizeFactor.y > 1)) {
			var rootCell = selectedCell.GetComponent<InventoryCell>();
			var rootPos = rootCell.index;

			for(int x = 0; x < draggingItem.values.sizeFactor.x; x++)
			for(int y = 0; y < draggingItem.values.sizeFactor.y; y++) {
				GameObject indexedObj = null;
				try {
					indexedObj = cells[rootPos + new Vector2Int(x, y)];
				} catch(System.Exception) {
					selectedCell = lastSelectedCell;
					sharedCells.Clear();
					return;
				}

				var indexedCell = indexedObj.GetComponent<InventoryCell>();
				if(indexedObj == null) {
					Debug.LogError("No Space! Out of range!");
					selectedCell = lastSelectedCell;
					sharedCells.Clear();
					return;
				}
				if(!indexedCell.IsEmpty) {
				//	Debug.LogError("Space already filled!");
					selectedCell = lastSelectedCell;
					sharedCells.Clear();
					return;
				}
				sharedCells.Add(indexedCell);
			}
		}
	}

	//Marks an item for tracking --> it appears in the Quick-HUD 
	public void MarkTrackItem(GameObject obj) {
		var ing = obj.GetComponent<ItemCell>().item;
		if(ingredientToLabel.ContainsKey(ing) && ingredientToLabel[ing].GetComponent<ActionOnHover>() != null) {
			UnlinkTrackerToItem(ing, null);
			Destroy(ingredientToLabel[ing].GetComponent<ActionOnHover>());
		}
		Ingredient deletedIngredient = null;
		GameObject objToDelete = null;
		if(trackingItems.ContainsValue(ing)) {
			foreach(KeyValuePair<GameObject, Ingredient> loop in trackingItems) {
				if(loop.Value == ing) {
					objToDelete = loop.Key;
					deletedIngredient = loop.Value;
					break;
				}
			}
			if(objToDelete != null) {
				for(int i = 0; i < trackIngredients.Count; i++) {
					if(trackIngredients[i] == objToDelete) {
						Destroy(objToDelete);
						trackIngredients[i] = null;
						trackingItems.Remove(objToDelete);
						trackersByIngredient.Remove(deletedIngredient);
						obj.GetComponent<ItemCell>().UnmarkForTracking();
						return;
					}
				}
			}
		}
		for(int i = 0; i < trackIngredients.Count; i++) {
			if(trackIngredients[i] == null) {
				var tracker = Instantiate(obj);
				tracker.name = "Item Cooking Tracker";
				tracker.transform.SetParent(ovenIndicator.transform);
				trackingItems.Add(tracker, tracker.GetComponent<ItemCell>().item);
				trackersByIngredient.Add(tracker.GetComponent<ItemCell>().item, tracker);
				Destroy(tracker.GetComponent<ItemCell>());

				foreach(Transform t in tracker.transform) {
					if(t.tag != "CookingBar") Destroy(t.gameObject);
					var mimic = t.gameObject.AddComponent<MimicParentProperties>();
					mimic.mimicAlpha = true;
					mimic.offset = 0.1f;
				}
				
				var highlighter = new GameObject("Highlighter");
				highlighter.transform.SetParent(tracker.transform);
				highlighter.tag = "ItemHighlighter";
				var highIMG = highlighter.AddComponent<Image>();
				highIMG.color = new Color(0, 0, 1, 0.0f);
				highlighter.GetComponent<RectTransform>().sizeDelta = new Vector2(130, 130);
				highlighter.transform.localScale = Vector3.one;
				highlighter.transform.localPosition = Vector3.zero;
				
				var hoverAction = tracker.AddComponent<ActionOnHover>();
				hoverAction.OnEnter.AddListener(() => LinkTrackerToItem(ing, highIMG));
				hoverAction.OnExit.AddListener(() => UnlinkTrackerToItem(ing, highIMG));

				var hoverCell = ingredientToLabel[ing].AddComponent<ActionOnHover>();
				hoverCell.OnEnter.AddListener(() => LinkTrackerToItem(ing, highIMG));
				hoverCell.OnExit.AddListener(() => UnlinkTrackerToItem(ing, highIMG));

				obj.GetComponent<ItemCell>().MarkForTracking(MarkedItemEye);
				
				trackIngredients[i] = tracker;
				SoundManager.PLAY_SOUND("markitem");
				return;
			}
		}
		Debug.LogError("You're tracking enough items already!");
	}
	public void Unmark(GameObject obj) {
		for(int i = 0; i < trackIngredients.Count; i++) {
			if(trackIngredients[i] == obj) {
				trackingItems.Remove(trackIngredients[i]);
				trackIngredients[i] = null;
				break;
			}
		}
		Destroy(obj);
	}

	protected void LinkTrackerToItem(Ingredient ing, Image bg) {
		bg.color = new Color(0, 0, 1, 0.1f);
		foreach(Transform t in ingredientToLabel[ing].transform) if(t.tag == "ItemHighlighter") {
			t.GetComponent<MaskableGraphic>().color = new Color(0, 0, 1, 0.1f);
			break;
		}
	}
	protected void UnlinkTrackerToItem(Ingredient ing, Image bg) {
		if(bg != null) bg.color = new Color(0, 1, 0, 0.0f);
		foreach(Transform t in ingredientToLabel[ing].transform) if(t.tag == "ItemHighlighter") {
			t.GetComponent<MaskableGraphic>().color = new Color(0, 1, 0, 0f);
			break;
		}
		SoundManager.PLAY_SOUND("markitem", 1, 0.8f);
	}

	public bool MouseInDropRange() {
		return colliderBox.OverlapPoint(Input.mousePosition);
	}

	public void DropDraggingItem() {
		foreach(KeyValuePair<RawImage, Ingredient> pair in thumbnails) if(pair.Value == draggingItem) {
			thumbnails.Remove(pair.Key);
			break;
		} 
		if(trackersByIngredient.ContainsKey(draggingItem)) {
			Unmark(trackersByIngredient[draggingItem]);
			trackersByIngredient.Remove(draggingItem);
		}
		Destroy(itemCells[draggingItem]);
		itemCells.Remove(draggingItem);
		ingredientToLabel.Remove(draggingItem.GetComponent<Ingredient>());
		var rigids = draggingItem.GetComponentsInChildren<Rigidbody>();
		foreach(var rigid in rigids) {
			rigid.isKinematic = false;
			rigid.useGravity = true;
		}
		foreach(var coll in draggingItem.GetComponentsInChildren<MeshCollider>()) coll.enabled = true;
		draggingItem.GetComponent<Ingredient>().owner = null;
		draggingItem.transform.SetParent(null);
		draggingItem.transform.localPosition += new Vector3(1, 2, 0);
		draggingItem.transform.localScale = Vector3.one;
		draggingItem = null;
		selectedCell = null;
	}

	private GameObject FindIngredientPrefab(Ingredient ing) {
		foreach(var i in ingredients) {
			if(i.values.color == ing.values.color) return i.gameObject;
		}
		return null;
	}

	private void InitInventory() {
		for(int x = 0; x < gridSize.x; x++)
		for(int y = 0; y < gridSize.y; y++) {
			var obj = Instantiate(cellPrefab);
			obj.name = cellPrefab.name + " (" + x + ", " + y + ")";
			obj.transform.SetParent(canvasElement.transform);
			obj.transform.localPosition = new Vector3(x, y, 0) * 100 * cellSize - new Vector3((gridSize.x / 2 - 0.5f) * cellSize, (gridSize.y / 2 + 0.1f) * cellSize, 0) * 100;
			obj.transform.localScale = Vector3.one * cellSize;
			obj.GetComponent<InventoryCell>().InitCell(new Vector2Int(x, y));
			obj.GetComponent<InventoryCell>().inventory = this;
			inv.Add(new Vector2Int(x, y), null);
			cells.Add(new Vector2Int(x, y), obj);
		}
	}

	public bool HasSpaceInInventory() {
		int invCount = 1;
		foreach(var i in inv) {
			if(i.Value != null) invCount++;
		}
		return invCount <= itemLimit;
	}

	public bool IsCurrentCell(InventoryCell cell) {
		return selectedCell != null && (cell.index == selectedCell.GetComponent<InventoryCell>().index || sharedCells.Contains(cell));
	}

	public void SetCurrentSelection(GameObject obj, InventoryCell cell) {
		if(!cell.IsEmpty) return;
		lastSelectedCell = selectedCell;
		selectedCell = obj;
	}

	public InventoryCell GetCurrentSelectedCell() {
		return selectedCell.GetComponent<InventoryCell>();
	}

	public override void AddItemToInventory(Ingredient ing) {
		if(!HasSpaceInInventory()) return;
		var space = GetClosestSpace(ing);
		if(space == Vector2.one * -1) {
			CreatePopup("no room in inventory");
			return;
		}
		inv[space] = ing;
		CreatePopup("item added: " + ing.name.ToUpper());
		CreateItemSlot(ing, space);
	}

	//Fills an inventory slot with an item, when one is picked up
	public void CreateItemSlot(Ingredient ing, Vector2Int index) {
		var obj = Instantiate(cellPrefab);
		obj.name = ing.name;
		obj.transform.SetParent(canvasElement.transform);
		obj.transform.localScale = Vector3.one * ing.values.sizeFactor * cells[index].transform.localScale.x;
		Destroy(obj.GetComponent<Image>());
		var img = obj.transform.GetComponentInChildren<RawImage>();
		img.color = new Color(1, 1, 1, 1);
		Destroy(obj.GetComponent<InventoryCell>());
		var itemcell = img.gameObject.AddComponent<ItemCell>();
		itemcell.InitCookingBar(cookednessBarPrefab, cookingGradient);
		itemcell.item = ing;
		itemcell.UpdateTexture();
		itemcell.inventory = this;
		itemcell.cell = cells[index].GetComponent<InventoryCell>();
		itemcell.PositionItem(cells, ing, index);

		for(int i = 0; i < ing.cutStage; i++) {
			var slash = Instantiate(slashIconPrefab);
			slash.name = "Cut Icon";
			slash.transform.SetParent(itemcell.transform);
			slash.transform.localPosition = new Vector3(40 - i * 10, 50, 0);
			slash.transform.localScale = Vector3.one;
		}

		thumbnails.Add(img, ing);
		SetItem(index, ing);
		itemCells.Add(ing, obj);
		ingredientToLabel.Add(ing, itemcell.gameObject);
	}

	public void SetItem(Vector2Int index, Ingredient ing) {
		inv[index] = ing;
		var root = cells[index].GetComponent<InventoryCell>();
		root.SetItem(ing.gameObject);
		for(int x = 0; x < ing.values.sizeFactor.x; x++)
		for(int y = 0; y < ing.values.sizeFactor.y; y++) {
			if(x == 0 && y == 0) continue;
			Vector2Int ind = new Vector2Int(index.x + x, index.y + y);
			var child = cells[ind].GetComponent<InventoryCell>();
			inv[ind] = ing;
			child.SetItem(ing.gameObject, true);
			root.sharedCells.Add(child);
		}
	}

	public void SetCursorClick(bool selected) {
		if(selected) Cursor.SetCursor(cursors[1], Vector2.zero, CursorMode.Auto);
		else Cursor.SetCursor(cursors[0], Vector2.zero, CursorMode.Auto);
	}

	//Creates a popup text for newly picked-up items
	public void CreatePopup(string i) {
		if(popups >= MAX_POPUPCOUNT) return;
		var popup = Instantiate(popupTextPrefab);
		popup.name = popupTextPrefab.name;
		popup.transform.SetParent(canvasElement.transform.parent, true);
		popup.transform.SetAsFirstSibling();
		var rect = popup.GetComponent<RectTransform>();
		rect.anchorMax = rect.anchorMin = new Vector2(1, 1);
		rect.anchoredPosition = new Vector2(-275, -55 - popups * 85);
		popup.GetComponent<Text>().text = i;
		popup.GetComponent<FadeAfter>().postEvent = decreasePopupCount;
		popups++;
	}
	private void decreasePopupCount() {
		popups--;
		if(popups < 0) popups = 0;
	}

	//Fetches the closest available space within the inventory for a certain ingredient
	public Vector2Int GetClosestSpace(Ingredient ing) {
		for(int x = 0; x < gridSize.x; x++)
		for(int y = 0; y < gridSize.y; y++) {
			var key = new Vector2Int(x, y);
			var element = inv[key];
			if(element == null) {
				bool breakLoop = false;
				for(int xx = 0; xx < ing.values.sizeFactor.x; xx++) {
				if(breakLoop) break;
					for(int yy = 0; yy < ing.values.sizeFactor.y; yy++) {
						if(inv[new Vector2Int(x + xx, y + yy)] != null) breakLoop = true;
					}
				}
				if(!breakLoop) return key;
			}
		}
		return Vector2Int.one * -1;
	}
}
