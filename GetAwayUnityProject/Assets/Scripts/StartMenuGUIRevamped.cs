using UnityEngine;
using System.Collections;

public class StartMenuGUIRevamped : MonoBehaviour {

	public StartMenuText start;
	public StartMenuText credits;
	public StartMenuText quit;
	public GUITexture logo;

	private bool startHovered = false;
	private bool creditsHovered = false;
	private bool quitHovered = false;

	bool isLoading = false;
	bool loadCredits = false;
	bool loadGame = false;

	private float letterBox = 0;

	private bool controllerPresent;
	private float controlDelay = 0.0f;
	private float controlDelayTime = 0.1f;

	void Start () {
		controllerPresent = Input.GetJoystickNames().Length > 0;
		if (controllerPresent) startHovered = true;

		if ((Screen.width / Screen.height) < 16f/11f) 
		{
			letterBox = .125f;
		}
	}

	void playAudio(){
		audio.Play();
	}

	private void UpdateTextPosition() {
		float x = 0.062f;
		float firstY = 80.0f / Screen.height;
		float incY = 60.0f / Screen.height;
		
		start.transform.position = new Vector3(x, letterBox + firstY + incY * 2, 0.0f);
		credits.transform.position = new Vector3(x, letterBox + firstY + incY, 0.0f);
		quit.transform.position = new Vector3(x, letterBox + firstY, 0.0f);
		
		logo.transform.position = new Vector3(x, 1.0f - letterBox - firstY, 0.0f);
	}

	// Update is called once per frame
	void Update () {
		UpdateTextPosition();

		if (controllerPresent && controlDelay < 0.0f) {
			if (Input.GetAxisRaw("Joystick UI Y") < -0.3) { // Up
				if (quitHovered) {
					quitHovered = false;
					creditsHovered = true;
				} else if (creditsHovered) {
					creditsHovered = false;
					startHovered = true;
				}
			} else if (Input.GetAxisRaw("Joystick UI Y") > 0.3) { // Down
				if (startHovered) {
					startHovered = false;
					creditsHovered = true;
				} else if (creditsHovered) {
					creditsHovered = false;
					quitHovered = true;
				}
			} else if (Input.GetButton("Joystick UI A")) { // A or X
				if (startHovered) start.Click();
				else if (creditsHovered) credits.Click();
				else quit.Click();
			}

			// Change hover states
			if (startHovered) start.SetHoverColor(); else start.SetNormalColor();
			if (creditsHovered) credits.SetHoverColor(); else credits.SetNormalColor();
			if (quitHovered) quit.SetHoverColor(); else quit.SetNormalColor();

			controlDelay = controlDelayTime;
		}
		
		controlDelay -= Time.deltaTime;
	}
}
