using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class BoulderExplosionBehavior : Photon.MonoBehaviour {

	AudioSource explosion;
	float startTime = 0;
	public bool isLive = false;


	// Use this for initialization
	void Start () {
		float startTime = Time.time;
		explosion = GetComponent<AudioSource>();
		explosion.time = 0f;
		explosion.Play ();
	}
	
	// Update is called once per frame
	void Update () {
		float currentTime = Time.time;
		if (currentTime - startTime > explosion.clip.length) {
			isLive = false;
			Destroy (gameObject);
		}
	}

	public void OnPhotonSerializedView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			stream.SendNext (isLive);
		} else {
			this.isLive = (bool)stream.ReceiveNext ();
		}
	}

}
