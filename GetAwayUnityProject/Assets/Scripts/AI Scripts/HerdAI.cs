using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PackAI))] //Auto-add PackAI script when this component is added
public class HerdAI : MonoBehaviour {
	//Note: This script requires the PackAI script and is intended for Pack-like gameobjects

	private Vector3 herdDirection;
	private Vector3 herdCenter;
	private float migrationDistance = 50f;
	private float migrationPointTimer;
	private float migrationPointInterval = 5f;

	// Use this for initialization
	void Start () {
		migrationPointTimer = migrationPointInterval;
		//Create a random direction to migrate in.
		float randomRotation = Random.Range (-179f, 179f);
		transform.rotation = Quaternion.AngleAxis(randomRotation +transform.localEulerAngles.y, new Vector3(0,1,0));
	}
	
	// Update is called once per frame
	void Update () {
		//Orient the herd in the proper direction every so often
		migrationPointTimer -= Time.deltaTime;
		if(migrationPointTimer<0f){
			migrationPointTimer = migrationPointInterval;

			for(int i = 0; i < this.GetComponent<PackAI>().entities.Length; i++){
				GameObject herdMember = this.GetComponent<PackAI>().entities[i];
				if(herdMember!=null && !herdMember.GetComponent<EvadePredators>().IsFleeing()){


					//Set a waypoint 50 units straight ahead if the dino is not currently avoiding an obstacle
					if(!herdMember.GetComponent<DinosaurMovement>().goToObstacleAvoidPoint){
						//Turn the herd member to face the migration direction (i.e. this.transform.forward)
						herdMember.transform.rotation = Quaternion.Slerp (herdMember.transform.rotation, transform.rotation, Time.deltaTime * herdMember.GetComponent<DinosaurMovement>().turnDamping);
						herdMember.GetComponent<DinosaurMovement>().AssignCurrentTargetPosition(transform.position + (transform.forward * migrationDistance));
					}
				}
			}
		}
	}
}
