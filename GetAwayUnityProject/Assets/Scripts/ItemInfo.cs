using UnityEngine;
using System.Collections;

public class ItemInfo : MonoBehaviour {
	public int itemId;
	public int itemQuantity;

	void Start(){
		//need code to set item information
	}


	public int getItemId(){
		return itemId;
	}

	public int getItemQuantity(){
		return itemQuantity;
	}
}
