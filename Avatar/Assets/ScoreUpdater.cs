using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUpdater : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void UpdatePlayerScore(float score){
		string newScore = "Score: " + score;
		gameObject.transform.GetChild (0).GetChild (0).gameObject.GetComponent<Text> ().text = newScore;
	}
}
