using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class PlayerBehavior : Photon.MonoBehaviour {

	public string flameType;
	public float health;
	public int cameraID;
	public PhotonPlayer thisPlayer;

	// To maintain player status between scenes. May belong higher up, on the Teleporting Rig instead..
	void Awake() {
		//DontDestroyOnLoad (this.gameObject);
	}

    void Start () {
		// Initialize health and healthbar:
		health = 100f;
		//PhotonView.Get (this).RPC ("", PhotonTargets.AllBufferedViaServer, health);
		gameObject.GetComponentInChildren<HealthBar>().player = gameObject;
		//Debug.Log ("FlameChoice is" + flameType);
		cameraID = GameObject.Find("Camera (eye)").GetInstanceID();
		thisPlayer = gameObject.GetComponent<PhotonView> ().owner;
    }

	void Update() {
		// Broken respawn code:
		if (health <= 0f) {
			//Destroy (gameObject.GetComponentInParent<Camera>());
			//GameObject.Find ("NetworkManager").GetComponent<Network> ().OnJoinedRoom ();
			// Delete all the old player stuff
			health = 100f;
		}
	}

	// When something hits the player:
	void OnCollisionEnter(Collision other) {
		//Debug.Log("Player collided with " + other.gameObject);

		if (other.gameObject.CompareTag ("boulder")) { // if you collider with a boulder
			float damage = 20f;
			//PhotonView.Get (this).RPC ("TakeDamage", PhotonTargets.AllBufferedViaServer, health);			
			PhotonView.Get (this).RPC ("TakeDamage", thisPlayer, damage);
			Debug.Log ("You were hit by a boulder! Health: " + damage);
		}

		if(other.gameObject.CompareTag("fireball")) {
			// If the owning client of the fireball is not the same as this player (by photonviews), then...
			if (other.gameObject.GetComponent<PhotonView> ().owner != thisPlayer) {
				// Subtract health locally, and...
				float damage = 25f;
				// Call an RPC on this player to deliver damage. (Key that the DAMAGE is only sent to this player!)
				PhotonView.Get (this).RPC ("TakeDamage", thisPlayer, damage);
				Debug.Log ("You were hit by a fireball! Damage: " + damage);
			}
		}
	}

	// Listen for collision with particle systems that have collision messages enabled:
	void OnParticleCollision(GameObject particles) {
		if (particles.CompareTag ("flamethrower")) {
			if (particles.gameObject.GetComponent<PhotonView> ().owner != thisPlayer) {
				Debug.Log (particles);
				float damage = 10f * Time.deltaTime;
				PhotonView.Get (this).RPC ("TakeDamage", thisPlayer, damage);
				Debug.Log ("You are hit by a flamethrower!");
			}
		}
	}

	[PunRPC]
	public void TakeDamage(float damage) {
		health -= damage;
	}
		
}