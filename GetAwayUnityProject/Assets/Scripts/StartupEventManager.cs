using UnityEngine;
using System.Collections;

public class StartupEventManager : MonoBehaviour
{
	public delegate void TransitionAction();
	public delegate void CameraAction();

	public static event TransitionAction CompletedFallingBearLogoStage;
	public static event TransitionAction CompletedAttributionLogosStage;
	public static event TransitionAction CompletedDevelopersStage;

	public static event CameraAction CameraFadedIn;
	public static event CameraAction CameraFadedOut;

	public enum Stage {
		FallingBearLogo,
		AttributionLogos,
		Developers
	}
	
	public enum CameraState {
		FadedIn,
		FadedOut
	}

	public static void CameraStateChanged(CameraState state) {
		switch(state) {
			case CameraState.FadedIn:
				if (CameraFadedIn != null) {
					CameraFadedIn();
				}
				break;
			case CameraState.FadedOut:
				if (CameraFadedOut != null) {
					CameraFadedOut();
				}
				break;
		}
	}

	public static void StageComplete(Stage stage) {
		switch(stage) {
			case Stage.FallingBearLogo:
				CompletedFallingBearLogoStage();
				break;
			case Stage.AttributionLogos:
				CompletedAttributionLogosStage();
				break;
			case Stage.Developers:
				CompletedDevelopersStage();
				break;
		}
	}
}

