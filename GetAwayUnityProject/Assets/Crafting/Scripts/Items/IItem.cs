/*
 * Represents an item in the game, no matter that items location (inventory,
 * game world, etc.)
 * 
 * Each item has an ID, a printed name, and some optional meta-information.
 * The ID is used to lookup the items model, thumbnail image, and whatever
 * other related assets are required to display it in a given context.
 * The printed name, while meta-information, is absolutely required to
 * convey the item's identity to the player (and is thus required).
 * 
 * All other information is optional. Implementations of IItem should
 * take care to properly address the case where a given piece of meta-info
 * does not exist (i.e. return a default, or empty version of the request
 * information).
 */
public interface IItem {

	// The item's ID.
	int Id { get; }

	// The item's printed name (as it appears to the user in-game).
	string Name { get; set; }

	// The item's description (as it appears to the user in-game).
	string Description { get; set; }

	// The damage done by this equippable.
	int Damage { get; set; }

	// The present durability of this equippable.
	int Durability { get; set; }

	// The base durability of this equippable.
	int BaseDurability { get; set; }

	// The percent of the durability remaining for this item.
	int PercentDurabilityRemaining { get; }

	// The hunger relieved by this consumable.
	int Hunger { get; set; }

	// The thirst relieved by this consumable.
	int Thirst { get; set; }

	// The sanity gained by this consumable.
	int Sanity { get; set; }

	// Adds a behavior to the item.
	void AddBehavior(IItemBehavior behavior);

	// Gets a behavior for the item.
	IItemBehavior GetBehavior(ItemBehaviorType type);
	
	// Triggers the behavior on the item. Performs HasBehavior check first.
	void TriggerBehavior(ItemBehaviorType type);
	
	// Returns true of the behavior is present. False otherwise.
	bool HasBehavior(ItemBehaviorType type);
}
