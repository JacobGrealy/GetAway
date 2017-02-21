using UnityEngine;
using System.Collections;

public class InterfaceBackdropController : MonoBehaviour {

	public void ToggleActive(bool active) {
		this.gameObject.SetActive(active);
	}
}
