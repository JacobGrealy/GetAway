using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider), typeof(CharacterController), typeof(EnemyHealth))]
[RequireComponent (typeof(AudioSource), typeof(Rigidbody))]
public class DinosaurMovement : MonoBehaviour {
	//Automatically add required components to object
	public bool debugWaypoints = false;
	private GameObject waypoint;

	//Determine how to implement movement
	private bool isFlockMember;
	private FlockingBehavior flockBehavior;

	//The amount by which the dinosaur's size can vary, as a decimal value
	public float sizeVariance;
	
	//Movement stuff
	private CharacterController controller;
	private Vector3 velocity;
	public float gravity,  walkSpeed, runSpeed, turnDamping, detectionDistance;
	private float movespeed;
	private bool stopMotion;

	//Vision stuff
	public float visionAngle;
	private Transform head;
	public float totalAwarenessRadius;

	//Patrolling stuff
	private bool patrolling;
	private Vector3[] patrolPoints;
	private int numberPatrolPoints = 8; //This is now hardcoded, if you change it you need to change how patrol points are allocated
	private int currentPatrolPointIndex = 0;
	//private GameObject patrolPointSphere; //visualization of patrol point, comment out or remove for final build
	public bool goToObstacleAvoidPoint;
	public float patrolRadius; //The radius of the area the dinosaur will patrol
	public float patrolPointSize; //How close to the patrol point the dinosaur is before selecting a new point
	private Vector3 currentTargetPosition; //The position of the current objective
	
	//Obstacle avoidance stuff
	public float avoidance;
	public float avoidanceCorrectionNumerator; //Multiplies the avoidance angle by the inverse of the distance to the obstacle times this number. Recommended value is 10.
	public float avoidObstacleInterval; //How many seconds between each checking for obstacles
	private float avoidObstacleTimer;
	private GameObject currentObstacle;
	private Quaternion avoidVector; //The vector that will avoid an obstacle, and the relative rotation needed to align with that vector
	private Vector3 obstacleAvoidanceWaypoint;
	private bool ignoreObstacles; //A way for other scripts to tell the dinosaur not to worry about running into things
	private SphereCollider collider;
	private ArrayList obstacles;

	//Sound stuff
	public int minIdleSoundFrames; //minimum number of frames before an idle sound can be played
	public int maxIdleSoundFrames; //maximum """""
	private int idleSoundtimer; //Number of frames til next idle sound, randomly chosen between the above two values
	public AudioClip roar1;
	public AudioClip roar2;
	public AudioClip growl1;
	public AudioClip growl2;
	private AudioSource sounds;
	
	//Stuff for idling after patrol points
	private bool idling = false;
	public float minIdleTime;
	public float maxIdleTime;
	private float idleTimer;
	
	//Health stuff
	private EnemyHealth health;
	private bool dying = false;

	void Awake(){
		obstacles = new ArrayList();
	}

	void OnEnable(){
		Walk();
	}

	// Use this for initialization
	void Start () {
		waypoint = GameObject.CreatePrimitive(PrimitiveType.Capsule);
		waypoint.transform.localScale += new Vector3(0, 80, 0);
		waypoint.SetActive(false);
//		obstacles = new ArrayList ();
		//If a FlockingBehavior script is attatched, set the flock member boolean
		if(GetComponent<FlockingBehavior>()){
			flockBehavior = GetComponent<FlockingBehavior>();
			isFlockMember = true;
		}
		controller = GetComponent<CharacterController>();
		sounds = GetComponent<AudioSource> ();

		//Detection trigger sphere
		collider = GetComponent<SphereCollider>();
		collider.radius = detectionDistance/transform.localScale.z;

		head = transform.FindChild ("Head");

		patrolling = true;
		goToObstacleAvoidPoint = false;
		ignoreObstacles = false;

		//Set up idling stuff
		idleTimer = Random.Range (minIdleTime, maxIdleTime);
		idleSoundtimer = Random.Range (minIdleSoundFrames, maxIdleSoundFrames);
		
		//Randomize size of t-rex
		float sizeModifier = 1f + Random.Range (-sizeVariance,sizeVariance); 
		transform.localScale = sizeModifier * transform.localScale;

		if(!isFlockMember)
			EstablishPatrolRadius ();

		//Start the dino out walking
		Walk();
		
		//Get dino's health stuff
		health = GetComponent<EnemyHealth>();

		avoidObstacleTimer = avoidObstacleInterval;


	}
	
