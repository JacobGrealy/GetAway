using UnityEngine;
using System.Collections;

public class RaftController : MonoBehaviour {

	// Call when the user has pressed the action button on this raft.
	public void UserPressedActionButton() {
		print ("WIN");
		// Temporary
		Application.LoadLevel ("Win");
	}
}
