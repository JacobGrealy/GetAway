using UnityEngine;
using System.Collections;

public class AxSway : MonoBehaviour {
	private int delay = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (delay == 50) {
			gameObject.animation.Play("AxSway");
			delay = 0;
		}
		else {
			delay++;
		}
	}
}
