using UnityEngine;
using System.Collections;
using System;
using SimpleJSON;

public class PlayerEquip : MonoBehaviour {

	public bool unarmed = true;
	public bool equippedAx = false;
	public bool equippedSpear = false;
	public bool equippedBow = false;
	public bool equippedTorch = false;
	public GameObject ax, spear, bow, torch, sceneArrow, projectileArrow;
	public int arrowSpeed;
	private int durability;
	private int delay = 0;
	private int weaponDamageScale=1, weaponDurability=1, weaponRange=3;
	private int unarmedRange=3, axRange=3, spearRange=5, torchRange=3;

	public AudioSource breakSound;
	public AudioSource spearSwing;
	public AudioSource axSwing;
	public AudioSource torchSwing;
	public AudioSource arrowShot;
	public AudioSource bowPull;

	private bool pulledBack;

	bool play = true;
	bool attacking = false;

	private float fullBack = 1.0f;
	private GameObject arrowFire;
	private Ray ray;
	private bool held=false;
	private float startTime;
	private float pullBack;
	private float fractionBack; 
	private float finalArrowSpeed;
	private float arrowDamage;

	private float torchCurrentTime;
	private float torchMaxTime = 10f;
	private int lowTorchDurability = 15;
	private float brightTorchIntensity;
	private float brightTorchFlickerMin;
	private float brightTorchFlickerMax;
	public float weakTorchIntensity=.1f;
	public float weakTorchFlickerMin=.1f;
	public float weakTorchFlickerMax=.15f;

	private bool canHitEnemy;
	private bool canHitTree;
	public Inventory inventory;
	private IItem iItem;

	private bool isPaused=false;
	private int maxDurability;
	// Use this for initialization
	void Start () {
		unarmPlayer ();
		pulledBack = true;
		canHitEnemy=true;
		canHitTree=true;
		maxDurability=0;
		//brightTorchFlickerMax=torch.gameObject.GetComponent<flickeringLight>().FluorescentFlickerMax;
		//brightTorchFlickerMin=torch.gameObject.GetComponentInChildren<flickeringLight>().FluorescentFlickerMin;
		//brightTorchIntensity=torch.gameObject.GetComponentInChildren<flickeringLight>().CampfireIntensityBaseValue;
	}
	
	// Update is called once per frame
	void Update () {
		if (!isPaused){
			if (!unarmed){
				weaponSway ();
			}
			if (equippedTorch){
				torchCurrentTime+=Time.deltaTime;
				if (torchCurrentTime>torchMaxTime){
					iItem.Durability--;
					if (iItem.Durability<=0){
						unarmPlayer ();
						breakSound.Play();
						if (maxDurability>0){
							iItem.Durability=maxDurability;	
						}
						inventory.RemoveItem(iItem,1);
					}
					torchCurrentTime=0;
					//else if (iItem.Durability<15){
					//
				//	}
				}
			}
			if (!isAttacking() && !canHitEnemy){
				canHitEnemy=true;
			}
			if (!isAttacking () && !canHitTree){
				canHitTree=true;
			}
		}
	}

	void weaponSway(){
		if (equippedAx){
			if (delay == 50) {
			//	ax.animation.Play("AxSway");
				delay = 0;
			}
		}
		else if (equippedSpear){

		}
		else if (equippedBow){

		}
		else{ //torch

		}
		delay++;
	}

	bool torchAnimation = true;
	public void weaponAttack(){
		if (equippedAx) {
			ax.animation.Play("BetterAxSwing");
			axSwing.Play();
			iItem.Durability--;
		}
		else if (equippedSpear) {
			spear.animation.Play("SpearJab");
			spearSwing.Play();
			iItem.Durability--;
		}
		else if (equippedTorch) {
			//if (torchAnimation) {
				torch.animation.Play("TorchSwing");
				torchSwing.Play();
				iItem.Durability--;
				//torchAnimation = false;
			//}
//			else {
//				torch.animation.Play("TorchSwing2");
//				torchSwing.Play();
//				torchAnimation = true;
//			}
		}
		else if (equippedBow){
			if (!held){ //create arrow
				ray = Camera.main.camera.ScreenPointToRay (new Vector3(Screen.width/2,Screen.height/2,0));
				arrowFire = Instantiate (projectileArrow,sceneArrow.transform.position,Quaternion.LookRotation(ray.direction*arrowSpeed)) as GameObject;// as Rigidbody;
				startTime = Time.time;
				held=true;
				arrowFire.rigidbody.useGravity=false;
				arrowFire.transform.GetComponent<ArrowPhysics>().enabled=false;
			}
			else {
				if (Input.GetButton ("Fire1") || Input.GetAxisRaw("RightTrigger") == -1){ //player is holding attack
					if (pulledBack){
						bowPull.Play();
						pulledBack = false;
					}
					pullBack = (Time.time - startTime);
					fractionBack = pullBack / fullBack;
					ray = Camera.main.camera.ScreenPointToRay (new Vector3(Screen.width/2,Screen.height/2,0));
					arrowFire.transform.rigidbody.rotation = Quaternion.LookRotation(ray.direction);
					arrowFire.transform.position = Vector3.Lerp (sceneArrow.transform.position,sceneArrow.transform.position-ray.direction/2, fractionBack);
					finalArrowSpeed=Mathf.Lerp ((float)arrowSpeed,4f*(float)arrowSpeed,fractionBack);
					arrowDamage=Mathf.Lerp ((float)weaponDamageScale,4f*(float)weaponDamageScale,fractionBack);
					//Debug.Log ("pulling");

				}
				else{ //player releases arrow
					Debug.Log ("fired");
					pulledBack = true;
					bowPull.Stop();
					arrowShot.Play();
					ray = Camera.main.camera.ScreenPointToRay (new Vector3(Screen.width/2,Screen.height/2,0));
					held=false;
					arrowFire.GetComponent<ArrowCollision>().SetArrowDamage((int)arrowDamage);
					arrowFire.rigidbody.velocity = ray.direction*finalArrowSpeed;//bullet speed
					arrowFire.rigidbody.rotation.SetLookRotation(arrowFire.rigidbody.velocity);
					//Debug.Log ("false");
					arrowFire.rigidbody.useGravity=true;
					arrowFire.transform.GetComponent<ArrowPhysics>().enabled=true;
					Destroy (arrowFire,20.0f); //destroy after 20 seconds
					iItem.Durability--;
				}
			}

		}
		if (!unarmed){

			if (iItem.Durability<=0){
				unarmPlayer ();
				breakSound.Play();
				if (maxDurability>0){
					iItem.Durability=maxDurability;
				}
				inventory.RemoveItem(iItem,1);
			}
		}
	//	Debug.Log (weaponDurability);
		//Debug.Log ("Attack");
	}

