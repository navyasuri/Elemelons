using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class AllFireBehavior : Photon.MonoBehaviour {

	GameObject player;
    public int playerID;

	public AudioSource fireballWhoosh;
	public AudioSource fireballImpact;
	public AudioSource defenseWhoosh;
	public AudioSource flamethrowerWhoosh;
	public ParticleSystem fireballPoof;
	public ParticleSystem fireParticles;
	public float lowPitch = 0.45f;
	public float highPitch = 0.85f;

	public bool fireball = false;
	public bool defense = false;
	public bool flamethrower = false;
	float startTime;

	void Start() {
		startTime = Time.time; // Keep track of how long this move has been alive.
    }

	// Called by DeveloperDefined gesture triggers and networked prefab instantiation:
	public void DoAfterStart(Vector3 direction) {
		// Orient the new object so particle effects display properly:
		Quaternion rotationForTrails = Quaternion.FromToRotation (Vector3.back, direction);
		transform.rotation = rotationForTrails;
		// Send attacks flying!
		if (fireball) {
			gameObject.GetComponent<Rigidbody> ().AddForce (direction * 1350f);
			fireballWhoosh.Play ();
		}
		if (defense) {
			defenseWhoosh.time = 0f;
			defenseWhoosh.Play ();
		}
		if (flamethrower) {
			flamethrowerWhoosh.Play ();
		}
	}

    void Update() {
		if (fireball) {
			if (Time.time - startTime > 3f) {
				PlayFireballExplosion (); // Add some effects before the attack disappears.
			}
		}

		if (defense) {
			if (Time.time - startTime > 2f) {
				// Call the network to destroy the defense right off the bat:
				PhotonView.Get(this).RPC("NetworkDestroy", PhotonTargets.All);
			}
		}
	}

	// For fireballs:
	void OnCollisionEnter(Collision collision) {
		PlayFireballExplosion (); // Destroy the fireball, with effects, on any collision.
	}

	// For defense:
	void OnTriggerEnter(Collider collision) {
		if (collision.gameObject.CompareTag("attack")) { // Check for attacking objects
			// Destroy (ON NETWORK) any colliding attacks that are not this player's:
			if (collision.gameObject.GetComponent<AllFireBehavior> ().playerID != playerID) {
				PhotonView.Get(collision.gameObject).RPC("NetworkDestroy", PhotonTargets.All);
				fireballImpact.Play ();
			}
		}
	}

	// Handler for fireball destruction effects:
	public void PlayFireballExplosion() {
		// If the clip is not playing (this is SelfDestruct's first call), play it,
		// turn off the Renderer/Collider, and turn on the explosion particle effect:
		if (!fireballImpact.isPlaying) {
			gameObject.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeAll;
			gameObject.GetComponent<SphereCollider> ().enabled = false;
			fireParticles.Stop();
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
