using UnityEngine;
using System.Collections;

public class WaitBeforeBurn : MonoBehaviour {
	public GameObject fire;
	public Transform campfire;
	// Use this for initialization
	void Start () {
		campfire = transform;
	}
	
	// Update is called once per frame
	void Update () {
	if (campfire.rigidbody.velocity==new Vector3(0,0,0)){
			fire.SetActive (true);
		}
	}
}
