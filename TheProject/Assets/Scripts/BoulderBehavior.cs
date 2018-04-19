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
		if (col.gameObject.tag != "Environment") {
			
			Destroy (this.gameObject);
		}

		rumbling.Play();

	}
}
