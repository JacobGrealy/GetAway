using UnityEngine;
using System.Collections;

public class InterfaceWindowTilt : MonoBehaviour {
	
	private const float X_OFFSET_MODIFIER = -0.001f;
	private const float Y_OFFSET_MODIFIER = 0.001f;

	private float xOffset;
	private float yOffset;

	private Vector3 lastTilt = new Vector3(0.0f, 0.0f, 0.0f);

	public void Update() {
		Vector3 mousePosition = Input.mousePosition - new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0.0f);
		xOffset = mousePosition.y * X_OFFSET_MODIFIER;
		yOffset = mousePosition.x * Y_OFFSET_MODIFIER;

		Vector3 baseRot = this.transform.localRotation.eulerAngles - lastTilt;
		lastTilt = new Vector3(xOffset, yOffset, 0.0f);
		this.transform.rotation = Quaternion.Euler(baseRot + lastTilt);
	}
}
