using UnityEngine;
using System.Collections;

public class DemoSceneChange : MonoBehaviour {
	public string[] scene = new string[7];
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	if (Input.GetKeyDown (KeyCode.Alpha1)){
			Application.LoadLevel(scene[0]);
		}
	else if (Input.GetKeyDown (KeyCode.Alpha2)){
		Application.LoadLevel(scene[1]);
		} 
		else if (Input.GetKeyDown (KeyCode.Alpha3)){
			Application.LoadLevel(scene[2]);
		} 
		else if (Input.GetKeyDown (KeyCode.Alpha4)){
			Application.LoadLevel(scene[3]);
		} 
		else if (Input.GetKeyDown (KeyCode.Alpha5)){
			Application.LoadLevel(scene[4]);
		} 
		else if (Input.GetKeyDown (KeyCode.Alpha6)){
			Application.LoadLevel(scene[5]);
		} 
	}
}
