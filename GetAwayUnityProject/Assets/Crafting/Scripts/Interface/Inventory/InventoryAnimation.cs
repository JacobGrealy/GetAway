using UnityEngine;
using System.Collections;

public class InventoryAnimation : MonoBehaviour {
	
	public Vector3 startPosition;
	public Vector3 finalPosition;
	
	public Vector3 startScale;
	public Vector3 finalScale;
		
	private float animTime = 0.0f;
	public const float ANIMATION_TIME_INCREMENT_MODIFIER = 4.5f;
	
	private bool animatingOntoScreen = false;
	private bool animatingOffOfScreen = false;
	private bool inventoryIsActive = false;
	private bool acceptingUserInput = false;
	
	// Use this for initialization
	void Start () {
		transform.localPosition = startPosition;
		transform.localScale = startScale;
	}
	
	// Activates the crafting menu into position.
	private void Activate() {
		inventoryIsActive = true;
		animatingOntoScreen = true;
		animatingOffOfScreen = false;
	}
	
	// Deactivates the crafting menu.
	private void Deactivate() {
		inventoryIsActive = false;
		acceptingUserInput = false;
		animatingOntoScreen = false;
		animatingOffOfScreen = true;
	}
	
	// Update the crafting menu's transform by time (0 to 1).
	// 1 = the menu is accepting user input. 0 = menu is closed.
	private void UpdateTransform(float time) {
		this.transform.localPosition = Vector3.Slerp(startPosition, finalPosition, time);
		this.transform.localScale = Vector3.Slerp(startScale, finalScale, time);
	}
	
	public void ToggleActive() {
		if (!inventoryIsActive) Activate();
		else Deactivate();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.I)) {
			if (!inventoryIsActive) Activate();
			else Deactivate();
		}

		if (animatingOntoScreen) {
			animTime = Mathf.Min(animTime + (Time.deltaTime * ANIMATION_TIME_INCREMENT_MODIFIER), 1.0f);
		} else if (animatingOffOfScreen) {
			animTime = Mathf.Max(animTime - (Time.deltaTime * ANIMATION_TIME_INCREMENT_MODIFIER), 0.0f);
		}
		if (animTime > 0.99f && animTime < 1.01f && !acceptingUserInput) {
			print ("here");
			acceptingUserInput = true;
			animatingOntoScreen = false;
		} else {
			UpdateTransform(animTime);
		}
	}
}
