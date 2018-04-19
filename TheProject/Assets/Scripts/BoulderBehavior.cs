using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderBehavior : MonoBehaviour {

	//public AudioSource rumbling;
	AudioSource rumbling;
	// Use this for initialization
	void Start () {
//		this.gameObject.GetComponent<MeshRenderer> ().material.color = Color.red;
		rumbling = GetComponent<AudioSource>();
		rumbling.time = 3f;
	}
	
	// Update is called once per frame
	void Update () {

		if (rumbling.time > 4.0f) {
			rumbling.Stop ();
			rumbling.time = 3f;
		}
		
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

		rumbling.Play();

	}
}
