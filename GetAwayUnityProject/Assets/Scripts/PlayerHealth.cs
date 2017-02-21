using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour {
	public int physicalHealth;
	public int mentalHealth;
	public int hunger;
	public int thirst;

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
		//physicalHealth = 100;

		//Test Value
		physicalHealth = 40;
		//mentalHealth = 10;

		//Test Value
		mentalHealth = 6;
		//hunger = 100;

		//Test Value
		hunger = 0;
		thirst = 100;
	}

	//Physical Health
	public void IncreaseHealth (int additional) {
		physicalHealth += additional;
	}
	public void SubtractHealth (int deduction) {
		physicalHealth -= deduction;
	}
	public int GetHealth (){
		return physicalHealth;
	}

	//Mental Health
	public void IncreaseMental (int additional) {
		mentalHealth += additional;
	}
	public void SubtractMental (int deduction) {
		mentalHealth -= deduction;
	}
	public int GetMental (){
		return mentalHealth;
	}

	//Hunger
	public void IncreaseHunger (int additional) {
		hunger += additional;
	}
	public void SubtractHunger (int deduction) {
		hunger -= deduction;
	}
	public int GetHunger (){
		return hunger;
	}

	//Thirst
	public void IncreaseThirst (int additional) {
		thirst += additional;
	}
	public void SubtractThirst(int deduction) {
		thirst -= deduction;
	}
	public int GetThirst (){
		return thirst;
	}

	// Update is called once per frame
	void Update () {
		//Will also have a check for time since last thirst quenching
		if (thirstTime > thirstDelay && thirst > 0) {
			SubtractThirst(5);
			thirstTime = 0.0f;
		}

		//Also, could add if haven't eaten anything for a certain amount of time
		if (hungerTime > hungerDelay && hunger > 0) {
			SubtractHunger(1);
			hungerTime = 0.0f;
		}
		hungerTime += Time.deltaTime;

		if (thirst == 0 && hunger < 25 && healthTime > healthDelay && physicalHealth > 0) {
			SubtractHealth(1);
			healthTime = 0.0f;
		}
		healthTime += Time.deltaTime;

		if ((hunger == 0 && physicalHealth < 50) && (mentalTime > mentalDelay) && mentalHealth > 0) {
			SubtractMental(1);
			mentalTime = 0.0f;
		}
		mentalTime += Time.deltaTime;
	}
}
