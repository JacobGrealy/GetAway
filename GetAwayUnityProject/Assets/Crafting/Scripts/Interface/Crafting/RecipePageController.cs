using UnityEngine;
using System.Collections;

public class RecipePageController : MonoBehaviour {

	// Game objects related to the recipe.
	public GameObject recipeObject;
	public TextMesh recipeTitle;
	public MeshRenderer recipeIcon;
	public MeshRenderer bookPageMesh;
	public TextMesh craftableText;
	public CraftButtonController craftButton;

	// The ingredient gameobjects for the recipe.
	public RecipePageIngredient[] recipeIngredients;

	// The player inventory that this crafting will affect.
	public Inventory playerInventory;

	// The Item Manager to draw items from.
	public ItemManager itemManager;

	// The recipe being represented.
	private Recipe recipe;

	// Can the item be crafted?
	private bool canCraft;

	void Start() {
		playerInventory.InventoryUpdated += UpdateState;
		craftButton.Clicked += Craft;
	}

	public void SetRecipe(Recipe recipe) {
		this.recipe = recipe;

		// Set the page's correct recipe material.
		string pageTexturePath = "Crafting/page-blank-right-" + recipe.Ingredients.Count;
		bookPageMesh.material.mainTexture = (Texture2D) Resources.Load(pageTexturePath);

		// Setup the recipe's header.
		IItem craftedItem = itemManager.GetItem(recipe.CraftedItemId);
		recipeTitle.text = craftedItem.Name;
		string iconTexturePath = "Items/icon-" + recipe.CraftedItemId;
		recipeIcon.material.mainTexture = (Texture2D) Resources.Load(iconTexturePath);

		UpdateState();
	}

	private int GetCurrentPossessedForItem(int itemId) {
		if (playerInventory.items.ContainsKey(itemId)) {
			return playerInventory.items[itemId];
		} else {
			return 0;
		}
	}

	// Craft 1 of the item created by the recipe.
	private void Craft() {
		// Add crafted item
		playerInventory.AddItem(recipe.CraftedItemId, 1);

		// Remove ingredients
		foreach (DictionaryEntry ingredient in recipe.Ingredients) {
			playerInventory.RemoveItem((int) ingredient.Key, (int) ingredient.Value); 
		}
	}

	// Update the ingredient counts, craft button status, etc.
	private void UpdateState() {
		// Update the recipe's ingredients.
		int i = 0;
		foreach (DictionaryEntry ingredient in recipe.Ingredients) {
			int possessed = GetCurrentPossessedForItem((int) ingredient.Key);
			recipeIngredients[i].UpdateIngredient((int) ingredient.Key, possessed, (int) ingredient.Value);
			i++;
		}
		
		// Set the rest of the recipe ingredients to blanks.
		while (i < 3) {
			recipeIngredients[i].Empty();
			i++;
		}

		// Update craft button
		int numCraftable = int.MaxValue;
		foreach (RecipePageIngredient ingredient in recipeIngredients) {
			if (!ingredient.IsEmpty() && ingredient.RequirementIsMet()) {
				numCraftable = Mathf.Min (numCraftable, ingredient.GetNumCraftable());
			} else if (!ingredient.IsEmpty()) {
				numCraftable = 0;
				break;
			}
		}
		craftButton.SetCraftable(numCraftable > 0);
		craftableText.text = numCraftable + " Can Be Crafted";
	}
}
