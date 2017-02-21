using UnityEngine;
using System.Collections;

public class ArrowPhysics : MonoBehaviour
{
	public float gravity = 300f;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(rigidbody.velocity.sqrMagnitude != 0)
			transform.rotation = Quaternion.LookRotation(rigidbody.velocity);
		rigidbody.AddForce (Vector3.down * Time.deltaTime * gravity);
	}
}
