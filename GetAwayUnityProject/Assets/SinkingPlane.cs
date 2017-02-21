using UnityEngine;
using System.Collections;

public class SinkingPlane : MonoBehaviour {

	private float sinkRate = 1f;
	private float totalTime = 0;
	public AudioSource sinkingSound;
	// Use this for initialization
	void Start ()
	{
		sinkingSound.Play ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.position -= Vector3.up * Time.deltaTime * sinkRate;
		if (totalTime > 22f) 
		{
			sinkingSound.Stop();
			GameObject.Destroy (this.gameObject);
		}
		totalTime += Time.deltaTime;
	}
}
