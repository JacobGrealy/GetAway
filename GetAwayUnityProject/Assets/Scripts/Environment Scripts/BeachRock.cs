using UnityEngine;
using System.Collections;

/**
 * Beach rock item.
 */
public class BeachRock: SpawnedItem {

	public GameObject rock1;
	public GameObject rock2;
	public GameObject rock3;

	public override GameObject GetSpawnedItem() {
		float randomChance = Random.value;
		GameObject rock;
		if (randomChance < 0.33f) {
			rock = (GameObject) Instantiate(rock1);
			rock.transform.localPosition = new Vector3(0.0f, 0.5f, 0.0f);
			rock.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
		} else if (randomChance < 0.66f) {
			rock = (GameObject) Instantiate(rock2);
			rock.transform.localPosition = new Vector3(0.0f, 0.5f, 0.0f);
			rock.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
		} else {
			rock = (GameObject) Instantiate(rock3);
			rock.transform.localPosition = new Vector3(0.0f, 0.5f, 0.0f);
			rock.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
		}

		ItemInfo infoComponent = rock.AddComponent<ItemInfo>();
		infoComponent.itemId = 10005;
		infoComponent.itemQuantity = 1;

		return rock;
	}
}
