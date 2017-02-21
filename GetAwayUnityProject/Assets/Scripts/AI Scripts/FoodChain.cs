using UnityEngine;
using System.Collections;

public class FoodChain : MonoBehaviour  {
	public enum FoodChainLevels {
		Player,
		Stegosaurus,
		Raptor,
		PlayerWithTorch,
		Carnotaurus,
		TRex
	};

	//public FoodChainEnums.FoodChain foodChainLevel;
	public FoodChainLevels foodChainLevel;
	public bool herbivorous;

	/*public FoodChainEnums.FoodChain getFoodChainLevel(){
		return foodChainLevel;
	}*/

	public FoodChainLevels getFoodChainLevel(){
		return foodChainLevel;
	}

	public void setFoodChainLevel(FoodChainLevels newLevel){
		foodChainLevel = newLevel;
	}

	public bool isHerbivore(){
		return herbivorous;
	}
}
