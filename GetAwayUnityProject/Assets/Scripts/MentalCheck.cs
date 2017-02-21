using UnityEngine;
using System.Collections;

public class MentalCheck : MonoBehaviour {
	public AudioSource voicesHead;
	public float hallucinationCheckInterval; //How often to check for hallucinations after sanity is low enough
	private float hallucinationCheckTimer;
	public float hallucinationChance;
	public GameObject hallucination;
	private HealthController healthControl;
	private bool soundPlaying;

	// Use this for initialization
	void Start () {
		hallucinationCheckTimer = hallucinationCheckInterval;
		healthControl = GameObject.Find("PlayerHealth").GetComponent<HealthController>();
		soundPlaying=false;
	}
	
	// Update is called once per frame
	void Update () {
		if ((healthControl.GetMental() <= 30) && (!soundPlaying)) {
			voicesHead.Play();
			soundPlaying = true;
		}
		if (healthControl.GetMental() > 30 && soundPlaying) {
			voicesHead.Stop();
			soundPlaying = false;
		}
		if ((healthControl.GetMental () <= 10)) {
			hallucinationCheckTimer -= Time.deltaTime;
			if(hallucinationCheckTimer <= 0){
				hallucinationCheckTimer = hallucinationCheckInterval;
				if(Random.Range(0f,1f) < hallucinationChance){
					Instantiate(hallucination,transform.position,transform.rotation);
				}
			}
		}
		//Debug.Log(healthControl.GetMental());
	}
}
