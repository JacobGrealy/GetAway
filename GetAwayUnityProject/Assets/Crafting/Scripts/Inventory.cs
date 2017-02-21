using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * This is the non-graphical representation of an inventory.
 * It is purely a data class which provides the methods necessary
 * to add items to the inventory, remove items from the inventory,
 * and otherwise modify the items within the inventory.
 * 
 * A boolean flag, inventoryUpdated, is maintained throughout.
 * Whenever the inventory is updated, this variable should be set
 * to true. During the update cycle, when this variable is true,
 * the InventoryUpdated() event is dispatched. An example use of
 * this event is the Inventory GUI. The Inventory GUI is described
 * in more detail in its related classes - but, the idea is that it
 * listens for this event and updates the graphical display of the
 * inventory to match the non-graphical representation of the
 * inventory found within.
 * 
 * Make sense :)? If not, talk to Zach!
 */
public class Inventory : MonoBehaviour {

	//Sounds
	public AudioSource consumeSound;
	public AudioSource equipSound;
	public AudioSource destroySound;
	public AudioSource useSound;

	// Inventory capacity.
	public int inventoryCapacity = 40;

	// Items currently in the player's posession. (int:ItemID, int:quantity).
	public Dictionary<int, int> items { get; private set; }

	// Subscribable inventory events.
	public delegate void InventoryAction();
	public event InventoryAction InventoryUpdated;	
	public delegate void InventoryItemActionEvent(int itemId);
	public event InventoryItemActionEvent ItemDestroyed;
	public event InventoryItemActionEvent ItemEquipped;
	public event InventoryItemActionEvent ItemConsumed;

	// Item Manager.
	public ItemManager itemManager;

	// Inventory maintenance management.
	private int framesBetweenCleans = 100;
	private int framesSinceLastClean;

	// True whenever the inventory has been updated.
	private bool inventoryUpdated = false;

	// Single statically referenceable inventory (:\)
	private static Inventory instance;

	void Start () {
		items = new Dictionary<int, int>();
		instance = this;
	}

	public static Inventory GetInstance() {
		return instance;
	}

	public void TriggerItemAction(InventoryItemAction action, int itemId) {
		switch (action) {
			
			case InventoryItemAction.EQUIP:
				if (ItemEquipped != null) {
					ItemEquipped(itemId);
				}
				break;
				
			case InventoryItemAction.CONSUME:
			if (ItemConsumed != null) {
					ItemConsumed(itemId);
				}
				break;
				
			case InventoryItemAction.DESTROY:
				if (ItemDestroyed != null) {
					ItemDestroyed(itemId);
				}
				break;
			}
	}

	// Adds a quantity of an item to the inventory. If the item does
	// not exist, the item is added to the inventory first.
	public void AddItem(int itemId, int quantity) {
		if (items.Count < inventoryCapacity) {

			if (items.ContainsKey(itemId)) {
				items[itemId] += quantity; // increment quantity
			} else {
				items.Add (itemId, quantity);
			}
			inventoryUpdated = true;
		}
	}

	// Remove a quantity of an item from the inventory.
	public void RemoveItem(int itemId, int quantity) {
		if (items.ContainsKey(itemId)) {
			items[itemId] = Mathf.Max(0, items[itemId] - quantity);
			inventoryUpdated = true;
		}
	}

	// Remove a quantity of an item from the inventory.
	public void RemoveItem(IItem item, int quantity) {
		RemoveItem (item.Id, 1);
	}

	// Returns the strongest weapon in the inventory of a given type.
	public IItem GetStrongestWeapon(EquipableItem itemType) {
		IItem topPick = null;
		int topPickDamage = 0;
		foreach (KeyValuePair<int, int> itemPair in items) {
			IItem item = itemManager.GetItem(itemPair.Key);
		
			if (item.HasBehavior(ItemBehaviorType.EQUIPABLE)) {
				EquipableItemBehavior behavior = (EquipableItemBehavior) item.GetBehavior(ItemBehaviorType.EQUIPABLE);

				if (behavior.itemType == itemType) {

					if (behavior.item.Damage > topPickDamage) {
						topPick = item;
					}
				}
			}
		}

		return topPick;
	}

	// Remove items of 0 quantity from the list. We can only
	// modify the items array during an update.
	private void Clean() {
		IList<int> itemIdsToDelete = new List<int>();
		foreach (int itemId in items.Keys) {
			int quantity = items[itemId];
			if (quantity <= 0) {
				itemIdsToDelete.Add(itemId);
			}
		}
		foreach (int itemId in itemIdsToDelete) {
			items.Remove(itemId);
		}
	}

	// Update is called once per frame
	void Update () {
		framesSinceLastClean--;
		if (framesSinceLastClean < 0) {
			framesSinceLastClean = framesBetweenCleans;
			Clean();
		}
		if (inventoryUpdated) {
			this.InventoryUpdated();
			inventoryUpdated = false;
		}
	}
}
