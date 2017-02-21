using UnityEngine;
using System.Collections;

public class PlayerCrosshair : MonoBehaviour {
	public Texture2D crosshairImage;
	private bool displayText = false;
	private RaycastHit hit = new RaycastHit();
	private Ray ray = new Ray();
	private GameObject pickUpText;
	private TextMesh textMeshComponent;
	private Font arialFont;
	private Vector3 textPosition;
	private Vector3 textRotation;
	private Vector3 textScale;
	private Transform player;

	// Use this for initialization
	void Start () {
		pickUpText = new GameObject("PickUpText");
		textMeshComponent = pickUpText.AddComponent<TextMesh>();
		arialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
		textRotation = new Vector3(0.0f, 180.0f, 0.0f);;
		textScale = new Vector3(0.08f, 0.08f, 0.08f);
		player = transform;
	}
	
	// Update is called once per frame
	void Update () {
//		RaycastHit myHit = new RaycastHit();
//		Ray myRay = new Ray();

		//myRay = Camera.main.ViewportPointToRay(transform.position);
		ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2));
		if ((Physics.Raycast(ray.origin, ray.direction, out hit, 3.0f)) && hit.collider.gameObject.CompareTag("PickUp")) {
			displayText = true;

		}
		else {
			displayText = false;
		}
		pickUpText.transform.rotation = Quaternion.LookRotation (pickUpText.transform.position - player.position);
	}

	void OnGUI () {
		float xMin = (Screen.width / 2) - (crosshairImage.width / 2);
		float yMin = (Screen.height / 2) - (crosshairImage.height / 2);
		//Follow Mouse
		//float xMin = Screen.width - (Screen.width - Input.mousePosition.x) - (crosshairImage.width / 2);
		//float yMin = (Screen.height - Input.mousePosition.y) - (crosshairImage.height / 2);
		GUI.DrawTexture(new Rect(xMin, yMin, crosshairImage.width, crosshairImage.height), crosshairImage);

		if (displayText) {
			//int width = 180;
			//string pickUpText = "Press e to Pick Up " + hit.collider.gameObject.name;
			//GUI.Box(new Rect((Screen.width/2 - width/2), (Screen.height/2 - 100), width, 25), pickUpText);

			textPosition = new Vector3(hit.collider.gameObject.transform.position.x + 0.75f, hit.collider.gameObject.transform.position.y + 0.6f, hit.collider.gameObject.transform.position.z);
			textMeshComponent.text = "Press e to Pick Up " + hit.collider.gameObject.name;
			pickUpText.renderer.material = arialFont.material;
			pickUpText.transform.position = textPosition;
			pickUpText.transform.rotation = Quaternion.Euler(textRotation);
			//pickUpText.transform.LookAt(Camera.main.transform);
			pickUpText.transform.localScale = textScale;
			pickUpText.SetActive(true);
		}
		if (!displayText) {
			pickUpText.SetActive(false);
		}
	}
}
