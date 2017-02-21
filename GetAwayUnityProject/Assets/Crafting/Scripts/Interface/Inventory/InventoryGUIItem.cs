using UnityEngine;
using System.Collections;

public class InventoryGUIItem : MonoBehaviour {

	// The item
	public MeshRenderer itemMesh;
	public TextMesh quantityText;
	private IItem item;

	// Item placeholder
	public MeshRenderer placeholderSpot;

	// Item hover effects
	public Material normalBorder;
	public Material hoveredBorder;
	private Vector3 originalScale;
	private Vector3 hoveredScale;
	private Vector3 originalIconPosition;
	private Vector3 hoveredIconPosition;

	// Item hover menu belonging to this item.
	public InventoryGUIItemMenu hoverMenu;

	// Inventory this item belongs to.
	public Inventory inventory;

	// The Item Manager to draw items from.
	public ItemManager itemManager;

	// States
	private bool mouseOverInThisFrame = false;
	private bool itemIsSet = false;
	private bool isHovered = false;
	private bool menuIsHovered = false;
	private bool menuIsHoveredByJoystick = false;
	private bool isActive = false;
	private float animTime = 0.0f;
	private const float ANIM_TIME_CONSTANT = 14.0f;

	// Internal record of item properties.
	private int quantity = 0;

	void Start() {
		// Initial transformations.
		transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
		originalScale = transform.localScale;
		hoveredScale = transform.localScale * 1.2f;
		originalIconPosition = itemMesh.transform.localPosition;
		hoveredIconPosition = itemMesh.transform.localPosition + new Vector3(0.0f, 0.0f, -0.15f);

		// Initial inventory item settings.
		Material[] mats = itemMesh.materials;
		mats[1] = normalBorder;
		itemMesh.materials = mats;

		// Hover menu events.
		hoverMenu.ButtonClicked += TriggerItemAction;

		// Initial visibility settings.
		placeholderSpot.gameObject.SetActive(true);
		itemMesh.gameObject.SetActive(false);
		hoverMenu.gameObject.SetActive(false);
	}

	// Configures this inventory GUI item to represent the item with passed item ID.
	public InventoryGUIItem SetItem(int itemId) {
		// Setup item.
		string iconTexturePath = "Items/icon-" + itemId;
		Material[] mats = itemMesh.materials;
		mats[0].shader = Shader.Find ("Unlit/Transparent");
		mats[0].mainTexture = (Texture2D) Resources.Load(iconTexturePath);
		itemMesh.materials = mats;

		placeholderSpot.gameObject.SetActive(false);
		itemMesh.gameObject.SetActive(true);

		// Setup hover menu.
		item = itemManager.GetItem(itemId);
		hoverMenu.SetItemName(item.Name);
		hoverMenu.SetItemDescription(item.Description);
		hoverMenu.SetMenuOptions(item);

		itemIsSet = true;
		return this;
	}

	// Clears the item from this spot. Sets the placeholder to active,
	// and everything representing the item to inactive.
	public void ClearItem() {
		this.quantity = 0;

		placeholderSpot.gameObject.SetActive(true);
		itemMesh.gameObject.SetActive(false);

		// If we're still hovered, set the hover state off.
		if (mouseOverInThisFrame || menuIsHovered || menuIsHoveredByJoystick) {
			SetNotHoveredState();
			mouseOverInThisFrame = false;
			menuIsHovered = false;
		}

		itemIsSet = false;
	}

	public bool ItemIsSet() {
		return itemIsSet;
	}

	// Update the quantity of this item.
	public InventoryGUIItem SetQuantity(int quantity) {
		this.quantity = quantity;
		quantityText.text = quantity.ToString();

		return this;
	}

	// Increment the quantity of this item.
	public InventoryGUIItem IncrementQuantity(int quantity) {
		this.quantity++;
		quantityText.text = this.quantity.ToString();
		
		return this;
	}

	public void SetHoveredByJoystick() {
		menuIsHoveredByJoystick = true;
	}

	public void SetNotHoveredByJoystick() {
		menuIsHoveredByJoystick = false;
	}

	// Triggers the inventory event corresponding to the passed
	// action taken upon this item.
	void TriggerItemAction(InventoryItemAction action) {
		if (action == InventoryItemAction.CONSUME) {
			item.TriggerBehavior(ItemBehaviorType.CONSUMABLE);
			inventory.RemoveItem(item.Id, 1);
		} else if (action == InventoryItemAction.EQUIP) {
			item.TriggerBehavior(ItemBehaviorType.EQUIPABLE);
		} else if (action == InventoryItemAction.USE) {
			item.TriggerBehavior(ItemBehaviorType.USEABLE);
			inventory.RemoveItem(item.Id, 1);
		} else if (action == InventoryItemAction.DESTROY) {
			item.TriggerBehavior(ItemBehaviorType.DESTROYABLE);
			inventory.RemoveItem(item.Id, 1);
		}
	}

	void OnMouseOver() {
		mouseOverInThisFrame = true;
	}
	
	void HandleUserHoveringMenu() {
		menuIsHovered = true;
	}
	
	void HandleUserNotHoveringMenu() {
		menuIsHovered = false;
	}

	void SetHoveredState(bool xboxMenu) {
		isActive = true;

		// Change border of icon.
		Material[] mats = itemMesh.materials;
		mats[1] = hoveredBorder;
		itemMesh.materials = mats;

		// Show hover menu.
		hoverMenu.Show(xboxMenu);
		menuIsHovered = true;
		hoverMenu.UserHovering += HandleUserHoveringMenu;
		hoverMenu.UserNotHovering += HandleUserNotHoveringMenu;
	}

	void SetNotHoveredState() {
		isActive = false;

		Material[] mats = itemMesh.materials;
		mats[1] = normalBorder;
		itemMesh.materials = mats;

		// Hide hover menu.
		hoverMenu.Hide();
		hoverMenu.UserHovering -= HandleUserHoveringMenu;
		hoverMenu.UserNotHovering -= HandleUserNotHoveringMenu;
	}
		
	void Update() {
		if (itemIsSet) {
			// Update animation.
			if (mouseOverInThisFrame || menuIsHovered || menuIsHoveredByJoystick) {
				animTime = Mathf.Min(1.0f, animTime += ANIM_TIME_CONSTANT * Time.deltaTime);
				if (!isActive) SetHoveredState(menuIsHoveredByJoystick);
			} else {
				animTime = Mathf.Max (0.0f, animTime -= ANIM_TIME_CONSTANT * Time.deltaTime);
				if (isActive) SetNotHoveredState();
			}
			transform.localScale = Vector3.Slerp (originalScale, hoveredScale, animTime);
			itemMesh.transform.localPosition = Vector3.Slerp (originalIconPosition, hoveredIconPosition, animTime);

			mouseOverInThisFrame = false;
		}
	}
}
