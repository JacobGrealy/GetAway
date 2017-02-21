using UnityEngine;
using System.Collections;

public class PlayerCrouch : MonoBehaviour {
	public float crouchScale=.25f;
	private float originalHeight;
	private CharacterController charController; 
	public bool isCrouched=true; //used for toggle crouch
	private Transform playerTransform;
	public float crouchSmooth = 5;
	// Use this for initialization
	void Start () {
		charController = GetComponent<CharacterController>();
		originalHeight=charController.height;
		playerTransform = transform;
	}
	
	// Update is called once per frame
	void Update () {
		float tmpHeight=originalHeight;
		if (Input.GetButton("Crouch")){
			//if crouched
			tmpHeight=originalHeight*crouchScale;
			isCrouched=true;
		}
		else {
			isCrouched=false;
		}
		float previousHeight = charController.height;
		charController.height = Mathf.Lerp (charController.height,tmpHeight,crouchSmooth*Time.deltaTime);
		playerTransform.position = new Vector3(playerTransform.position.x,playerTransform.position.y+(charController.height-previousHeight)/2,playerTransform.position.z);
	}

	public bool isCrouching(){
		return isCrouched;
	}
}
