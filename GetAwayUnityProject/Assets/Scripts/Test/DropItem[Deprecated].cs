using UnityEngine;
using System.Collections;

public class DropItem : MonoBehaviour {
	public GameObject droppable;
	public Transform player;
	public float radius = 2;
	public float scale=3f;
	bool flag1, flag2;
	int layer = 0;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		flag1=true; //can place item
		flag2=true; //can place item
	if (Input.GetKeyDown (KeyCode.R)){
			dropItem ();
		}
	}

	public void dropItem(){
		Ray ray = Camera.main.camera.ScreenPointToRay (new Vector3(Screen.width/2,Screen.height/2,0));
		RaycastHit hit = new RaycastHit();
		RaycastHit hit2 = new RaycastHit();

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
			}
	}
}
