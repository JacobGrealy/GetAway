using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public bool allowPlayerAction;
	public InterfaceController interfaceControl;
	private MouseLook playerMouseLook, cameraMouseLook;
	private CharacterMotor playerMotor;
	public HealthController healthControl;
	private Console console;
	//pause
	private PauseMenu pause;
	//equipment
	public PlayerEquip playerEquipment;
	//crouching
	public float crouchScale=.25f;
	private float originalHeight;
	private CharacterController charController; 
	public bool isCrouched=true; //used for toggle crouch
	private Transform playerTransform;
	public float crouchSmooth = 5;
	//running
	private CharacterMotor motor;
	private float fWalkingSpeed;
	private float bWalkingSpeed;
	public float forwardRunningScale = 1.75f;
	public float backwardsRunningScale = 1.25f;
	private float currentCrouchScale;
	private PlayerWalkSound walkingSounds;
	private bool isRunning = false;
	//item pickup
	private RaycastHit itemHit = new RaycastHit();
	private Ray ray = new Ray();
	public Inventory playerInventory;
	public AudioSource itemPickupSound;
	//attacking
	public int damage = 1, damageScale = 1;

	// Environment cameras (i.e. not interface cameras).
	public Camera mainCamera;
	public Camera[] otherCameras;

	//crosshair
	public bool crosshairOff;
	public Texture2D crosshairImage;
	private bool displayText = false;
	private RaycastHit hit = new RaycastHit();
	private Ray crosshairRay = new Ray();
	private GameObject pickUpText;
	private TextMesh textMeshComponent;
	private Font arialFont;
	private Vector3 textPosition;
	private Vector3 textRotation;
	private Vector3 textScale;
	private Transform player;
	private GameObject hitObject;
	private int layer = 0;
	//head bobbing
	// drop into the main character's camera
	// editor-accessible vars
	public float bobbingFreq = 1.8f; // in Hz, TODO make it dependent on speed
	private float bobbingFreqCached;
	public float bobbingRatio = 0.08f; // as a factor of character height
	// non editor-accessible
	private float phase = Mathf.PI;
	private float height; //we'll need this for bobbing as a function of height
	private float bobbingAmount;
	private float heightDependency;
	private float currentBobbing; // keep track of y axis modification 
	private float bobbingDelta; // how much has bobbing changed in the last frame
	// Keep a mask for the displacement states where isWalking is xxx1 isStepping xx1x and isStopping x1xx
	private int stateMask = 0;
	private const int isWalking = 1; 
	// this next variable can be useful for triggering stepSounds, see getter below
	private const int isStepping = 2; 
	private const int isStopping = 4;
	// need to remember where camera was before we began toying with it. 
	// NOTE this wont work if anything else is moving the camera!
	private Vector3 OriginalCameraLocalPosition;

	//player stat values for detrimental effects
	private int lowHealth=30;
	private int lowHunger=30;
	private int lowThirst=30;
	private int lowMental=30;
	private float weakenedAttackScale=.75f;
	private float orignalMaxSpeed;
	private float weakenedWalkScale=.75f;

	//Sleeping System
	public DayNightController dayNight;
	int hoursSlept = 10;
	int mentalStateImprove = 30;
	private float lastSlept, currentTime1,currentTime2, sleepWait=20f;
	public GameObject cameraFade;
	private float fadeTime=3f;
	private float startTime=0f;
	private bool sleeping=false;
	private GameObject campfire;
	private int sleepThirst=10;
	private int sleepHunger=10;

	//scoring
	public Score scoreKeeper;

	//torch switch
	private bool torchFlag;
	private FoodChain foodChain;

	//falling damage
	private bool falling=false;
	private float fallHeight=0;
	private float landingHeight=0;
	private float safeHeight=5;
	private int fallingDamage=10;

	//health stat drainage
	//Hunger
	private float hungerTime = 0.0f;
	private float hungerDelay = 30.0f;
	//Thirst
	private float thirstTime = 0.0f;
	private float thirstDelay = 30.0f;
	//Health
	private float healthTime = 0.0f;
	private float healthDelay = 2.0f;
	//Mental
	private float mentalTime = 0.0f;
	private float mentalDelay = 30.0f;
	//daytimes
	private float sunStart = 9;
	private float sunEnd = 15;
	private float moonStart=19;
	private float moonEnd=5;

	//cutscene
	public bool cutscenePlaying=true;
	public Camera playerInterface;
	public bool cutsceneOver=false;

	//player stats
	public StatsController stats;

	
	// Use this for initialization
	void Start () {
		allowPlayerAction=true;
	//	interfaceControl = GetComponent<InterfaceController>();
		//pause
		pause = GetComponent<PauseMenu>();
		//remove mouse cursor
		Screen.showCursor = false;
		//crouching
		charController = GetComponent<CharacterController>();
		originalHeight=charController.height;
		playerTransform = transform;
		//running
		motor = GetComponent<CharacterMotor>();
		fWalkingSpeed = motor.movement.maxForwardSpeed;
		bWalkingSpeed = motor.movement.maxBackwardsSpeed;
		walkingSounds=playerTransform.GetComponent<PlayerWalkSound>();
		//crosshair
		crosshairOff = false;
		pickUpText = new GameObject("PickUpText");
		textMeshComponent = pickUpText.AddComponent<TextMesh>();
		arialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
		textRotation = new Vector3(0.0f, 180.0f, 0.0f);;
		textScale = new Vector3(0.08f, 0.08f, 0.08f);
		player = transform;
		hitObject = new GameObject();
		hitObject.AddComponent("MeshRenderer");
		hitObject.renderer.material.shader = Shader.Find("Diffuse");
		//headbobbing
		height = charController.height;
		OriginalCameraLocalPosition = mainCamera.transform.localPosition;
		heightDependency = playerTransform.localScale.y * height / 2;
		currentBobbing = 0.0f;
		bobbingFreqCached = bobbingFreq;

		playerMouseLook = playerTransform.GetComponent<MouseLook>();
		playerMotor =  playerTransform.GetComponent<CharacterMotor>();
		cameraMouseLook = mainCamera.GetComponent<MouseLook>();
		console = playerTransform.GetComponent<Console>();

		lastSlept=0f;


		//Scoring
		Score.damageDone = 0;
		Score.damageTaken = 0;
		Score.daysSurvived = 0;
		DontDestroyOnLoad(scoreKeeper);

		//food chain
		torchFlag=false;
		foodChain=playerTransform.GetComponent<FoodChain>();

	}
	
	// Update is called once per frame
	void Update () {
		//adjustAllHealth ();
		if (cutscenePlaying){
			CutsceneControls();
		}
		else if (!cutsceneOver){
			CutsceneControls ();
			cutsceneOver=true;
		}
		else {
			if (sleeping){
				UpdateFade ();
			}
			else if (allowPlayerAction && !cutscenePlaying){
				crouchControl ();
				runControl ();
				attackControl ();
				crosshairDisplay ();
				cameraBob ();
				itemPickUp();
				updateHealthStats();
				if ((!motor.IsGrounded() && !falling)||(motor.IsGrounded() && falling)){
					FallingDamage ();
				}
				if (playerEquipment.ArrowPulled()){
					playerEquipment.weaponAttack();
				}
				//Testing
			/*	if (Input.GetKeyDown (KeyCode.L)){
					playerEquipment.setEquip (EquipableItem.AX,1,20);
				}
				else if (Input.GetKeyDown (KeyCode.O)) {
					playerEquipment.setEquip (EquipableItem.SPEAR,1,20);
				}
				else if (Input.GetKeyDown (KeyCode.P)){
					playerEquipment.setEquip (EquipableItem.TORCH,1,20);
				}
				else if (Input.GetKeyDown (KeyCode.Semicolon)){
					playerEquipment.setEquip (EquipableItem.BOW,1,20);
				}*/

				if (playerEquipment.TorchOut() && !torchFlag){
					foodChain.setFoodChainLevel(FoodChain.FoodChainLevels.PlayerWithTorch);
					torchFlag=true;
				}
				else if (!playerEquipment.TorchOut() && torchFlag){
					foodChain.setFoodChainLevel(FoodChain.FoodChainLevels.Player);
					torchFlag=false;
				}
			}
			checkPlayerAction ();
		}

	}

	public void setPlayerAction(bool setting){
		allowPlayerAction=setting;
	}

	private bool isMenuOpen(){
		bool flag1 = interfaceControl.isCraftingMenuActive();
		bool flag2 = interfaceControl.isInventoryActive();
		bool flag3 = console.isConsoleActive ();
		bool flag4 = pause.IsGamePaused();
		bool flag5 = sleeping;
		return (flag1 || flag2 || flag3 || flag4 || flag5);
	}

	private void checkPlayerAction(){
		bool flag = isMenuOpen ();
		playerMouseLook.enabled=!flag;
		playerMotor.canControl=!flag;
		setPlayerAction(!flag);
		cameraMouseLook.enabled=!flag;
		//pause.enabled=!sleeping;
		playerEquipment.SetPause(pause.IsGamePaused ());
		walkingSounds.enabled=!flag;
		Screen.showCursor=flag;
		foreach (Camera camera in otherCameras) {
			camera.GetComponent<MouseLook>().enabled = !flag;
		}
	}
	
	//Physical Health
	public void IncreaseHealth (int additional) {
		healthControl.IncreaseHealth(additional);
	}
	public void SubtractHealth (int deduction) {
		healthControl.SubtractHealth(deduction);
	}
	public int GetHealth (){
		return healthControl.GetHealth ();
	}
	
	//Mental Health
	public void IncreaseMental (int additional) {
		healthControl.IncreaseMental(additional);
	}
	public void SubtractMental (int deduction) {
		healthControl.SubtractMental(deduction);
	}
	public int GetMental (){
		return healthControl.GetHealth();
	}
	
	//Hunger
	public void IncreaseHunger (int additional) {
		healthControl.IncreaseHunger(additional);
	}
	public void SubtractHunger (int deduction) {
		healthControl.SubtractHunger(deduction);
	}
	public int GetHunger (){
		return healthControl.GetHunger();
	}
	
	//Thirst
	public void IncreaseThirst (int additional) {
		healthControl.IncreaseThirst(additional);
	}
	public void SubtractThirst(int deduction) {
		healthControl.SubtractThirst(deduction);
	}
	public int GetThirst (){
		return healthControl.GetThirst();
	}

	//CROUCHING
	public void crouchControl(){
		float tmpHeight=originalHeight;
		if (Input.GetButton("Crouch")){
			//if crouched
			tmpHeight=originalHeight*crouchScale;
			isCrouched=true;
		}
		else {
			isCrouched=false;
		}
		float previousHeight = charController.height;
		charController.height = Mathf.Lerp (charController.height,tmpHeight,crouchSmooth*Time.deltaTime);
		playerTransform.position = new Vector3(playerTransform.position.x,playerTransform.position.y+(charController.height-previousHeight)/2,playerTransform.position.z);
	}

	public bool isCrouching(){
		return isCrouched;
	}

	public void runControl(){
		if (isCrouching ()){
			currentCrouchScale = crouchScale;
		}
		else{
			currentCrouchScale = 1f;
		}
		if ((Input.GetButton("Run") || (isRunning && (Input.GetAxis("Joystick X") > 0 || Input.GetAxis("Vertical") > 0))) && motor.IsGrounded()){ //if on ground and running
			isRunning = true;
			motor.movement.maxForwardSpeed=fWalkingSpeed*forwardRunningScale*currentCrouchScale; //scale forward speed by run scale
			motor.movement.maxBackwardsSpeed=bWalkingSpeed*backwardsRunningScale*currentCrouchScale; //scale backward speed by run scale
		}
		else if (motor.IsGrounded()){ //if on ground, but not running
			isRunning = false;
			motor.movement.maxForwardSpeed=fWalkingSpeed*currentCrouchScale; //return to normal forward speed
			motor.movement.maxBackwardsSpeed=bWalkingSpeed*currentCrouchScale; //return to normal backwards speed
		}
		if ((GetHealth()<lowHealth || GetThirst()<lowThirst) && motor.IsGrounded()){ //if weakened
			motor.movement.maxForwardSpeed=motor.movement.maxForwardSpeed*weakenedWalkScale;
			motor.movement.maxBackwardsSpeed=motor.movement.maxBackwardsSpeed*weakenedWalkScale;
		}
		//else maintain current speed

	}

	public void itemPickUp(){
		if (Input.GetButtonDown("PickUp")) {
		ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2,0));
		if ((Physics.Raycast(ray, out itemHit, 3.0f,1<<layer)) && itemHit.transform.gameObject.CompareTag("PickUp")) {
				itemPickupSound.Play();
				//Debug.Log ("Pickup");
				playerInventory.AddItem (itemHit.transform.gameObject.GetComponent<ItemInfo>().getItemId(),itemHit.collider.gameObject.GetComponent<ItemInfo>().getItemQuantity());
				Destroy(itemHit.collider.gameObject);
				//destroy item from world
			}
			else if ((Physics.Raycast(ray, out itemHit, 3.0f,1<<layer)) && itemHit.transform.gameObject.CompareTag("Raft")) {
				Win ();
			}
			else if ((Physics.Raycast(ray, out itemHit, 3.0f,1<<layer)) && itemHit.transform.gameObject.GetComponent<WaitBeforeBurn>()!=null){
				Sleep ();
				campfire=itemHit.transform.gameObject;
			}
		}
	}

	public void attackControl(){
		RaycastHit target;
		Ray ray = mainCamera.ScreenPointToRay (new Vector3(Screen.width/2,Screen.height/2,0));
		if ((Input.GetButtonDown ("Fire1") || Input.GetAxisRaw("RightTrigger") == -1) && playerEquipment.CanHitEnemy ()){
			playerEquipment.weaponAttack();
			playerEquipment.SetCanHitEnemy (false);
			if (Physics.Raycast(ray,out target,playerEquipment.getWeaponRange ()) && !playerEquipment.BowOut()){
			//	Debug.Log (target.transform.name);

				 if (target.transform.gameObject.GetComponent<EnemyHealth>()!=null){
					Debug.Log ("HIT");
					damageScale = playerEquipment.getDamageScale ();
					if (GetHunger ()<lowHunger || GetThirst ()<lowThirst){
						damageScale=(int)((float)damageScale*weakenedAttackScale);
					}
					target.transform.GetComponent<EnemyHealth>().decrementHealth(damage*damageScale, gameObject);
					Score.damageDone+=(damage*damageScale);
				}
			}
		}
	//	Debug.DrawRay(ray.origin, ray.direction * 3, Color.yellow);
		//Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
		//Debug.Log ("V1: "+transform.position+ "V2: "+transform.forward);
	}
	
	private void crosshairDisplay(){
		crosshairRay = mainCamera.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2,0));
		//Debug.DrawRay (crosshairRay.origin,crosshairRay.direction*5);
		if ((Physics.Raycast(crosshairRay, out hit, 3.0f,1<<layer)) && hit.collider.gameObject.CompareTag("PickUp")) {
			displayText = true;
		}
		else {
			displayText = false;

		}
	//	Debug.Log (hit.transform.gameObject.name);
		//pickUpText.transform.rotation = Quaternion.LookRotation (pickUpText.transform.position - player.position);
	}
	
	void OnGUI () {
		float xMin = (Screen.width / 2) - (crosshairImage.width / 2);
		float yMin = (Screen.height / 2) - (crosshairImage.height / 2);
		//Follow Mouse
		//float xMin = Screen.width - (Screen.width - Input.mousePosition.x) - (crosshairImage.width / 2);
		//float yMin = (Screen.height - Input.mousePosition.y) - (crosshairImage.height / 2);
		if (allowPlayerAction){
			if (!crosshairOff){
				GUI.DrawTexture(new Rect(xMin, yMin, crosshairImage.width, crosshairImage.height), crosshairImage);
				if (displayText) {
					//int width = 180;
					//string pickUpText = "Press e to Pick Up " + hit.collider.gameObject.name;
					//GUI.Box(new Rect((Screen.width/2 - width/2), (Screen.height/2 - 100), width, 25), pickUpText);
			
					/*textPosition = new Vector3(hit.collider.gameObject.transform.position.x + 0.75f, hit.collider.gameObject.transform.position.y + 0.6f, hit.collider.gameObject.transform.position.z);
					textMeshComponent.text = "Press e to Pick Up " + hit.collider.gameObject.name;
					pickUpText.renderer.material = arialFont.material;
					pickUpText.transform.position = textPosition;
					pickUpText.transform.rotation = Quaternion.Euler(textRotation);
					//pickUpText.transform.LookAt(Camera.main.transform);
					pickUpText.transform.localScale = textScale;
					//pickUpText.SetActive(true);
*/
					hitObject = hit.transform.gameObject;
					hit.transform.gameObject.renderer.material.shader = Shader.Find("Outlined Diffuse");
				}
			}
			if (!displayText) {
				pickUpText.SetActive(false);
				if (hitObject!=null){
				hitObject.renderer.material.shader = Shader.Find("Diffuse");
				}
			}
		}
	}

	private void cameraBob(){
		bobbingAmount = bobbingRatio * heightDependency;
		if ( (motor.inputMoveDirection != Vector3.zero) && motor.IsGrounded() ) { 
			UpdatePhaseAndBobbingDelta();
			stateMask = stateMask | isWalking; // isWalking = true;
			stateMask = stateMask & ~isStopping; // isStopping = false;
		} 
		else {
			// bobbingDelta = 0.0f; // see note in our first branch
			if( (stateMask & isWalking) > 0 ){ // if we got here, we're stopping
				stateMask = stateMask | isStopping; // isStopping = true
				UpdatePhaseAndBobbingDelta( (phase > 0) ? 1 : -1 ); // if our phase is less than 0, rewind it
				if ( (stateMask & isStepping) > 0 ) {// we're back down
					mainCamera.transform.localPosition = OriginalCameraLocalPosition; // avoid cumulative errors
					stateMask = stateMask & ~isWalking;
					phase = Mathf.PI; // ready to start again
				}
			}
			else {
				bobbingDelta = 0.0f;
				stateMask = 0; // we're not doing anything
			}
		} 
		mainCamera.transform.Translate(Vector3.up * bobbingDelta, Space.World);    
	}

	// by default we add phase but it might be shorter to rewind it when stopping 
	private void  UpdatePhaseAndBobbingDelta (){
		UpdatePhaseAndBobbingDelta( 1.0f );
	}
	
	private void  UpdatePhaseAndBobbingDelta ( float direction  ){
		
		float twoPi = Mathf.PI * 2;
		// bobbing happens here, add one to Sin to avoid going under the terrain
		float previousBobbing = currentBobbing;
		currentBobbing = (Mathf.Cos(phase) + 1.0f) * bobbingAmount;
		bobbingDelta = currentBobbing - previousBobbing;
		// update phase in a frame-independent fashion
		phase = phase + (direction * (twoPi * Time.deltaTime * bobbingFreq));
		// wrap phase
		if (Mathf.Abs(phase) > Mathf.PI) { 
			phase = phase - (direction * twoPi); 
			stateMask = stateMask | isStepping;
		} 
		else
			stateMask = stateMask & ~isStepping;
	}
	
	public bool getIsStepping(){
		return ( (isStepping & stateMask) > 0 );
	}  

	public void Sleep(){ //sleep for set amount of hours
		cameraFade.SendMessage ("fadeOut");
		sleeping=true;
		startTime=0;
		dayNight.SetTime ((dayNight.GetTime()+hoursSlept));
		healthControl.IncreaseMental (mentalStateImprove);
		healthControl.SubtractHunger(sleepHunger);
		healthControl.SubtractThirst (sleepThirst);
	}

	private void UpdateFade(){
		startTime+=Time.deltaTime;
		if (startTime>fadeTime){
			sleeping=false;
			cameraFade.SendMessage ("fadeIn");
			DeactivateCampfire();
		}
	}

	private void DeactivateCampfire(){
		Destroy (campfire.transform.gameObject.GetComponent<WaitBeforeBurn>());
		campfire.transform.audio.mute=true;
		foreach (Transform child in campfire.transform){
			if (child.name=="fire2"){
				Destroy (child.transform.gameObject);
			}
		}
	}
	
	public void Win(){ //raft is clicked
		Application.LoadLevel ("Win");
	}

	public void FallingDamage(){
		if (!motor.IsGrounded() && !falling){ //if player leaves the ground
			fallHeight=playerTransform.position.y;
			falling = true;
		}
		else if (motor.IsGrounded() && falling){ //if player lands
			landingHeight=playerTransform.position.y;
			falling=false;
			if ((fallHeight-landingHeight)>safeHeight){ //player fell more than a safe distance
				SubtractHealth (fallingDamage*(int)((fallHeight-landingHeight-safeHeight)/3));
			}
		}

	 }

	private void updateHealthStats(){
		//Will also have a check for time since last thirst quenching
		if (thirstTime > thirstDelay) {
			SubtractThirst(1);
			thirstTime = 0.0f;
		}

		//Also, could add if haven't eaten anything for a certain amount of time
		if (hungerTime > hungerDelay) {
			SubtractHunger(1);
			hungerTime = 0.0f;
		}
		
		/*if (thirst == 0 && hunger < 25 && healthTime > healthDelay && physicalHealth > 0) {
			SubtractHealth(1);
			healthTime = 0.0f;
		}
		healthTime += Time.deltaTime;*/
		
		if (mentalTime > mentalDelay) {
			SubtractMental(1);
			mentalTime = 0.0f;
		}

		if (dayNight.GetTime() > sunStart && dayNight.GetTime () < sunEnd){ //if sunlight
			thirstTime+= 3*Time.deltaTime;
		}
		else{
			thirstTime+=Time.deltaTime;
		}
		if (dayNight.GetTime ()>moonStart || dayNight.GetTime () < moonEnd){ //if nighttime
			if (playerEquipment.TorchOut()){
				mentalTime += Time.deltaTime;
			}
			else{
				mentalTime += 3*Time.deltaTime;
			}
		}
		if (Input.GetButton ("Run")){
			hungerTime += 3*Time.deltaTime;
		}
		else{
			hungerTime += Time.deltaTime;
		}
	}

	private void SetCutsceneStatus(bool flag){
		cutscenePlaying=flag;
		/*playerTransform.GetComponent<PlayerWalkSound>().enabled=!flag;
		playerInterface.gameObject.SetActive(!flag);
		playerTransform.GetComponent<PauseMenu>().enabled=!flag;*/
		if (flag){
		cutsceneOver=!flag;
		}
	}

	private void CutsceneControls(){
		playerMouseLook.enabled=cutscenePlaying;
		playerMotor.enabled = !cutscenePlaying;
		setPlayerAction(!cutscenePlaying);
		cameraMouseLook.enabled=cutscenePlaying;
		playerTransform.GetComponent<PlayerWalkSound>().enabled=!cutscenePlaying;
		playerInterface.gameObject.SetActive(!cutscenePlaying);
		playerTransform.GetComponent<PauseMenu>().enabled=!cutscenePlaying;
		Screen.showCursor=!cutscenePlaying;
		stats.enabled=!cutscenePlaying;
		foreach (Camera camera in otherCameras) {
			camera.GetComponent<MouseLook>().enabled = cutscenePlaying;
		}
	}

}
