using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TableOfContents : MonoBehaviour {

	public TableOfContentsRecipe tocItemPrefab;
	public Vector3 defaultRecipePosition;
	public Vector3 perRecipeOffset;
	public BookPageManager bookPageManager;
	public ItemManager itemManager;
	public TextMesh recipesDisplayedText;

	private TableOfContentsRecipe currentSelectedRecipe;
	private Dictionary<Recipe, TableOfContentsRecipe> recipes = new Dictionary<Recipe, TableOfContentsRecipe>();

	private int firstDisplayedRecipe = 0;
	private int totalRecipesDisplayed = 14;
	private int totalRecipes;
	private float timeSinceLastScroll = 0.0f;
	private float timeBetweenScrolls = 0.05f;

	private List<TableOfContentsRecipe> tocRecipes = new List<TableOfContentsRecipe>();

	// Use this for initialization
	void Start () {
		totalRecipes = itemManager.Recipes.Count;
		int count = 0;
		foreach(Recipe recipe in itemManager.Recipes) {
			TableOfContentsRecipe tocRecipe = (TableOfContentsRecipe) Instantiate(tocItemPrefab);
			tocRecipe.itemManager = itemManager;
			tocRecipe.transform.parent = this.transform;
			tocRecipe.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			tocRecipe.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
			tocRecipe.SetRecipe(recipe, count);
			tocRecipe.bookPageManager = bookPageManager;
			tocRecipe.gameObject.SetActive(false);

			recipes.Add (recipe, tocRecipe);
			tocRecipes.Add (tocRecipe);

			count++;
		}
		UpdateTableOfContentsList();
	}

	// Sets the selected recipe in the table of contents.
	public void SetSelectedRecipe(Recipe recipe) {
		if (currentSelectedRecipe != null) {
			currentSelectedRecipe.SetNotSelected();
		}
		if (!recipes[recipe].gameObject.activeSelf) {
			if (recipes[recipe].recipeNumber > currentSelectedRecipe.recipeNumber) {
				// Scroll down
				firstDisplayedRecipe++;
			} else {
				// Scroll up
				firstDisplayedRecipe--;
			}
			UpdateTableOfContentsList();
		}
		currentSelectedRecipe = recipes[recipe];
		currentSelectedRecipe.SetSelected();
	}

	public void Hide(bool hide) {
		gameObject.SetActive(!hide);
	}

	void UpdateTableOfContentsList() {
		int recipeOffsetIncrementer = 0;
		for (int i = 0; i < tocRecipes.Count; i++) {
			if (i >= firstDisplayedRecipe && i < firstDisplayedRecipe + totalRecipesDisplayed) {
				tocRecipes[i].gameObject.SetActive(true);
				tocRecipes[i].transform.localPosition = defaultRecipePosition + (perRecipeOffset * recipeOffsetIncrementer);
				recipeOffsetIncrementer++;
			} else {
				tocRecipes[i].gameObject.SetActive(false);
			}
		}

		// Update "Recipes Displayed" Text
		recipesDisplayedText.text = "Recipes " + (firstDisplayedRecipe + 1) + " through " + (firstDisplayedRecipe + totalRecipesDisplayed + 1) + ":";
	}

	void Update() {
		if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
		{
			if (timeSinceLastScroll < 0.0f) {
				firstDisplayedRecipe = Mathf.Min (firstDisplayedRecipe + 1, totalRecipes - totalRecipesDisplayed);
				UpdateTableOfContentsList();
				timeSinceLastScroll = timeBetweenScrolls;
			}
		}
		else if (Input.GetAxis("Mouse ScrollWheel") > 0) // back
		{
			if (timeSinceLastScroll < 0.0f) {
				firstDisplayedRecipe = Mathf.Max (0, firstDisplayedRecipe - 1);
				UpdateTableOfContentsList();
				timeSinceLastScroll = timeBetweenScrolls;
			}
		}
		timeSinceLastScroll -= Time.deltaTime;
	}
}
