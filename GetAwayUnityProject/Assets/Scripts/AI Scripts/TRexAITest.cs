using UnityEngine;
using System.Collections;

public class TRexAITest : MonoBehaviour {
	public float detectionDistance, attackRange, avoidance, visionAngle;
	private GameObject[] Prey;
	private GameObject currentTarget, currentObstacle;
	private bool preyDetected, avoidObstacle, setRotationPoint, preySearch,rotatedToPatrolPoint;
	private Vector3 currentDirection;
	private Quaternion avoidVector;
	private enum direction {left, right};
	private direction rotationDirection;
	private Transform head;
	private float searchTimer = 0.5f;
	public int attackStrength;

	//T-rex movement stuff (used)
	private CharacterController controller;
	private float movespeed;
	private Vector3 velocity;
	public float gravity,  walkSpeed, runSpeed, turnDamping;

	//Defines what level of the food chain this dinosaur is on
	//public FoodChainEnums.FoodChain foodChainLevel = FoodChainEnums.FoodChain.TRex;
	public FoodChain.FoodChainLevels foodChainLevel;

	//Stuff for T-rex idling after patrol points(used)
	private bool idling = false;
	public float minIdleTime;
	public float maxIdleTime;
	private float idleTimer;

	//Eating stuff(used)
	public float eatingDuration;
	private float eatingtimer;
	private bool eating = false;
	public int eatingAggressionDistance;

	//The amount by which the t-rex's size can vary, as a decimal value(used)
	public float sizeVariance;

	//T-rex sound stuff(used)
	public int minIdleSoundFrames; //minimum number of frames before an idle sound can be played
	public int maxIdleSoundFrames; //maximum """""
	private int idleSoundtimer; //Number of frames til next idle sound, randomly chosen between the above two values
	public AudioClip biteSound;
	public AudioClip roar1;
	public AudioClip roar2;
	public AudioClip growl1;
	public AudioClip growl2;
	private AudioSource sounds;

	//Patrolling stuff(used)
	private Vector3[] patrolPoints;
	private int numberPatrolPoints = 8; //This is now hardcoded, if you change it you need to change how patrol points are allocated
	public float patrolRadius;
	private int currentPatrolPointIndex = 0;
	private GameObject patrolPointSphere;
	private Vector3 currentTargetPosition, lastPreyPosition;
	private bool goToObstacleAvoidPoint;
	public float patrolPointSize;

	//Health stuff
	private EnemyHealth health;
	private bool dying = false;