	// Update is called once per frame
	void Update () {
		if(debugWaypoints){
			if(!waypoint.activeInHierarchy){
				waypoint.SetActive(true);
			}

			waypoint.transform.position = currentTargetPosition;
		}
		if (health.isDead ()){
			if (!animation.IsPlaying("die") && dying){
				Destroy(gameObject);
			}
			else if (!animation.IsPlaying("die") && !dying){
				sounds.Stop();
				animation.Stop();
				animation.CrossFade ("die");
				//AudioSource.PlayClipAtPoint(growl1,transform.position);
				sounds.Play();
				dying=true;
			}
		}
		else{

			DetectObstacles();

			//Debug.DrawLine(transform.position, transform.position+transform.forward*detectionDistance, Color.red);
			if(idling){
				Idle();
			}

			idleSoundtimer--; 

			//If idling or motion has been stopped by another script, only apply gravity and do not rotate
			if(idling || stopMotion){
				velocity = new Vector3 (0f, -gravity, 0f);
			}
			else{
				velocity = new Vector3 (transform.forward.x, -gravity, transform.forward.z);
				if(isFlockMember && !(GetComponent<ChaseAttackPrey>().isChasing()))
					currentTargetPosition = flockBehavior.targetObject.transform.position;
				/*if(isFlockMember){
					//currentTargetPosition = flockBehavior.calculateForwardVector();
					//transform.forward = flockBehavior.calculateForwardVector();
					currentTargetPosition = flockBehavior.targetObject.transform.position;
				}
				//else*/

					TurnTowardObjective();
			}

			controller.Move (Time.deltaTime * movespeed * velocity);
			
			//Make sure dinosaur only rotates around Y axis
			transform.eulerAngles = new Vector3 (0f,transform.eulerAngles.y,0f);
			avoidObstacleTimer -= Time.deltaTime;

		}
//		Debug.Log(Terrain.activeTerrain.SampleHeight(transform.position));
	}

	//Allows other scripts to assign an objective
	public void AssignCurrentTargetPosition(Vector3 pos){
		currentTargetPosition = new Vector3 (pos.x, 0f, pos.z);
		patrolling = false;

		//patrolPointSphere.transform.position = currentTargetPosition;
	}

	public Vector3 getCurrentTargetPosition(){
		return currentTargetPosition;
	}

	//Establishes a patrol radius around the dinosaur's current position
	public void EstablishPatrolRadius (){
		//Allocate the 8 patrol points around the initial position of the t-rex
		//patrolPointSphere = GameObject.Find ("patrolPoint");
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
		//patrolPointSphere.transform.position = currentTargetPosition;
	}

	//Change speed functions
	public void Walk(){
		animation.CrossFade ("walk");
		movespeed = walkSpeed + Random.value;
		idling = false;
		stopMotion = false;
	}
	public void Run(){
		animation.CrossFade ("run");
		movespeed = runSpeed;
		idling = false;
		stopMotion = false;
	}

	//Allows other scripts to tell the dino whether to avoid obstacles
	public void SetIgnoreObstacles(bool decision){
		ignoreObstacles = decision;
		if (ignoreObstacles) {
			goToObstacleAvoidPoint = false;
		}
	}

	//Stops/starts movement(aside from gravity) and rotation. This is left up to caller script to change back when necessary.
	public void StopMotion(){
		idling = false;
		stopMotion = true;
	}

