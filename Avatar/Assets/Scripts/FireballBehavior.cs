using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class FireballBehavior : Photon.MonoBehaviour {

	GameObject player;
	public int playerID; // Must be public for Photon, no need to provide a value.

	public AudioSource fireballWhoosh;
	public AudioSource fireballImpact;
	public ParticleSystem fireballPoof;
	public ParticleSystem fireballParticles;
	public float lowPitch = 0.45f;
	public float highPitch = 0.85f;
	float startTime;

	void Start() {
		startTime = Time.time; // Keep track of how long the fireball has been alive.
	}

	// Called by DeveloperDefined gesture triggers and networked prefab instantiation:
	public void DoAfterStart(Vector3 direction) {
		// Orient the new object so particle effects display properly:
		Quaternion rotationForTrails = Quaternion.FromToRotation (Vector3.back, direction);
		transform.rotation = rotationForTrails;
		// Send attacks flying!
		gameObject.GetComponent<Rigidbody> ().AddForce (direction * 1350f);
	}

	void Update() {
		if (Time.time - startTime > 3f) {
			PlayFireballExplosion (); // Add some effects before the attack disappears.
		}
	}

	void OnCollisionEnter(Collision collision) {
		// If the fireball hits the player or something they spawned, do nothing:
		if(collision.gameObject.GetPhotonView() != null) {
			if (collision.gameObject.GetPhotonView ().isMine) {
				return;
			}
		}
		// Destroy the fireball, with effects, on any other collision.
		PlayFireballExplosion ();
	}

	// Handler for fireball destruction effects:
	public void PlayFireballExplosion() {
		// If the clip is not playing (this is SelfDestruct's first call), play it,
		// turn off the Renderer/Collider, and turn on the explosion particle effect:
		if (!fireballImpact.isPlaying) {
			gameObject.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeAll;
			gameObject.GetComponent<SphereCollider> ().enabled = false;
			fireballParticles.Stop();
			//fireballPoof.Play;
			fireballImpact.pitch = Random.Range (lowPitch, highPitch);
			fireballImpact.Play ();
		}
		StartCoroutine ("SelfDestruct", fireballImpact.clip.length);
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
