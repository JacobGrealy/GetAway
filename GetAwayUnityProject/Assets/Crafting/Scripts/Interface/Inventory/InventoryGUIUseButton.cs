using UnityEngine;
using System.Collections;

public class InventoryGUIUseButton : MonoBehaviour {

	private GameObject playerInventory;
	private Inventory inventory;
	
	public delegate void UserInteraction();
	public event UserInteraction UserClicked;
	
	public Material normalMaterial;
	public Material hoverMaterial;
	public Material xboxMaterial;
	
	private bool mouseOverInThisFrame;
	private bool displayingXboxMaterial = false;

	void Start () {
		playerInventory = GameObject.Find("PlayerInventory");
		inventory = playerInventory.GetComponent<Inventory>();
	}
	
	void OnMouseOver() {
		mouseOverInThisFrame = true;
	}
	
	void OnMouseDown() {
		UserClicked.Invoke();
		inventory.useSound.Play();
	}
	
	public void DisplayXboxMaterial() {
		displayingXboxMaterial = true;
	}
	
	public void HideXboxMaterial() {
		displayingXboxMaterial = false;
	}
	
	void Update() {
		if (displayingXboxMaterial) {
			renderer.material = xboxMaterial;
		} else {
			if (!mouseOverInThisFrame) {
				renderer.material = normalMaterial;
			} else if (mouseOverInThisFrame) {
				renderer.material = hoverMaterial;
			}
			mouseOverInThisFrame = false;
		}
		
		if (Input.GetButton ("Joystick UI Use")) {
			UserClicked.Invoke();
			inventory.useSound.Play();
		}
	}
}
