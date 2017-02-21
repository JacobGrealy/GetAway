using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreeSpawner : MonoBehaviour 
{
	[System.Serializable]
	public class Locations
	{
		public bool beach;
		public bool grassLand;
		public bool jungle;
		public bool cliff;
		public bool mountain;
		public bool road;
		public bool ocean;
	}
	
	[System.Serializable]
	public class SpawnableObject
	{
		public LODObject lodObject;
		public float minScale = 1f;
		public float maxScale = 1f;
		public int numberToSpawn;
		public Locations whereToSpawn;
	}

	[System.Serializable]
	public class SpawnableNonLodObject
	{
		public GameObject gameObject;
		public float minScale = 1f;
		public float maxScale = 1f;
		public int numberToSpawn;
		public Locations whereToSpawn;
	}

	public GameObject postTerrainGeneration;
	public TerrainGenerator terrainGenerator;
	public Terrain terrain;
	public LodSystem lodSystem;
	public LoadingScreen loadingScreen;
	
	public SpawnableObject[] grassToSpawn; 
	public SpawnableObject[] objectsToSpawn; 
	public SpawnableNonLodObject[] nonLodObjectsToSpawn;

	private int idCount = 0; //unique id for every spawned item

	public TreeSpawner(){}

	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	public IEnumerator SpawnCoroutine()
	{
		IList<Vector3> junglePositions;
		TerrainData tData = terrain.terrainData;
		int width = tData.heightmapWidth;
		int length =  tData.heightmapHeight;
		
		lodSystem.init(terrain.GetPosition(),tData.size.x,tData.size.z); //initialize the lodSystem
		
		//generate lists of spawn locations
		Console.print("Creating List Of Spawn Locations...");
		IList<Vector3>[] spawnPositions = new IList<Vector3>[8];
		for (int i = 0; i < 8; i++)
		{
			spawnPositions[i] = new List<Vector3>();
		}
		
		//Sort spawn locations by terrain type
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < length; y++)
			{	
				float height = tData.GetInterpolatedHeight(1.0f*x/width,1.0f*y/length);
				float posX = terrain.GetPosition().x + tData.size.x * (1.0f * x/width);
				float posZ = terrain.GetPosition().z + tData.size.z * (1.0f * y/length);
				float posY = terrain.GetPosition().y + height;
				Vector3 tempPos = new Vector3(posX,posY,posZ);
				
				spawnPositions[terrainGenerator.getTerrainType(x,y,width,length)].Add(tempPos);
			}
			yield return null;
		}
		
		int index;
		Vector3 posToSpawn;
		IList<int> selectedLocations = new List<int> ();
		
		//spawn lod objects
		Console.print("Spawning Enviroment Objects...");
		foreach (SpawnableObject spawnableObject in objectsToSpawn) 
		{
			Locations loc = spawnableObject.whereToSpawn;
			for(int i = 0; i < spawnableObject.numberToSpawn; i++)
			{
				int tType = 0;
				if(loc.beach) selectedLocations.Add (TerrainTypes.BEACH);
				if(loc.grassLand) selectedLocations.Add (TerrainTypes.MEADOW);
				if(loc.jungle) selectedLocations.Add (TerrainTypes.JUNGLE);
				if(loc.cliff) selectedLocations.Add (TerrainTypes.CLIFF);
				if(loc.mountain) selectedLocations.Add (TerrainTypes.MOUNTAIN);
				if(loc.road) selectedLocations.Add (TerrainTypes.ROAD);
				if(loc.ocean) selectedLocations.Add (TerrainTypes.OCEAN);
				
				if(selectedLocations.Count < 1) Console.print ("Error, no location was selected for object to spawn");
				tType = selectedLocations[Random.Range(0,selectedLocations.Count)];
				selectedLocations = new List<int> ();
				
				index = Random.Range(0,spawnPositions[tType].Count);
				posToSpawn = spawnPositions[tType][index];
				Quaternion spawnRotation = spawnableObject.lodObject.orignalLods[0].transform.rotation; spawnRotation.eulerAngles = new Vector3(spawnRotation.eulerAngles.x,Random.Range(0,360f),spawnRotation.eulerAngles.z); 
				float scale = Random.Range(spawnableObject.minScale,spawnableObject.maxScale);
				lodSystem.addObject(spawnableObject.lodObject.Instantiate(posToSpawn,spawnRotation,scale));
				//remove spawned position from list
				spawnPositions[tType].RemoveAt(index);
			}
			yield return null;
		}

		Console.print("Spawning Dinosaurs...");
		//spawn non lod objects (dinosaurs)
		GameObject dinosaurs = new GameObject ("Dinosaurs");
		dinosaurs.transform.parent = postTerrainGeneration.transform;
		foreach (SpawnableNonLodObject spawnableObject in nonLodObjectsToSpawn) 
		{
			Locations loc = spawnableObject.whereToSpawn;
			for(int i = 0; i < spawnableObject.numberToSpawn; i++)
			{
				int tType = 0;
				if(loc.beach) selectedLocations.Add (TerrainTypes.BEACH);
				if(loc.grassLand) selectedLocations.Add (TerrainTypes.MEADOW);
				if(loc.jungle) selectedLocations.Add (TerrainTypes.JUNGLE);
				if(loc.cliff) selectedLocations.Add (TerrainTypes.CLIFF);
				if(loc.mountain) selectedLocations.Add (TerrainTypes.MOUNTAIN);
				if(loc.road) selectedLocations.Add (TerrainTypes.ROAD);
				if(loc.ocean) selectedLocations.Add (TerrainTypes.OCEAN);
				
				//possible error if no location was selected
				if(selectedLocations.Count < 1) Console.print ("Error, no location was selected for object to spawn");
				tType = selectedLocations[Random.Range(0,selectedLocations.Count)];
				selectedLocations = new List<int> ();
				
				index = Random.Range(0,spawnPositions[tType].Count-1);
				posToSpawn = spawnPositions[tType][index];
				Quaternion spawnRotation = spawnableObject.gameObject.transform.rotation;
				float scale = Random.Range(spawnableObject.minScale,spawnableObject.maxScale);
				(GameObject.Instantiate(spawnableObject.gameObject,posToSpawn,spawnRotation) as GameObject).transform.parent = dinosaurs.transform;
				//remove spawned position from list
				spawnPositions[tType].RemoveAt(index);
			}			
			yield return null;
		}

		Console.print("Spawning Grass...");
		//spawn grass objects 
		foreach (SpawnableObject spawnableObject in grassToSpawn) 
		{
			Locations loc = spawnableObject.whereToSpawn;
			for(int i = 0; i < spawnableObject.numberToSpawn; i++)
			{
				int tType = 0;
				if(loc.beach) selectedLocations.Add (TerrainTypes.BEACH);
				if(loc.grassLand) selectedLocations.Add (TerrainTypes.MEADOW);
				if(loc.jungle) selectedLocations.Add (TerrainTypes.JUNGLE);
				if(loc.cliff) selectedLocations.Add (TerrainTypes.CLIFF);
				if(loc.mountain) selectedLocations.Add (TerrainTypes.MOUNTAIN);
				if(loc.road) selectedLocations.Add (TerrainTypes.ROAD);
				if(loc.ocean) selectedLocations.Add (TerrainTypes.OCEAN);
				
				if(selectedLocations.Count < 1) Console.print ("Error, no location was selected for object to spawn");
				tType = selectedLocations[Random.Range(0,selectedLocations.Count)];
				selectedLocations = new List<int> ();
				
				index = Random.Range(0,spawnPositions[tType].Count);
				posToSpawn = spawnPositions[tType][index];
				Quaternion spawnRotation = spawnableObject.lodObject.orignalLods[0].transform.rotation; spawnRotation.eulerAngles = new Vector3(spawnRotation.eulerAngles.x,Random.Range(0,360f),spawnRotation.eulerAngles.z);  
				float scale = Random.Range(spawnableObject.minScale,spawnableObject.maxScale);
				lodSystem.addObject(spawnableObject.lodObject.Instantiate(posToSpawn,spawnRotation,scale));
			}
			yield return null;
		}
		loadingScreen.loadingDone = true;
	}
}