	// Use this for initialization
	#region Start()
	void Start () {
		//Get this dinosaur's food chain level
		foodChainLevel = GetComponent<FoodChain>().foodChainLevel;
		controller = GetComponent<CharacterController>();
		sounds = GetComponent<AudioSource> ();
		//Detection trigger sphere
		SphereCollider collider = GetComponent<SphereCollider>();
		collider.radius = detectionDistance;

		preyDetected = false;
		avoidObstacle = false;
		setRotationPoint = true;
		preySearch = false;
		goToObstacleAvoidPoint = false;

		head = transform.FindChild("Head"); 

		idleTimer = Random.Range (minIdleTime, maxIdleTime);
		idleSoundtimer = Random.Range (minIdleSoundFrames, maxIdleSoundFrames);
		eatingtimer = eatingDuration;

		//Randomize size of t-rex
		float sizeModifier = 1f + Random.Range (-sizeVariance,sizeVariance); 
		transform.localScale = sizeModifier * transform.localScale;
		Debug.Log ("size Modifer = ");
		Debug.Log (sizeModifier);

		//Allocate the 8 patrol points around the initial position of the t-rex
		patrolPointSphere = GameObject.Find ("patrolPoint");
		patrolPoints = new Vector3[numberPatrolPoints];
		float nonCardinalDirectionModifier = 1.5f;
		patrolPoints [0] = new Vector3 (transform.position.x + patrolRadius, 0f, transform.position.z);
		patrolPoints [1] = new Vector3 (transform.position.x + patrolRadius/nonCardinalDirectionModifier, 0f, transform.position.z - patrolRadius/nonCardinalDirectionModifier);
		patrolPoints [2] = new Vector3 (transform.position.x, 0f, transform.position.z - patrolRadius);
		patrolPoints [3] = new Vector3 (transform.position.x - patrolRadius/nonCardinalDirectionModifier, 0f, transform.position.z - patrolRadius/nonCardinalDirectionModifier);
		patrolPoints [4] = new Vector3 (transform.position.x - patrolRadius, 0f, transform.position.z);
		patrolPoints [5] = new Vector3 (transform.position.x - patrolRadius/nonCardinalDirectionModifier, 0f, transform.position.z + patrolRadius/nonCardinalDirectionModifier);
		patrolPoints [6] = new Vector3 (transform.position.x, 0f, transform.position.z + patrolRadius);
		patrolPoints [7] = new Vector3 (transform.position.x + patrolRadius/nonCardinalDirectionModifier, 0f, transform.position.z + patrolRadius/nonCardinalDirectionModifier);
		currentTargetPosition = patrolPoints [0];
		patrolPointSphere.transform.position = patrolPoints[0];


		health = GetComponent<EnemyHealth>();
	}
#endregion
	// Update is called once per frame
	#region Update()
	void Update () {
		if (health.isDead ()){
			if (!animation.IsPlaying("die") && dying){
				Destroy(gameObject);
			}
			else if (!animation.IsPlaying("die") && !dying && !sounds.isPlaying){
				animation.Stop();
				animation.Play ("die");
				//AudioSource.PlayClipAtPoint(growl1,transform.position);
				sounds.Play();
				dying=true;
			}
		}
		else{
			Debug.DrawLine(transform.position, transform.position+transform.forward*detectionDistance, Color.red);
			//If prey is in the collider, but hasn't been seen yet, check to see if it is with the vision cone
			if(preySearch){
				searchTimer -= Time.deltaTime;
				if(searchTimer <= 0){
					if(preyInSight(currentTarget))
						preyDetected = true;
					idling = false;
					searchTimer = 0.5f;
				}
			}
			if(avoidObstacle){
				AvoidObstacle(currentObstacle);
			}
			if(preyDetected){
				if(!animation.IsPlaying("bite"))
					animation.CrossFade("run");
				if(!avoidObstacle)
					lookAtPrey(currentTarget);
				chasePrey(currentTarget);
				//lastPreyPosition = currentTarget.transform.position;
			}
			else if(idling){
				idle();
			}
			else if(eating){
				//After done biting, play the eating animation and a growl sound. Eating animation is currently placeheld by idle
				if(!animation.IsPlaying("idle") && !animation.IsPlaying("bite")){
					animation.CrossFade("idle");
					AudioSource.PlayClipAtPoint(growl1,transform.position);
				}
				eatingtimer --;
				if(eatingtimer<=0){
					eatingtimer = eatingDuration;
					eating = false;
					Debug.Log("Stopped eating");
				}
			}
			else{
				if(!animation.IsPlaying("bite"))
					animation.CrossFade("walk");
				Patrol();
			}
			idleSoundtimer--; 
			velocity = new Vector3 (transform.forward.x, -gravity, transform.forward.z);

			if (!idling && !eating &&!animation.IsPlaying ("bite")) {
				controller.Move (Time.deltaTime * movespeed * velocity);
			}

			//Keep the t-rex rotated properly
			transform.eulerAngles = new Vector3 (0f,transform.eulerAngles.y,0f);
		}
	}
	#endregion
	//Checks to see if the prey is within vision cone
	#region preyInSight(GameObject prey)
	bool preyInSight(GameObject prey){
		RaycastHit hit;
		Physics.Raycast(head.transform.position, prey.transform.position - head.transform.position, out hit);
	
		if(hit.transform.gameObject.Equals(prey.gameObject) //Nothing between T-rex and prey, and prey withing vision cone
		   && Vector3.Angle(transform.forward, prey.transform.position - transform.position) <= visionAngle){
			return true;
		}
		else
			return false;
	}
	#endregion

