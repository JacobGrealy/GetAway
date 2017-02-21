using UnityEngine;
using System.Collections;

public class FallingBearLogoStageController : MonoBehaviour {

	public AudioSource bearSounds;

	public FallingBearLogoController logo;

	public BearSpawnerController bearSpawner;

	public LoadingCameraFaderController loadingCameraFader;

	private bool isActive = false;
	private bool isEnding = false;

	private float timeToEndBearSounds = 3.7f;

	private float timeSinceLastBearSpawn = 0.0f;

	private float timeToEndScene = 6.0f;

	private const float TIME_BETWEEN_BEAR_SPAWNS = 0.25f;

	public void Start() {
		//StartupEventManager.CameraFadedIn += StartBearSounds;
		StartupEventManager.CameraFadedOut += EndStage;
	}

	public void StartStage() {
		isActive = true;
		StartBearSounds();
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

	public void StartBearSounds() {
		print ("Starting bear sounds");
		if (isActive) {
			bearSounds.Play();
		}
	}

	public void UpdateBearSounds() {
		timeToEndBearSounds -= Time.deltaTime;

		if (timeToEndBearSounds <= 0) {
			bearSounds.Stop();
		}
	}
	
	void Update() {
		if (isActive) {
			timeSinceLastBearSpawn -= Time.deltaTime;
			timeToEndScene -= Time.deltaTime;

			// Decide whether or not to spawn bears.
			if (timeSinceLastBearSpawn <= 0 && timeToEndScene > 0) {
				timeSinceLastBearSpawn = TIME_BETWEEN_BEAR_SPAWNS;
				bearSpawner.SpawnBear();
			}
			
			// See if bear sound should stop.
			UpdateBearSounds();

			// Check to see if the scene should end.
			if (!isEnding && timeToEndScene <= 0) {
				StartupEventManager.StageComplete(StartupEventManager.Stage.FallingBearLogo);
				isEnding = true;
			}
		}
	}
}
