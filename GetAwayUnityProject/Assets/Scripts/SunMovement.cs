using UnityEngine;
using System.Collections;

public class SunMovement : MonoBehaviour {
	public DayNightController controller;
	public Flare sunFlare;
	public GameObject sunTexture;
	public GameObject sunLight;
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
		Color daySunColor = controller.daySunColor;
		float time = controller.GetTime();

		float sunRiseMiddle = (sunRiseEndTime-sunRiseStartTime)/2f;
		float sunSetMiddle = (sunSetEndTime-sunSetStartTime)/2f;

		//set positions of sun texture and sun light, now handled seperately
		//sun texture
		sunTexture.transform.position = controller.getSunTexturePosition(); //move according to time of day
		sunTexture.transform.LookAt (controller.transform); sunTexture.transform.Rotate (new Vector3(90,0,0)); //direct light at center
		//sunlight
		sunLight.transform.position = controller.getSunLightPosition(); //move according to time of day
		sunLight.transform.LookAt (controller.transform);//direct light at center

		//turn off flare until sun is above horizon
		if (time <sunRiseStartTime + sunRiseMiddle || time > sunSetStartTime + sunSetMiddle)
			sunLight.light.flare = null;
		else 
		{
			sunLight.light.flare = sunFlare;		
		}


		//change color based on time of day
		Color temp = Color.black;
		if (time >= sunRiseEndTime && time <= sunSetStartTime)//day time
						temp = daySunColor;
		else if(time >= sunSetEndTime || time <= sunRiseStartTime)//night time
				temp = Color.black;
		else if(time > sunRiseStartTime && time < sunRiseEndTime)//sunrise
		{
			float timeIntoSunRise = time - sunRiseStartTime;
			float colorRatio = Mathf.Abs((timeIntoSunRise - sunRiseMiddle)/sunRiseMiddle);
			if(timeIntoSunRise < sunRiseMiddle) //blending black to red
				temp = colorRatio * Color.black  + (1-colorRatio)* sunSetRed;
			else //blending red to white
				temp = colorRatio * daySunColor  + (1-colorRatio)* sunSetRed;	
		}
		else if(time > sunSetStartTime && time < sunSetEndTime)
		{
			float timeIntoSunSet = time - sunSetStartTime;
			float colorRatio = Mathf.Abs((timeIntoSunSet - sunSetMiddle)/sunSetMiddle);
			if(timeIntoSunSet < sunSetMiddle) //blending black to red
				temp = colorRatio * daySunColor  + (1-colorRatio)*sunSetRed;
			else //blending red to white
				temp = colorRatio * Color.black  + (1-colorRatio)*sunSetRed;		
		}
		sunLight.light.color = temp;
		sunTexture.renderer.material.SetColor ("_TintColor", temp);
	}
}
