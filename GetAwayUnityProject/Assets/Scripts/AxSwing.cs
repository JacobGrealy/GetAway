using UnityEngine;
using System.Collections;

public class AxSwing : MonoBehaviour {
	public int durability = 10;
	public AudioSource breakSound;
	bool play = true;
	bool chopping = false;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if (durability>0){ //if ax not broken
			if ((Input.GetButton("Fire1") || Input.GetAxisRaw("RightTrigger") == -1) && !chopping) { //if swing ax
					gameObject.GetComponent<AxSway>().enabled = false;
					gameObject.animation.Play("BetterAxSwing");
					chopping=true;
				}
			if (chopping==true && !gameObject.animation.isPlaying){
				durability--;
				chopping=false;
			}
		}
		else{
			if (play){
				breakSound.Play ();
				play=!play;
			}
		
			else if (!play && !breakSound.isPlaying){
				Destroy(gameObject); //Destroy Ax
			}
		}
		//Console.print(Input.GetAxis("LeftTrigger"));
	}
}
