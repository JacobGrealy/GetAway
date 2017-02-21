using UnityEngine;
using System.Collections;

public class StatsController : MonoBehaviour {

	// The watch component of the stats screen.
	public WatchController watch;

	// The stat bar background.
	public GameObject background;

	// The player health object to get stat info.
	public HealthController playerHealth;

	// The stat bar components.
	public StatController healthBar;
	public StatController sanityBar;
	public StatController hungerBar;
	public StatController thirstBar;

	// The days the player has survived
	public GUISkin gSkin;
	private bool guiBool;

	void Start () {
		SetActive(false);
	}

	public void SetActive(bool active) {
		watch.gameObject.SetActive(active);
		background.gameObject.SetActive(active);
		healthBar.gameObject.SetActive(active);
		sanityBar.gameObject.SetActive(active);
		hungerBar.gameObject.SetActive(active);
		thirstBar.gameObject.SetActive(active);
		guiBool=active;
	}

	void Update() {
		healthBar.SetPercentageFilled((float) playerHealth.GetHealth() / (float) playerHealth.GetMaxHealth());
		sanityBar.SetPercentageFilled((float) playerHealth.GetMental() / (float) playerHealth.GetMaxMental());
		hungerBar.SetPercentageFilled((float) playerHealth.GetHunger() / (float) playerHealth.GetMaxHunger());
		thirstBar.SetPercentageFilled((float) playerHealth.GetThirst() / (float) playerHealth.GetMaxThirst());

	}

	void OnGUI(){
		if (guiBool){
		GUI.skin=gSkin;
		GUI.Label(new Rect((Screen.width/2)-200, (Screen.height/2), 400, 70), "Days Survived:"+Score.daysSurvived,"Days");
		} //(Screen.height/2)+200
	}
}