	//Functions to start and stop patrolling
	public void StopPatrolling(){
		patrolling = false;
	}
	public void ResumePatrolling(){
		Walk ();
		patrolling = true;
		if(isFlockMember)
			flockBehavior.setTargetPos(flockBehavior.flockController.GetComponent<FlockController>().getCurrentTargetPosition());
			//flockBehavior.setFollowObject(flockBehavior.flockController);
		else
			EstablishPatrolRadius ();
		SetIgnoreObstacles (false);
	}

	//Checks if the dino can see the given game object
	public bool CanSee(GameObject item){
		bool canSee = false;
		RaycastHit hit;
		//If nothing between dinosaur and item, and item withing vision cone, return true
		if ((Vector3.Angle (transform.forward, item.transform.position - transform.position) <= visionAngle && Vector3.Distance(item.transform.position,transform.position) <= detectionDistance)) {
			if(Physics.Raycast(head.transform.position, item.transform.position - head.transform.position, out hit)){
				if(hit.transform.IsChildOf(item.transform)){
					canSee = true;
				}
			}

		}
		else if(Vector3.Distance(item.transform.position, transform.position)< totalAwarenessRadius){
			canSee = true;
		}
		return canSee;
	}


	/////////////////Movement and idling functions\\\\\\\\\\\\\\\\\\\\\\
	//Avoid a detected obstacle if appropriate
	void AvoidObstacle(GameObject obstacle){
		float angle = Vector2.Angle(new Vector2(transform.forward.x, transform.forward.z), new Vector2((obstacle.transform.position-transform.position).x, (obstacle.transform.position-transform.position).z));
		//float angle = Vector3.Angle(transform.forward, obstacle.transform.position-transform.position);
		float avoidanceCorrection = avoidanceCorrectionNumerator/Vector3.Distance(new Vector3(transform.position.x, 0f, transform.position.z), new Vector3(obstacle.transform.position.x,0f,obstacle.transform.position.z));
		//Debug.DrawLine(transform.position, obstacle.transform.position, Color.green);
		
		//Rather than rotating direction based on where the obstacle is, we rotate based on where the objective is relative to the obstacle
		Vector3 targetRelativePosition = transform.InverseTransformPoint(currentTargetPosition);
		Vector3 obstacleRelativePosition = transform.InverseTransformPoint(obstacle.transform.position);
		if(angle < avoidance){
			
			float avoidanceRotationAngle;
			//If the objective is to the left of the obstacle as well, rotate left. If it is to the right of the obstacle, rotate right
			if(targetRelativePosition.x < obstacleRelativePosition.x){
				avoidanceRotationAngle = avoidanceCorrection * (avoidance-angle);
			}
			else{
				avoidanceRotationAngle = avoidanceCorrection * (angle-avoidance);
			}

			//Correct to make sure dino doesn't rotate more than 90 degrees
			if(avoidanceRotationAngle < -90f)
				avoidanceRotationAngle = -90f;
			else if(avoidanceRotationAngle > 90f)
				avoidanceRotationAngle = 90f;

			//Rotate by the avoidanceRotationAngle
			avoidVector = Quaternion.AngleAxis(avoidanceRotationAngle +transform.localEulerAngles.y, new Vector3(0,1,0));

			//Set an avoidance waypoint on the x-z plane to go around the obstacle by rotating the dino and extending its forward vector to create the waypoint, then rotating it back to its original rotation
			Quaternion previousRotation = transform.rotation;
			transform.rotation = avoidVector;
			goToObstacleAvoidPoint = true;

			//Base the avoidance waypoint on x-z plane distance
			obstacleAvoidanceWaypoint = new Vector3(transform.position.x,0f,transform.position.z)+transform.forward* Vector3.Distance(new Vector3(transform.position.x, 0f, transform.position.z), new Vector3(obstacle.transform.position.x,0f,obstacle.transform.position.z));
			transform.rotation = previousRotation;

			//This is just debugging stuff, comment it out or delete it for the final build
			//patrolPointSphere.transform.position = obstacleAvoidanceWaypoint;
			//patrolPointSphere.renderer.material.color = Color.cyan;

		}
	}

