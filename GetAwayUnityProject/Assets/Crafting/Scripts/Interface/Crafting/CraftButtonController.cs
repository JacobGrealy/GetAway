using UnityEngine;
using System.Collections;

public class CraftButtonController : MonoBehaviour {

	private GameObject craftingMenuController;
	private CraftingMenuController crafting;

	public Material normal;
	public Material hovered;
	public Material clicked;
	public Material missing;
	public Material normalXbox;
	public Material hoveredXbox;
	public Material clickedXbox;
	public Vector3 clickedRotation;

	public delegate void UserAction();
	public event UserAction Clicked;

	private bool craftable = false;
	private bool xboxControllerPresent = false;

	private float debounceTimeLeft = 0.0f;
	private float debounceTime = 0.2f;

	void Start () {
		craftingMenuController = GameObject.FindGameObjectWithTag("CraftingBook");
		crafting = craftingMenuController.GetComponent<CraftingMenuController>();

		if (Input.GetJoystickNames().Length > 0) {
			xboxControllerPresent = true;
		}
		SetMaterialNormal();
	}

	public void SetCraftable(bool craftable) {
		this.craftable = craftable;
		if (craftable) SetMaterialNormal();
		else SetMaterialMissing();
	}

	void OnMouseEnter() {
		if (craftable) {
			SetMaterialHovered();
		}
	}

	void OnMouseExit() {
		if (craftable) {
			SetMaterialNormal();
		}
	}

	void OnMouseDown() {
		if (craftable) {
			SetMaterialClicked();
			if (Clicked != null) {
				Clicked();
				crafting.craftingSound.Play();
			}
		}
	}

	void OnMouseUp() {
		if (craftable) {
			SetMaterialHovered();
		}
	}

	void SetMaterialNormal() {
		renderer.material = xboxControllerPresent ? normalXbox : normal;
	}
	
	void SetMaterialHovered() {
		renderer.material = xboxControllerPresent ? hoveredXbox : hovered;
	}
	
	void SetMaterialClicked() {
		renderer.material = xboxControllerPresent ? clickedXbox : clicked;
	}

	void SetMaterialMissing() {
		renderer.material = missing;
	}

	void Update () {
		if (craftable && Input.GetButton ("Joystick UI Craft Button") && debounceTimeLeft < 0) {
			Clicked();
			debounceTimeLeft = debounceTime;
			crafting.craftingSound.Play();
		}
		debounceTimeLeft -= Time.deltaTime;
		if (!craftable) {
			SetMaterialMissing();
		}
	}
}
