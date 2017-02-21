using UnityEngine;
using System.Collections;

public class FallingBear : MonoBehaviour {

	// Maximum possible down-scaling.
	private const float RANDOM_SCALE_FACTOR = 0.6f;

	// Max torque amount per-axis. Final torque is random from 0.
	private const int RANDOM_TORQUE_FACTOR = 180;

	// Minimum position (y-axis) a bear can have before being destroyed.
	private const float BEAR_POSITION_CULLING_Y = -15.0f;

	void Start () {
		float randomScale = Random.Range(0.0f, RANDOM_SCALE_FACTOR);
		this.transform.localScale -= new Vector3(randomScale, randomScale, randomScale);

		rigidbody.AddRelativeTorque (
			Random.Range(-RANDOM_TORQUE_FACTOR, RANDOM_TORQUE_FACTOR),
			Random.Range(-RANDOM_TORQUE_FACTOR, RANDOM_TORQUE_FACTOR),
			Random.Range(-RANDOM_TORQUE_FACTOR, RANDOM_TORQUE_FACTOR)
		);
	}

	void Update () {
		if (this.transform.position.y <= BEAR_POSITION_CULLING_Y) {
			Destroy(this.gameObject);
		}
	}
}
