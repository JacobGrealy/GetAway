using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class CameraShake : MonoBehaviour {
	public float shakeStrength;
	public float shake;

	private HealthController healthControl;
	private Vector3 originalPosition;

	private Camera cam;

	private float vibrateDelay;
	private float vibrateOffDelay;
	
	void Start()
	{
		originalPosition = transform.localPosition;
		healthControl = GameObject.Find("PlayerHealth").GetComponent<HealthController>();
		//cam = Camera.main;
		cam = GameObject.Find("Main Camera").camera;
	}
	
	void Update() {
		if (healthControl.GetMental() <= 20) {
			shake = shakeStrength;
			if (vibrateDelay < 4) {
				GamePad.SetVibration(0, 0.6f, 0.6f);
			}
			else {
				if (vibrateOffDelay < 4) {
					GamePad.SetVibration(0, 0, 0);
				}
				else {
					vibrateDelay = 0;
					vibrateOffDelay = 0;
				}
				vibrateOffDelay += 0.1f;
			}
			vibrateDelay += 0.1f;
		}
		else {
			GamePad.SetVibration(0, 0, 0);
		}
		
		cam.transform.localPosition = originalPosition + (Random.insideUnitSphere * shake);
		
		shake = Mathf.MoveTowards(shake, 0, Time.deltaTime * shakeStrength);
		
		if(shake == 0) {
			cam.transform.localPosition = originalPosition;
		}
	}
}