using UnityEngine;
using System.Collections;

public class Recipe {

	public int CraftedItemId { get; private set; }
	public Hashtable Ingredients { get; private set; }

	public Recipe(int id) {
		this.CraftedItemId = id;
		this.Ingredients = new Hashtable();
	}

	public void AddIngredient(int itemId, int quantity) {
		this.Ingredients.Add(itemId, quantity);
	}
}
