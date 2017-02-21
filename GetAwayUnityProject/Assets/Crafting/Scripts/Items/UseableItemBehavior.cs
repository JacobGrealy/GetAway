using UnityEngine;
using System.Collections;

public class UseableItemBehavior : IItemBehavior {

	// todo: refactor into something else later
	GameObject itemToUse;

	// The controller enabling the playing to drop things.
	PlayerDropController playerDropController;

	// Construct a useable behavior.
	public UseableItemBehavior(PlayerDropController playerDropController, GameObject itemToUse) {
		this.playerDropController = playerDropController;
		this.itemToUse = itemToUse;
	}

	public void Execute() {
		playerDropController.DropItem(itemToUse);
	}

	public ItemBehaviorType GetBehaviorType() {
		return ItemBehaviorType.USEABLE;
	}
}
