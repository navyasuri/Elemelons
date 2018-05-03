using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillStoneBehavior : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//start particle system
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision col){
		if (col.gameObject.tag == "player") {
			//start UI tutorial
			//when tutorial ends, stop particle system
		}
	}
}
