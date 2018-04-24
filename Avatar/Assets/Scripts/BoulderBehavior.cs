using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon;

public class BoulderBehavior : Photon.MonoBehaviour {

	//public AudioSource rumbling;
	public AudioSource rumbling;
	public AudioSource explode;
	float lowPitch = 0.45f;
	float highPitch = 0.85f;
	float randomPitch;
	//public GameObject explosionPrefab;
	float timeSinceDestruct;
	public bool isLive = true;

	// Set audio starts to 0, apply a random scale and pitch to this boulder:
	void Start () {
		rumbling.time = 0f;
		explode.time = 0f;
		float randomScale = Random.Range (0.01f, 0.03f);
		gameObject.transform.localScale += new Vector3 (randomScale, randomScale, randomScale);
		randomPitch = Random.Range (lowPitch, highPitch);
	}
	
	void Update () {
		// If the boulder has died, process the explosion:
		if (!isLive) {
			SelfDestruct ();
		}

		// If the boulder falls off the map, destroy it silently:
		if (gameObject.transform.position.y < -10f) {
			isLive = false;
			// Call the NetworkDestroy RPC via the PhotonView component to destroy ON NETWORK:
			PhotonView.Get(this).RPC("NetworkDestroy", PhotonTargets.All);
		}
	}

	void OnCollisionEnter(Collision col){
		// Rumble on each collision if silent:
		if (!rumbling.isPlaying) {
			rumbling.pitch = randomPitch;
			Debug.Log ("Rumbling!");
			rumbling.Play ();
		}

		//Debug.Log ("Boulder hit " + col.gameObject);
		// If the boulder hits anything not tagged 'environment':
		if (!col.gameObject.CompareTag("Environment")) {
			isLive = false;
			SelfDestruct();
		}
	}

	public void SelfDestruct() {
		// If the clip is not playing (this is SelfDestruct's first call), play it,
		// turn off the Renderer/Collider, and turn on the explosion particle effect:
		if (!explode.isPlaying) {
			gameObject.GetComponent<MeshRenderer> ().enabled = false;
			gameObject.GetComponent<SphereCollider> ().enabled = false;
			//gameObject.GetComponent<ParticleSystem> ().IsAlive = true;
			explode.pitch = randomPitch;
			Debug.Log ("Exploding!");
			explode.Play ();
		}

		// If the explosion clip has finished playing, destroy the boulder prefab:
		Debug.Log(timeSinceDestruct);
		timeSinceDestruct += Time.deltaTime;
		if (timeSinceDestruct > explode.clip.length) {
			Debug.Log ("Calling network destroy");
			PhotonView.Get(this).RPC("NetworkDestroy", PhotonTargets.All);
		}

	}
		
	// Remote Procedure Calls happen indirectly on all network clients as follows:
	//PhotonView.Get(this).RPC("NetworkDestroy", PhotonTargets.All);
	[PunRPC]
	void NetworkDestroy() {
		// Only destroy on one client, the rest will be updated.
		// This eliminates duplicate calls:
		if (PhotonNetwork.isMasterClient) {
			PhotonNetwork.Destroy (gameObject);
		}
	}

	// Communicate the boulder over the network:
//	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
//		if (stream.isWriting) {
//			stream.SendNext (isLive);
//		} else {
//			this.isLive = (bool)stream.ReceiveNext ();
//		}
//	}

}
