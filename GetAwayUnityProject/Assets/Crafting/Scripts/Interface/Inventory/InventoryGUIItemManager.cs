using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * The graphical representation of the inventory.
 */
public class InventoryGUIItemManager : MonoBehaviour {

	// A cloneable prefab of an inventory item.
	public InventoryGUIItem inventoryItemCloneable;

	// The size of the graphical inventory. This should match
	// the size of the inventory this GUI item is representing.
	public int inventoryItemsHorizontal;
	public int inventoryItemsVertical;

	// The horizontal and vertical spacing between each inventory
	// item. Modify this to control the size of the inventory.
	public float inventoryItemsSpacingHorizontal;
	public float inventoryItemsSpacingVertical;

	// The inventory being represented by this GUI element.
	public Inventory inventory;

	// The item manager to draw items from.
	public ItemManager itemManager;

	// The total number of inventory items (horizontal * vertical).
	private int totalInventoryItems;

	// A collection of the items being represented. Indexed from
	// 0 to "totalInventoryItems".
	private IList<InventoryGUIItem> items;

	// A collection of the spaces available for representing an item.
	// This starts off containing every space (0 through "totalInventoryItems")
	// and works its way towards containing nothing as items are added.
	private IList<int> availableSpaces;

	// A collection of the spaces presently being occupied. As the inventory
	// is used, this will contain an increasingly fragmented subset of
	// its sister collection, availableSpaces. Each servces a precisely
	// different but necessary purpose.
	private Hashtable occupiedSpaces;

	// Item (index in the items List) last selected by joystick.
	private int joystickSelectedItem;

	// Settings for joystick controls.
	private float lastJoystickMovement = 0;
	private float timeBetweenJoystickMovements = 0.1f;

	void Start() {
		items = new List<InventoryGUIItem>();
		totalInventoryItems = inventoryItemsHorizontal * inventoryItemsVertical;
		availableSpaces = new List<int>();
		occupiedSpaces = new Hashtable();

		GenerateInventoryPlaceholders();

		inventory.InventoryUpdated += UpdateAllItems;
	}

	// Updates the quantity of a single item in the inventory. If the item
	// of itemId being updated is not yet in the inventory, it is added.
	// See "AddItem(...)" for more information on conditions under which
	// things go awry.
	public void UpdateItem(int itemId, int quantity) {
		if (occupiedSpaces.ContainsKey (itemId)) {
			InventoryGUIItem item = GetInventoryItem ((int) occupiedSpaces[itemId]);
			item.SetQuantity(quantity);
		} else {
			AddItem (itemId, quantity);
		}
	}
	
	// Updates all items in the inventory interface. This is useful to call
	// when the underlying inventory data structure has been modified,
	// and the entire graphical interface needs to be updated to reflect it.
	public void UpdateAllItems() {
		foreach (KeyValuePair<int, int> item in inventory.items) {
			if (item.Value <= 0) {
				DeleteItem(item.Key);
			} else {
				UpdateItem(item.Key, item.Value);
			}
		}
	}

	// Deletes an item of itemId from the inventory. The space it occupied
	// is marked as available for a future item to use. The item no longer
	// appears in the graphical interface.
	public void DeleteItem(int itemId) {
		if (occupiedSpaces.ContainsKey(itemId)) {
			int spaceNumber = (int) occupiedSpaces[itemId];

			// Reset the GUI item.
			InventoryGUIItem item = GetInventoryItem(spaceNumber);
			item.ClearItem();

			// Update available spaces, occupied spaces.
			availableSpaces.Add(spaceNumber);
			occupiedSpaces.Remove(itemId);
		}
	}

	// Adds an item with an initial quantity to the graphical inventory.
	// If no space exists, nothing happens.
	public void AddItem(int itemId, int quantity) {
		int space = GetSpace(itemId);

		if (space < 0) {
			// TODO: alert the client of this script.
		} else {
			GetInventoryItem(space).IncrementQuantity(quantity);
		}
	}
	
	// Call when the inventory is being opened. Some house-cleaning is performed
	// here.
	public void Activate() {
		if (Input.GetJoystickNames().Length > 0) {
			joystickSelectedItem = 0;
			items[joystickSelectedItem].SetHoveredByJoystick();
		}
	}

	// Call when the inventory is being closed. Some house-cleaning is performed
	// here.
	public void Deactivate() {
		items[joystickSelectedItem].SetNotHoveredByJoystick();
	}

	// Gets a space in the graphical inventory for an item of
	// itemId. If the item with itemId is already in the graphical
	// inventory, then its space is simply returned. Otherwise,
	// a space is found for it, and that space is returned.
	//
	// If a space cannot be allocated (i.e. inventory is full),
	// then a value -1 is returned.
	private int GetSpace(int itemId) {
		if (occupiedSpaces.ContainsKey(itemId)) {
			return (int) occupiedSpaces[itemId];

		} else if (availableSpaces.Count > 0) {
			int space = GetFirstAvailableSpace();
			availableSpaces.Remove(space);
			occupiedSpaces.Add (itemId, space);
			GetInventoryItem(space).SetItem(itemId);
			return space;

		} else {
			return -1;
		}
	}

	// Returns the first available space (i.e. the lowest valued space
	// in the availableSpaces array).
	private int GetFirstAvailableSpace() {
		int smallestSpace = availableSpaces[0];
		for (int i = 1; i < availableSpaces.Count; i++) {
			if (availableSpaces[i] < smallestSpace) {
				smallestSpace = availableSpaces[i];
			}
		}
		return smallestSpace;
	}

