using UnityEngine;
using System.Collections;

public class LoadingCameraFaderController : MonoBehaviour {

	private Plane cameraFadingPlane;

	private bool decreasingAlpha = false;
	private bool increasingAlpha = false;

	private const float FADE_CONSTANT = 0.6f;

	public void FadeIn() {
		decreasingAlpha = true;
	}

	public void FadeOut() {
		increasingAlpha = true;
	}

	void Update () {
		if (increasingAlpha && renderer.material.color.a >= 0.95f) {
			increasingAlpha = false;
			this.renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 1.0f);
			StartupEventManager.CameraStateChanged(StartupEventManager.CameraState.FadedOut);
		}
		if (decreasingAlpha && renderer.material.color.a <= 0.05f) {
			decreasingAlpha = false;
			this.renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 0.0f);
			StartupEventManager.CameraStateChanged(StartupEventManager.CameraState.FadedIn);
		}
		if (decreasingAlpha) {
			float alpha = renderer.material.color.a - FADE_CONSTANT * Time.deltaTime;
			this.renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, alpha);
		} else if (increasingAlpha) {
			float alpha = renderer.material.color.a + FADE_CONSTANT * Time.deltaTime;
			this.renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, alpha);
		}
	}
}
