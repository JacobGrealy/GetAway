using UnityEngine;
using System.Collections;
[ExecuteInEditMode()] 
public class StartMenuGUI : MonoBehaviour {
	public GameObject dino;
	public GUISkin gSkin;
	public float GUIX = (Screen.width/2)-70;
	bool isLoading = false;
	bool loadCredits = false;
	bool loadGame = false;
	private float letterBox = 0;
	// Use this for initialization
	void Start () 
	{
		if ((Screen.width / Screen.height) < 16f/11f) 
		{
			letterBox = Screen.height*.125f;
		}
	}
	
	// Update is called once per frame
	void Update () {

	}

	void playAudio(){
		audio.Play();
	}

	void OnGUI(){
		if(gSkin)
			GUI.skin = gSkin;
		else
			Debug.Log("StartMenuGUI: Missing GUI Skin");
		if(GUI.Button(new Rect(GUIX,-letterBox + Screen.height-190, 140, 30), "Play")){
			audio.Play();
			if(dino != null)
				dino.animation.Play("bite");
			loadGame = true;
			isLoading = true;
		}
		if(GUI.Button(new Rect(GUIX,-letterBox + Screen.height-140, 140, 30), "Credits")){
			audio.Play();
			loadCredits = true;
			isLoading = true;
		}
		bool isWebPlayer = (Application.platform == RuntimePlatform.OSXWebPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer);
		if(!isWebPlayer){
			if(GUI.Button(new Rect(GUIX,-letterBox + Screen.height-90, 140, 30), "Quit")){
				Application.Quit();
			}

		}
		//if(isLoading)
		//	GUI.Label(new Rect((Screen.width/2)-70, (Screen.height/2)-60, 400, 70), "Loading...","mainmenutitle");
		if(loadGame && !audio.isPlaying)
			Application.LoadLevel("Game");
		else if(loadCredits && !audio.isPlaying)
			Application.LoadLevel("Credits");
	}

}
