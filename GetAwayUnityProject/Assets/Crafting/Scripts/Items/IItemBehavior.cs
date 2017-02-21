using UnityEngine;
using System.Collections;

/**
 * An item behavior is a module of behaviour that can be added
 * onto an item at run-time. Each contains an execute method to
 * execute the behavior of the object.
 */
public interface IItemBehavior {

	// Execute the behavior.
	void Execute();

	ItemBehaviorType GetBehaviorType();
}
