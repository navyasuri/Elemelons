using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusUpdate : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void UpdateStatus(string status){
		gameObject.transform.GetChild (0).GetChild (0).gameObject.GetComponent<Text> ().text = status;
		StartCoroutine (removeStatusUpdate());
	}

	IEnumerator removeStatusUpdate(){
		gameObject.SetActive (true);
		yield return new WaitForSeconds (2.5f);
		gameObject.SetActive (false);
	}
}
