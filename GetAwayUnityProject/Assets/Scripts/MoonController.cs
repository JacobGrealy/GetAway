using UnityEngine;
using System.Collections;

public class MoonController : MonoBehaviour {
	public DayNightController controller;
	public GameObject moonTexture;
	public GameObject moonLight;
	public GameObject manInMoon;
	public float manInMoonStartTime;
	public float manInMoonEndTime;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		moonTexture.transform.position = controller.getMoonTexturePosition(); //move according to time of day
		moonTexture.transform.LookAt (controller.transform); //direct light at center
		moonLight.transform.position = controller.getMoonLightPosition(); //move according to time of day
		moonLight.transform.LookAt (controller.transform); //direct light at center

		//Fade in
		//renderer.material.SetColor ("_TintColor", new Color(84f/255f,84f/255f,84f/255f));
		//fade out

		float sunRiseStartTime = controller.sunRiseStartTime;
		float sunRiseEndTime = controller.sunRiseEndTime;
		float sunSetStartTime = controller.sunSetStartTime;
		float sunSetEndTime = controller.sunSetEndTime;	
		Color moonColor = controller.moonColor;
		float time = controller.GetTime();

		float sunRiseLength = (sunRiseEndTime-sunRiseStartTime);
		float sunSetLength = (sunSetEndTime-sunSetStartTime);
		float ratio = 0f;

		if (time >= sunRiseEndTime && time <= sunSetStartTime)//day time
			ratio = 0f;
		else if(time >= sunSetEndTime || time <= sunRiseStartTime)//night time
			ratio = 1f;
		else if(time > sunRiseStartTime && time < sunRiseEndTime)//sunrise
		{
			float timeIntoSunRise = time - sunRiseStartTime;
			ratio =Mathf.Pow(1 - ((timeIntoSunRise / sunRiseLength)),10);		
		}
		else if(time > sunSetStartTime && time < sunSetEndTime)//sunset
		{
			float timeIntoSunSet = time - sunSetStartTime;
			ratio = Mathf.Pow((timeIntoSunSet / sunSetLength),10);	
		}
		moonTexture.renderer.material.SetColor ("_TintColor", ratio * moonColor);

		//man in moon
		if(time >= manInMoonStartTime && time <= manInMoonEndTime)
			manInMoon.SetActive(true);
		else
			manInMoon.SetActive(false);
	}
}
