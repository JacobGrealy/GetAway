using UnityEngine;
using System.Collections;

[System.Serializable]
public class LODObject
{
	private int lodNum; // as a zero indexed array value, -1 from expected value
	private Vector3 position;
	private Quaternion rotation;
	private float scale;
	private GameObject[] lods = new GameObject[4]; //holds the current clones of the lods
	private bool shouldDelete = false;
	
	public GameObject[] orignalLods = new GameObject[4]; //1: shrubs, clutter, full trees, //2: 3d trees with no interactions, //3: tree billboards
	public bool persistent; //false: lods are destroyed and created when in and out of lod distance. True: Lods are enabled and disabled based on lod distance
	
	public LODObject Instantiate(Vector3 position, Quaternion rotation, float scale) //call this before using LODObject
	{
		LODObject temp = new LODObject ();
		temp.orignalLods = this.orignalLods;
		temp.persistent = this.persistent;
		temp.SetAttributes (position, rotation,scale);
		return temp;
	}

	public void SetAttributes(Vector3 position, Quaternion rotation, float scale)
	{
		this.position = position;
		this.rotation = rotation;
		this.scale = scale;
		this.lodNum = 3;
		//if there is a gameobject for the starting lod, create it
		if (orignalLods [lodNum] != null)
		{
			lods [lodNum] = (MonoBehaviour.Instantiate (orignalLods [lodNum], this.position, this.rotation)) as GameObject;
			lods[lodNum].transform.localScale *= scale;
		}
	}
	
	public void ApplyLod(int lodNum)
	{
		//change to zero index array value
		lodNum = lodNum - 1; 
		
		//if it is already in that lod do nothing
		if (lodNum == this.lodNum) 
			return;
		
		//check to see if the object was deleted
		if (lods [this.lodNum] == null && orignalLods [this.lodNum] != null)
		{
			this.shouldDelete = true;
			return;
		}
		
		//check to make sure there is an lod in the original lod array for this lod
		if (orignalLods[lodNum] != null)
		{
			//keep position and rotations through lod switch if the prev lod wasn't null
			if(lods [this.lodNum] != null)
			{
				this.position = lods [this.lodNum].transform.position;
				this.rotation = lods [this.lodNum].transform.rotation;
			}
			
			if (lods [lodNum] == null) //if this lod doesnt exist instantiante a copy of it
			{
				lods [lodNum] = (MonoBehaviour.Instantiate (orignalLods [lodNum], this.position, this.rotation)) as GameObject;
				lods[lodNum].transform.localScale *= scale;
			}
			else //otherwise it is disabled so enable it
			{
				lods [lodNum].SetActive (true);
				lods [lodNum].transform.position = position;
				lods [lodNum].transform.rotation = rotation;
			}
		}
		//check if the old lod was null
		if(lods [this.lodNum] != null)
		{
			//if we want the object to be persistent just disable the last lod
			if (persistent) 
				lods [this.lodNum].SetActive (false);
			else 
				MonoBehaviour.Destroy(lods [this.lodNum]);
		}
		//finnally, switch the lodnum
		this.lodNum = lodNum;
	}
	
	public int getLodNum()
	{
		return this.lodNum + 1;
	}
	
	public Vector3 getPos()
	{
		return this.position;
	}
	
	public bool ShouldDelete()
	{
		return shouldDelete;
	}
}