using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderBehavior : MonoBehaviour {

	private AudioSource audio;
	// Use this for initialization
	void Start () {
//		this.gameObject.GetComponent<MeshRenderer> ().material.color = Color.red;
		audio = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision col){
		// Subtract health from player on impact:
		if (col.gameObject.CompareTag("Player")) {
			col.gameObject.GetComponent<PlayerBehavior> ().health -= 1;
			Debug.Log ("Player hit! Health remaining: " + col.gameObject.GetComponent<PlayerBehavior> ().health);
		}
		// If the boulder hits anything not tagged 'environment':
		if (!col.gameObject.CompareTag("Environment")) {
			Destroy (this.gameObject);
		}
	}
}
