using UnityEngine;
using System.Collections;

public class InventoryGUIButtonSet : MonoBehaviour {

	public InventoryGUIConsumeButton consumeButton;
	public InventoryGUIEquipButton equipButton;
	public InventoryGUIDestroyButton destroyButton;
	public InventoryGUIUseButton useButton;

	public void DisplayXboxButtons() {
		if (consumeButton != null) consumeButton.DisplayXboxMaterial();
		if (equipButton != null) equipButton.DisplayXboxMaterial();
		if (destroyButton != null) destroyButton.DisplayXboxMaterial();
		if (useButton != null) useButton.DisplayXboxMaterial();
	}

	public void DisplayMouseButtons() {
		if (consumeButton != null) consumeButton.HideXboxMaterial();
		if (equipButton != null) equipButton.HideXboxMaterial();
		if (destroyButton != null) destroyButton.HideXboxMaterial();
		if (useButton != null) useButton.HideXboxMaterial();
	}
}