	#region AvoidObstacle(GameObject obstacle)
	void AvoidObstacle(GameObject obstacle){
		
		float angle = Vector3.Angle(transform.forward, obstacle.transform.position-transform.position);
		Debug.DrawLine(transform.position, obstacle.transform.position, Color.green);

		//Rather than rotating direction based on where the obstacle is, the direction of rotation should be towards the objective
		Vector3 relative = transform.InverseTransformPoint(currentTargetPosition);
		if(angle < avoidance){
			
			float dist = Vector3.Distance(transform.position, obstacle.transform.position);
			if(relative.x < 0&&setRotationPoint){ //Objective is to the left
				avoidVector = Quaternion.AngleAxis(avoidance-angle+transform.localEulerAngles.y, new Vector3(0,1,0));
				setRotationPoint = false;
				rotationDirection = direction.left;
			}
			else if(setRotationPoint){//Objective is to the right
				avoidVector = Quaternion.AngleAxis(angle-avoidance+transform.localEulerAngles.y, new Vector3(0,1,0));
				setRotationPoint = false;
				rotationDirection = direction.right;
			}
			transform.rotation = Quaternion.Slerp(transform.rotation, avoidVector, Time.deltaTime*turnDamping);
		}
		else{
			avoidObstacle = false;
			setRotationPoint = true;
			
			if(!preyDetected)
			{
				//Set a temporary patrol point so that the t-rex does not rotate back towards the obstacle
				goToObstacleAvoidPoint = true;
				currentTargetPosition = transform.position+transform.forward*(Vector3.Distance(new Vector3(transform.position.x, 0f, transform.position.z), new Vector3(currentObstacle.transform.position.x,0f,currentObstacle.transform.position.z)));
				patrolPointSphere.transform.position = currentTargetPosition;
				patrolPointSphere.renderer.material.color = Color.cyan;
			}
		}
	}
	#endregion

	//Rotate toward the prey
	#region lookAtPrey(GameObject prey)
	void lookAtPrey(GameObject prey){
		Quaternion targetVector = Quaternion.LookRotation(new Vector3(prey.transform.position.x - transform.position.x, 0, prey.transform.position.z - transform.position.z));
		transform.rotation = Quaternion.Slerp(transform.rotation, targetVector, Time.deltaTime*turnDamping);
	}
	#endregion

	#region chasePrey(GameObject prey)
	void chasePrey(GameObject prey){
		//If the "prey" gets too close while the t-rex is eating, or if the t-rex isn't eating, chase the prey
		if (!eating || Vector3.Distance (transform.position, prey.transform.position) <= eatingAggressionDistance) {
			movespeed = runSpeed;
			
			//If the prey is within attack distance, attack. !animation.IsPlaying part there to keep from attacking every frame
			if (Vector3.Distance (this.transform.position, prey.transform.position) < attackRange && !animation.IsPlaying("bite")) {
				animation.Play ("bite");
				AudioSource.PlayClipAtPoint(biteSound, transform.position);
				if(prey.GetComponent<PlayerController>()){//If the prey object is a player
					prey.GetComponent<PlayerController>().SubtractHealth(attackStrength);//Attack Player
					if(prey.GetComponent<PlayerController>().GetHealth() <= 0){//If the player is dead
						Debug.Log(prey.GetComponent<PlayerController>().GetHealth());
						eating = true;
						preyDetected = false;
						preySearch = false;
						Debug.Log("Eating");
					}
				}
				else{ //The prey object is another AI
					//prey.GetComponent<EnemyHealth>().decrementHealth(attackStrength);//Attack AI
					if(prey.GetComponent<EnemyHealth>().isDead()){//If the AI is dead
						eating = true;
						preyDetected = false;
						preySearch = false;
						Debug.Log("Eating");
					}
				}
			}
		}
	}
	#endregion

	#region idle()
	void idle(){
		//Play idle animation
		if(!animation.IsPlaying("idle")){
			animation.CrossFade("idle");
		}
		
		//Maintain idle timer
		idleTimer--;
		if(idleTimer<=0){
			idleTimer = Random.Range (minIdleTime, maxIdleTime);
			idling = false;
		}

		//Maintain idle sounds
		if (idleSoundtimer <= 0) {
			//If enough time has elapsed, play a random idle sound
			int sound = Random.Range(0,3);
			Debug.Log(sound);
			switch(sound){
			case 0:
				AudioSource.PlayClipAtPoint(growl1,transform.position);
				break;
			case 1:
				AudioSource.PlayClipAtPoint(roar1,transform.position);
				break;
			case 2:
				AudioSource.PlayClipAtPoint(growl2,transform.position);
				break;
			case 3:
				AudioSource.PlayClipAtPoint(roar2,transform.position);
				break;
			default:
				break;
			}

			idleSoundtimer = Random.Range(minIdleSoundFrames,maxIdleSoundFrames);
		}

	}
	#endregion

