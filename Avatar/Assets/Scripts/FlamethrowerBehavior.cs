using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class FlamethrowerBehavior : Photon.MonoBehaviour {

	public AudioSource flamethrowerWhoosh;
	public ParticleSystem throwerParticles;
	public float lowPitch = 0.45f;
	public float highPitch = 0.85f;
	float startTime;
	bool isActive = false;

	void Start() {
		//throwerParticles = transform.gameObject.Find("Flamethrower").gameObject.GetComponent<ParticleSystem> (); // Find thrower particles.
	}

	// Called by DeveloperDefined gesture triggers and networked prefab instantiation:
	public void DoAfterStart() {
		startTime = Time.time; // Keep track of how long this move has been alive.
		// Orient the new object so particle effects display properly:
//		Quaternion rotationForTrails = Quaternion.FromToRotation (Vector3.back, direction);
//		transform.rotation = rotationForTrails;
		throwerParticles.Play();
		flamethrowerWhoosh.Play ();
	}

	void Update() {
		// Check for an active flamethrower:
//		if (throwerParticles.isPlaying) {
//			// Stop playing after 3 seconds of infinite POWERRR
//			if ((Time.time - startTime > 5)) {
//				throwerParticles.Stop ();
//				flamethrowerWhoosh.Stop ();
//			}
//		}
	}

	// TODO: Figure out how to damage objects here.

}