	public void setEquip(EquipableItem itemToEquip, int damageScale, int durability){ //don't use
		if (maxDurability>0){
			iItem.Durability=maxDurability;
		}
	unarmPlayer ();
		if (itemToEquip == EquipableItem.AX){
			ax.SetActive (true);
			equippedAx=true;
			unarmed=false;
			weaponRange=axRange;
		}
		else if (itemToEquip == EquipableItem.SPEAR){
			spear.SetActive (true);
			equippedSpear=true;
			unarmed=false;
			weaponRange=spearRange;
		}
		else if (itemToEquip == EquipableItem.BOW){
			bow.SetActive (true);
			equippedBow=true;
			unarmed=false;
			//something for bow range?
		}
		else{ // torch
			torch.SetActive (true);
			//relightTorch();
			equippedTorch=true;
			unarmed=false;
			weaponRange=torchRange;
			torchCurrentTime=0f;
		//	GetComponent<FoodChain>().setFoodChainLevel(FoodChain.FoodChainLevels.PlayerWithTorch);
		}
		weaponDurability = durability;
		weaponDamageScale = damageScale;
	}

	void unarmPlayer(){
		ax.SetActive (false);
		spear.SetActive (false);
		bow.SetActive (false);
		torch.SetActive (false);
		weaponDamageScale=1;
		weaponRange=unarmedRange;
		equippedAx=false;
		equippedSpear=false;
		equippedBow=false;
		equippedTorch=false;
		//if(!(GetComponent<FoodChain>().getFoodChainLevel().Equals(FoodChain.FoodChainLevels.Player)))
		//	GetComponent<FoodChain>().setFoodChainLevel(FoodChain.FoodChainLevels.Player);
		unarmed=true;
	}

	public int getDamageScale(){
		return weaponDamageScale;
	}

	public int getDurability(){
		return weaponDurability;
	}

	public bool AxOut(){
		return equippedAx;
	}

	public int getWeaponRange(){
		return weaponRange;
	}

	public bool isAttacking(){
		return (ax.animation.isPlaying || spear.animation.isPlaying || bow.animation.isPlaying || torch.animation.isPlaying);
	}

	public bool BowOut(){
		return equippedBow;
	}

	public bool TorchOut(){
		return equippedTorch;
	}

	public bool ArrowPulled(){
		return held;
	}

	public bool CanHitEnemy(){
		return canHitEnemy;
	}

	public void SetCanHitEnemy(bool flag){
		canHitEnemy=flag;
	}

	public bool CanHitTree(){
		return canHitTree;
	}

	public void SetCanHitTree(bool flag){
		canHitTree=flag;
	}
	/*private void weakenTorch(){
		torch.gameObject.GetComponentInChildren<flickeringLight>().FluorescentFlickerMin=weakTorchFlickerMin;
		torch.gameObject.GetComponentInChildren<flickeringLight>().FluorescentFlickerMax=weakTorchFlickerMax;
		torch.gameObject.GetComponentInChildren<flickeringLight>().CampfireIntensityBaseValue=weakTorchIntensity;
	}

	private void relightTorch(){
		torch.gameObject.GetComponentInChildren<flickeringLight>().FluorescentFlickerMin=brightTorchFlickerMin;
		torch.gameObject.GetComponentInChildren<flickeringLight>().FluorescentFlickerMax=brightTorchFlickerMax;
		torch.gameObject.GetComponentInChildren<flickeringLight>().CampfireIntensityBaseValue=brightTorchIntensity;
	}*/

	public void setEquip(EquipableItem itemToEquip, IItem itemInfo){
		if (maxDurability>0){
			iItem.Durability=maxDurability;
		}
		unarmPlayer ();
		if (itemToEquip == EquipableItem.AX){
			ax.SetActive (true);
			equippedAx=true;
			unarmed=false;
			weaponRange=axRange;
		}
		else if (itemToEquip == EquipableItem.SPEAR){
			spear.SetActive (true);
			equippedSpear=true;
			unarmed=false;
			weaponRange=spearRange;
		}
		else if (itemToEquip == EquipableItem.BOW){
			bow.SetActive (true);
			equippedBow=true;
			unarmed=false;
			//something for bow range?
		}
		else{ // torch
			torch.SetActive (true);
			//relightTorch();
			equippedTorch=true;
			unarmed=false;
			weaponRange=torchRange;
			torchCurrentTime=0f;
			//	GetComponent<FoodChain>().setFoodChainLevel(FoodChain.FoodChainLevels.PlayerWithTorch);
		}
		iItem=itemInfo;
		weaponDamageScale=iItem.Damage;
		maxDurability=iItem.Durability;
	}

	public void SetPause(bool flag){
		isPaused=flag;
	}
}
