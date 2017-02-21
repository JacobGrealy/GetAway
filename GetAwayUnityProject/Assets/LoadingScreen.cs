using UnityEngine;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
	public GUIText loadingSubText;
	public GameObject postTerrainGeneration;
	private int frameCounter = 0;
	public bool loadingDone = false;
	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!loadingDone) 
		{
			frameCounter++;
			float per = Mathf.RoundToInt ((frameCounter / 5140f) * 100f);
			if (per >= 100f)
					per = 99f;
			loadingSubText.text = "" + per + "%";
		}
		else
		{
			loadingSubText.text = "100% Press 'Jump' Button to Start The Game";
			//add if statement for A button here------------------------------
			if(Input.GetButton("Jump"))
			{
				loadingSubText.text = "...Starting Game...";
				startGame();
			}
			//----------------------------------------------------------------
		}
	}
	public void startGame()
	{
		postTerrainGeneration.SetActive (true); //load the rest of the content
		GameObject.Destroy(this.gameObject); //turn off the loading screen
	}
}
