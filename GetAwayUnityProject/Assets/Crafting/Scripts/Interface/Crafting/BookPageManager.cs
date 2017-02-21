using UnityEngine;
using System.Collections;

public class BookPageManager : MonoBehaviour {

	//Sounds
	public AudioSource pageTurnSound;

	public TableOfContentsPageController tocPage;
	public RecipePageController recipePage;
	public TableOfContents tableOfContents;
	public ItemManager itemManager;

	private bool recipeChanged = false;
	private int numRecipes;
	private int currentRecipe;

	// Settings for joystick controls.
	private float lastJoystickMovement = 0;
	private float timeBetweenJoystickMovements = 0.1f;

	// Use this for initialization
	void Start () {
		numRecipes = itemManager.Recipes.Count;
		currentRecipe = 0;
		recipeChanged = true;
	}

	public void FlipToCraftingRecipe(int recipeNumber) {
		currentRecipe = recipeNumber;
		recipeChanged = true;
	}

	public void ShowRecipe(int recipeNumber) {
		recipePage.SetRecipe(itemManager.GetRecipe(recipeNumber));
		tableOfContents.SetSelectedRecipe(itemManager.GetRecipe(recipeNumber));
	}

	// Update is called once per frame
	void Update () {
		if (lastJoystickMovement < 0) {
			if (Input.GetAxisRaw("Joystick UI Y") < -0.3) { // Up
				if (currentRecipe > 0) {
					currentRecipe--;
					recipeChanged = true;
				}
				lastJoystickMovement = timeBetweenJoystickMovements;
			}
			if (Input.GetAxisRaw("Joystick UI Y") > 0.3) { // Down
				if (currentRecipe < (numRecipes - 1)) {
					currentRecipe++;
					recipeChanged = true;
				}
				lastJoystickMovement = timeBetweenJoystickMovements;
			}
		} else {
			lastJoystickMovement -= Time.deltaTime;
		}

		if (recipeChanged) {
			ShowRecipe(currentRecipe);
			recipeChanged = false;
		}
	}
}
