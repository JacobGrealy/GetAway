using UnityEngine;
using System.Collections;

public class NavigationButtonController : MonoBehaviour {
	
	public Material normal;
	public Material hovered;
	public Vector3 clickedScale;
	public Vector3 hoverTranslation;

	public delegate void UserInteraction();
	public event UserInteraction ButtonPressed;

	private Vector3 normalPosition;
	private Vector3 normalScale;

	private bool isHovered = false;
	private bool isClicked = false;
	private bool mouseOverInThisFrame = false;
	private float animTime;
	private const float ANIM_TIME_MODIFIER = 8.0f;
	
	void Start () {
		renderer.material = normal;
		normalScale = this.transform.localScale;
		normalPosition = this.transform.localPosition;
	}

	void OnMouseEnter() {
		isHovered = true;
		renderer.material = hovered;
	}

	void OnMouseOver() {
		mouseOverInThisFrame = true;
	}
	
	void OnMouseExit() {
		isHovered = false;
		renderer.material = normal;
	}
	
	void OnMouseDown() {
		isClicked = true;
		this.transform.localScale = clickedScale;
		ButtonPressed.Invoke();
	}
	
	void OnMouseUp() {
		isClicked = false;
		this.transform.localScale = normalScale;
	}
	
	void Update () {
		if (!isClicked) {
			if (isHovered) {
				animTime = Mathf.Min(animTime + (Time.deltaTime * ANIM_TIME_MODIFIER), 1.0f);
			} else {
				animTime = Mathf.Max(animTime - (Time.deltaTime * ANIM_TIME_MODIFIER), 0.0f);
			}
			if (animTime > 0.99f && animTime < 1.01f) {
				
			}
			
			this.transform.localPosition = Vector3.Slerp (normalPosition, normalPosition + hoverTranslation, animTime);
		}

		// If mouse hover inconsistency, fix it.
		if (!mouseOverInThisFrame && isHovered) {
			isHovered = false;
			renderer.material = normal;
		}
		mouseOverInThisFrame = false;
	}
}
