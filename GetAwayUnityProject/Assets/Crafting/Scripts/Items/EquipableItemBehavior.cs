using UnityEngine;
using System.Collections;

public class EquipableItemBehavior : IItemBehavior {

	private PlayerEquip playerEquipController;
	public EquipableItem itemType { get; private set; }
	public IItem item { get; private set; }

	// Construct an equipable behavior.
	public EquipableItemBehavior(PlayerEquip playerEquipController, EquipableItem itemType, IItem item) {
		this.playerEquipController = playerEquipController;
		this.itemType = itemType;
		this.item = item;
	}

	// Returns a duplicate of this behavior. This is useful when the item
	// using this behavior has been duplicated, and thus needs a duplicated
	// behavior to accompany it.
	public IItemBehavior DuplicateBehavior() {
		return new EquipableItemBehavior(playerEquipController, itemType, item);
	}

	public void Execute() {
		playerEquipController.setEquip(itemType, item);
	}

	public ItemBehaviorType GetBehaviorType() {
		return ItemBehaviorType.EQUIPABLE;
	}
}
