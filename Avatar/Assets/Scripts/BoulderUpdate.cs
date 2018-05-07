using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoulderUpdate : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void UpdateBoulderStatus(int count){
		string s = "Boulders: " + count;
		gameObject.transform.GetChild (0).GetChild (0).gameObject.GetComponent<Text> ().text = s;

	}
}
