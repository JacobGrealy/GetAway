using UnityEngine;
using System.Collections;

/**
 * Bush item.
 */
public class Bush: SpawnedItem {
	
	public GameObject bush1;
//	public GameObject bush2;
//	public GameObject bush3;
	
	public override GameObject GetSpawnedItem() {
		float randomChance = Random.value;
		GameObject bush;
//		if (randomChance < 0.33f) {
			bush = (GameObject) Instantiate(bush1);
			bush.transform.localPosition = new Vector3(0.0f, 0.5f, 0.0f);
			//bush.transform.localRotation.y = bush.transform.eulerAngles.y + 90;
//
//			//bush.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
//		} else if (randomChance < 0.66f) {
//			bush = (GameObject) Instantiate(bush2);
//			bush.transform.localPosition = new Vector3(0.0f, 0.5f, 0.0f);
//			//bush.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
//		} else {
//			bush = (GameObject) Instantiate(bush3);
//			bush.transform.localPosition = new Vector3(0.0f, 0.5f, 0.0f);
//			//bush.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
//		}
		
//		ItemInfo infoComponent = bush.AddComponent<ItemInfo>();
//		infoComponent.itemId = 10006;
//		infoComponent.itemQuantity = 1;
		
		return bush;
	}
}
