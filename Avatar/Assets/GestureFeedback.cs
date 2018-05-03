using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GestureFeedback : MonoBehaviour {

	float gScore;

	// Use this for initialization
	void Start () {
		gScore = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
//		Rect r = gameObject.transform.GetChild (0).GetChild (0).GetComponent<RectTransform> ().rect;
//		float updateSize = r.width;
//		updateSize = gScore * 0.2f / 1.5f;
//		r.width = updateSize;
//		gameObject.transform.GetChild (0).GetChild (0).GetComponent<RectTransform> ().rect = new Rect(r);

		Vector3 updateSize = gameObject.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().localScale;
		updateSize.x = gScore *0.2f / 1.5f;
		gameObject.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().localScale = new Vector3 (updateSize.x, updateSize.y, updateSize.z);
	
	}


	public void UpdateGestureFeedback(float gestureScore){
		gScore = gestureScore;
	}
}
