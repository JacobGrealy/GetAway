using UnityEngine;
using System.Collections;
using System.IO;
using SimpleJSON;

public class CreditsController : MonoBehaviour 
{
	// TODO: Change spacing between credits stuff to factor in screen height.

	public GUIText creditsHeader;
	public GUIText creditsSubHeader;
	public GUIText creditsItem;

	private bool scrollFaster = false;
	private float distanceScrolled = 0.0f;
	private float endDistance;

	private const float CREDITS_STARTING_POSITION = 0.0f;
	private const float CREDITS_SUBHEADER_SPACING = 0.1f;
	private const float CREDITS_ITEMS_SPACING = 0.1f;
	private const float CREDITS_ITEM_SPACING = 0.08f;
	private const float CREDITS_GROUP_SPACING = 0.28f;
	private const float SCROLL_SPEED = 0.1f;
	private const float SCROLL_SPEED_FAST = 0.3f;
	private const float END_CREDITS_SCROLL_DISTANCE_PADDING = 0.7f;

	private const string relativeJsonPath = "credits.json";
	
	// Use this for initialization
	void Start () {
		string jsonPath = Path.Combine(Application.streamingAssetsPath, relativeJsonPath);
		string creditsJson = getCreditsFile(jsonPath);
		var credits = JSON.Parse(creditsJson);

		int i = 0;
		Vector3 position = new Vector3(0.5f, CREDITS_STARTING_POSITION, 0.0f);
		while (credits[i] != null) {
			if (credits[i]["header"] != null) {
				CreateHeader(credits[i]["header"], position);
			}

			if (credits[i]["sub-header"] != null) {
				position -= new Vector3(0.0f, CREDITS_SUBHEADER_SPACING, 0.0f);
				CreateSubHeader(credits[i]["sub-header"], position);
			}

			if (credits[i]["items"] != null) {
				position -= new Vector3(0.0f, CREDITS_ITEMS_SPACING, 0.0f);
				int item = 0;
				while (credits[i]["items"][item] != null) {
					CreateItem (credits[i]["items"][item], position);
					position -= new Vector3(0.0f, CREDITS_ITEM_SPACING, 0.0f);
					item++;
				}
			}

			position -= new Vector3(0.0f, CREDITS_GROUP_SPACING, 0.0f);
			i++;
		}

		endDistance = CREDITS_STARTING_POSITION - position.y + END_CREDITS_SCROLL_DISTANCE_PADDING;
	}

	string getCreditsFile(string path) {
		TextReader tr = new StreamReader(path);
		
		string line;
		string json = "";
		while((line = tr.ReadLine()) != null)
		{
			json += line;
		}
		// Close the stream
		tr.Close();

		return json;
	}

	void CreateHeader(string text, Vector3 position) {
		GUIText newCredit = (GUIText) Instantiate(creditsHeader, position, Quaternion.Euler(new Vector3(0, 0, 0)));
		newCredit.transform.parent = this.gameObject.transform;
		newCredit.text = text;
	}
	
	void CreateSubHeader(string text, Vector3 position) {
		GUIText newCredit = (GUIText) Instantiate(creditsSubHeader, position, Quaternion.Euler(new Vector3(0, 0, 0)));
		newCredit.transform.parent = this.gameObject.transform;
		newCredit.text = text;
	}

	void CreateItem(string text, Vector3 position) {
		GUIText newCredit = (GUIText) Instantiate(creditsItem, position, Quaternion.Euler(new Vector3(0, 0, 0)));
		newCredit.transform.parent = this.gameObject.transform;
		newCredit.text = text;
	}

	void Update() {
		if (Input.GetKey(KeyCode.Space)) {
			scrollFaster = true;
		} else {
			scrollFaster = false;
		}

		if (scrollFaster) {
			distanceScrolled += SCROLL_SPEED_FAST * Time.deltaTime;
			this.transform.Translate(0.0f, SCROLL_SPEED_FAST * Time.deltaTime, 0.0f, Space.Self);
		} else {
			distanceScrolled += SCROLL_SPEED * Time.deltaTime;
			this.transform.Translate(0.0f, SCROLL_SPEED * Time.deltaTime, 0.0f, Space.Self);
		}

		if (distanceScrolled > endDistance) {
			Application.LoadLevel("Start Menu");
		}
	}
}