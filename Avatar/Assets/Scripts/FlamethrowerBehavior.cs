using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class FlamethrowerBehavior : Photon.MonoBehaviour {

	public PhotonPlayer flamethrowerPlayer;
	public AudioSource flamethrowerWhoosh;
	public ParticleSystem throwerParticles;
	public int playerID; // Must be public for Photon, no need to provide a value.
	float startTime;
	public bool isActive = false;

	void Start() {
		flamethrowerPlayer = gameObject.GetComponent<PhotonView> ().owner;
	}

	// Called by DeveloperDefined gesture triggers and networked prefab instantiation:
	public void DoAfterStart() {
		PhotonView.Get (this).RPC ("ActivateFlamethrower", PhotonTargets.All);
	}

	void Update() {
		// Check for an active flamethrower:
		if (throwerParticles.isPlaying) {
			// Stop playing after 4 seconds of infinite POWAAAA
			if ((Time.time - startTime > 4)) {
				throwerParticles.Stop ();
				flamethrowerWhoosh.Stop ();
				isActive = false;
			}
		}

		// If this flamethrower should be active, activate it's effects:
		if (isActive && !throwerParticles.isPlaying) {
			throwerParticles.Play(); // Start the particle effects
			flamethrowerWhoosh.Play (); // Sound like a flamethrower
		}
	}

	void OnParticleCollision(GameObject hitByFlames) {
		Debug.Log ("Flamethrower hit object named: " + hitByFlames.gameObject.name);
		if (hitByFlames.gameObject.GetPhotonView () != null ) {
			if (hitByFlames.gameObject.CompareTag ("Player")) {
				if (hitByFlames.gameObject.GetComponent<PlayerBehavior> ().thisPlayer == flamethrowerPlayer) { // if this is the player who launched the fireball
				} else { // Otherwise, send that player damage:
					float damage = 10f * Time.deltaTime;
					hitByFlames.gameObject.GetPhotonView ().RPC ("TakeDamage", hitByFlames.gameObject.GetPhotonView ().owner, damage);
				}
			}
			if (hitByFlames.gameObject.CompareTag ("boulder")) {
				float damage = 10f * Time.deltaTime;
				hitByFlames.gameObject.GetPhotonView ().RPC ("TakeDamage", PhotonTargets.All, damage);
			}
		}
	}

	[PunRPC]
	public void ActivateFlamethrower() {
		startTime = Time.time; // Keep track of how long this move has been alive.
		isActive = true;
	}
}
