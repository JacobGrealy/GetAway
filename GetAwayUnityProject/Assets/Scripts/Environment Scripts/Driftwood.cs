using UnityEngine;
using System.Collections;

/**
 * Driftwood item.
 */
public class Driftwood: SpawnedItem {
	
	public GameObject driftwood1;
	public GameObject driftwood2;
	public GameObject driftwood3;
	
	public override GameObject GetSpawnedItem() {
		float randomChance = Random.value;
		GameObject driftwood;
		if (randomChance < 0.33f) {
			driftwood = (GameObject) Instantiate(driftwood1);
			driftwood.transform.localPosition = new Vector3(0.0f, 0.5f, 0.0f);
			//driftwood.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
		} else if (randomChance < 0.66f) {
			driftwood = (GameObject) Instantiate(driftwood2);
			driftwood.transform.localPosition = new Vector3(0.0f, 0.5f, 0.0f);
			//driftwood.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
		} else {
			driftwood = (GameObject) Instantiate(driftwood3);
			driftwood.transform.localPosition = new Vector3(0.0f, 0.5f, 0.0f);
			//driftwood.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
		}
		
		ItemInfo infoComponent = driftwood.AddComponent<ItemInfo>();
		infoComponent.itemId = 10006;
		infoComponent.itemQuantity = 1;
		
		return driftwood;
	}
}
