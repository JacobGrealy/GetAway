using UnityEngine;
using System.Collections;

public class TvFlicker : MonoBehaviour {
	private System.Random rand = new System.Random(); // This seems to repeat itself and is slow!
	private float passedTimeColor = 0;
	private float randomTimeColor = 0;
	private float passedTimeIntensity = 0;
	private float randomTimeIntensity = 0;
	public float intensityMin = .9f;
	public float intensityMax = 1.2f;
	// Use this for initialization
	void Start () {

	}
	// Update is called once per frame
	void Update () {
		passedTimeColor += Time.deltaTime;
		passedTimeIntensity += Time.deltaTime;

		if (passedTimeColor > randomTimeColor) 
		{
			float red = rand.Next (220, 255) / 255f;
			float green = rand.Next (220, 255) / 255f;
			float blue = rand.Next (220, 255) / 255f;
			light.color = new Color (red, green, blue);
			passedTimeColor = 0;
			randomTimeColor = Random.Range(0.0f,5.0f);
		}

		if (passedTimeIntensity > randomTimeIntensity)
		{
			light.intensity = Random.Range (intensityMin, intensityMax);
			passedTimeIntensity = 0;
			randomTimeIntensity = Random.Range(0.0f,1f);
		}
	}
}
