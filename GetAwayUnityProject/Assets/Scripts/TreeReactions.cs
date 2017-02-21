using UnityEngine;
using System.Collections;

public class TreeReactions : MonoBehaviour {
	private int numberOfHits = 5;
	private GameObject sticks;
	private float treeDelay = 10.0f;
	public static int idCounter = 0;

	//Sounds
	private AudioSource logSound;
	private GameObject logSoundObject;

	public int coconutRate;
	public GameObject coconut;

	// Use this for initialization
	void Start () 
	{
		//give uniquiee name
		gameObject.name = gameObject.name + idCounter;
		idCounter++;

		sticks = (GameObject)Instantiate(Resources.Load("LogClump"));
		sticks.SetActive(false);

		logSoundObject = GameObject.Find("LogSound");
		logSound = logSoundObject.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		if (numberOfHits <= 0) {
			gameObject.rigidbody.AddForce(Vector3.left * 5);
			treeDelay -= Time.deltaTime;
			if (treeDelay < 0) {
				Destroy(gameObject);
				sticks.SetActive(true);
				logSound.PlayDelayed(1.5f);
			}
		}
	}

	public void TreeHit() {
		numberOfHits -= 1;
		int itemDropped = Random.Range(0,coconutRate);
		if (itemDropped==0){
			GameObject newDrop=(GameObject)Instantiate(coconut,transform.position+new Vector3(0,5,2),transform.rotation);
			newDrop.name=coconut.name;
		}
		if(numberOfHits <=0)
			gameObject.rigidbody.constraints = RigidbodyConstraints.None;
	}
	public int TreeHitCount() {
		return numberOfHits;
	}
	public void SetSticks(Vector3 position) {
		position.y += 8.0f;
		position.x -= 2.0f;
		position.z -= 2.0f;
		sticks.transform.position = position;

		logSoundObject.transform.position = position;
	}
}
