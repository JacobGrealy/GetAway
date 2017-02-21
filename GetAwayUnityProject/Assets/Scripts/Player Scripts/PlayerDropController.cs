using UnityEngine;
using System.Collections;

public class PlayerDropController : MonoBehaviour {

	// The player to drop the item.
	public Transform player;

	// The radius of the spherecast for dropability.
	public float radius = 2;

	// The scale of the spherecast for dropability.
	public float scale=3f;
	private int layer = 0;	
	public bool DropItem(GameObject droppable){
		bool canDrop=false;
		Ray ray = Camera.main.camera.ScreenPointToRay (new Vector3(Screen.width/2,Screen.height/2,0));
		RaycastHit hit = new RaycastHit();
		RaycastHit hit2 = new RaycastHit();

		bool flag1 = true;
		bool flag2 = true;
		if ((Physics.SphereCast (player.position+new Vector3(0,2,0),radius,player.forward,out hit,scale,1<<layer))){
			//raycast hit
			if (!(hit.transform.name=="Terrain")){
				//not terrain
				flag1=false;
			}
		}
		if ((Physics.Raycast(ray.origin, ray.direction, out hit2, scale,1<<layer))){
			//raycast hit
			if (!(hit2.transform.name=="Terrain")){
				//not terrain
				flag2=false;
			}
		}
		if (flag1 && flag2){ //if item won't be obstructed
			Instantiate (droppable, player.position+player.forward*scale, player.rotation);
			canDrop=true;
		}
		return canDrop;
	}
}
