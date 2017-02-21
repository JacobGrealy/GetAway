using UnityEngine;
using System.Collections;

public class ItemEvent : MonoBehaviour {

	private float animationTime = 0.0f;
	private float animationDistance = 0.0f;
	private float animationTimeElapsed = 0.0f;

	private bool animatingOff = false;
	private bool animatingOn = false;
	private bool animatingDown = false;

	public void AnimateDown(float distance, float time) {
		animationTime = time;
		animationDistance = distance;
		animationTimeElapsed = 0.0f;
		animatingDown = true;
	}

	public void AnimateOn(float time) {
		animationTime = time;
		animatingOn = true;
	}

	public void AnimateOff(float time) {
		animationTime = time;
		animatingOff = true;
	}

	// Update is called once per frame
	void Update () {
		if (animationTimeElapsed < animationTime) {

			float animationAmount = (Time.deltaTime / animationTime);
			transform.Translate(0.0f, -animationDistance * animationAmount, 0.0f);
			animationTimeElapsed += Time.deltaTime;
		}
	}
}
