using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	public GameObject item;

	public InventoryBoard inventory;

	public Vector2Int index;

	private float baseAlpha, targetAlpha, txtBaseAlpha;
	private Image image;
	private Color baseColor;

	public List<InventoryCell> sharedCells = new List<InventoryCell>();

	private float baseScale;

	public bool IsEmpty {
		get {return item == null;}
	}

	public bool holdingSharedItem = false;

	void Awake () {
		image = GetComponent<Image>();
		baseAlpha = targetAlpha = image.color.a;
		baseColor = image.color;
	}

	void Update () {
		if(!inventory.IsDraggingItem) {
			var targetCol = new Color(baseColor.r, baseColor.g, baseColor.b, targetAlpha);
			image.color = Color.Lerp(image.color, targetCol, Time.deltaTime * 3f); 
		}
		else {
			float col = (IsEmpty)? 1 : 0;
			var targetCol = new Color(col - baseColor.r, col - baseColor.g, col - baseColor.b);
			image.color = Color.Lerp(image.color, targetCol, Time.deltaTime * 7f);
		}
	}

	void LateUpdate() {
		float selectedScale = baseScale;
		Color selectedColor = image.color;
		if(inventory != null && inventory.IsCurrentCell(this)) {
			selectedScale += 0.25f;
			selectedColor -= new Color(0.15f, 0.15f, 0.15f);
		}
		transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(selectedScale, selectedScale, selectedScale), Time.deltaTime * 8f);
	
		image.color = Color.Lerp(image.color, selectedColor, Time.deltaTime * 5f);
	}

	public void InitCell(Vector2Int index) {
		this.index = index;
		baseScale = transform.localScale.x;
	}

	public void SetItem(GameObject item, bool sharedItem = false) {
		if(item != null) targetAlpha = 1f;
		else {
			targetAlpha = baseAlpha;
			foreach(var i in sharedCells) i.SetItem(null);
			sharedCells.Clear();
		}
		this.holdingSharedItem = sharedItem;
		this.item = item;
	}

	public void OnPointerEnter(PointerEventData data) {
		inventory.SetCurrentSelection(gameObject, this);
	}
	public void OnPointerExit(PointerEventData data) {}
}

public class ItemCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler {
	public Ingredient item;
	public InventoryCell cell;

	private bool dragOnSurfaces = true;

	private RawImage image;

	public static Color SELECTED_COLOR = new Color(0.8f, 0.8f, 0.8f);
	private Color targetColor = Color.white;

	private Image cookBar, markedIcon;
	private Gradient cookingGradient;

	[HideInInspector]
	public InventoryBoard inventory;

	private float clickTime;
	private int clicked = 0;

	void Start() {
		image = GetComponentInChildren<RawImage>();
	}

	public void UpdateTexture() {
		if(image == null) image = GetComponentInChildren<RawImage>();
		int size = 128;
		var text = new RenderTexture(size, size, 24);
		item.iconCamera.targetTexture = text;
		Texture2D screenshot = new Texture2D(size, size, TextureFormat.RGB24, false);
		item.iconCamera.Render();
		RenderTexture.active = text;
		screenshot.ReadPixels(new Rect(0, 0, size, size), 0, 0, false);
		screenshot.Apply();
		image.texture = screenshot;
		item.iconCamera.targetTexture = null;
		RenderTexture.active = null;
		DestroyImmediate(text);
	}

	public void InitCookingBar(GameObject prefab, Gradient cookingGradient) {
		var bar = Instantiate(prefab);
		bar.name = "Cookedness Bar";
		bar.transform.SetParent(transform);
		cookBar = bar.GetComponent<Image>();
		bar.transform.localScale = Vector3.one;
		bar.transform.localPosition = new Vector3(0, -5, 0);
		bar.tag = "CookingBar";
		this.cookingGradient = cookingGradient;

		//Tracking Eye
		var markObj = Instantiate(prefab);
		markObj.name = "Tracking Eye";
		markObj.transform.SetParent(transform);
		markedIcon = markObj.GetComponent<Image>();
		markObj.transform.localScale = new Vector3(0.2f, 0.15f, 0);
		markObj.transform.localPosition = new Vector3(-40, 50, 0);
		markedIcon.enabled = false;
		markedIcon.color = new Color(0.6f, 0.6f, 0.6f, 0.7f);
	}

