using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LodSystem : MonoBehaviour 
{
	public GameObject player;
	public int numOfSectors; //the amount of sectors in a column or row
	public int framesToUpdate; //how many frames inbetween updates
	public int numAdjacentSectorsToUpdate; //how many adjacent sectors to the player are updated
	public float lodDistance1;
	public float lodDistance2;
	public float lodDistance3;

	private Vector3 origin;
	private float width;
	private float length;
	private Sector[,] sectorArray;

	private int frames = 0;

	// Use this for initialization
	public void init(Vector3 origin, float width, float length)
	{
		//set variables
		this.origin = origin;
		this.width = width;
		this.length = length;
		this.sectorArray = new Sector[numOfSectors, numOfSectors];

		//create sectors
		for(int x =0; x < numOfSectors; x++)
		{
			for(int y =0; y < numOfSectors; y++)
			{
				float posX = origin.x + width * ((1.0f * x + 0.5f) /numOfSectors);
				float posZ = origin.z + length * ((1.0f * y + 0.5f)/numOfSectors);
				float posY = origin.y;
				Vector3 position = new Vector3(posX,posY,posZ);
				Sector sector = new Sector(position,x,y);
				sectorArray[x,y] = sector;
			}
		}
	}

	public void addObject(LODObject lodObject)
	{
		Vector3 pos = lodObject.getPos() - origin;
		int x =(int)((pos.x / width) * numOfSectors);
		int y =(int)((pos.z / length) * numOfSectors);
		//Console.print ("x: " + x + " y: " + y + " numOfSectors: " + numOfSectors);
		sectorArray[x,y].add(lodObject);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (frames >= framesToUpdate)
		{
			ApplyLOD ();
			frames = 0;
		}
		frames++;
	}

	public void ApplyLOD()
	{
		//find the sector the player is in
		Vector3 pos = player.transform.position - origin;
		int x =(int)((pos.x / width) * numOfSectors);
		int y =(int)((pos.z / length) * numOfSectors);

		//update sector and adjacent sectors
		for (int i = x-numAdjacentSectorsToUpdate; i <= x+numAdjacentSectorsToUpdate; i++)
		{
			for (int j = y-numAdjacentSectorsToUpdate; j <= y+numAdjacentSectorsToUpdate; j++)
			{
				if(!(i < 0 || i >= numOfSectors || j < 0 || j >= numOfSectors))
				{
					sectorArray[i,j].update(lodDistance1,lodDistance2,lodDistance3,player.transform.position);
				}
			}
		}
	}

	void Start ()
	{

	}
}
