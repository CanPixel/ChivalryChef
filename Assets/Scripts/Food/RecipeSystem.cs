using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

//WORK IN PROGRESS
public class RecipeSystem : MonoBehaviour {
	[System.Serializable]
	public enum RecipeSource {
		MEALDB, OFFLINE
	}
	public RecipeSource databaseSource;
	private RecipeBase recipeSystem;

	[System.Serializable]
	public class RecipeIngredient {
		public string name;
		public Ingredient ingredient;
	}
	public Ingredient[] recipeIngredients;
	public Dictionary<string, Ingredient> loadedIngredients = new Dictionary<string, Ingredient>();

	//public static int MAX_LOADED_RECIPES = 2;
	private static RecipeSystem self;

	float time = 0;
	
	public List<RecipeBase.Recipe> recipes = new List<RecipeBase.Recipe>();

	[Header("UI")]
	#region RECIPESCREEN
	public KeyCode recipeScreenKey;
	public	bool showRecipeScreen = true;
	public GameObject RecipeScreen;
	public GameObject RecipeUI, leftPage, rightPage, cover;
	public Text recipeInstructions;
	public int maxUIRecipes = 3;
	private int RecipeIndex = 0;
	[HideInInspector]
	public List<RecipeBase.Recipe> RecipesInUI = new List<RecipeBase.Recipe>();
	private List<GameObject> RecipeCards = new List<GameObject>();
	#endregion

	void Start () {
		Load();
		CueAnimate();
	}

	protected void LoadIngredients() {
		loadedIngredients.Clear();
		foreach(var i in recipeIngredients) loadedIngredients.Add(i.name.ToLower().Trim(), i);
	}

	public void ClearUI() {
		foreach(var i in RecipeCards) DestroyImmediate(i);
		recipes.Clear();
		RecipesInUI.Clear();
		RecipeCards.Clear();
		RecipeIndex = 0;
	}

	public void Load() {
		LoadIngredients();
		ClearUI();
		Util.ClearConsole();

		if(self == null) self = this;

		switch(databaseSource) {
			case RecipeSource.MEALDB:
				recipeSystem = new MealDBRecipeSystem(this, loadedIngredients, CueAnimate);
				break;
			default:
			case RecipeSource.OFFLINE:
				break;
		}
		if(recipeSystem == null) {
			NotifyOfflineError();
			return;
		}
		else recipeSystem.LoadBase();

		recipes = recipeSystem.loadedRecipes;
	}

	public static bool IsShowing() {
		return self != null && self.showRecipeScreen;
	}

	void Update() {
		time += Time.deltaTime;
		RecipeScreen.SetActive(showRecipeScreen);

		if(Input.GetKeyUp(recipeScreenKey)) {
			showRecipeScreen = !showRecipeScreen;
			//if(showRecipeScreen) CueAnimate();
		}
	}

	protected void CueAnimate() {
		leftPage.SetActive(false);
		cover.transform.localRotation = Quaternion.Euler(0, 0, 0);
		cover.SetActive(true);
		StartCoroutine(OpenBook());
	}

	IEnumerator OpenBook() {
		float duration = 0;
		yield return new WaitForSeconds(0.5f);
		while(duration < 1f) {
			cover.transform.localRotation = Quaternion.Euler(0, Mathf.LerpAngle(cover.transform.localEulerAngles.y, -180, Time.deltaTime * 2), 0);
			duration += Time.deltaTime;
			yield return new WaitForSeconds(0.01f);
		}
		leftPage.SetActive(true);
	}

	public static RecipeUI GenerateRecipeUI(RecipeBase.Recipe recipe) {
		return self.AddRecipeToUI(recipe);
	}
	
	private RecipeUI AddRecipeToUI(RecipeBase.Recipe recipe) {
		if(RecipeIndex >= maxUIRecipes || RecipeIndex - 1 >= recipes.Count) return null;
		RecipeIndex++;
		var obj = Instantiate(RecipeUI, new Vector3(0, 0, 0), Quaternion.identity);
		//bool isOnLeftPage = (RecipeIndex - 1 % 2) == 0;
		Transform parent = leftPage.transform;//isOnLeftPage ? leftPage.transform : rightPage.transform;
		obj.transform.SetParent(parent);
		obj.transform.localScale = new Vector3(0.8f, 0.6f, 1);
		obj.transform.GetComponent<RectTransform>().anchoredPosition  = new Vector3(42, 0, -10);
		RecipeCards.Add(obj);
		var src = obj.GetComponent<RecipeUI>();
		StartCoroutine(SetRecipeIcon(src.Photo, recipe.strMealThumb));
		src.text.text = src.textShadow.text = recipe.strMeal.Substring(0, 1) + recipe.strMeal.Substring(1).ToLower();
		src.category.text = recipe.strCategory;
		src.cuisine.text = recipe.strArea;
		CheckIngredients(recipe, src);
		RecipesInUI.Add(recipe);
		recipeInstructions.text = recipeSystem.FilterInstructions(ref recipe.strInstructions);
		return src;
	}

