using UnityEngine;
using System.Collections;

public class StartMenuText : MonoBehaviour {

	public string sceneToLoad;
	public bool isQuitButton;

	private Color normalColor = new Color(1.0f, 1.0f, 1.0f);
	private Color hoverColor = new Color(195.0f / 255.0f, 36.0f / 255.0f, 36.0f / 255.0f);

	private bool mouseOverOnThisFrame = false;

	void OnMouseEnter() {
		SetHoverColor();
	}
	
	void OnMouseExit() {
		SetNormalColor();
	}

	void OnMouseUp() {
		Click();
	}

	public void SetHoverColor() {
		this.guiText.color = hoverColor;
	}

	public void SetNormalColor() {
		this.guiText.color = normalColor;
	}

	public void Click() {
		if (isQuitButton) {
			Application.Quit ();
		} else {
			Application.LoadLevel(sceneToLoad);
		}
	}
}
