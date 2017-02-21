using UnityEngine;
using System.Collections;

public class FlockingBehavior : MonoBehaviour {
	public GameObject targetObject, flockController;
	private CharacterController characterController;
	private Vector3 velocity, separation, targetPos;
	bool inited = false;
	public bool chasingPrey = false;

	void Start(){
		StartCoroutine("calculateSeparation");
	}

	public void setFlockController(GameObject controller){
		flockController = controller;
		FlockController controllerValues = flockController.GetComponent<FlockController>();
		targetObject = controller;
		targetPos = controller.transform.position;
		inited = true;
	}

	//Calculate a forward vector using flocking algorithm
	public Vector3 calculateForwardVector(){
		if(Vector3.Distance(transform.position, targetPos) >= 20){
			Vector3 randomize = new Vector3 ((Random.value *2) -1, (Random.value * 2) -1, (Random.value * 2) -1); //Add some randomness to each flock member
			FlockController flockcontroller = flockController.GetComponent<FlockController>();
			Vector3 flockCenter = flockcontroller.flockCenter;
			Vector3 flockDirection = flockcontroller.flockDirection;
			Vector3 follow = targetPos;

			flockCenter -= transform.position; //Get vector from this object to the flock center
			follow -= new Vector3(transform.position.x, 0, transform.position.z); //Get vector from this object to the target object
			return (flockCenter + flockDirection + follow + randomize + separation);
		}
		else
			return targetPos - transform.position;
	}
	//Calculates a new separation vector every 0.3-0.5 seconds
	IEnumerator calculateSeparation(){
		while(true){
			if(inited){
				int closeFlockMembers = 0;
				foreach(GameObject flockMember in flockController.GetComponent<FlockController>().flock){
					if(!gameObject.Equals(flockMember) && Vector3.Distance(transform.position, flockMember.transform.position) <= 15){
						closeFlockMembers++;
						Vector3 relativePosition = transform.position - flockMember.transform.position;
						separation += (relativePosition/(Vector3.Distance(transform.position, flockMember.transform.position))).normalized;

					}
				}
				//If no flock members are close during this update, reset separation vector
				//This allows flock member to go toward objective rather than keeping separation vector
				if(closeFlockMembers == 0)
					separation = Vector3.zero;
			}
			yield return new WaitForSeconds(Random.Range(0.3f, 0.5f));
		}
	}

	public void setFollowObject(GameObject target){
		targetObject = target;
	}

	public void setTargetPos(Vector3 pos){
		targetPos = pos;
	}
}
