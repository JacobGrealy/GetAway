using UnityEngine;
using System.Collections;

public class EvadePredators : MonoBehaviour {

	private SphereCollider detectionCollider;
	private DinosaurMovement movement;
	private ChaseAttackPrey chasing;
	private FoodChain.FoodChainLevels foodchainlevel;
	public float escapeDistance;
	private bool fleeing;
	private Vector3 escapeWaypoint;
	// Use this for initialization
	void Start () {
		detectionCollider = gameObject.GetComponent<SphereCollider> ();
		movement = this.GetComponent<DinosaurMovement> ();

		if(GetComponent<ChaseAttackPrey>())
			chasing = this.GetComponent<ChaseAttackPrey> ();

		foodchainlevel = GetComponent<FoodChain>().foodChainLevel;
		fleeing = false;
	}
	
	// Update is called once per frame
	void Update () {

		//If the dino reaches its escape point, resume normal behavior
		if (fleeing) {
			if(Vector3.Distance(new Vector3(transform.position.x,0f,transform.position.z),new Vector3(escapeWaypoint.x, 0, escapeWaypoint.z)) < 5){
				fleeing = false;
				movement.ResumePatrolling();
				if(GetComponent<ChaseAttackPrey>()){
					chasing.EnableChasing();
				}
			}
		}
	}

	public void EscapePredator(GameObject predator){
		movement.Run ();
		movement.StopPatrolling ();
		fleeing = true;
		if(GetComponent<ChaseAttackPrey>())
			chasing.chasingOverride ();

		//Create a waypoint away from the predator
		Vector3 oppositeDirection =  transform.position - predator.transform.position;
		oppositeDirection = new Vector3 (oppositeDirection.x, 0f, oppositeDirection.z);
		oppositeDirection.Normalize();
		oppositeDirection = oppositeDirection * escapeDistance;

		escapeWaypoint = transform.position + oppositeDirection;

		//Set the current objective to be the waypoint.
		movement.AssignCurrentTargetPosition(escapeWaypoint);

	}

	public bool IsFleeing(){
		return fleeing;
	}
	void OnTriggerStay(Collider potentialPredator)
	{
		if (potentialPredator.transform.position != transform.position) {
						//If the potential predator is a carnivore that is  higher on the food chain and the dino can see it, run away
						if (potentialPredator.GetComponent<FoodChain> ()) {
								if (!potentialPredator.GetComponent<FoodChain> ().isHerbivore ()) {
										if (potentialPredator.GetComponent<FoodChain> ().getFoodChainLevel () > foodchainlevel) {
												if (movement.CanSee (potentialPredator.gameObject)) {
														EscapePredator (potentialPredator.gameObject);
												}
										}
								}
						}
				}
	}
}
