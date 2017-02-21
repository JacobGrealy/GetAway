using UnityEngine;
using System.Collections;

/**
 * Describes an item which can be spawned into the world. To save
 * on Unity Update() calls, please avoid having your spawnable item
 * extend MonoBehaviour if possible.
 * 
 * Items implementing this interface will generally be controlled by
 * some other spawner that will take care of doing stuff during Update()
 * calls.
 */
public abstract class SpawnedItem : MonoBehaviour {

	// Spawn the item.
	public abstract GameObject GetSpawnedItem();
}