	void Idle(){
		//Play idle animation
		if(!animation.IsPlaying("idle")){
			animation.CrossFade("idle");
		}

		//Maintain idle timer
		idleTimer--;
		
		//If no longer idling, reset the idle timer and play walking animation again
		if(idleTimer<=0){
			idleTimer = Random.Range (minIdleTime, maxIdleTime);
			idling = false;
			Walk ();
		}
		
		//Maintain idle sounds
		if (idleSoundtimer <= 0) {
			//If enough time has elapsed, play a random idle sound
			int sound = Random.Range(0,3);
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
	
	public void TurnTowardObjective(){

		Quaternion desiredMovementDirection;
		float xDifference, zDifference;

		//Rotate towards the patrol point if not rotating away from obstacle
		if (goToObstacleAvoidPoint){
			desiredMovementDirection = Quaternion.LookRotation (obstacleAvoidanceWaypoint - new Vector3(transform.position.x,0f,transform.position.z));
			xDifference = Mathf.Abs (obstacleAvoidanceWaypoint.x - transform.position.x);
			zDifference = Mathf.Abs (obstacleAvoidanceWaypoint.z - transform.position.z);
		}
		else{
			if(isFlockMember)
				desiredMovementDirection = Quaternion.LookRotation(flockBehavior.calculateForwardVector());
			else
				desiredMovementDirection = Quaternion.LookRotation (currentTargetPosition - new Vector3(transform.position.x,0f,transform.position.z));
			xDifference = Mathf.Abs (currentTargetPosition.x - transform.position.x);
			zDifference = Mathf.Abs (currentTargetPosition.z - transform.position.z);
			if(debugWaypoints){
				Debug.Log("xDifference = " + xDifference + "\nzDifference = " + zDifference);
				Debug.Log("patrolling = " + patrolling.ToString());
			}
		}

		//Rotate towards the appropriate waypoint if not already rotating to avoid an obstacle
		transform.rotation = Quaternion.Slerp (transform.rotation, desiredMovementDirection, Time.deltaTime * turnDamping);

		//If the dino comes within however many "feet" of current patrol point, switch to the next one (possibly randomize this eventually?)
		//Note that I'm not using Vector3.Distance because the patrol points are meant to be checked on the x and z plane, so the T-rex will 
		//hit them regardless of terrain height in the Y dimension --Nate
		if ((xDifference < patrolPointSize && zDifference < patrolPointSize)) {
			//If the dino was patrolling
			if(patrolling){
				//If the dino was avoiding an obstacle
				if(goToObstacleAvoidPoint){
					goToObstacleAvoidPoint = false;
					//patrolPointSphere.renderer.material.color = Color.gray;
				}
				//If the dino reached a patrol point, idle and select a new patrol point. Other cases will be handled by other scripts
				else{
					idling = true;
					//Randomly choose a new patrol point to go to
					currentPatrolPointIndex = Random.Range(0,7);	
				}
				if(!isFlockMember)
				currentTargetPosition = patrolPoints[currentPatrolPointIndex];
				//patrolPointSphere.transform.position = currentTargetPosition;
			}
			//Otherwise, the dino was moving to some other objective given to it by another script, so wait for further instructions
		}
	}

	void DetectObstacles(){
		if(!ignoreObstacles && avoidObstacleTimer <=0){

			avoidObstacleTimer = avoidObstacleInterval;
			foreach(GameObject target in obstacles){
				if(target!=null){
					//Debug.DrawLine(transform.position, target.transform.position);
					
					//Check if the obstacle is between the dino and its target
					Vector3 currentWaypoint;
					RaycastHit hit;
					if(goToObstacleAvoidPoint){
						currentWaypoint = obstacleAvoidanceWaypoint;
					}
					else{
						currentWaypoint = currentTargetPosition;
					}

					Vector3 positionYZeroed = new Vector3(transform.position.x, 0f, transform.position.z);
					Vector3 vectorToWaypoint = currentWaypoint - positionYZeroed;

					if(!target.transform.tag.Equals("Terrain") && !target.GetComponent<FoodChain>()){

						//Avoid the obstacle if it is within the avoidance angle and the dino is looking at its destination and the obstacle is closer to the dino than its destination
						float angleBetween = Vector2.Angle(new Vector2(transform.forward.x, transform.forward.z), new Vector2((target.transform.position-transform.position).x, (target.transform.position-transform.position).z));
						//Debug.Log(angleBetween);
						//if(Vector3.Angle(transform.forward, target.transform.position-transform.position) < avoidance && Vector3.Angle(transform.forward, vectorToWaypoint)<10){
						if(angleBetween < avoidance && Vector3.Angle(transform.forward, vectorToWaypoint)<10){
							//Debug.Log("in avoidance " + angleBetween);
							if(Physics.Raycast(head.transform.position, currentWaypoint - head.transform.position, out hit)){
								if(hit.transform.gameObject.Equals(target.gameObject)){
										AvoidObstacle(target.transform.gameObject);
								}
							}
						}
					}
					else if(target.transform.tag.Equals("Terrain")){//If the target is terrain, make sure that the slope is low enough for the dino to walk up

						if(Physics.Raycast(head.transform.position, transform.forward, out hit, detectionDistance)){

							if(hit.transform.gameObject.Equals(target.gameObject)){
								Debug.Log(transform.name+" checking slope");
								//Create a terrain object to hold info about the terrain
								Terrain terrain = (Terrain) GetComponent(typeof(Terrain));
								
								//Get the position where the raycast hit the terrain
								Vector3 terrainLocalPos = terrain.transform.InverseTransformPoint(hit.point);
								Vector2 normalizedPos = new Vector2(Mathf.InverseLerp(0.0f, terrain.terrainData.size.x, terrainLocalPos.x), Mathf.InverseLerp(0.0f, terrain.terrainData.size.z, terrainLocalPos.z));
								
								//Get the slope of the terrain at this point
								float slope = terrain.terrainData.GetSteepness(normalizedPos.x, normalizedPos.y);
								
								//If the slope is too steep for the dinosaur to walk up, walk around it.
								if(slope > this.GetComponent<CharacterController>().slopeLimit){
									Debug.Log("too steep");

									//Rotate by the avoidanceRotationAngle
									avoidVector = Quaternion.AngleAxis(avoidance + transform.localEulerAngles.y, new Vector3(0,1,0));
									
									//Set an avoidance waypoint on the x-z plane to go around the obstacle by rotating the dino and extending its forward vector to create the waypoint, then rotating it back to its original rotation
									Quaternion previousRotation = transform.rotation;
									transform.rotation = avoidVector;
									goToObstacleAvoidPoint = true;
									
									//Base the avoidance waypoint on x-z plane distance
									obstacleAvoidanceWaypoint = new Vector3(transform.position.x,0f,transform.position.z)+transform.forward* Vector3.Distance(new Vector3(transform.position.x, 0f, transform.position.z), new Vector3(hit.point.x,0f,hit.point.z));
									transform.rotation = previousRotation;
								}
							} //end if hit
						}//end if raycast
					}//end else if
				}//end if(target!=null)
			}//end foreach
		}//end first if
	}//end function

	//////////////////////Trigger and Collider functions\\\\\\\\\\\\\\\\\\\\\\\
	void OnTriggerEnter(Collider target){		//Debug.Log("Target obstacle = " + target.gameObject.name + " Obstacles detected = " + obstacles.Count);
		obstacles.Add (target.gameObject);
	}

	void OnTriggerExit(Collider target){        //Debug.Log("Removed obstacle = " + target.gameObject.name + " Obstacles detected = " + obstacles.Count);
		obstacles.Remove (target.gameObject);
	}

}