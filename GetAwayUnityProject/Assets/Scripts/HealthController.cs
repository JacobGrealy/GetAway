using UnityEngine;
using System.Collections;

public class HealthController : MonoBehaviour {
	//Sounds
	public AudioSource damageSound;

	public int physicalHealth=100;
	public int mentalHealth=10;
	public int hunger=100;
	public int thirst=100;
	private int maxPhysical, maxMental, maxHunger, maxThirst;
	//Hunger
	private float hungerTime = 0.0f;
	private float hungerDelay = 30.0f;
	//Thirst
	private float thirstTime = 0.0f;
	private float thirstDelay = 20.0f;
	//Health
	private float healthTime = 0.0f;
	private float healthDelay = 2.0f;
	//Mental
	private float mentalTime = 0.0f;
	private float mentalDelay = 15.0f;
	// Use this for initialization
	void Start () {
		maxPhysical =100;
		maxMental = 100;
		maxHunger = 100;
		maxThirst = 100;
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	//Physical Health
	public void IncreaseHealth (int additional) {
		int tmp = physicalHealth+additional;
		if (tmp>maxPhysical){
			physicalHealth=maxPhysical;
		}
		else{
			physicalHealth=tmp;
		}
	}
	public void SubtractHealth (int deduction) {
		physicalHealth -= deduction;
		damageSound.Play();
		if (physicalHealth<=0){
			Application.LoadLevel ("GameOver");
		}
		Score.damageTaken+=deduction;
	}
	public int GetHealth (){
		return physicalHealth;
	}
	public int GetMaxHealth(){
		return maxPhysical;
	}
	
	//Mental Health
	public void IncreaseMental (int additional) {
		int tmp = mentalHealth + additional;
		if (tmp>maxMental){
			mentalHealth=maxMental;
		}
		else{
			mentalHealth=tmp;
		}
	}
	public void SubtractMental (int deduction) {
		int tmp = mentalHealth - deduction;
		if (tmp<0){
			mentalHealth=0;
		}
		else{
			mentalHealth=tmp;
		}
	}
	public int GetMental (){
		return mentalHealth;
	}
	public int GetMaxMental(){
		return maxMental;
	}

	//Hunger
	public void IncreaseHunger (int additional) {
		int tmp = hunger + additional;
		if (tmp>maxHunger){
			hunger = maxHunger;
		}
		else{
			hunger = tmp;
		}
	}
	public void SubtractHunger (int deduction) {
		int tmp = hunger - deduction;
		if (tmp<0){
			hunger = 0;
		}
		else{
			hunger = tmp;
		}
	}
	public int GetHunger (){
		return hunger;
	}
	public int GetMaxHunger(){
		return maxHunger;
	}

	//Thirst
	public void IncreaseThirst (int additional) {
		int tmp = thirst + additional;
		if (tmp > maxThirst){
			thirst=maxThirst;
		}
		else{
			thirst=tmp;
		}
	}
	public void SubtractThirst(int deduction) {
		int tmp = thirst - deduction;
		if (tmp<0){
			thirst = 0;
		}
		else{
			thirst = tmp;
		}
	}
	public int GetThirst (){
		return thirst;
	}

	public int GetMaxThirst(){
		return maxThirst;
	}
}
