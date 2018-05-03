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
		
	}


	public void UpdateGestureFeedback(float gestureScore){
		gScore = gestureScore;
	}
}
