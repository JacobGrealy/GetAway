using UnityEngine;
using System.Collections;
using System.Text;

public class InventoryGUIItemMenu : MonoBehaviour {

	// Title of item represented by menu.
	public TextMesh itemName;
	public TextMesh itemDescription;

	// Distance from center to position arrows on either side of menu.
	private float arrowCenterOffset;

	// User interactions events.
	private bool mouseOverInThisFrame = false;
	public delegate void UserMenuInteraction();
	public delegate void UserButtonInteraction(InventoryItemAction action);
	public event UserMenuInteraction UserHovering;
	public event UserMenuInteraction UserNotHovering;
	public event UserButtonInteraction ButtonClicked;

	// Item stats
	public InventoryGUIItemMenuStats stats;
	private IItem itemForStats;

	// Button Sets
	public Transform buttonSetContainer;
	public GameObject consumableButtonSet;
	public GameObject equipableButtonSet;
	public GameObject useButtonSet;
	public GameObject destroyableButtonSet;
	public InventoryGUIButtonSet currentButtonSet;

	// Sets the name of the item in the hover menu.
	public void SetItemName(string itemName) {
		this.itemName.text = itemName.ToUpper();
	}

	public void SetItemDescription(string itemDescription) {
		char[] delimeter = { ' ' };
		string[] words = itemDescription.Split(delimeter);
		StringBuilder description = new StringBuilder();
		StringBuilder line = new StringBuilder();
		int maxLineLength = 24;
		foreach (string word in words) {
			if (line.Length + word.Length + 1 < maxLineLength) {
				line.Append(word + " ");
			} else {
				description.Append(line);
				description.Append("\n");
				line.Remove(0, line.Length);
				line.Append(word + " ");
			}
		}

		if (line.Length > 0) {
			description.Append(line);
		}

		this.itemDescription.text = description.ToString();
	}

	// Configures the buttons available in the hover menu.
	// If xboxController is true, the buttons will be themed to the xbox control scheme.
	public void SetMenuOptions(IItem item) {
		// Remove existing menu options if they exist.
		foreach(InventoryGUIButtonSet buttonSet in gameObject.GetComponentsInChildren<InventoryGUIButtonSet>(true)) {
			Destroy(buttonSet.gameObject);
		}

		GameObject newButtonSet;
		if (item.HasBehavior(ItemBehaviorType.CONSUMABLE)) {
			newButtonSet = (GameObject) Instantiate(consumableButtonSet);
			newButtonSet.GetComponentInChildren<InventoryGUIDestroyButton>().UserClicked += DestroyButtonClicked;
			newButtonSet.GetComponentInChildren<InventoryGUIConsumeButton>().UserClicked += ConsumeButtonClicked;
			newButtonSet.transform.parent = buttonSetContainer;
			newButtonSet.transform.localPosition = consumableButtonSet.transform.position;
		} else if (item.HasBehavior(ItemBehaviorType.EQUIPABLE)) {
			newButtonSet = (GameObject) Instantiate(equipableButtonSet);
			newButtonSet.GetComponentInChildren<InventoryGUIDestroyButton>().UserClicked += DestroyButtonClicked;
			newButtonSet.GetComponentInChildren<InventoryGUIEquipButton>().UserClicked += EquipButtonClicked;
			newButtonSet.transform.parent = buttonSetContainer;
			newButtonSet.transform.localPosition = equipableButtonSet.transform.position;
		} else if (item.HasBehavior(ItemBehaviorType.USEABLE)) {
			newButtonSet = (GameObject) Instantiate(useButtonSet);
			newButtonSet.GetComponentInChildren<InventoryGUIDestroyButton>().UserClicked += DestroyButtonClicked;
			newButtonSet.GetComponentInChildren<InventoryGUIUseButton>().UserClicked += UseButtonClicked;
			newButtonSet.transform.parent = buttonSetContainer;
			newButtonSet.transform.localPosition = useButtonSet.transform.position;
		} else {
			newButtonSet = (GameObject) Instantiate(destroyableButtonSet);
			newButtonSet.GetComponentInChildren<InventoryGUIDestroyButton>().UserClicked += DestroyButtonClicked;
			newButtonSet.transform.parent = buttonSetContainer;
			newButtonSet.transform.localPosition = destroyableButtonSet.transform.position;
		}

		newButtonSet.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		currentButtonSet = newButtonSet.GetComponent<InventoryGUIButtonSet>();

		// Configure stats
		itemForStats = item;
		if (item.HasBehavior(ItemBehaviorType.EQUIPABLE)) {
			stats.SetEquippableStats(item.Damage, item.PercentDurabilityRemaining);
		} else if (item.HasBehavior(ItemBehaviorType.CONSUMABLE)) {
			print(item.Thirst);
			stats.SetConsumableStats(item.Hunger, item.Thirst, item.Sanity);
		} else {
			stats.SetNoStats();
		}
	}

	public void Show(bool xboxMenu) {
		gameObject.SetActive(true);
		if (xboxMenu) {
			currentButtonSet.DisplayXboxButtons();
		} else {
			currentButtonSet.DisplayMouseButtons();
		}

		if (itemForStats.HasBehavior(ItemBehaviorType.EQUIPABLE)) {
			stats.UpdateEquippableDurability(itemForStats.PercentDurabilityRemaining);
		}
	}

	public void Hide() {
		gameObject.SetActive(false);
	}

	// Called when the destroy button has been clicked.
	void DestroyButtonClicked() {
		if (ButtonClicked != null) {
			ButtonClicked(InventoryItemAction.DESTROY);
		}
	}
	
	// Called when the consume button has been clicked.
	void ConsumeButtonClicked() {
		if (ButtonClicked != null) {
			ButtonClicked(InventoryItemAction.CONSUME);
		}
	}
	
	// Called when the equip button has been clicked.
	void EquipButtonClicked() {
		if (ButtonClicked != null) {
				ButtonClicked(InventoryItemAction.EQUIP);
		}
	}
	
	// Called when the use button has been clicked.
	void UseButtonClicked() {
		if (ButtonClicked != null) {
			ButtonClicked(InventoryItemAction.USE);
		}
	}

	void OnMouseOver() {
		mouseOverInThisFrame = true;
	}

	void Update () {
		// Check to see if the mouse pointer is on the menu.
		Ray ray = GameObject.Find("Interface Camera").camera.ScreenPointToRay(Input.mousePosition);
		RaycastHit[] hits = Physics.RaycastAll(ray);
		if (hits.Length > 0) {
			for (int i = 0; i < hits.Length; i++) {
				if (hits[i].collider == this.collider) {
					mouseOverInThisFrame = true;
					break;
				}
			}
		}

		if (!mouseOverInThisFrame) {
			if (UserNotHovering != null) {
				UserNotHovering.Invoke();
			}
		} else {
			if (UserHovering != null) {
				UserHovering.Invoke();
			}
		}
		mouseOverInThisFrame = false;
	}
}
