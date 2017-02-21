using UnityEngine;
using System.Collections;

public class ItemPickUp : MonoBehaviour {
	private RaycastHit hit = new RaycastHit();
	private Ray ray = new Ray();
	public Inventory playerInventory;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2));
		if ((Physics.Raycast(ray.origin, ray.direction, out hit, 3.0f))) {
			print ("yo");
			if (Input.GetButtonDown("PickUp")) {
				GameObject hitGameObject = hit.collider.gameObject;
				if (hitGameObject.CompareTag("PickUp")) {
					if (playerInventory == null) {
						playerInventory = Inventory.GetInstance();
					}

					ItemInfo info = hitGameObject.GetComponent<ItemInfo>();
					playerInventory.AddItem (info.itemId, info.itemQuantity);
					Destroy(hitGameObject);

				// This should perhaps be made more generic in the future.
				} else if (hitGameObject.CompareTag("Raft")) {
					RaftController raft = hitGameObject.GetComponent<RaftController>();
					raft.UserPressedActionButton();
				}
			}
		}
	}
}