	void Update() {
		image.color = Color.Lerp(image.color, targetColor, Time.deltaTime * 10f);

		if(cookBar != null) cookBar.color = cookingGradient.Evaluate(item.cookedness / 100f);
	}
	public Color GetCookingColor() {
		return cookingGradient.Evaluate(item.cookedness / 100f);
	}

	public void MarkForTracking(Sprite tex) {
		markedIcon.sprite = tex;
		markedIcon.enabled = true;
		markedIcon.tag = "EyeTracker";
	}
	public void UnmarkForTracking() {
		markedIcon.enabled = false;
	}

	public void OnPointerDown(PointerEventData data) {
		clicked++;
		if(clicked == 1) clickTime = Time.time;
		else if(clicked > 1 && Time.time - clickTime < 0.3f) {
			inventory.MarkTrackItem(gameObject);
			clicked = 0;
			clickTime = 0;
		}
	}

	public void OnPointerEnter(PointerEventData data) {
		targetColor = SELECTED_COLOR;
		inventory.SetCursorClick(true);
	}
	public void OnPointerExit(PointerEventData data) {
		targetColor = Color.white;
		inventory.SetCursorClick(false);
	}

	public void OnBeginDrag(PointerEventData data) {
		clickTime = clicked = 0;
		SetDraggedPosition(data);
		cell.SetItem(null);
		inventory.draggingItem = item;
		image.raycastTarget = false;
	}
	public void OnDrag(PointerEventData data) {
		clickTime = clicked = 0;
		SetDraggedPosition(data);
	}
	public void OnEndDrag(PointerEventData data) {
		var currentCell = inventory.GetCurrentSelectedCell();
		if(IsCellAvailable(currentCell) && inventory.MouseInDropRange()) {
			cell = currentCell;
			inventory.SetItem(cell.index, item);
		} else if(Input.GetMouseButtonUp(0)) {
			inventory.DropDraggingItem();
			ClearDrag();
			return;
		}
		PositionItem(inventory.cells, item, cell.index);
		ClearDrag();
	}

	public void PositionItem(Dictionary<Vector2Int, GameObject> cells, Ingredient ing, Vector2Int index) {
		if(image == null) image = GetComponentInChildren<RawImage>();
		var lBCell = cells[index].transform.position;
		Vector3 rTCell = Vector3.zero;
		try {
			rTCell = cells[new Vector2Int(index.x + 1, index.y + 1)].transform.position;
		} catch(System.Exception) {
			if(!IsSingleCellSize(ing)) return;
		}

		var offs = (rTCell - lBCell) / 2;
		gameObject.transform.position = cells[index].transform.position + (offs * (IsSingleCellSize(ing) ? 0 : 1));
	}

	public bool IsSingleCellSize(Ingredient ing) {
		return ing.values.sizeFactor.x + ing.values.sizeFactor.y <= 2;
	}

	protected void ClearDrag() {
		inventory.draggingItem = null;
		image.raycastTarget = true;
	}

	protected bool IsCellAvailable(InventoryCell cell) { //Edit for SHAREDCELLS
		return cell.IsEmpty;
	}

	private void SetDraggedPosition(PointerEventData data) {
		RectTransform draggingPlane = null;
        if (dragOnSurfaces && data.pointerEnter != null && data.pointerEnter.transform as RectTransform != null) draggingPlane = data.pointerEnter.transform as RectTransform;

        var rt = gameObject.GetComponent<RectTransform>();
		if(draggingPlane == null) draggingPlane = rt;

        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingPlane, data.position, data.pressEventCamera, out globalMousePos))
        {
            rt.position = globalMousePos;
            rt.rotation = draggingPlane.rotation;
        }
    }
}
