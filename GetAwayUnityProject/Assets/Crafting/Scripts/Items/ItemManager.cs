using UnityEngine;
using System;
using System.Collections;
using System.IO;
using SimpleJSON;

/**
 * The Item Manager contains collections of all items, weapons, and other resources that
 * exist within the game. This manager should be used purely to lookup these resources
 * and their associated information. It is created and populated at first reference 
 * (ideally when the game starts up), and should not be modified at any point following.
 */
public class ItemManager: MonoBehaviour {

	// Collection of all recipes.
	public ArrayList Recipes { get; private set; }

	// Collection of items. (int:ItemID, Item:Item)
	public Hashtable Items { get; private set; }

	// Singleton
	private static ItemManager instance;

	// Requirements for creating items.
	public HealthController playerHealthController;
	public PlayerEquip playerEquipController;
	public PlayerDropController playerDropController;

	void Awake() {
		this.Recipes = new ArrayList();
		this.Items = new Hashtable();

		string recipesJsonPath = Path.Combine(Application.streamingAssetsPath, "Items/recipes.json");
		string resourcesJsonPath = Path.Combine(Application.streamingAssetsPath, "Items/resources.json");
		string weaponsJsonPath = Path.Combine(Application.streamingAssetsPath, "Items/weapons.json");

		LoadRecipes(recipesJsonPath);
		LoadItems(resourcesJsonPath);
		LoadItems(weaponsJsonPath);
	}

	// Reads recipes from a json file into the recipes list.
	private void LoadRecipes(string recipesJsonPath) {
		var recipes = JSON.Parse(getFile(recipesJsonPath));
		
		for (int i = 0;  recipes[i] != null; i++) {
			Recipe recipe = new Recipe(Int32.Parse(recipes[i]["id"]));

			for (int j = 0; recipes[i]["ingredients"][j] != null; j++) {
				var ingredient = recipes[i]["ingredients"][j];
				recipe.AddIngredient(Int32.Parse(ingredient["id"]), Int32.Parse(ingredient["quantity"]));
			}
			this.Recipes.Add(recipe);
		}
	}

	// Reads items from a json file into the items list.
	private void LoadItems(string itemsJsonpath) {
		var items = JSON.Parse(getFile(itemsJsonpath));
		
		for (int i = 0;  items[i] != null; i++) {
			int id = Int32.Parse(items[i]["id"]);
		
			// Create the item.
			IItem item = new Item(id);
			item.Name = (string) items[i]["name"];
			item.Description = (string) items[i]["description"];

			// Parse the item's type.
			String type = (string) items[i]["type"];
			if (type == "EQUIPABLE") {
				item.Damage = Int32.Parse(items[i]["damage"]);
				item.BaseDurability = Int32.Parse(items[i]["durability"]);
				item.Durability = item.BaseDurability;
				EquipableItem weaponType = GetEquipableItemFromString((string) items[i]["weapontype"]);
				item.AddBehavior(new EquipableItemBehavior(playerEquipController, weaponType, item));
			} else if (type == "CONSUMABLE") {
				print (item.Name);
				int healthOnConsume = Int32.Parse(items[i]["health"]);
				item.Hunger = Int32.Parse(items[i]["hunger"]);
				item.Thirst = Int32.Parse(items[i]["thirst"]);
				print (item.Thirst);
				item.Sanity = Int32.Parse(items[i]["sanity"]);
				item.AddBehavior(new ConsumableItemBehavior(playerHealthController, healthOnConsume, item.Hunger, item.Sanity, item.Thirst));
			} else if (type == "USEABLE") {
				item.AddBehavior(new UseableItemBehavior(playerDropController, GetGameObjectFromId(id)));
			} else {
				item.AddBehavior(new DestroyableItemBehavior());
			}

			// Add the item to collection.
			this.Items.Add (id, item);
		}
	}

	public Recipe GetRecipe(int recipeNumber) {
		return (Recipe) Recipes[recipeNumber];
	}
	
	public IItem GetItem(int itemId) {
		return (IItem) Items[itemId];
	}

	private EquipableItem GetEquipableItemFromString(string equipableItem) {
		if (equipableItem == "ax") return EquipableItem.AX;
		else if (equipableItem == "spear") return EquipableItem.SPEAR;
		else if (equipableItem == "bow") return EquipableItem.BOW;
		else if (equipableItem == "torch") return EquipableItem.TORCH;
		else return EquipableItem.AX;
	}

	private GameObject GetGameObjectFromId(int itemId) {
		string gameObjectPath = "Items/droppable-" + itemId;
		return (GameObject) Resources.Load(gameObjectPath);
	}

	// Loads a file, untouched, into a string and returns it.
	private string getFile(string path) {
		TextReader tr = new StreamReader(path);
		
		string line;
		string json = "";
		while((line = tr.ReadLine()) != null) {
			json += line;
		}
		// Close the stream
		tr.Close();
		
		return json;
	}
}
