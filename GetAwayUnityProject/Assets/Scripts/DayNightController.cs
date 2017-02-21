using UnityEngine;
using System.Collections;

public class DayNightController : MonoBehaviour {
	public float sunRadius = 5000; // the radius of the sun to it's rotation point
	public float dayLength = 180; //length of the day in seconds
	public float startTime = 12f; //start time in 24 hour time
	public float sunRiseStartTime = 5f;
	public float sunRiseEndTime = 9f;
	public float sunSetStartTime = 15f;
	public float sunSetEndTime = 19f;
	public Color daySunColor = new Color (135f / 255f, 135f / 255f, 83f / 255f);
	public Color sunSetRed = new Color (218f / 255f, 91f / 255f, 62f / 255f);
	public Color moonColor = new Color (57f / 255f, 57f / 255f, 57f / 255f);
	public Color dayFog = new Color (57f / 255f, 57f / 255f, 57f / 255f);
	public Color nightFog = new Color (0f / 255f, 0f / 255f, 0f / 255f);

	private float timeOfDay;
	private bool paused = false;

	// Use this for initialization
	void Start () 
	{
		timeOfDay = startTime/24f * dayLength;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!paused)
		{
			timeOfDay += Time.deltaTime; //advance the day so many second
			if (timeOfDay > dayLength) 
			{
				timeOfDay = 0;
				Score.daysSurvived++;
			}
			//change fog color
			float nightFogRatio = Mathf.Clamp((Mathf.Abs(12f-GetTime())),0,7f)/7f;
			float dayFogRatio = 1f - nightFogRatio;
			RenderSettings.fogColor = dayFog * dayFogRatio + nightFog * nightFogRatio;
		}
	}

	public void SetTime(float hour)
	{
		if (hour < 0){
			hour = 0;
			Score.daysSurvived++;
		}
		else if (hour>24){
			hour = hour - 24.0f * Mathf.Floor (hour/24.0f);
			Score.daysSurvived++;
		}
		timeOfDay = hour / 24f * dayLength;
	}

	public float GetTime()
	{
		return ((timeOfDay / dayLength) * 24);
	}
	
	public void Pause()
	{
		paused = true;
	}

	public void UnPause()
	{
		paused = false;
	}

	public Vector3 getSunTexturePosition()
	{
		float x = transform.position.x + sunRadius * Mathf.Sin(Mathf.Deg2Rad * (timeOfDay/dayLength)*(360));
		float y = transform.position.y + sunRadius * -Mathf.Cos(Mathf.Deg2Rad * (timeOfDay/dayLength)*(360));		
		float z = transform.position.z;
		return new Vector3(x,y,z);
	}

	public Vector3 getSunLightPosition()
	{
		float x = transform.position.x + sunRadius * Mathf.Sin(Mathf.Deg2Rad * ((timeOfDay/dayLength)*(90)+135));
		float y = transform.position.y + sunRadius * -Mathf.Cos(Mathf.Deg2Rad * ((timeOfDay/dayLength)*(90)+135));	
		float z = transform.position.z;
		return new Vector3(x,y,z);
	}
	
	public Vector3 getMoonTexturePosition()
	{
		float degree = (timeOfDay / dayLength) * (360f) + 180f; if (degree > 360f) degree = degree - 360f;
		float x = transform.position.x + sunRadius * Mathf.Sin(Mathf.Deg2Rad * degree);
		float y = transform.position.y + sunRadius * -Mathf.Cos(Mathf.Deg2Rad * degree);		
		float z = transform.position.z;
		return new Vector3(x,y,z);
	}

	public Vector3 getMoonLightPosition()
	{
		float degree = (timeOfDay/dayLength)*(90f) +45f; if (degree > 90f) degree = degree - 90f; degree += 135f; 
		float x = transform.position.x + sunRadius * Mathf.Sin(Mathf.Deg2Rad * degree);
		float y = transform.position.y + sunRadius * -Mathf.Cos(Mathf.Deg2Rad * degree);	
		float z = transform.position.z;
		return new Vector3(x,y,z);
	}

}
