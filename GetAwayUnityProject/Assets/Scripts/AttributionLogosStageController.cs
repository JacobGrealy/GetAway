using UnityEngine;
using System.Collections;

public class AttributionLogosStageController : MonoBehaviour {

	public LoadingCameraFaderController loadingCameraFader;

	private bool isActive = false;
	private bool isEnding = false;

	private float timeToEndScene = 4.0f;
		
	public void StartStage() {
		isActive = true;
	}
	
	public void EndStage() {
		isActive = false;
	}
	
	public void MoveIntoView() {
		this.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
	}
	
	public void MoveOutOfView() {
		this.transform.position = new Vector3(-100.0f, 0.0f, 0.0f);
	}

	void Update() {
		if (isActive) {
			timeToEndScene -= Time.deltaTime;

			if (!isEnding && timeToEndScene <= 0) {
				StartupEventManager.StageComplete(StartupEventManager.Stage.AttributionLogos);
				isEnding = true;
			}
		}
	}
}
