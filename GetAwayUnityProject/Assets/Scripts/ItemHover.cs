using UnityEngine;
using System.Collections;

public class ItemHover : MonoBehaviour {
	private Color originalShade;
	private Transform cTransform;
	public Color newShade;
	// Use this for initialization
	void Start () {
		originalShade = transform.renderer.material.GetColor("_Color");
		cTransform = transform;
		newShade = Color.red;

	}
	
	// Update is called once per frame
	void Update () {
		/*RaycastHit target;
		if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Screen.width/2,Screen.height/2,0f)),out target,3)){
			if (target.transform.gameObject.GetComponent<EnemyHealth>()!=null){
				cTransform.renderer.material.SetColor("_Color",newShade);
			}
		}*/
	}

	void OnMouseEnter(){
		cTransform.renderer.material.SetColor("_Color",newShade);
	}

	void OnMouseExit(){
		cTransform.renderer.material.SetColor("_Color", originalShade);
	}
}
