using UnityEngine;
using System.Collections;

[RequireComponent(typeof(FoodChain))]
public class ChaseAttackPrey : MonoBehaviour {

	//Defines what level of the food chain this dinosaur is on
	//public FoodChainEnums.FoodChain foodChainLevel = FoodChainEnums.FoodChain.TRex;
	private FoodChain.FoodChainLevels foodChainLevel;
	private bool isHerbivore;
	//Allow this script to alter the dinosaur's movement patterns
	private DinosaurMovement movement;

	//Searching/Chasing for prey stuff
	private float searchTimer;
	public float searchTimerLength;
	private Transform head; //Get the head from the dinosaur to raycast from for detecting prey
	private Vector3 lastSeenPreyPosition;
	private GameObject currentPrey;
	private bool chasingPrey;
	private SphereCollider detectionCollider;
	private bool canChase;

	//Attack stuff
	public AudioClip attackSound;
	public string attackAnimationName;
	public int attackStrength;
	public float attackRange;
	public float attackAngle;
	//Eating stuff
	public string eatingAnimationName;
	public AudioClip eatingSound;
	public float eatingDuration;
	private float eatingtimer;
	private bool eating;
	public int eatingAggressionDistance;
	private GameObject preyBeingEaten;

	//Temporary fix for chasing player object
	private GameObject player;

	//Flocking stuff
	private bool isFlockMember = false;
	private FlockingBehavior flockBehavior;

