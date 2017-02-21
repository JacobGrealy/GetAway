using UnityEngine;
using System.Collections;

public class FlockController : MonoBehaviour {
	public int maxFlockSize, minFlockSize;
	public int flockBoundary; //Determins the radius that the flock will be spawned at. May not need this
	public GameObject[] flock;
	public GameObject prefab; //The prefab that will be used
	public Vector3 flockDirection, flockCenter;
	public float patrolRadius;
	private int numberPatrolPoints = 8;
	private Vector3[] patrolPoints;
	private Vector3 currentTargetPosition;
	private bool preyDetected;
	private GameObject preyObject;
	private int numChasingPrey;

	// Use this for initialization
	void Start () {
		flock = new GameObject[Random.Range(minFlockSize, maxFlockSize)];
		//Instantiate the flock, assigning a random value to each initial start position
		for(int i = 0; i<flock.Length; i++){
			GameObject flockMember = Instantiate(prefab, transform.position, transform.rotation) as GameObject;
			flockMember.transform.position = new Vector3(transform.position.x+Random.value * flockBoundary, transform.position.y +15, transform.position.z+Random.value * flockBoundary);
			flockMember.name = "FlockMember_"+i.ToString();

			flockMember.transform.parent = transform;
			flock[i] = flockMember;
			flockMember.GetComponent<FlockingBehavior>().setFlockController(gameObject);
		}
		EstablishPatrolRadius();
		foreach(GameObject flockMember in flock){
			flockMember.GetComponent<FlockingBehavior>().setTargetPos(currentTargetPosition);				
		}
		preyDetected = false;
	}
	
	// Update is called once per frame
	// Each update recalculates variables used in flocking algorithm
	void Update () {
		Vector3 theDirection = Vector3.zero;
		Vector3 theCenter = Vector3.zero; 
		foreach(GameObject flockMember in flock){
			if(flockMember!=null){
				theDirection += flockMember.transform.forward;
				theCenter += flockMember.transform.position;
			}
			//Debug.Log(flockMember.name + "position = " + flockMember.transform.position);
		}
		flockDirection = theDirection/flock.Length;
		flockDirection.Normalize();
		flockCenter = theCenter/flock.Length;

		//If one flock member detects prey, all members are notified
		if(preyDetected){
			foreach(GameObject flockMember in flock){
				if(!flockMember.GetComponent<FlockingBehavior>().chasingPrey){
					flockMember.GetComponent<ChaseAttackPrey>().ChasePrey(preyObject);
					flockMember.GetComponent<FlockingBehavior>().chasingPrey = true;
				}
			}
		}
		//If the flock center is close to the patrol point, move the patrol point
		else if(Vector3.Distance(currentTargetPosition, flockCenter) <= 20){
			currentTargetPosition = patrolPoints[Random.Range(0, 8)];
			foreach(GameObject flockMember in flock){
				flockMember.GetComponent<FlockingBehavior>().setTargetPos(currentTargetPosition);				
			}
		}
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

	public void setPreyDetected(GameObject prey){
		preyDetected = true;
		preyObject = prey;
		numChasingPrey = flock.Length;
	}

	public void clearPreyDetected(){
		preyDetected = false;
	}

	public bool getPreyDetected(){
		return preyDetected;
	}

	public void decrementChasingCount(){
		numChasingPrey--;
		if(numChasingPrey<=0){
			preyDetected = false;
			foreach(GameObject flockMember in flock)
				flockMember.GetComponent<FlockingBehavior>().chasingPrey = false;
		}
	}

	public int getChasingCount(){
		return numChasingPrey;
	}

	public Vector3 getCurrentTargetPosition(){
		return currentTargetPosition;
	}
}
