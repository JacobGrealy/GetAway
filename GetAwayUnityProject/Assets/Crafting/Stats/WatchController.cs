using UnityEngine;
using System.Collections;

public class WatchController : MonoBehaviour {

	public Transform minutesHand;
	public Transform hoursHand;
	
	// The day night controller to get the time of day.
	public DayNightController dayNightController;

	// Moves to hour hand to 0 to begin with.
	private float hoursHandOffsetRotation = 90.0f;

	public void SetTime(int hour, int minute) {
		float minutesRotation = (minute / 60.0f) * 360.0f;
		minutesHand.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, minutesRotation));

		float hoursRotation = hoursHandOffsetRotation + ((hour / 12.0f) + (minute / 60.0f / 12.0f)) * 360.0f;
		hoursHand.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, hoursRotation));
	}

	// Update is called once per frame
	void Update () {
		int hours = (int) Mathf.Floor (dayNightController.GetTime());
		int minutes = (int) ((dayNightController.GetTime() % 1) * 60.0f);
		SetTime (hours, minutes);
	}
}
