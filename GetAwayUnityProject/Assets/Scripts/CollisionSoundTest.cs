using UnityEngine;
using System.Collections;

public class CollisionSoundTest : MonoBehaviour {
	public AudioSource audiosource;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void OnControllerColliderHit (ControllerColliderHit col){
		if (col.gameObject.name == "Box"){
		Debug.Log ("COLLISION");
			audiosource.Play ();
		}
	}
}