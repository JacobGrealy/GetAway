using UnityEngine;
using System.Collections;

public class SkyBoxController : MonoBehaviour {

	public DayNightController controller;
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		float sunRiseStartTime = controller.sunRiseStartTime;
		float sunRiseEndTime = controller.sunRiseEndTime;
		float sunSetStartTime = controller.sunSetStartTime;
		float sunSetEndTime = controller.sunSetEndTime;
		Color sunSetRed = controller.sunSetRed;
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
			ratio =1 - (timeIntoSunRise / sunRiseLength);		
		}
		else if(time > sunSetStartTime && time < sunSetEndTime)//sunset
		{
			float timeIntoSunSet = time - sunSetStartTime;
			ratio = timeIntoSunSet / sunSetLength;	
		}
		RenderSettings.skybox.SetFloat ("_Blend", ratio); 
	}
}
