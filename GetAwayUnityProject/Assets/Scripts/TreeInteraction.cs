using UnityEngine;
using System.Collections;

public class TreeInteraction : MonoBehaviour {
	public AudioSource treeHit;
	public PlayerEquip equipped;
	private Ray ray = new Ray();
	private RaycastHit hit = new RaycastHit();
	private TreeReactions tree;
	private GameObject treeFallingSound;
	private GameObject woodChips;
	private bool woodChipsActive = false;
	private float timeDelay = 0.0f;
	public Camera mainCamera;

	// Use this for initialization
	void Start () {
		woodChips = GameObject.Find("WoodChips");
		woodChips.SetActive(false);
		treeFallingSound = GameObject.Find("Tree Fall Sound");
		treeFallingSound.SetActive(false);
		tree = new TreeReactions();
	}
	
	// Update is called once per frame
	void Update () {
		ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2));
		//Will also check if Ax is actually equipped
		//Debug.Log ("AxOut"+equipped.AxOut());
		//Debug.Log ("Attacking"+equipped.isAttacking());
		//Debug.Log ("GetButtonDown"+Input.GetButtonDown("Fire1"));
	//	Debug.Log ("GAA"+((Input.GetButtonDown("Fire1") || Input.GetAxisRaw("RightTrigger") == -1) && equipped.AxOut () && !equipped.isAttacking()));
		if ((Input.GetButtonDown("Fire1") || Input.GetAxisRaw("RightTrigger") == -1) && equipped.AxOut () && equipped.CanHitTree()) {
			equipped.SetCanHitTree (false);
			if (Physics.Raycast(ray.origin, ray.direction, out hit, 1.0f)){
				if (hit.transform.GetComponent<TreeReactions>()!=null){
					tree = GameObject.Find(hit.collider.gameObject.name).GetComponent<TreeReactions>();
					tree.TreeHit();
					if (tree.TreeHitCount() > 0) {
						treeHit.PlayDelayed(0.25f);
						tree.SetSticks(hit.collider.gameObject.transform.position);
						SetWoodChipsPosition(hit.collider.gameObject.transform.position);
						SetWoodChipsActive();
						treeFallingSound.SetActive(false);
					}
					else {
						SetFallingSoundPosition(hit.collider.gameObject.transform.position);
						treeFallingSound.SetActive(true);
						treeFallingSound.audio.Play();
					}	
				}
			}
		}

		if (woodChipsActive) {
			timeDelay += 0.1f;
			if (timeDelay > 4.0f) {
				woodChips.SetActive(false);
				woodChipsActive = false;
				timeDelay = 0.0f;
			}
		}
	}

	public void SetWoodChipsPosition(Vector3 position) {
		woodChips.transform.position = position;
	}
	public void SetWoodChipsActive() {
		woodChipsActive = true;
		woodChips.SetActive(true);
	}

	public void SetFallingSoundPosition(Vector3 position) {
		treeFallingSound.transform.position = position;
	}
}
