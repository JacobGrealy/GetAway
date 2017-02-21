using UnityEngine;
using System.Collections;

public class EquipInfo : MonoBehaviour {
	public int equipId;
	public string equipType;
	public int equipDamageScale;
	public int equipDurability;

 	void Start(){
		//Need code to obtain item information
	}

	public int getEquipId(){
		return equipId;
	}

	public string getEquipType(){
		return equipType;
	}

	public int getEquipDamageScale(){
		return equipDamageScale;
	}

	public int getEquipDurability(){
		return equipDurability;
	}
}
