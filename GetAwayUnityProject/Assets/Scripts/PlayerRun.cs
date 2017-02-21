using UnityEngine;
using System.Collections;

public class PlayerRun : MonoBehaviour {

	private CharacterMotor motor;
	private float fWalkingSpeed;
	private float bWalkingSpeed;
	public float forwardRunningScale = 1.75f;
	public float backwardsRunningScale = 1.25f;
	public float setCrouchScale = .5f;
	private PlayerCrouch crouch;
	private bool isCrouched;
	private float currentCrouchScale;
	// Use this for initialization
	void Start () {
		motor = GetComponent<CharacterMotor>();
		crouch = GetComponent<PlayerCrouch>();
		fWalkingSpeed = motor.movement.maxForwardSpeed;
		bWalkingSpeed = motor.movement.maxBackwardsSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		if (crouch.isCrouching ()){
			currentCrouchScale = setCrouchScale;
		}
		else{
			currentCrouchScale = 1f;
		}
		if (Input.GetButton("Run") && motor.IsGrounded()){ //if on ground and running
			motor.movement.maxForwardSpeed=fWalkingSpeed*forwardRunningScale*currentCrouchScale; //scale forward speed by run scale
			motor.movement.maxBackwardsSpeed=bWalkingSpeed*backwardsRunningScale*currentCrouchScale; //scale backward speed by run scale
		}
		else if (motor.IsGrounded()){ //if on ground, but not running
			motor.movement.maxForwardSpeed=fWalkingSpeed*currentCrouchScale; //return to normal forward speed
			motor.movement.maxBackwardsSpeed=bWalkingSpeed*currentCrouchScale; //return to normal backwards speed
		}
		//else maintain current speed
	}
}
