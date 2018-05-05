using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GestureFeedback : MonoBehaviour {

	public float gScore;

	// Use this for initialization
	void Start () {
		StartCoroutine ("ResetScore");
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void UpdateGestureFeedback(float gestureScore){
		gScore = gestureScore;
		// Update the grey gesture feedback bar if the score was low:
		if (gScore < 0.85) {
			// Set the blue bar to 0, in case it was already blue:
			Vector3 updateSize = gameObject.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().localScale;
			gameObject.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().localScale = new Vector3 (0, updateSize.y, updateSize.z);
			// Then update the grey bar:
			updateSize = gameObject.transform.GetChild(0).GetChild(1).GetComponent<RectTransform>().localScale;
			updateSize.x = gScore / 1.35f;
			gameObject.transform.GetChild(0).GetChild(1).GetComponent<RectTransform>().localScale = new Vector3 (updateSize.x, updateSize.y, updateSize.z);
		}
		// Update the blue gesture bar if the score was high enough to activate the gesture:
		if (gScore >= 0.85) {
			Vector3 updateSize = gameObject.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().localScale;
			updateSize.x = gScore / 1.35f;
			gameObject.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().localScale = new Vector3 (updateSize.x, updateSize.y, updateSize.z);
		}
		StopCoroutine ("ResetScore");
		StartCoroutine ("ResetScore");
	}

	IEnumerator ResetScore() {
		yield return new WaitForSeconds (1.5f);
		Vector3 updateSize = gameObject.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().localScale;
		gameObject.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().localScale = new Vector3 (0, updateSize.y, updateSize.z);
		updateSize = gameObject.transform.GetChild(0).GetChild(1).GetComponent<RectTransform>().localScale;
		gameObject.transform.GetChild(0).GetChild(1).GetComponent<RectTransform>().localScale = new Vector3 (0, updateSize.y, updateSize.z);
	}



}
