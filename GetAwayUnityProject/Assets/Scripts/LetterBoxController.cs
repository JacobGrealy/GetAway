using UnityEngine;
using System.Collections;

public class LetterBoxController : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		if (Screen.width*1.0f/Screen.height < 16f/11f)  //most likely 4:3
		{
			camera.fieldOfView = camera.fieldOfView * (1.03f);
			camera.rect = new Rect(0,.125f,1f,.75f);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{

	}
}
