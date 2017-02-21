using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemEventsController : MonoBehaviour {

	public GameObject itemEvent;

	private float notificationYOffset = 0.05908203f;
	private Queue<ItemEvent> itemEvents = new Queue<ItemEvent>();

	private float timeToPersistEvent = 3.0f;
	private float timeToExpireEvent = 0.0f;

	public void AddEvent(int itemId, string text) {
		GameObject iego = (GameObject) Instantiate (itemEvent);
		iego.transform.parent = transform;

		ItemEvent ie = iego.GetComponent<ItemEvent>();
		ie.transform.Translate(0.0f, notificationYOffset * itemEvents.Count, 0.0f);
		itemEvents.Enqueue(ie);

		if (timeToExpireEvent <= 0) {
			timeToExpireEvent = timeToPersistEvent;
		}
	}

	void RemoveEvent() {
		ItemEvent eventToRemove = itemEvents.Peek();
		itemEvents.Dequeue();
		Destroy (eventToRemove.gameObject);

		foreach (ItemEvent ie in itemEvents) {
			ie.AnimateDown(notificationYOffset, 1.0f);
		}
	}

	// Update is called once per frame
	void Update () {
		if (itemEvents.Count > 0 && timeToExpireEvent <= 0) {
			RemoveEvent();
			timeToExpireEvent = timeToPersistEvent;
		}

		timeToExpireEvent -= Time.deltaTime;

		if (Input.GetKey (KeyCode.O)) {
			AddEvent (1, "Heyoooo");
		}
	}
}
