using UnityEngine;
using System.Collections;

public class CraftingMenuController : MonoBehaviour {

	//Sounds
	public AudioSource craftingSound;

	private Vector3 INITIAL_POSITION = new Vector3(0.001490379f, -2.267987f, 3.365621f);
	private Vector3 INITIAL_ROTATION = new Vector3(90.0f, 0.0f, 0.0f);
	private Vector3 INITIAL_SCALE = new Vector3(0.0f, 0.0f, 0.0f);

	private Vector3 FINAL_POSITION = new Vector3(0.0f, -0.3723212f, 3.356408f);
	private Vector3 FINAL_ROTATION = new Vector3(0.0f, 0.0f, 0.0f);
	private Vector3 FINAL_SCALE = new Vector3(1.0f, 1.0f, 1.0f);

	private Quaternion INITIAL_ROTATION_QUAT;
	private Quaternion FINAL_ROTATION_QUAT;

	private float animTime = 0.0f;
	private const float ANIM_TIME_INCREMENT_MODIFIER = 4.5f;

	private bool animatingOntoScreen = false;
	private bool animatingOffOfScreen = false;
	private bool menuIsActive = false;
	private bool acceptingUserInput = false;

	public ParticleEmitter smokeParticleEmitter;
	public ParticleEmitter glowParticleEmitter;
	public ItemManager itemManager;

	// Use this for initialization
	void Start () {
		INITIAL_ROTATION_QUAT = Quaternion.Euler(INITIAL_ROTATION);
		FINAL_ROTATION_QUAT = Quaternion.Euler(FINAL_ROTATION);
		UpdateTransform(0.0f);

		glowParticleEmitter.enabled = false;
	}

	// Activates the crafting menu into position.
	private void Activate() {
		menuIsActive = true;
		animatingOntoScreen = true;
		animatingOffOfScreen = false;
		glowParticleEmitter.enabled = true;
	}

	// Deactivates the crafting menu.
	private void Deactivate() {
		menuIsActive = false;
		acceptingUserInput = false;
		animatingOntoScreen = false;
		animatingOffOfScreen = true;
		glowParticleEmitter.enabled = false;
		glowParticleEmitter.ClearParticles();
		smokeParticleEmitter.ClearParticles();
	}

	// Update the crafting menu's transform by time (0 to 1).
	// 1 = the menu is accepting user input. 0 = menu is closed.
	private void UpdateTransform(float time) {
		this.transform.localPosition = Vector3.Slerp(INITIAL_POSITION, FINAL_POSITION, time);
		this.transform.localRotation = Quaternion.Slerp (INITIAL_ROTATION_QUAT, FINAL_ROTATION_QUAT, time);
		this.transform.localScale = Vector3.Slerp(INITIAL_SCALE, FINAL_SCALE, time);
	}

	public void ToggleActive(bool activate) {
		if (activate) Activate();
		else Deactivate();
	}

	// Update is called once per frame
	void Update () {
		if (animatingOntoScreen) {
			animTime = 1.0f;
			UpdateTransform (animTime);
		} else if (animatingOffOfScreen) {
			animTime = 0.0f;
			UpdateTransform(animTime);
		}
		if (animatingOntoScreen && !acceptingUserInput) {
			smokeParticleEmitter.Emit();
		}
		if (animTime > 0.99f && animTime < 1.01f) {
			acceptingUserInput = true;
			animatingOntoScreen = false;
			//smokeParticleEmitter.emit = true;
		} else {
			UpdateTransform(animTime);
		}
	}
}
