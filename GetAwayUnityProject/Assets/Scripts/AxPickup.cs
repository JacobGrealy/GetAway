using UnityEngine;
using System.Collections;

public class AxPickup : MonoBehaviour {
	private GameObject ax;
	private GameObject axPickup;
	private RaycastHit hit = new RaycastHit();
	private Ray ray = new Ray();

	// Use this for initialization
	void Start () {
		ax = GameObject.Find("Ax");
		axPickup = GameObject.Find("AxPickup");
		ax.SetActive(false);
	}

	// Update is called once per frame
	void Update () {
		ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2));
		if ((Physics.Raycast(ray.origin, ray.direction, out hit, 3.0f)) && hit.collider.gameObject.CompareTag("PickUp")) {
			if (Input.GetButtonDown("PickUp")) {
				axPickup.SetActive(false);
				ax.SetActive(true);
				//Vector3 axPosition = new Vector3(0.7617492f, 0.5572257f, 1.018097f);
				//Vector3 axRotation = new Vector3(33.38638f, 317.9695f, 338.1754f);
				//ax.gameObject.transform.position = axPosition;
			}
		}
	}
}
