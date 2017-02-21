using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sector
{
	private IList<LODObject> lodObjects = new List<LODObject>();

	public Sector(Vector3 pos, int x, int y)
	{

	}

	public void add(LODObject lodObject)
	{
		lodObjects.Add (lodObject);
	}

	public void update(float lod1, float lod2, float lod3, Vector3 pos)
	{
		IList<LODObject> temp = new List<LODObject>();

		foreach (LODObject lodObject in lodObjects)
		{
			//lod1
			if(calculateHorizontalDistance(pos,lodObject.getPos()) < lod1)
				lodObject.ApplyLod(1);

			//lod2
			else if(calculateHorizontalDistance(pos,lodObject.getPos()) < lod2)
				lodObject.ApplyLod(2);

			//lod3
			else if(calculateHorizontalDistance(pos,lodObject.getPos()) < lod3)
				lodObject.ApplyLod(3);

			else
				lodObject.ApplyLod(4);

			//if it not to be deleted then add it to the buffer
			if(!lodObject.ShouldDelete())
			{
				temp.Add(lodObject);
			}
		}

		//swap old list with new list minus deleted objects
		lodObjects = temp;
	}

	public static float calculateHorizontalDistance(Vector3 a,Vector3 b)
	{
		a = new Vector3 (a.x, 0, a.z);
		b = new Vector3 (b.x, 0, b.z);
		return Vector3.Distance(a,b);
	}
}
