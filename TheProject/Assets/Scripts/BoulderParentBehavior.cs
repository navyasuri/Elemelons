using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderParentBehavior : MonoBehaviour {

	AudioSource explosion;
	float startTime;

	// Use this for initialization
	void Start () {
		explosion = GetComponent<AudioSource>();
		explosion.Play ();
		float startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
//		if (!this.gameObject.GetComponentInChildren<MeshRenderer> ().enabled) {
//			explosion.Play();
//			Destroy (this.gameObject, 1f);
//		}
		if (Time.time - startTime > 2) {
			explosion.Stop();
			Destroy (this.gameObject);
		}
	}

}
