using UnityEngine;
using System.Collections;

public class FallingBearLogoController : MonoBehaviour {

	private bool isFadingIn = false;

	private const float FADE_CONSTANT = 0.6f;

	// Use this for initialization
	void Start () {

	}

	public void FadeIn() {
		isFadingIn = true;
	}

	// Update is called once per frame
	void Update () {
		if (isFadingIn) {
			float alpha = Mathf.Lerp(renderer.material.color.a, 1.0F, Time.deltaTime * FADE_CONSTANT);
			renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, alpha);
		}
	}
}