	// Gets the inventory item with index "num" in the internal
	// data structure of items.
	private InventoryGUIItem GetInventoryItem(int num) {
		return items[num];
	}

	// Gets the inventory item at grid position (x, y).
	private InventoryGUIItem GetInventoryItem(int x, int y) {
		return GetInventoryItem(y * inventoryItemsVertical + x);
	}

	// Generates the grid of placeholders that make up the graphical
	// inventory interface. Each placeholder is a prefab which contains
	// everything required to ultimately be an item.
	private void GenerateInventoryPlaceholders() {
		int i = 0;
		for(int y = inventoryItemsVertical - 1; y >= 0; y--) {
			for(int x = 0; x < inventoryItemsHorizontal; x++) {
				InventoryGUIItem inventoryItem = (InventoryGUIItem) Instantiate(inventoryItemCloneable);
				inventoryItem.transform.parent = transform;
				inventoryItem.transform.localPosition = GetInventoryItemPosition(x, y);
				inventoryItem.inventory = inventory;
				inventoryItem.itemManager = itemManager;

				items.Add(inventoryItem);
				availableSpaces.Add(i++);
			}
		}
	}

	// Gets the position vector for an inventory item at a given grid (x, y)
	// position. This is a utility method of GenerateInventoryPlaceholders(),
	// and isn't really otherwise useful to you. So... go away.
	private Vector3 GetInventoryItemPosition(int gridX, int gridY) {
		float start = 55.0f;
		float end = 125.0f;
		float modifierX = 1.0f;
		float modifierY = 1.5f;

		float percentX = (float) gridX / (inventoryItemsHorizontal - 1);
		float percentY = (float) gridY / (inventoryItemsVertical - 1);

		float point = start + (end - start) * percentX;
		float pointTop = start + (end - start) * 0.5f;
		float radius = 0.0f + percentY * modifierY;

		float x = (-percentY * 0.5f) + 0.0f + (2.5f + (percentY * 1.0f)) * percentX;
		float y = percentY + modifierY * (Mathf.Sin(point / 180.0f * Mathf.PI));
		float z = 0.0f;

		float centerModifierX = (-percentY * 0.5f) + 0.0f + (2.5f + (percentY * 1.0f)) * 0.5f;
		float centerModifierY = 0.5f + modifierY * (Mathf.Sin(pointTop / 180.0f * Mathf.PI));

		return (new Vector3(x - centerModifierX, y - centerModifierY, z));
	}
	
	void Update () {
		if (lastJoystickMovement < 0) {
			if (Input.GetAxisRaw("Joystick UI X")> 0.3) { // Right
				if (joystickSelectedItem < ((inventoryItemsHorizontal - 1) % inventoryItemsHorizontal)) {
					// Set current item to a "not hovered" state.
					items[joystickSelectedItem].SetNotHoveredByJoystick();

					// Set next item to a "hovered" state.
					joystickSelectedItem++;
					while (!items[joystickSelectedItem].ItemIsSet() && joystickSelectedItem > 0) {
						joystickSelectedItem--;
					}
					InventoryGUIItem item = items[joystickSelectedItem];
					item.SetHoveredByJoystick();

					lastJoystickMovement = timeBetweenJoystickMovements;
				}
			}
			if (Input.GetAxisRaw("Joystick UI X") < -0.3) { // Left
				if (joystickSelectedItem > 0) {
					// Set current item to a "not hovered" state.
					items[joystickSelectedItem].SetNotHoveredByJoystick();
					
					// Set next item to a "hovered" state.
					joystickSelectedItem--;
					InventoryGUIItem item = items[joystickSelectedItem];
					item.SetHoveredByJoystick();
					
					lastJoystickMovement = timeBetweenJoystickMovements;
				}
			}
			if (Input.GetAxisRaw("Joystick UI Y") < -0.3) { // Up
				if (joystickSelectedItem > (inventoryItemsHorizontal - 1)) {
					// Set current item to a "not hovered" state.
					items[joystickSelectedItem].SetNotHoveredByJoystick();
					
					// Set next item to a "hovered" state.
					joystickSelectedItem -= inventoryItemsHorizontal;
					while (!items[joystickSelectedItem].ItemIsSet() && joystickSelectedItem > 0) {
						joystickSelectedItem--;
					}
					InventoryGUIItem item = items[joystickSelectedItem];
					item.SetHoveredByJoystick();
					
					lastJoystickMovement = timeBetweenJoystickMovements;
				}
			}
			if (Input.GetAxisRaw("Joystick UI Y") > 0.3) { // Down
				if (joystickSelectedItem / inventoryItemsHorizontal < (inventoryItemsVertical - 1)) {
					// Set current item to a "not hovered" state.
					items[joystickSelectedItem].SetNotHoveredByJoystick();

					// Set next item to a "hovered" state.
					joystickSelectedItem += inventoryItemsHorizontal;
					while (!items[joystickSelectedItem].ItemIsSet() && joystickSelectedItem > 0) {
						joystickSelectedItem--;
					}
					InventoryGUIItem item = items[joystickSelectedItem];
					item.SetHoveredByJoystick();
					
					lastJoystickMovement = timeBetweenJoystickMovements;
				}
			}
		} else {
			lastJoystickMovement -= Time.deltaTime;
		}
	}
}
