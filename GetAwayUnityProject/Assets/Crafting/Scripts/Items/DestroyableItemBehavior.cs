using UnityEngine;
using System.Collections;

public class DestroyableItemBehavior : IItemBehavior {

	// Construct a destroyable behavior.
	public DestroyableItemBehavior() {
	}
	
	public void Execute() {
		// ???
		// Maybe play some sort of animation for the player?
		// All gc is handled by C#. No destructing required.
	}
	
	public ItemBehaviorType GetBehaviorType() {
		return ItemBehaviorType.DESTROYABLE;
	}
}