	// Use this for initialization
	void Start () {
		foodChainLevel = GetComponent<FoodChain>().foodChainLevel;
		isHerbivore = GetComponent<FoodChain> ().herbivorous;
		movement = GetComponent<DinosaurMovement> ();
		eatingtimer = eatingDuration;
		head = transform.FindChild("Head");
		chasingPrey = false;
		canChase = true;
		eating = false;
		searchTimer = searchTimerLength;
		player = GameObject.FindGameObjectWithTag("Player");
		if(GetComponent<FlockingBehavior>()){
			flockBehavior = GetComponent<FlockingBehavior>();
			isFlockMember = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		//Handle eating if necessary
		if(eating){
			Eat ();
		}

		//otherwise, if the dinosaur is chasing prey and has lost track of it after reaching its last known position, stop chasing prey
		if(currentPrey!= null)
			CheckForLostPrey (currentPrey);


	}

	//Checks if the dino still knows where its prey is
	void CheckForLostPrey(GameObject prey){
		if(chasingPrey){
			searchTimer -= Time.deltaTime;
			if(searchTimer <= 0){
				//If the dino has gotten to the last place it saw the prey and can't find it, stop chasing it and resume patrolling
				if (Vector3.Distance (new Vector3(transform.position.x, 0, transform.position.z),new Vector3(lastSeenPreyPosition.x, 0, lastSeenPreyPosition.z)) < 5 && !movement.CanSee(currentPrey)){
					if(isFlockMember){
						flockBehavior.flockController.GetComponent<FlockController>().decrementChasingCount();
						if(flockBehavior.flockController.GetComponent<FlockController>().getChasingCount() <= 0){
							chasingPrey = false;
							movement.ResumePatrolling();
							movement.Walk();
							//Debug.Log("prey lost");
						}
					}
					else{
					chasingPrey = false;
					movement.ResumePatrolling();
					movement.Walk();
					//Debug.Log("prey lost");
					}
				}
				searchTimer = searchTimerLength;
			}
		}
	}

	void Eat(){
		//After done biting, play the eating animation and a growl sound. Eating animation is currently placeheld by idle
		if(!animation.IsPlaying(eatingAnimationName) && !animation.IsPlaying(attackAnimationName)){
			AudioSource.PlayClipAtPoint(eatingSound,transform.position);
			animation.Play(eatingAnimationName);
			movement.StopMotion();
		}
		chasingPrey = false;
		//Debug.Log("Eating");
		eatingtimer --;
		if(eatingtimer<=0){
		//	Debug.Log("stopped eating");
			eatingtimer = eatingDuration;
			eating = false;
			movement.ResumePatrolling();
		}

	}


	public void ChasePrey(GameObject prey){
		//If the "prey" gets too close while the t-rex is eating, or if the t-rex isn't eating, chase the prey
		if (!eating || Vector3.Distance (transform.position, prey.transform.position) <= eatingAggressionDistance) {
			//Debug.Log("chasing " + prey.gameObject.name);
			chasingPrey = true;
			movement.SetIgnoreObstacles(true);

			if(!animation.IsPlaying(attackAnimationName))
				movement.Run();
			eating = false;
			//Debug.Log("Eating is false");

			//Assign the prey as the dinosaur's current objective
			lastSeenPreyPosition = prey.transform.position;
			currentPrey = prey;
			if(isFlockMember){
				flockBehavior.setTargetPos(lastSeenPreyPosition);
				if(!flockBehavior.flockController.GetComponent<FlockController>().getPreyDetected())
					flockBehavior.flockController.GetComponent<FlockController>().setPreyDetected(prey);
			}
				//flockBehavior.setFollowObject(prey);


			//Assign the prey as the new target and stop patrolling
			movement.StopPatrolling();
			movement.AssignCurrentTargetPosition(prey.transform.position);

			//If the prey is within attack distance, attack. !animation.IsPlaying part there to keep from attacking every frame
			if (Vector3.Distance (this.transform.position, prey.transform.position) < attackRange && !animation.IsPlaying(attackAnimationName) && Vector3.Angle(transform.forward, prey.transform.position-transform.position) <= attackAngle) {

				animation.Play (attackAnimationName);
				movement.StopMotion();
				AudioSource.PlayClipAtPoint(attackSound, transform.position);
				//Debug.Log(gameObject.name + "Attacking" + currentPrey.gameObject.name);
				if(prey.GetComponent<PlayerController>()){//If the prey object is a player
					prey.GetComponent<PlayerController>().SubtractHealth(attackStrength);//Attack Player
					if(prey.GetComponent<PlayerController>().GetHealth() <= 0){//If the player is dead
						eating = true;
						//Debug.Log("Eating");
						chasingPrey = false;
					}
				}
				else{ //The prey object is another AI
					prey.GetComponent<EnemyHealth>().decrementHealth(attackStrength, gameObject);//Attack AI
					if(prey.GetComponent<EnemyHealth>().isDead()){//If the AI is dead
						eating = true;
						//Debug.Log("Eating = " + eating);
						chasingPrey = false;
					}
				}
			}
		}
	}

	public bool IsHerbivore(){
		return isHerbivore;
	}

	public void EnableChasing(){
		canChase = true;
	}
	public void chasingOverride(){
		chasingPrey = false;
		eating = false;
		canChase = false;
		movement.SetIgnoreObstacles(false);
	}

	public bool isChasing(){
		return chasingPrey;
	}

	void OnTriggerStay(Collider prey){
		if(!prey.gameObject.Equals(this.gameObject)){
		if(canChase){
			//Checks if the collider has a FoodChain script, then checks to see what level the prey is on
			if(prey.GetComponent<FoodChain>()){

				
				//If prey is on a lower level of food chain, possibly chase the prey if it's in sight
				if(prey.GetComponent<FoodChain>().getFoodChainLevel() < foodChainLevel && !isHerbivore){

					if(movement.CanSee(prey.transform.gameObject)){
							if(!prey.GetComponent<EnemyHealth>().isDead()){
							//Debug.DrawLine(head.transform.position,prey.transform.position, Color.red);
							bool samePrey; 

							if(currentPrey != null){
								samePrey = prey.transform.position == currentPrey.transform.position;
							}
							else {
								samePrey = false;
							}
								
							//if the dino isn't currently chasing prey, chase this prey
							if(!chasingPrey){
								ChasePrey(prey.transform.gameObject);
							}
							//Otherwise, if the new prey is closer than the last place the dino saw its old prey, chase the new prey instead instead
							else if(Vector3.Distance(transform.position,prey.transform.position) < Vector3.Distance(transform.position,lastSeenPreyPosition) &&!samePrey){
							    ChasePrey(prey.transform.gameObject);
							}
							//Otherwise, if it is the same preyobject and there is nothing closer, chase it
							else if(samePrey){
								ChasePrey(prey.transform.gameObject);
							}
						}
					}
				}
			}
		}
	}
	}
	

}//end Monobehavior