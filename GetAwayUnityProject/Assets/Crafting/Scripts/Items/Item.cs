using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * A standard implementation of the IItem which has support for
 * run-time defined behaviors (e.g. consumable items).
 */
public class Item : IItem {

	// Item properties
	public int Id { get; private set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public int Damage { get; set; }
	public int Durability { get; set; }
	public int BaseDurability { get; set; }
	public int PercentDurabilityRemaining { 
		get {
			float percent = (float) Durability / (float) BaseDurability * 100;
			return Mathf.RoundToInt(percent);
		}
		private set {}
	}
	public int Hunger { get; set; }
	public int Thirst { get; set; }
	public int Sanity { get; set; }

	// Item behaviors.
	private Dictionary<ItemBehaviorType, IItemBehavior> behaviors;

	public Item(int id) {
		this.Id = id;
		behaviors = new Dictionary<ItemBehaviorType, IItemBehavior>();
	}

	// Duplicate an item. This is a hack to enable managing equipables as
	// separate items with individual state. 
	//
	// WARNING: This is intended to be used by equipable items only.
	public IItem DuplicateItem() {
		IItem item = new Item(Id);
		item.Name = Name;
		item.Description = Description;
		item.Damage = Damage;
		item.Durability = Durability;
		item.BaseDurability = BaseDurability;

		if (behaviors.ContainsKey(ItemBehaviorType.EQUIPABLE)) {
			EquipableItemBehavior originalBehavior = (EquipableItemBehavior) behaviors[ItemBehaviorType.EQUIPABLE];
			IItemBehavior duplicateBehavior = originalBehavior.DuplicateBehavior();
			item.AddBehavior(duplicateBehavior);
		}

		return item;
	}

	// Adds a behavior to the item.
	public void AddBehavior(IItemBehavior behavior) {
		if (!behaviors.ContainsKey(behavior.GetBehaviorType())) {
			behaviors.Add(behavior.GetBehaviorType(), behavior);
		}
	}
	
	// Adds a behavior to the item.
	public void TriggerBehavior(ItemBehaviorType behavior) {
		if (HasBehavior(behavior)) {
			behaviors[behavior].Execute();
		}
	}

	public IItemBehavior GetBehavior(ItemBehaviorType behavior) {
		if (behaviors.ContainsKey(behavior)) {
		    return behaviors[behavior];
		} else {
		    return null;
		}
	}

	// Returns true of the behavior is present. False otherwise.
	public bool HasBehavior(ItemBehaviorType type) {
		return behaviors.ContainsKey(type);
	}
}
