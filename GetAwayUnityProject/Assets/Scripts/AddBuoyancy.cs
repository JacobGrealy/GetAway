using UnityEngine;
using System.Collections;

public class AddBuoyancy : MonoBehaviour 
{
	Ocean m_ocean;
	
	public float m_spread = 1.0f;
	public float m_offset = 0.0f;
	public float m_tilt = 20.0f;

	private GameObject playerWalk;
	private PlayerWalkSound walk;
	
	void Start() 
	{
		GameObject ocean = GameObject.Find("Ocean");
		
		if(ocean == null)
		{
			Debug.Log("AddBuoyancy::Start - Could not find ocean game object");	
			return;
		}
		
		m_ocean = ocean.GetComponent<Ocean>();
		
		if(m_ocean == null)
			Debug.Log("AddBuoyancy::Start - Could not find ocean script");

		playerWalk = GameObject.FindGameObjectWithTag("FPS Controller");
		walk = playerWalk.GetComponent<PlayerWalkSound>();
	
	}
	
	void LateUpdate() 
	{
		if(m_ocean)
		{
			Vector3 pos = transform.position;

			float ht0 = m_ocean.SampleHeight(pos + new Vector3(m_spread,0,0));
			float ht1 = m_ocean.SampleHeight(pos + new Vector3(-m_spread,0,0));
			float ht2 = m_ocean.SampleHeight(pos + new Vector3(0,0,m_spread));
			float ht3 = m_ocean.SampleHeight(pos + new Vector3(0,0,-m_spread));
			
			pos.y = (ht0+ht1+ht2+ht3)/4.0f + m_offset;

			float dx = ht0 - ht1;
			float dz = ht2 - ht3;

			if(transform.position.y <= pos.y)
			{
				transform.position = new Vector3(pos.x,2f*pos.y,pos.z);	
				//transform.localEulerAngles = new Vector3(-dz*m_tilt,0,dx*m_tilt);

				walk.walk.Stop();
			}
		}
	}
}
