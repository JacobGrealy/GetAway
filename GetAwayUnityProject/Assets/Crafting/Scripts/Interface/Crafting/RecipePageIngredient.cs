using UnityEngine;
using System.Collections;

public class RecipePageIngredient : MonoBehaviour {

	public TextMesh text;
	public TextMesh needed;
	public MeshRenderer icon;
	public ItemManager itemManager;

	private string itemName;
	private bool requirementMet;
	private int numCraftable;

	public void UpdateIngredient(int itemId, int possessed, int needed) {
		IItem item = itemManager.GetItem(itemId);
		itemName = item.Name;
		UpdatePossessed(possessed, needed);
		this.needed.text = "x" + needed.ToString ();

		string iconTexturePath = "Items/icon-" + itemId;
		icon.material.mainTexture = (Texture2D) Resources.Load(iconTexturePath);
		icon.gameObject.SetActive(true);
	}

	public bool RequirementIsMet() {
		return requirementMet;
	}

	public int GetNumCraftable() {
		return numCraftable;
	}

	public void UpdatePossessed(int possessed, int needed) {
		text.text = itemName + "(" + possessed + ")";
		requirementMet = possessed >= needed;
		numCraftable = possessed / needed;
	}

	public bool IsEmpty() {
		return needed.text.Length == 0;
	}

	public void Empty() {
		text.text = "";
		needed.text = "";
		icon.gameObject.SetActive(false);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
