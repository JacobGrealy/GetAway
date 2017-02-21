using UnityEngine;
using System.Collections;

public class WeaponSwapper : MonoBehaviour {

	public Inventory playerInventory;

	void SwapToAx() {
		IItem ax = playerInventory.GetStrongestWeapon(EquipableItem.AX);
		if (ax != null) {
			ax.TriggerBehavior(ItemBehaviorType.EQUIPABLE);
		}
	}

	void SwapToTorch() {
		IItem torch = playerInventory.GetStrongestWeapon(EquipableItem.TORCH);
		if (torch != null) {
			torch.TriggerBehavior(ItemBehaviorType.EQUIPABLE);
		}
	}

	void SwapToSpear() {
		IItem spear = playerInventory.GetStrongestWeapon(EquipableItem.SPEAR);
		if (spear != null) {
			spear.TriggerBehavior(ItemBehaviorType.EQUIPABLE);
		}
	}

	void SwapToBow() {
		IItem bow = playerInventory.GetStrongestWeapon(EquipableItem.BOW);
		if (bow != null) {
			bow.TriggerBehavior(ItemBehaviorType.EQUIPABLE);
		}
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetAxis("Joystick Weapon Swap X") > 0.3 || Input.GetKeyDown(KeyCode.Alpha1)) {
			SwapToAx();
		} else if (Input.GetAxis("Joystick Weapon Swap X") < -0.3 || Input.GetKeyDown(KeyCode.Alpha4)) {
			SwapToTorch();
		} else if (Input.GetAxis("Joystick Weapon Swap Y") > 0.3 || Input.GetKeyDown(KeyCode.Alpha2)) {
			SwapToSpear ();
		} else if (Input.GetAxis("Joystick Weapon Swap Y") < -0.3 || Input.GetKeyDown(KeyCode.Alpha3)) {
			SwapToBow ();
		}
	}
}
