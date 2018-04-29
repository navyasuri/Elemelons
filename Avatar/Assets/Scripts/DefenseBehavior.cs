using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

/*
 *  Script to be placed on the DefenseWall prefab.
 */

public class DefenseBehavior : Photon.MonoBehaviour {

	GameObject player;
	public int playerID; // Must be public for Photon, no need to provide a value.

	public AudioSource defenseWhoosh;
	float startTime;

	void Start() {
		startTime = Time.time; // Keep track of how long this defense has been alive.
	}

	// Called by DeveloperDefined gesture triggers and networked prefab instantiation:
	public void DoAfterStart(Vector3 direction) {
		// Orient the new object so particle effects display properly:
		Quaternion rotationForTrails = Quaternion.FromToRotation (Vector3.back, direction);
		transform.rotation = rotationForTrails;
	}

	void Update() {
		// Keep the shield alive for the length of its audio, then call network to destroy:
		StartCoroutine ("SelfDestruct", defenseWhoosh.clip.length);
	}

	// Behavior for when fireballs hit the shield:
	void OnTriggerEnter(Collider collision) {
		if (collision.gameObject.CompareTag("attack")) { // Check for attacking objects
			// Destroy (ON NETWORK) any colliding attacks that are not this player's:
			if (collision.gameObject.GetComponent<AllFireBehavior> ().playerID != playerID) {
				PhotonView.Get(collision.gameObject).RPC("NetworkDestroy", PhotonTargets.All);
			}
		}
	}

	IEnumerator SelfDestruct(float clipLength) {
		yield return new WaitForSeconds(clipLength);
		PhotonView.Get(this).RPC("NetworkDestroy", PhotonTargets.All);
	}

	[PunRPC]
	void NetworkDestroy() {
		if (GetComponent<PhotonView> ().isMine) {
			PhotonNetwork.Destroy (gameObject);
		}
	}

}
