using UnityEngine;
using System.Collections;

public class PackAI : MonoBehaviour {

	public int minimumPackSize;
	public int maximumPackSize;
	public float bufferZoneRadius;  	//How close the player needs to be to spawn entities
	public float packSpawnRadius;   	//The square within which the entities will be spawned
	public GameObject spawnedEntity;	//The entity prefab to be spawned
	public float checkSpawnTime;		//The amount of time to wait between checks of the player's position

	private GameObject player;
	public GameObject[] entities;		//An array to keep track of the spawned entites in the pack
	private bool entitiesSpawned;		//Tells whether the pack has spawned its entities
	private float checkSpawnTimer;		//Counts down how long until next checking of player position, resets to checkSpawnTime
	private int packSize;
	private Vector3 packCenter;
	// Use this for initialization
	void Start () {
		player = GameObject.Find ("First Person Controller");
		entitiesSpawned = false;
		checkSpawnTimer = checkSpawnTime;

		packSize = Random.Range (minimumPackSize, maximumPackSize);
		entities = new GameObject[packSize];
		CreateEntities();
	}
	
	// Update is called once per frame
	void Update () {

		//Reposition the spawner based on the location of its pack members
		if(entitiesSpawned){
			Vector3 theCenter = Vector3.zero; 
			int packLeft = 0;
			foreach(GameObject packMember in this.GetComponent<PackAI>().entities){
				if(packMember!=null){
					packLeft++;
					theCenter += packMember.transform.position;
					//Debug.Log(packMember.name + "position = " + packMember.transform.position);
				}
			}
			packCenter = theCenter/packLeft;
			this.transform.position = packCenter;
			
		}

		//Lock the spawner's height to that of the terrain at its position
		transform.position = new Vector3 (transform.position.x,Terrain.activeTerrain.SampleHeight(transform.position), transform.position.z);

		//Check if the spawner needs to spawn or despawn its entities
		checkSpawnTimer -= Time.deltaTime;
		if(checkSpawnTimer < 0){
			float playerDistance = Vector3.Distance(transform.position, player.transform.position);
			//Debug.Log(playerDistance);
			//If the player comes within the buffer radius and the entites have not already been spawned, spawn them
			if(playerDistance < bufferZoneRadius && !entitiesSpawned){
				SpawnEntities();
			}

			//If the player leaves the buffer radius and the entites are spawned, despawn them
			if(playerDistance > bufferZoneRadius && entitiesSpawned){
				DespawnEntities();
			}

			checkSpawnTimer = checkSpawnTime;
		}
	}

	void CreateEntities(){
		for(int i = 0; i < packSize; i++){

				//Get a random position within the spawn radius
				Vector3 position = new Vector3(Random.Range(transform.position.x, transform.position.x + packSpawnRadius),0f,Random.Range(transform.position.z, transform.position.z + packSpawnRadius));

				//Set its y value to that of the terrain height (plus a little to prevent clipping through floor)
				position = new Vector3(position.x,Terrain.activeTerrain.SampleHeight(position) + 1,position.z);

				//Spawn the entity
				entities[i] = (GameObject)Instantiate(spawnedEntity, position, Quaternion.identity);
		}
		entitiesSpawned = true;
	}
	void SpawnEntities(){
		for(int i = 0; i < packSize; i++){
			//If the entity hasn't been killed
			if(entities[i]!=null){

				//Get a random position within the spawn radius
				Vector3 position = new Vector3(Random.Range(transform.position.x, transform.position.x + packSpawnRadius),0f,Random.Range(transform.position.z, transform.position.z + packSpawnRadius));
				
				//Set its y value to that of the terrain height (plus a little to prevent clipping through floor)
				position = new Vector3(position.x,Terrain.activeTerrain.SampleHeight(position) + 1,position.z);

				//Transport the entity to its new position
				entities[i].transform.position = position;

				//Spawn the entity
				entities[i].SetActive(true);

				//Debug.Log (entities[i].name + " spawned.");
			}
		}
		entitiesSpawned = true;
	}
	void DespawnEntities(){
		for(int i = 0; i < packSize; i++){
			//If the entity hasn't been killed
			if(entities[i]!=null){

				//Despawn the entity
				entities[i].SetActive(false);
				//Debug.Log (entities[i].name + " despawned.");
			}
		}
		entitiesSpawned = false;
	}

	public bool EntitiesSpawned(){
		return entitiesSpawned;
	}
}
