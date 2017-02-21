using UnityEngine;
using System.Collections;

public class HallucinationBehavior : MonoBehaviour {
	
	private GameObject player;
	public float duration;
	public float destructionDistance; //How close the hallucination can get to the player
	public float movespeed;
	public float distanceBehindPlayer;
	public AudioClip hallucinationSound;
	private float remainingDuration;
	private bool dying = false;
	// Use this for initialization
	void Start () {
		remainingDuration = duration;
		player = GameObject.Find ("First Person Controller");
		
		//Create the hallucination behind the player
		//Get the direction opposite the one the player is facing
		Vector3 oppositeDirection = -player.transform.forward;
		oppositeDirection = new Vector3 (oppositeDirection.x, 0f, oppositeDirection.z);
		
		//Move the hallucination to the specified distance behind the player
		transform.position = player.transform.position + (oppositeDirection * distanceBehindPlayer);
		transform.rotation = player.transform.rotation;
		
		//Make sure hallucinatin only rotates around the y axis
		transform.eulerAngles = new Vector3 (0f,transform.eulerAngles.y,0f);
		
		//Keep the hallucination at terrain height
		transform.position = new Vector3 (transform.position.x, Terrain.activeTerrain.SampleHeight (this.transform.position), transform.position.z);
		animation.CrossFade ("run");
		AudioSource.PlayClipAtPoint (hallucinationSound, player.transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		remainingDuration -= Time.deltaTime;
		
		if (remainingDuration < 0 || Vector3.Distance(player.transform.position,transform.position) < destructionDistance) {
			if (!animation.IsPlaying("die") && dying){
				Destroy(gameObject);
			}
			else if (!animation.IsPlaying("die") && !dying){
				animation.Stop();
				animation.CrossFade ("die");
				dying=true;
			}	
		}
		else{
			///Rotate towards the player

			//Get the player's position relative to the hallucination
			Vector3 playerRelativePosition = transform.InverseTransformPoint(player.transform.position);

			//Get the vector and angle to the player
			Vector3 vectorToPlayer = player.transform.position - transform.position;
			float angleToPlayer = Vector3.Angle (transform.forward, vectorToPlayer);

			//If the player is to the left, rotate to the left instead
			if(playerRelativePosition.x < 0){
				angleToPlayer = -angleToPlayer;
			}

			//This will make a scary twitchy t-rex
			//transform.rotation = Quaternion.AngleAxis(angleToPlayer + transform.localEulerAngles.y, new Vector3(0,1,0));

			//Rotate towards the player
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation(vectorToPlayer), Time.deltaTime * 3);

			//Make sure hallucinatin only rotates around the y axis
			transform.eulerAngles = new Vector3 (0f,transform.eulerAngles.y,0f);
			
			//Move towards the player
			Vector3 velocity = movespeed * transform.forward * Time.deltaTime;

			velocity = velocity + new Vector3(0f,-15f,0f);

			//Do gravity
			GetComponent<CharacterController>().Move(velocity);

			//Keep the hallucination at terrain height
			//float terrainHeight = Terrain.activeTerrain.SampleHeight (player.transform.position);
			//Debug.Log ("terrainHeight = " + terrainHeight);
			//transform.position = new Vector3 (transform.position.x,terrainHeight,transform.position.z);
		}
	}
}