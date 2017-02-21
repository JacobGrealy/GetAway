using UnityEngine;
using System.Collections;
[ExecuteInEditMode()] 
public class WinGUI : MonoBehaviour {
	//public GameObject dino;
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
		bool isWebPlayer = (Application.platform == RuntimePlatform.OSXWebPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer);
		if(!isWebPlayer){
			if(GUI.Button(new Rect(Screen.width/2-80,11*Screen.height/16, 160, 30), "Main Menu")){//Screen.height/2
				Debug.Log("You pressed main menu");
				Application.LoadLevel("Start Menu");
			}
			
		}

		GUI.Label(new Rect((Screen.width/2)-200, (5*Screen.height/16), 400, 70), "YOU'VE ESCAPED!","GameTitle");//-60,+90,+130,+170
		GUI.Label(new Rect((Screen.width/2)-200, (7*Screen.height/16), 400, 70), "Days Survived:\t"+Score.daysSurvived,"MainMenuTitle");
		GUI.Label(new Rect((Screen.width/2)-200, (8*Screen.height/16), 400, 70), "Damage Done:\t"+Score.damageDone,"MainMenuTitle");
		GUI.Label(new Rect((Screen.width/2)-200, (9*Screen.height/16), 400, 70), "Damage Taken:\t"+Score.damageTaken,"MainMenuTitle");
	}
	
}
