using UnityEngine;
using System.Collections;

public class TerrainGenerator : MonoBehaviour 
{
	public TreeSpawner treeSpawner;
	public Terrain terrain;
	public int randomSeed;
	public bool shouldGenerate = true;
	
	private float mediumMaxHeight = .3f;
	private float baseHeight = .01f;
	private float beachHeight = 25f;
	private int randomShift;
	
	private const int GRASS = 0, SAND = 2, CLIFF = 4, ROCKY = 6, JUNGLE = 8, ROAD = 10; 

	// Use this for initialization
	public void Start () 
	{
		StartCoroutine(GenerateCoroutine());
	}

	public IEnumerator GenerateCoroutine()
	{
		if (!shouldGenerate) {treeSpawner.SpawnCoroutine(); yield break;}

		//if the seed value isn't set then assign it the value of the current time (pseudo random)
		if(randomSeed == null || randomSeed == 0)
			Random.seed = System.DateTime.Now.Millisecond;
		else
			Random.seed = randomSeed;
		print("Random Seed = " + Random.seed);
		TerrainData tData = terrain.terrainData;
		float[, ,] splatmapData = new float[tData.alphamapWidth, tData.alphamapHeight, tData.alphamapLayers];
		randomShift = (int)(Random.value * 10000f);
		int width = tData.heightmapWidth;
		int length =  tData.heightmapHeight;
		float[,] heightMap = new float[width,length];
		int islandSize = (int)(width/2 * .66f);
		float maxHeight = 0f;
		//Generate Heightmap
		Console.print("Generating Terrain Heightmap...");
		for(int x = 0; x < tData.heightmapWidth; x++)
		{
			for(int y = 0; y < tData.heightmapHeight; y++)
			{
				//zero the height valu
				heightMap[x,y] = 0;
				
				//dome shape the island
				heightMap[x,y] += baseHeight*(Mathf.Pow((1 - (Mathf.Pow(x-width/2f,2)/Mathf.Pow(islandSize,2f) + Mathf.Pow (y-length/2f,2)/Mathf.Pow(islandSize,2))),.7f)); //parobola island shape
				
				//fractal
				float h = 1;
				int hillLayers = 20;
				if(heightMap[x,y]>0)
				{
					for(float i = 1f; i < hillLayers; i++)
					{
						//how large features are (width/depth)
						h = ((SimplexNoise.Noise.Generate((x+randomShift*i)/(450f/Mathf.Pow(i,1f)),(y+randomShift*i)/(450f/Mathf.Pow(i,1f)))+1f)/2f)* mediumMaxHeight;
						//how spread out features are
						h -= .6f*(1f/Mathf.Pow(i,.01f)) * mediumMaxHeight; if(h<0) h = 0;
						//how tall features are
						heightMap[x,y] += h/((Mathf.Pow(i,2f)));
					}
				}
				
				//make every point above sea level
				heightMap[x,y] += baseHeight;
				
				//dome shape the island
				heightMap[x,y] *= Mathf.Pow((1 - (Mathf.Pow(x-width/2f,2)/Mathf.Pow(islandSize,2f) + Mathf.Pow (y-length/2f,2)/Mathf.Pow(islandSize,2))),.7f); //parobola island shape
				//set the max height if its higher
				
				if(maxHeight < heightMap[x,y]) maxHeight = heightMap[x,y];
			}
			yield return null;
		}
		
	Console.print("Normalizing Height Map...");
		for(int x = 0; x < tData.heightmapWidth; x++)
		{
			for(int y = 0; y < tData.heightmapHeight; y++)
			{
				//normalize the heightmap
				heightMap[x,y] *= 1f/(maxHeight);
			}
		}
		
		tData.SetHeights(0, 0, heightMap);
		
		//Generate Splat Map
		Console.print("Generating Terrain Splatmap...");
		width = tData.alphamapWidth;
		length =  tData.alphamapHeight;
		for (int y = 0; y < length; y++)
		{
			for (int x = 0; x < width; x++)
			{
				// now assign the values to the correct location in the array
				//oceqan floor
				if(getTerrainType(x,y,width,length) == TerrainTypes.OCEAN){splatmapData[y, x, CLIFF] = .25f; splatmapData[y, x, CLIFF+1] = .25f; splatmapData[y, x, SAND] = .25f; splatmapData[y, x, SAND+1] = .25f;} 
				//cliff
				else if(getTerrainType(x,y,width,length) == TerrainTypes.CLIFF){ splatmapData[y, x, CLIFF] = .5f; splatmapData[y, x, CLIFF+1] = .5f;}
				//beach
				else if(getTerrainType(x,y,width,length) == TerrainTypes.BEACH){ splatmapData[y, x, SAND] = .65f; splatmapData[y, x, SAND+1] = .35f;}
				//Is road
				else if(getTerrainType(x,y,width,length) == TerrainTypes.ROAD){splatmapData[y, x, ROAD] = .65f; splatmapData[y, x, ROAD+1] = .35f;}
				//hill to cliff transition
				else if(getTerrainType(x,y,width,length) == TerrainTypes.DIRT){ splatmapData[y, x, ROCKY] = .65f; splatmapData[y, x, ROCKY+1] = .35f;}
				//Is jungle
				else if(getTerrainType(x,y,width,length) == TerrainTypes.JUNGLE){splatmapData[y, x, JUNGLE] = .65f; splatmapData[y, x, JUNGLE+1] = .35f;}
				//is grass
				else if(getTerrainType(x,y,width,length) == TerrainTypes.MEADOW){splatmapData[y, x, GRASS] = .65f; splatmapData[y, x, GRASS+1] = .35f;}
				//non cliff part of mountain
				else if(getTerrainType(x,y,width,length) == TerrainTypes.MOUNTAIN){ splatmapData[y, x, GRASS] = .65f; splatmapData[y, x, GRASS+1] = .35f;}
			}
			yield return null;
		}
		// and finally assign the new splatmap to the terrainData:
		tData.SetAlphamaps(0, 0, splatmapData);

		//start spawning objects
		StartCoroutine(treeSpawner.SpawnCoroutine());
	}

