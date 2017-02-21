using UnityEngine;
using System.Collections;

public class InterfaceController : MonoBehaviour {

	//Walking Disable
	private GameObject firstPersonController;
	private PlayerWalkSound walkSound;


	// Bag Sounds
	public AudioSource bagZip;
	public AudioSource bagUnzip;

	// Book Sounds
	public AudioSource bookOpen;
	public AudioSource bookClose;

	// The crafting menu.
	public CraftingMenuController craftingMenu;

	// The inventory.
	public InventoryController inventory;

	// The stats screen.
	public StatsController stats;

	// The interface backdrop plane.
	public InterfaceBackdropController backdrop;

	public KeyCode craftingMenuKey = KeyCode.K;
	public KeyCode inventoryKey = KeyCode.I;
	public KeyCode statsKey = KeyCode.Tab;

	private bool craftingMenuActive = false;
	private bool inventoryActive = false;
	
	void Start() {
		// Set the interface items to active at start, so they update atleast once.
		// They're likely initially inactive so that they don't clog up screen space
		// in the scene editor.
		craftingMenu.gameObject.SetActive(true);
		inventory.gameObject.SetActive(true);
		stats.gameObject.SetActive(true);
		backdrop.ToggleActive(false);

		//Walking
		firstPersonController = GameObject.Find("First Person Controller");
		walkSound = firstPersonController.GetComponent<PlayerWalkSound>();
	}

	// Update is called once per frame
	void Update () {
		if (inventoryActive || craftingMenuActive) {
			walkSound.walk.Stop();
		}

		if ((Input.GetKeyDown(craftingMenuKey) || Input.GetButtonDown("Crafting")) && !inventoryActive) {
			craftingMenuActive = !craftingMenuActive;
			craftingMenu.gameObject.SetActive(craftingMenuActive);
			if (craftingMenuActive) {
				bookOpen.Play();
			} else {
				bookClose.Play();
			}
			craftingMenu.ToggleActive(craftingMenuActive);
			backdrop.ToggleActive(craftingMenuActive);
			Screen.showCursor = craftingMenuActive;
		} else if ((Input.GetKeyDown(inventoryKey) || Input.GetButtonDown("Inventory")) && !craftingMenuActive) {
			inventoryActive = !inventoryActive;
			inventory.gameObject.SetActive(inventoryActive);
			if (inventoryActive) {
				bagUnzip.Play();
			} else {
				bagZip.Play();
			}
			inventory.ToggleActive(inventoryActive);
			backdrop.ToggleActive(inventoryActive);
			Screen.showCursor = inventoryActive;
		} else if (Input.GetButtonDown("Stats") || Input.GetKeyDown(statsKey)) {
			stats.SetActive(true);
		} else if (Input.GetButtonUp("Stats") || Input.GetKeyUp(statsKey)) {
			stats.SetActive(false);
		}
	}

	public bool isCraftingMenuActive(){
		return craftingMenuActive;
	}

	public bool isInventoryActive(){
		return inventoryActive;
	}
}
