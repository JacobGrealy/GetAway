using UnityEngine;
using System.Collections;

public class StatController : MonoBehaviour {

	// The percentage (0 to 1) the bar should be filled.
	public void SetPercentageFilled (float percentage) {
		renderer.material.SetFloat("_Cutoff", Mathf.Lerp (1.0f, 0.001f, percentage));
	}
}
