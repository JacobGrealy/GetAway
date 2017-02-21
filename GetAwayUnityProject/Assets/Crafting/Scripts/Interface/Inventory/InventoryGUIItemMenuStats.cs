using UnityEngine;
using System.Collections;

public class InventoryGUIItemMenuStats : MonoBehaviour {

	public GameObject[] icon;
	public TextMesh[] title;
	public TextMesh[] subtitle;

	public Material damageIcon;
	public Material durabilityIcon;
	public Material ammoIcon;
	public Material hungerIcon;
	public Material thirstIcon;
	public Material sanityIcon;

	public void SetConsumableStats(int hunger, int thirst, int sanity) {
		gameObject.SetActive(true);

		icon[0].renderer.material = hungerIcon;
		icon[1].renderer.material = thirstIcon;
		icon[2].renderer.material = sanityIcon;

		title[0].text = hunger != null ? hunger.ToString() : "0";
		title[1].text = thirst != null ? thirst.ToString() : "0";
		title[2].text = sanity != null ? sanity.ToString() : "0";

		subtitle[0].text = "HUNGER";
		subtitle[1].text = "THIRST";
		subtitle[2].text = "SANITY";
	}

	public void SetEquippableStats(int damage, int durability) {
		gameObject.SetActive(true);

		icon[0].renderer.material = damageIcon;
		icon[1].renderer.material = durabilityIcon;
		icon[2].renderer.material = ammoIcon;

		title[0].text = damage != null ? damage.ToString() : "0";
		title[1].text = durability != null ? durability.ToString() + "%" : "100%";
		if (true) { // no dedicated ammo display temporarily
			icon[2].gameObject.SetActive(false);
			title[2].gameObject.SetActive(false);
			subtitle[2].gameObject.SetActive(false);
		} else {
			icon[2].gameObject.SetActive(true);
			title[2].gameObject.SetActive(true);
			subtitle[2].gameObject.SetActive(true);
		}

		subtitle[0].text = "DAMAGE";
		subtitle[1].text = "DURABILITY";
		subtitle[2].text = "AMMO";
	}

	public void UpdateEquippableDurability(float durability) {
		title[1].text = durability.ToString();
	}

	public void SetNoStats() {
		gameObject.SetActive(false);
	}
}
