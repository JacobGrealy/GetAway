using UnityEngine;
using System.Collections;

public class ConsumableItemBehavior : IItemBehavior {

	private int healthOnConsume;
	private int hungerOnConsume;
	private int sanityOnConsume;
	private int thirstOnConsume;
	private HealthController playerHealthController;

	// Construct a consumable behavior with a parameter specifying
	// the health to be gained uponc consuming this item.
	public ConsumableItemBehavior(HealthController playerHealthController, int healthOnConsume, int hungerOnConsume, int sanityOnConsume, int thirstOnConsume) {
		this.playerHealthController = playerHealthController;
		this.healthOnConsume = healthOnConsume;
		this.hungerOnConsume = hungerOnConsume;
		this.sanityOnConsume = sanityOnConsume;
		this.thirstOnConsume = thirstOnConsume;
	}

	public void Execute() {
		playerHealthController.IncreaseHealth(healthOnConsume);
		playerHealthController.IncreaseHunger(hungerOnConsume);
		playerHealthController.IncreaseMental(sanityOnConsume);
		playerHealthController.IncreaseThirst(thirstOnConsume);
	}

	public ItemBehaviorType GetBehaviorType() {
		return ItemBehaviorType.CONSUMABLE;
	}
}