	#region Patrol()
	void Patrol(){
		
		//Rotate towards the patrol point if not rotating away from obstacle
		if (!avoidObstacle) {
			Quaternion patrolVector = Quaternion.LookRotation (currentTargetPosition - transform.position);
			transform.rotation = Quaternion.Slerp (transform.rotation, patrolVector, Time.deltaTime * turnDamping);
		}

		movespeed = walkSpeed;
		
		//If the trex comes within however many "feet" of current patrol point, switch to the next one (possibly randomize this eventually?)
		//Note that I'm not using Vector3.Distance because the patrol points are meant to be checked on the x and z plane, so the T-rex will 
		//hit them regardless of terrain height in the Y dimension --Nate
		float xDifference = Mathf.Abs (currentTargetPosition.x - transform.position.x);
		float zDifference = Mathf.Abs (currentTargetPosition.z - transform.position.z);
		if ((xDifference < patrolPointSize && zDifference < patrolPointSize)) {
			if(goToObstacleAvoidPoint){
				goToObstacleAvoidPoint = false;
				patrolPointSphere.renderer.material.color = Color.gray;
			}
			else{
				//Randomly choose a new patrol point to go to
				currentPatrolPointIndex = Random.Range(0,7);
				idling = true;
			}
			currentTargetPosition = patrolPoints[currentPatrolPointIndex];
			patrolPointSphere.transform.position = currentTargetPosition;
		}
		
	}
	#endregion

	//Keeps track of objects within detection radius
	#region OnTriggerStay(Collider target)
	void OnTriggerStay(Collider target){
		if(!target.gameObject.Equals(this.gameObject)){
		//Checks if the collider has a FoodChain script, then checks to see what level the target is on
		if(target.GetComponent<FoodChain>()){
			if(target.GetComponent<FoodChain>().getFoodChainLevel() < foodChainLevel){ //If target is on a lower level of food chain, chase the prey
				if(preyInSight(target.transform.gameObject)){
					preyDetected = true;
					//AudioSource.PlayClipAtPoint(roar1,transform.position);
				}
				else
					preySearch = true;
				currentTarget = target.transform.gameObject;
			}
		}

		else if(!target.transform.tag.Equals("Terrain") && !avoidObstacle){
			Debug.DrawLine(transform.position, target.transform.position);

			//Get the x and z distance between the dino and its destination and compare that to the distance between the dino and the obstacle
			float dinoToObstacleDistance = Vector3.Distance(new Vector3(transform.position.x, 0f, transform.position.z), new Vector3(target.transform.position.x,0f,target.transform.position.z));
			float dinoToDestinationDistance = Vector3.Distance(new Vector3(transform.position.x, 0f, transform.position.z), new Vector3(currentTargetPosition.x,0f,currentTargetPosition.z));

			//Avoid the obstacle if it is within the avoidance angle and the dino is looking at its destination and the obstacle is closer to the dino than its destination
			if(Vector3.Angle(transform.forward, target.transform.position-transform.position) < avoidance && Vector3.Angle(transform.forward, currentTargetPosition-transform.position) < 5 && dinoToDestinationDistance > dinoToObstacleDistance ){
				avoidObstacle = true;
				currentObstacle = target.transform.gameObject;
			}
			else
				avoidObstacle = false;
			
		}
		}
	}
	#endregion

	//Stop searching for prey object if it leaves detection radius
	#region OnTriggerExit(Collider target)
	void OnTriggerExit(Collider target){
		//if(target.transform.tag.Equals("T-Rex_Prey")){
		if(target.GetComponent<FoodChain>()){
			if(target.GetComponent<FoodChain>().getFoodChainLevel() < foodChainLevel){
			preySearch = false;
			preyDetected=false;
			searchTimer = 0.5f;
			}
		}
	}
	#endregion
}