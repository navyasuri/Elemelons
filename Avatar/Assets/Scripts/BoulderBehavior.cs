using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon;

public class BoulderBehavior : Photon.MonoBehaviour {

	//public AudioSource rumbling;
	AudioSource rumbling;
	public float lowPitch;
	public float highPitch;
	public GameObject explosionPrefab;
	float startTime;
	public bool isLive = false;


	// Use this for initialization
	void Start () {
//		this.gameObject.GetComponent<MeshRenderer> ().material.color = Color.red;
		rumbling = GetComponent<AudioSource>();
		rumbling.time = 0f;
		float randomScale = Random.Range (0.01f, 0.03f);
		gameObject.transform.localScale += new Vector3 (randomScale, randomScale, randomScale);
		float startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
//		if (rumbling.time > 4.0f) {
//			rumbling.Stop ();
//			rumbling.time = 3f;
//		}

		if (gameObject.transform.position.y < -10f) {
			isLive = false;
			Destroy (this.gameObject);
		}
	}

	void OnCollisionEnter(Collision col){
		//float randomPitch = Random.Range (lowPitch, highPitch);
		//rumbling.pitch = randomPitch;
		//rumbling.time = 0f;
		if (!rumbling.isPlaying) {
			rumbling.Play ();
		}
		Debug.Log ("Boulder hit " + col.gameObject);
		// If the boulder hits anything not tagged 'environment':
		if (!col.gameObject.CompareTag("Environment")) {
			PhotonNetwork.Instantiate(explosionPrefab.name, transform.position, Quaternion.identity, 0);
			isLive = false;
			Destroy (this.gameObject);

			//SelfDestruct();
		}
	}

	public void SelfDestruct() {
		// get parent, play audio source
		//PhotonNetwork.Instantiate(explosionPrefab.name, transform.position, Quaternion.identity, 0);
		GameObject.Instantiate(explosionPrefab, transform.position, Quaternion.identity);
		isLive = false;
		Destroy (this.gameObject);
	}

	public void OnPhotonSerializedView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			stream.SendNext (isLive);
		} else {
			this.isLive = (bool)stream.ReceiveNext ();
		}
	}

}
