using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillStoneBehavior : MonoBehaviour {
	float startTime;
	// Use this for initialization
	void Start () {
		//start particle system
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
//		if (Time.time - startTime > 5)
//			GameObject.Find("SceneManager").GetComponent<SceneLoader>().goToScene("VRPunScene");
	}

	void OnCollisionEnter(Collision col){
		if (col.gameObject.tag == "player") {
			//start UI tutorial
			//when tutorial ends, stop particle system
			GameObject.Find("SceneManager").GetComponent<SceneLoader>().goToScene("VRPunScene");
		}
	}
}
