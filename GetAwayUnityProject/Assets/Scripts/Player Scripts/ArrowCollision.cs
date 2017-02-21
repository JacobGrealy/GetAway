using UnityEngine;
using System.Collections;

public class ArrowCollision : MonoBehaviour {
	public int arrowDamage=1;

	void OnCollisionEnter(Collision collision){
		//Debug.Log ("ArrowHit: "+collision.gameObject.transform.name);
		if (collision.transform.gameObject.GetComponent<EnemyHealth>()!=null){
		//Debug.Log ("EnemyHit!");
			collision.transform.GetComponent<EnemyHealth>().decrementHealth(arrowDamage, gameObject);//damage*damageScale);
			Score.damageDone+=arrowDamage;
			transform.gameObject.rigidbody.isKinematic = true;
			transform.Translate (Vector3.forward);
			transform.parent=collision.transform;
			transform.localPosition = collision.transform.InverseTransformPoint(collision.contacts[0].point);
			transform.gameObject.collider.enabled=false;
		}
		else{
			transform.gameObject.rigidbody.isKinematic = true;
			transform.parent=collision.transform;
			transform.localPosition = collision.transform.InverseTransformPoint(collision.contacts[0].point);
			transform.gameObject.collider.enabled=false;
			//Destroy (transform.gameObject);
		}
	//	Debug.Log (arrowDamage);
	}

	public void SetArrowDamage(int damage){
		arrowDamage=damage;
	}
}
