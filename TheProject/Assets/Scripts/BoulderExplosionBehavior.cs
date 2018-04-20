using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Photon;

public class BoulderExplosionBehavior : MonoBehaviour {

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
		if (!this.gameObject.GetComponentInChildren<MeshRenderer> ().enabled) {
			explosion.Play();
			Destroy (this.gameObject, 1f);
		}
		Debug.Log (Time.time - startTime);
		//Debug.Log (explosion.audioClip.length);
//		if (Time.time - startTime > 2f) {
//			explosion.Stop();
//			Destroy (this.gameObject);
//		}
	}

}
