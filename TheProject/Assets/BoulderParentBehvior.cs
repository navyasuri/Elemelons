using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderParentBehvior : MonoBehaviour {

	AudioSource explosion;
	// Use this for initialization
	void Start () {
		explosion = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!this.gameObject.GetComponentInChildren<MeshRenderer> ().enabled) {
			explosion.Play();
			Destroy (this.gameObject, 1f);
		}
	}
}
