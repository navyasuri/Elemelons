using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon;

public class BoulderBehavior : Photon.MonoBehaviour {

	//public AudioSource rumbling;
	public AudioSource rumbling;
	public AudioSource explode;
	public float lowPitch = 0.45f;
	public float highPitch = 0.85f;
	float randomPitch;

	// Set audio starts to 0, apply a random scale and pitch to this boulder:
	void Start () {
		rumbling.time = 0f;
		explode.time = 0f;
		float randomScale = Random.Range (0.01f, 0.03f); // Scale the size of the boulder.
		randomPitch = Map (0.01f, 0.03f, highPitch, lowPitch, randomScale); // Set the pitch based on the size.
		gameObject.transform.localScale += new Vector3 (randomScale, randomScale, randomScale);
	}
	
	void Update () {
		// If the boulder falls off the map, destroy it silently:
		if (gameObject.transform.position.y < -20f) {
			// Call the NetworkDestroy RPC via the PhotonView component to destroy ON NETWORK:
			PhotonView.Get(this).RPC("NetworkDestroy", PhotonTargets.All);
		}
	}

	// Listen for collisions:
	void OnCollisionEnter(Collision col){
		// Rumble on each collision if silent:
		if (!rumbling.isPlaying) {
			rumbling.pitch = randomPitch;
			rumbling.Play ();
		}
		// If the boulder hits anything not tagged 'environment':
		if (!col.gameObject.CompareTag("Environment")) {
			PlayExplosion ();
		}
	}

	// Trigger explosion audio, "hide" the main boulder, call network to destroy once audio is done:
	public void PlayExplosion() {
		// If the clip is not playing (this is SelfDestruct's first call), play it,
		// turn off the Renderer/Collider, and turn on the explosion particle effect:
		if (!explode.isPlaying) {
			gameObject.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeAll;
			gameObject.GetComponent<MeshRenderer> ().enabled = false;
			gameObject.GetComponent<SphereCollider> ().enabled = false;
			//gameObject.GetComponent<ParticleSystem> ().IsAlive = true;
			explode.pitch = randomPitch;
			explode.Play ();
		}

		StartCoroutine ("SelfDestruct", explode.clip.length);
		// The following is a BAD way to handle it, since PlayExplosion() is only called once, on collision.
//		timeSinceDestruct += Time.deltaTime;
//		if (timeSinceDestruct > explode.clip.length) {
//			Debug.Log ("Calling network destroy");
//			PhotonView.Get(this).RPC("NetworkDestroy", PhotonTargets.All);
//		}
	}

	IEnumerator SelfDestruct(float clipLength) {
		yield return new WaitForSeconds(clipLength);
		PhotonView.Get(this).RPC("NetworkDestroy", PhotonTargets.All);
	}
		
	// Remote Procedure Calls happen indirectly on all network clients as follows:
	//PhotonView.Get(this).RPC("NetworkDestroy", PhotonTargets.All);
	[PunRPC]
	void NetworkDestroy() {
		// Call destroy once on the master client, the rest will be updated.
		// This eliminates duplicate calls:
		if (PhotonNetwork.isMasterClient) {
			PhotonNetwork.Destroy (gameObject);
		}
	}

	public float Map (float oldMin, float oldMax, float newMin, float newMax, float val){
		float oldRange = (oldMax - oldMin);
		float newRange = (newMax - newMin);
		float newVal = (((val - oldMin) * newRange) / oldRange) + newMin;

		return(newVal);
	}

}
