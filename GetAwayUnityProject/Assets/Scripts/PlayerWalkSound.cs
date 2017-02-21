using UnityEngine;
using System.Collections;

public class PlayerWalkSound : MonoBehaviour {
	public AudioSource walk;
	private CharacterMotor motor;
	private bool sound;
	// Use this for initialization
	void Start () {
		walk.volume = 0.2f;
		motor = GetComponent<CharacterMotor>();
	}
	
	// Update is called once per frame
	void Update () {
		//Running
		if (Input.GetButton("Horizontal") || Input.GetButton("Vertical") 
		    || Input.GetAxisRaw("Joystick UI X") > 0.0f || Input.GetAxisRaw("Joystick UI X") < 0.0f
		    || Input.GetAxisRaw("Joystick UI Y") > 0.0f || Input.GetAxisRaw("Joystick UI Y") < 0.0f){
//			if (Input.GetButtonDown("Run") && motor.IsGrounded()) {
//				Debug.Log("Running");
//				walk.pitch = 2.5f;
//			}
//			else if (Input.GetButtonUp("Run")) {
//				walk.pitch = 1.0f;
//			}
			if (sound!=true){
			walk.Play();
			sound=true;
			}
		}
		else {
			sound=false;
		}
		if (Input.GetButtonDown("Run")) {
			walk.pitch = 2.5f;
		}
		if (Input.GetButtonUp("Run")) {
			walk.pitch = 1.0f;
		}

		if (!Input.GetButton("Horizontal") && !Input.GetButton("Vertical") 
		    && Input.GetAxisRaw("Joystick UI X") == 0.0f 
		    && Input.GetAxisRaw("Joystick UI Y") == 0.0f) {
			walk.Stop();
		}
	}
}
