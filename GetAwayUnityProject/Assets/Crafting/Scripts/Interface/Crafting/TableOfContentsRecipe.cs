using UnityEngine;
using System.Collections;

public class TableOfContentsRecipe : MonoBehaviour {

	public MeshRenderer recipeIcon;
	public TextMesh recipeText;
	public Color normalTextColor;
	public Color hoveredTextColor;

	public BookPageManager bookPageManager;
	public ItemManager itemManager;

	public int recipeNumber;

	private bool isHovered = false;
	private bool mouseOverInThisFrame = false;

	void Start() {
		recipeText.color = normalTextColor;
	}

	public void SetRecipe(Recipe recipe, int recipeNumber) {
		this.recipeNumber = recipeNumber;

		IItem recipeItem = itemManager.GetItem(recipe.CraftedItemId);
		recipeText.text = recipeItem.Name;

		recipeIcon.material.shader = Shader.Find ("Unlit/Transparent");
		string iconTexturePath = "Items/icon-" + recipe.CraftedItemId;
		recipeIcon.material.mainTexture = (Texture2D) Resources.Load(iconTexturePath);
	}

	// Call if this recipe is the one currently selected by the user.
	public void SetSelected() {
		recipeText.color = hoveredTextColor;
	}

	// Call if this recipe is no longer the one currently selected by the user.
	public void SetNotSelected() {
		recipeText.color = normalTextColor;
	}

	void OnMouseEnter() {
		recipeText.color = hoveredTextColor;
	}
	
	void OnMouseOver() {
		mouseOverInThisFrame = true;
	}
	
	void OnMouseExit() {
		isHovered = false;
		recipeText.color = normalTextColor;
	}

	void OnMouseDown() {
		bookPageManager.FlipToCraftingRecipe(recipeNumber);
		bookPageManager.pageTurnSound.Play();
	}

	void Update() {
		// If mouse hover inconsistency, fix it.
		if (!mouseOverInThisFrame && isHovered) {
			isHovered = false;
			recipeText.color = normalTextColor;
		}
		mouseOverInThisFrame = false;
	}
}
