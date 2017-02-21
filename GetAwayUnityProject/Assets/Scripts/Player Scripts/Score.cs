using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour {
	public static int playerScore = 0;
	public static int damageDone = 0;
	public static int damageTaken = 0;
	public static int daysSurvived = 0;

	void Update(){
	if (Input.GetKeyDown(KeyCode.Q)){
			playerScore++;
		}
	if (Input.GetKeyDown (KeyCode.R)){
			Debug.Log ("SCORE:"+playerScore);
		}

   }
}
