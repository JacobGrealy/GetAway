using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class CutSceneController : MonoBehaviour
{
	public bool skipIntro = false;
	public GameObject game;
	public Terrain terrain;
	public GameObject plane;
	public GameObject propL;
	public GameObject propR;
	public GameObject cameraFade;
	public Camera camera;
	public GameObject logoStuff;
	public GameObject player;
	public PlayerController playerController;
	public GameObject crashMarker;
	public AudioSource steadyFlying;
	public AudioSource crashSound;
	public AudioSource timeSound;
	public GameObject timeParticle;
	public GameObject sinkingPlane;

	private float speed = 50f;
	private float propSpeed = 750f;
	private float turbulanceFreq = .05f;
	private float turbulanceAmp = 5f;
	private float altitude = 60f;
	private int counter = 0;
	private int state = 1;
	private float state2Time = 0;
	private float state4Time = 0;
	private float state5Time = 0;
	private bool state2First = true;
	private bool state4First = true;
	private bool state5First = true;
	
	//Controller
	private bool vibrating;
	private float vibrateTime;

	// Use this for initialization
	void Start ()
	{
		steadyFlying.loop = true;
		steadyFlying.Play();
		cameraFade.SendMessage ("fadeIn");
		//make the player a child of the plane
		player.transform.parent = plane.transform;
		//make the player start looking at the island
		//player.transform.LookAt (terrain.transform.position);

	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetButtonDown ("Pause")){
			skipIntro=true;
		}
		//rotate props
		if(!skipIntro &&(state == 1 || state ==2 || state ==3))
		{
			propL.transform.Rotate (propSpeed * Time.deltaTime, 0, 0);
			propR.transform.Rotate (propSpeed * Time.deltaTime, 0, 0);

			//fly plane toward island
			plane.transform.localPosition += plane.transform.right * Time.deltaTime * speed;
			//apply turbulance and correct for terrain height
			Vector3 pos = plane.transform.position;
			if(state == 1)
				plane.transform.position = new Vector3 (pos.x,altitude + terrain.SampleHeight(pos) + (Mathf.Sin (counter*turbulanceFreq)*turbulanceAmp), pos.z);
			//
			if (pos.x > crashMarker.transform.position.x) 
			{
				state = 2;
			}

			if (!vibrating) {
				GamePad.SetVibration(0, 0.08f, 0.08f);
				vibrating = true;
			}
		}
		counter++;

		//timeWarp
		if (!skipIntro && state == 2) 
		{
			if(state2Time == 0)
				timeSound.Play ();
			timeParticle.SetActive(true);
			state2Time+= Time.deltaTime;
			if (state2Time > 3f)
				state = 3;

			GamePad.SetVibration(0, 0, 0);
		}

		//crashing
		if (!skipIntro && state == 3)
		{
			vibrating = true;
			timeParticle.SetActive(false);
			if(plane.transform.rotation.eulerAngles.z > 190f || plane.transform.rotation.eulerAngles.z ==0)
				plane.transform.Rotate(0, 0, -10*Time.deltaTime);

			//first step of state 2
			if(plane.transform.rotation.eulerAngles.z!=0 &&plane.transform.rotation.eulerAngles.z < 350f && state2First)
			{
				cameraFade.SendMessage ("fadeOut");
				steadyFlying.Stop ();
				crashSound.Play();
				state2First = false;
				state = 4;
			}
		}

		//logo
		if (!skipIntro && state == 4) 
		{
			if(plane.transform.rotation.eulerAngles.z > 190f || plane.transform.rotation.eulerAngles.z ==0)
				plane.transform.Rotate(0, 0, -10*Time.deltaTime);
			if(state4First && state4Time > 2f)
			{
				//camera.enabled = false;
				logoStuff.SetActive(true);
				cameraFade.SendMessage ("fadeIn");
				state4First = false;
			}
			if(state4Time > 6f)
			{
				state = 5;
				cameraFade.SendMessage ("fadeOut");
			}
			state4Time += Time.deltaTime;
		}

		//fade in and spawn player and sinking plane
		if (state == 5 || skipIntro)
		{
			if(state5First && state5Time > 2f)
			{
				steadyFlying.Stop ();
				//camera.enabled = true;
				logoStuff.SetActive(false);
				state5First = false;
				Vector3 playerStartPosition = crashMarker.transform.position;
				playerStartPosition = new Vector3 (playerStartPosition.x,terrain.SampleHeight(playerStartPosition) + 5f, playerStartPosition.z);
				player.transform.parent = game.transform;
				GameObject.Destroy(plane);
				player.transform.position = playerStartPosition;
				playerController.cutscenePlaying = false;
				player.transform.up = Vector3.up;
				GameObject.Instantiate(sinkingPlane,playerStartPosition + Vector3.right * 10f, sinkingPlane.transform.rotation);
			}
			if(state5Time > 3f)
			{
				cameraFade.SendMessage ("fadeIn");
				GameObject.Destroy(this);
			}
			state5Time += Time.deltaTime;
		}

		//Controller
		if (vibrating) {
			vibrateTime += 0.1f;
			if (vibrateTime < 30) {
				GamePad.SetVibration(0, 0.9f, 0.9f);
			}
			else {
				GamePad.SetVibration(0, 0, 0);
			}
		}
	}
}
