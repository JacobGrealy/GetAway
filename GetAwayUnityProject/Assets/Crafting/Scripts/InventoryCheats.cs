using UnityEngine;
using System.Collections;

public class InventoryCheats : MonoBehaviour {

	public KeyCode itemsKey;
	public KeyCode weaponsKey;
	public KeyCode foodKey;

	public Inventory inventory;

	void Update() {
		if (Input.GetKeyDown (itemsKey)) {
			// Add an item using its ID and its quantity respectively.
			inventory.AddItem(10000, 1);
			inventory.AddItem(10001, 1);
			inventory.AddItem(10009, 1);
			inventory.AddItem(10011, 1);

			// You can modify the items currently in the game by editing the .json
			// files under Assets/StreamingAssets. Items are globally managed by
			// the singleton "ItemManager".
		}
		if (Input.GetKeyDown (weaponsKey)) {
			inventory.AddItem(1, 1);
			inventory.AddItem(2, 1);
			inventory.AddItem(20, 1);
			inventory.AddItem(40, 1);
			inventory.AddItem(41, 1);
			inventory.AddItem(60, 1);
			inventory.AddItem(1337, 1);
		}
		
		if (Input.GetKeyDown (foodKey)) {
			inventory.AddItem(10004, 1);
			inventory.AddItem(10010, 1);
		}
	}
}
