using UnityEngine;
using System.Collections;

public class BearSpawnerController : MonoBehaviour {

	private const float BEAR_SPAWN_POSITION_RANGE_X = 15.0f;
	private const float BEAR_SPAWN_POSITION_Y = 15.0f;
	private const float BEAR_SPAWN_POSITION_RANGE_Z = 10.0f;

	public FallingBear bear;

	// Spawns a bear at a random location.
	public void SpawnBear() {
		float randomBearX = Random.Range(-BEAR_SPAWN_POSITION_RANGE_X, BEAR_SPAWN_POSITION_RANGE_X);
		float randomBearZ = Random.Range(-BEAR_SPAWN_POSITION_RANGE_Z, BEAR_SPAWN_POSITION_RANGE_Z);

		Vector3 randomPosition = new Vector3(randomBearX, BEAR_SPAWN_POSITION_Y, randomBearZ);
		
		FallingBear newBear = (FallingBear) Instantiate(bear, randomPosition, Quaternion.Euler(new Vector3(0, 0, 0)));
		newBear.transform.parent = gameObject.transform;
	}

	void Update () {
	}
}
