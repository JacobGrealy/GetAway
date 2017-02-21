using UnityEngine;
using System.Collections;

public class LoadingSceneController : MonoBehaviour {

	public FallingBearLogoStageController fallingBearLogoStage;
	public AttributionLogosStageController attributionLogosStage;
	public DevelopersStageController developersStage;

	public LoadingCameraFaderController cameraFader;

	public void TransitionFromFallingBearLogosStage() {
		print ("transitioning...");
	}

	// Use this for initialization
	void Start () {
		StartupEventManager.CompletedFallingBearLogoStage += TransitionToAttributionLogosStage;
		StartupEventManager.CompletedAttributionLogosStage += TransitionToDevelopersStage;
		StartupEventManager.CompletedDevelopersStage += TransitionToMainMenu;

		fallingBearLogoStage.MoveIntoView();
		attributionLogosStage.MoveOutOfView();
		developersStage.MoveOutOfView();

		cameraFader.FadeIn();
		fallingBearLogoStage.StartStage();
	}

	void TransitionToAttributionLogosStage() {
		print ("Transitions to attribution logos stage");
		cameraFader.FadeOut();
		StartupEventManager.CameraFadedOut += StartAttributionLogosStage;
	}

	void StartAttributionLogosStage() {
		print ("Starting attribution logos stage");
		StartupEventManager.CameraFadedOut -= StartAttributionLogosStage;

		fallingBearLogoStage.EndStage();
		fallingBearLogoStage.MoveOutOfView();
		attributionLogosStage.MoveIntoView();
		cameraFader.FadeIn();
		attributionLogosStage.StartStage();
	}

	void TransitionToDevelopersStage() {
		print ("Transitioning to developers stage");
		cameraFader.FadeOut();
		StartupEventManager.CameraFadedOut += StartDevelopersStage;
	}
	
	void StartDevelopersStage() {
		print ("Starting attribution logos stage");
		StartupEventManager.CameraFadedOut -= StartDevelopersStage;
		
		attributionLogosStage.EndStage();
		attributionLogosStage.MoveOutOfView();
		developersStage.MoveIntoView();
		cameraFader.FadeIn();
		developersStage.StartStage();
	}
	
	void TransitionToMainMenu() {
		print ("Transition to main menu");
		cameraFader.FadeOut();
		StartupEventManager.CameraFadedOut += MoveToMainMenu;
	}

	void MoveToMainMenu() {
		StartupEventManager.CameraFadedOut -= MoveToMainMenu;
		Application.LoadLevel("Start Menu");
	}

	// Update is called once per frame
	void Update () {
		if (Input.anyKeyDown) {
			MoveToMainMenu();
		}
	}
}
