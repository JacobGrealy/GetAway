using UnityEngine;
using System.Collections;
[ExecuteInEditMode()] 
public class GameOverGUI : MonoBehaviour {
	public GameObject dino;
	public GUISkin gSkin;
	public float GUIX = (Screen.width/2)*.125f;
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
		if(GUI.Button(new Rect(Screen.width/2-30,12*Screen.height/16, 140, 30), "Restart")){//-letterbox+Screen.height-260
			Debug.Log("You pressed restart");
			audio.Play();
			if(dino != null)
				dino.animation.Play("bite");
			loadGame = true;
			isLoading = true;
		}
		bool isWebPlayer = (Application.platform == RuntimePlatform.OSXWebPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer);
		if(!isWebPlayer){
			if(GUI.Button(new Rect(Screen.width/2-30,(10*Screen.height/16), 160, 30), "Main Menu")){//-letterbox+Screen.height-200
				Debug.Log("You pressed main menu");
				Application.LoadLevel("Start Menu");
			}
			
		}
		if(isLoading)
			GUI.Label(new Rect((Screen.width/2)-70, (1*Screen.height/2)/8, 400, 70), "Loading...","mainmenutitle");//-60
		else{
			GUI.Label(new Rect((Screen.width/2)-70, (3*Screen.height/16), 400, 70), "Game Over","GameTitle");//-60,+30,+70,+110
			GUI.Label(new Rect((Screen.width/2)-60, (5*Screen.height/16), 400, 70), "Days Survived:\t"+Score.daysSurvived,"mainmenutitle");
			GUI.Label(new Rect((Screen.width/2)-60, (6*Screen.height/16), 400, 70), "Damage Done:\t"+Score.damageDone,"mainmenutitle");
			GUI.Label(new Rect((Screen.width/2)-60, (7*Screen.height/16), 400, 70), "Damage Taken:\t"+Score.damageTaken,"mainmenutitle");
		}
		if(loadGame && !audio.isPlaying)
			Application.LoadLevel("Game");
		else if(loadCredits && !audio.isPlaying)
			Application.LoadLevel("Credits");
	}
	
}