	private void CheckIngredients(RecipeBase.Recipe recipe, RecipeUI src) {
		string[] ings = {recipe.strIngredient1, recipe.strIngredient2, recipe.strIngredient3, recipe.strIngredient4, recipe.strIngredient5, recipe.strIngredient6, recipe.strIngredient7, recipe.strIngredient8};
		for(int i = 0; i < ings.Length; i++) {
			if(src.ingredientIMG.Length >= ings.Length && ings[i].Length <= 1) DestroyImmediate(src.ingredientIMG[i].transform.parent.gameObject);
		}
	}

	IEnumerator SetRecipeIcon(Image image, string url) {
		//WWW www = new WWW(url);
		//yield return www;

		using(UnityWebRequest www = UnityWebRequestTexture.GetTexture(url)) {
			yield return www.SendWebRequest();

			if(www.isNetworkError || www.isHttpError) Debug.LogError(www.error);
			else {
				var texture = DownloadHandlerTexture.GetContent(www);
				if(image != null) image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2());
			}
		}
	}

	void LateUpdate() {
		for(int i = 0; i < RecipesInUI.Count; i++) if(RecipesInUI[i].loaded) RecipeCards[i].GetComponent<RecipeUI>().Activate(true);
	}

	public static void ScanForRecipe(string txt) {
		self.recipeSystem.ScanForRecipe(txt);
	}

	protected void NotifyOfflineError() {
		Debug.LogError("No valid database URL found for recipes! Running Offline Mode.");
	}
}

public abstract class RecipeBase {
	public delegate void OnFinishLoad();
	public OnFinishLoad onLoadFinished;

	public abstract void LoadBase();

	public List<Recipe> loadedRecipes = new List<Recipe>();

	[System.Serializable]
	public class Recipe {
		[HideInInspector]
		public string idMeal; 
		public string strMeal, strCategory, strArea, strInstructions, strMealThumb, strIngredient1, strIngredient2, strIngredient3, strIngredient4, strIngredient5, strIngredient6, strIngredient7, strIngredient8; 
		public Sprite[] ingredients = new Sprite[8];

		public bool loaded = false;

		public IEnumerator LoadIngredients(string root, Image[] img) {
			string[] ing = {strIngredient1, strIngredient2, strIngredient3, strIngredient4, strIngredient5, strIngredient6, strIngredient7, strIngredient8};
			for(int i = 0; i < ing.Length; i++) {
				if(ing[i].Length <= 1) continue;
				var coreIngredient = FormatIngredientName(ref ing[i]);
				var link = root + coreIngredient + ".png";

				using(UnityWebRequest www = UnityWebRequestTexture.GetTexture(link)) {
					yield return www.SendWebRequest();

					if(www.isNetworkError || www.isHttpError) Debug.Log(www.error);
					else {
						//yield return www;
						var texture = DownloadHandlerTexture.GetContent(www);

						if(texture == null) Debug.LogError("[IMG_ERROR]");
						else {
							ingredients[i] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2());
							if(i < img.Length && img[i] != null) img[i].sprite = ingredients[i];
						}
					}
				}
			}
			loaded = true;
		}

		private string FormatIngredientName(ref string i) {
			return i.Trim();
		} 
	}
	protected MonoBehaviour host;

	protected Dictionary<string, Ingredient> availableIngredients = new Dictionary<string, Ingredient>();

	public RecipeBase(MonoBehaviour host, Dictionary<string, Ingredient> ings) {
		this.host = host;
		this.availableIngredients = ings;
	}

	public abstract void ScanForRecipe(string txt);
	public abstract string FilterInstructions(ref string instre);
}

public class MealDBRecipeSystem : RecipeBase {
	private const string TableName = "Recipes";
    private const string MAIN_LINK = "https://www.themealdb.com/api/json/v1/1/";
	private const string INGREDIENT_LINK = "https://www.themealdb.com/images/ingredients/";

	//public const int RECIPES = 2;

