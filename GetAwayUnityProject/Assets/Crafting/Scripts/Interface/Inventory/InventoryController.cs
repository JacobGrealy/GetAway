using UnityEngine;
using System.Collections;

public class InventoryController : MonoBehaviour {

	// The initial and final position of the inventory animation.
	public Vector3 startPosition;
	public Vector3 finalPosition;

	// The item manager for the inventory.
	public InventoryGUIItemManager itemManager;

	private float animTime = 0.0f;
	public const float ANIMATION_TIME_INCREMENT_MODIFIER = 4.5f;
	
	private bool animatingOntoScreen = false;
	private bool animatingOffOfScreen = false;
	private bool inventoryIsActive = false;
	private bool acceptingUserInput = false;
	
	// Use this for initialization
	void Start () {
		transform.localPosition = startPosition;
	}
	
	// Activates the crafting menu into position.
	private void Activate() {
		inventoryIsActive = true;
		animatingOntoScreen = true;
		animatingOffOfScreen = false;
		itemManager.Activate();
	}
	
	// Deactivates the crafting menu.
	private void Deactivate() {
		inventoryIsActive = false;
		acceptingUserInput = false;
		animatingOntoScreen = false;
		animatingOffOfScreen = true;
		itemManager.Deactivate();
	}

	// Update the crafting menu's transform by time (0 to 1).
	// 1 = the menu is accepting user input. 0 = menu is closed.
	private void UpdateTransform(float time) {
		this.transform.localPosition = Vector3.Slerp(startPosition, finalPosition, time);
	}
	
	public void ToggleActive(bool activate) {
		if (activate) Activate();
		else Deactivate();
	}
	
	// Update is called once per frame
	// Note this logic might be weird. Used to be an animation, and its hacked to be instant.
	void Update () {
		if (animatingOntoScreen) {
			animTime = 1.0f;
			UpdateTransform(animTime);
		} else if (animatingOffOfScreen) {
			animTime = 0.0f;
			UpdateTransform(animTime);
		}
		if (animTime > 0.99f && animTime < 1.01f && !acceptingUserInput) {
			acceptingUserInput = true;
			animatingOntoScreen = false;
		} else {
			UpdateTransform(animTime);
		}
	}
}
