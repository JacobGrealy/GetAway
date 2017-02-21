using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour {
	public int health=100;
	public AudioSource enemyHit;
	private Animation animate;
	private bool animating = true;
	private bool dead = false;
	public int numDrops=1;
	public GameObject[] drops;
	void Start (){
		animate = GetComponent<Animation>();
	}

	public void decrementHealth(int damage, GameObject attacker){
			if (enemyHit != null) enemyHit.Play();
			health=health-damage;
			if (health<=0 && !dead){
				dead=true;
				int i=0;
				while (i<numDrops){
					int itemDropped = Random.Range(0,drops.Length);
//				Debug.Log (itemDropped);
					GameObject newDrop=(GameObject)Instantiate(drops[itemDropped],transform.position,transform.rotation);
					newDrop.name=drops[itemDropped].name;
					i++;
				}
			}
		if(GetComponent<Retaliate>()&&!dead){
			GetComponent<Retaliate>().shouldRetaliate = true;
			GetComponent<Retaliate>().setAttacker(attacker);
		}
	}

	public int getHealth(){
		return health;
	}

	private IEnumerable waitDeathAnimation(){
		do {
			yield return null;
			} while (animate.isPlaying);
	}

	public bool isDead(){
		return dead;
	}
}