	public List<string> usedIDs = new List<string>();
	private List<RecipeFetch> fetches = new List<RecipeFetch>();
	public class RecipeFetch {
		public string id;

		public RecipeFetch(string id) {
			this.id = id;
		}
	}
	public MealDBRecipeSystem(MonoBehaviour host, Dictionary<string, Ingredient> ings, OnFinishLoad on) : base(host, ings) {
		onLoadFinished += on;
	}

	public override void LoadBase() {
		fetches.Clear();
		usedIDs.Clear();
		loadedRecipes.Clear();
	//	for(int i = 0; i < RECIPES; i++) host.StartCoroutine(FetchRecipe());
		host.StartCoroutine(FetchRecipe());
	}

	protected void GenerateIngredientImages(ref Recipe recipe, Image[] UI) {
		host.StartCoroutine(recipe.LoadIngredients(INGREDIENT_LINK, UI));
	}

	private IEnumerator FetchRecipe() {
        UnityWebRequest www = UnityWebRequest.Get(MAIN_LINK + "random.php");
		yield return www.SendWebRequest();

		if(www.isNetworkError || www.isHttpError) Debug.Log(www.error);
		else {
			var txt = www.downloadHandler.text;
			var fetch = new RecipeFetch(txt.Substring(txt.IndexOf("idMeal") + 9, 5));
			
			//Avoid two of the same recipes
			if(usedIDs.Contains(fetch.id)) {
				host.StopCoroutine(FetchRecipe());
				host.StartCoroutine(FetchRecipe());
				yield return null;
			}

			UnityWebRequest wwwSub = UnityWebRequest.Get(MAIN_LINK + "lookup.php?i=" + fetch.id);
			yield return wwwSub.SendWebRequest();

			if(wwwSub.isNetworkError || wwwSub.isHttpError) Debug.Log(wwwSub.error);
			else {
				var subText = wwwSub.downloadHandler.text;
				subText = subText.Substring(10, subText.Length - 12);
				//Debug.Log(subText);
				var recipe = JsonUtility.FromJson<Recipe>(subText);
				recipe = FilterRecipe(ref recipe);

				loadedRecipes.Add(recipe);
				var UI = RecipeSystem.GenerateRecipeUI(recipe);
				
				Debug.Log(FilterInstructions(ref recipe.strInstructions));
				
				GenerateIngredientImages(ref recipe, UI.ingredientIMG);
				usedIDs.Add(fetch.id);
				//if(loadedRecipes.Count < RecipeSystem.MAX_LOADED_RECIPES) yield break;
				onLoadFinished();
			}
		} 
    }

	public override string FilterInstructions(ref string instr) {
		return instr;
	}

	public Recipe FilterRecipe(ref Recipe recipe) {
		recipe.strIngredient1 = Util.SingularizeWord(recipe.strIngredient1);
		recipe.strIngredient2 = Util.SingularizeWord(recipe.strIngredient2);
		recipe.strIngredient3 = Util.SingularizeWord(recipe.strIngredient3);
		recipe.strIngredient4 = Util.SingularizeWord(recipe.strIngredient4);
		recipe.strIngredient5 = Util.SingularizeWord(recipe.strIngredient5);
		recipe.strIngredient6 = Util.SingularizeWord(recipe.strIngredient6);
		recipe.strIngredient7 = Util.SingularizeWord(recipe.strIngredient7);
		recipe.strIngredient8 = Util.SingularizeWord(recipe.strIngredient8);

		string[] ings = {recipe.strIngredient1, recipe.strIngredient2, recipe.strIngredient3, recipe.strIngredient4, recipe.strIngredient5, recipe.strIngredient6, recipe.strIngredient7, recipe.strIngredient8};
		for(int i = 0; i < ings.Length; i++) {
			var ing = ings[i].ToLower().Trim();
			if(ing.Length < 1) continue;
			ing = Util.SingularizeWord(ing);

			if(MatchesIngredient(ing)) Debug.LogError("Contains " + ing + "!!!!!!!!!!");
			else Debug.LogError("Does not contain " + ing);
		}
		return recipe;
	}

	public bool MatchesIngredient(string ing) {
		foreach(KeyValuePair<string, Ingredient> pair in availableIngredients) {
			if(pair.Key.Contains(ing)) { //Recognize keywords such as 'potato' and 'tomato', to distinguish cooking methods from ingredient names. (ex. Chopped onion, Diced tomato) 
				return true;
			}
		}
		return availableIngredients.ContainsKey(ing);
	}

	public override void ScanForRecipe(string txt) {}
}
