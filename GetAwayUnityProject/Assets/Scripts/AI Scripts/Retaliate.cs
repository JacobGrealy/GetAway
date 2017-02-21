using UnityEngine;
using System.Collections;

public class Retaliate : MonoBehaviour {
	public string attackAnimationName;
	public AudioClip attackSound;
	public int attackStrength;
	public bool shouldRetaliate;
	public float attackRange;
	public float attackAngle;
	private float runningTimer;
	public float runningDuration;
	private bool storeTarget;
	private GameObject attacker;
	private Vector3 prevTargetPosition;
	// Use this for initialization
	void Start () {
		shouldRetaliate = false;
		storeTarget = true;
	}
	
	// Update is called once per frame
	void Update () {
		if(shouldRetaliate){
			if(storeTarget){
				prevTargetPosition = GetComponent<DinosaurMovement>().getCurrentTargetPosition();
				storeTarget = false;
			}
			if(!attacker.Equals(null)){
			//Turn to face attacker and stop motion
			GetComponent<DinosaurMovement>().AssignCurrentTargetPosition(attacker.transform.position);
			GetComponent<DinosaurMovement>().StopMotion();
			GetComponent<DinosaurMovement>().TurnTowardObjective();
			//If the attacker is close, attack back
			if (Vector3.Distance (transform.position, attacker.transform.position) < attackRange && !animation.IsPlaying(attackAnimationName) && Vector3.Angle(transform.forward, attacker.transform.position-transform.position) <= attackAngle) {
				animation.Play (attackAnimationName);
				AudioSource.PlayClipAtPoint(attackSound, transform.position);
				if(attacker.GetComponent<PlayerController>()){//If the attacker object is a player
					attacker.GetComponent<PlayerController>().SubtractHealth(attackStrength);//Attack Player
					if(attacker.GetComponent<PlayerController>().GetHealth() <= 0){//If the player is dead
						shouldRetaliate = false;
						storeTarget = true;
						resetTargetPosition(prevTargetPosition);
						GetComponent<DinosaurMovement>().Walk();
						attacker = null;
					}
				}
				else{ //The attacker object is another AI
					attacker.GetComponent<EnemyHealth>().decrementHealth(attackStrength, gameObject);//Attack AI
					if(attacker.GetComponent<EnemyHealth>().isDead()){//If the AI is dead
						shouldRetaliate = false;
						storeTarget = true;
						resetTargetPosition(prevTargetPosition);
						GetComponent<DinosaurMovement>().Walk();
						attacker = null;
					}
				}
			}
			else if(Vector3.Distance(transform.position, attacker.transform.position) >= attackRange){//Attempt to escape again
				shouldRetaliate = false;
				storeTarget = true;
				GetComponent<EvadePredators>().EscapePredator(attacker);

				//resetTargetPosition(prevTargetPosition);
				//GetComponent<DinosaurMovement>().Run();
				//runningTimer = runningDuration;
			}
			}
		}
		else{//Return to walking after a while if running from an attacker
			if(runningTimer >0){
				runningTimer -= Time.deltaTime;
				if(runningTimer <= 0){
					GetComponent<DinosaurMovement>().Run();
				}
			}
		}


	}

	public void setAttacker(GameObject att){
		attacker = att;
//		Debug.Log (attacker.name);
		if(attacker.name == "arrowProjectile(Clone)" || attacker.name == "arrowProjectile"){
			attacker = GameObject.Find("First Person Controller");
		}
	}

	private void resetTargetPosition(Vector3 targetPosition){
		GetComponent<DinosaurMovement>().AssignCurrentTargetPosition(targetPosition);
	}
}