	public int getTerrainType(int x, int y, int width, int length)
	{
		float xPrec = 1.0f * x / width;
		float yPrec = 1.0f * y / length;
		TerrainData tData = terrain.terrainData;
		// read the height and slope at this location
		float height = tData.GetInterpolatedHeight(xPrec,yPrec);
		float slope = tData.GetSteepness(xPrec,yPrec);			
		x = (int)(xPrec * 2048f);
		y = (int)(yPrec * 2048f);
		float roadNoise = ((SimplexNoise.Noise.Generate((x+randomShift+555)/(350f),(y+randomShift+555)/(350f))+1f)/2f);
		float jungleNoise = ((SimplexNoise.Noise.Generate((x+randomShift)/(350f),(y+randomShift)/(350f))+1f)/2f);
		
		//oceqan floor
		if (height == 0) {return TerrainTypes.OCEAN;} 
		//cliff
		else if (slope >= 40f) {return TerrainTypes.CLIFF;}
		//beach
		else if(height < beachHeight){return TerrainTypes.BEACH;}
		//Is road
		else if(roadNoise > .48f && roadNoise <.52f){return TerrainTypes.ROAD;}
		//hill to cliff transition
		else if(slope >= 35f && slope < 40f){return TerrainTypes.DIRT;}
		//jungle and grassland
		else if(height >= beachHeight && height < 800f)
		{
			//Is jungle
			if(jungleNoise < .62f){return TerrainTypes.JUNGLE;}
			//is grass
			else {return TerrainTypes.MEADOW;}
		}			 
		//non cliff part of mountain
		else if(height >= 800f){return TerrainTypes.MOUNTAIN;}
		//error
		else return TerrainTypes.MOUNTAIN;
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}

public static class TerrainTypes
{
	public static int MOUNTAIN = 0;
	public static int OCEAN = 1;
	public static int CLIFF = 2;
	public static int BEACH = 3;
	public static int ROAD = 4;
	public static int DIRT = 5;
	public static int JUNGLE = 6;
	public static int MEADOW = 7;
}
