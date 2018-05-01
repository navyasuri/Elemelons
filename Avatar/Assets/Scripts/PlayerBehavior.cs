using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class PlayerBehavior : Photon.MonoBehaviour {

	public string flameType;
	public float health;
	int cameraID;

	// To maintain player status between scenes. May belong higher up, on the Teleporting Rig instead..
	void Awake() {
		DontDestroyOnLoad (this.gameObject);
	}

    void Start () {
		// Initialize health and healthbar:
		health = 100f;
		PhotonView.Get (this).RPC ("UpdateHealth", PhotonTargets.All, health);
		gameObject.GetComponentInChildren<HealthBar>().player = gameObject;
		//Debug.Log ("FlameChoice is" + flameType);
		cameraID = GameObject.Find("Camera (eye)").GetInstanceID();
    }

	void Update() {
		// Broken respawn code:
		if (health <= 0f) {
			Destroy (gameObject.GetComponentInParent<Camera>());
			GameObject.Find ("NetworkManager").GetComponent<Network> ().OnJoinedRoom ();
			// Delete all the old player stuff
			health = 100f;
		}
	}

	// When something hits the player:
	void OnCollisionEnter(Collision other) {
		//Debug.Log("Player collided with " + other.gameObject);

		if (other.gameObject.CompareTag ("boulder")) {
			health -= 20f;
			PhotonView.Get (this).RPC ("UpdateHealth", PhotonTargets.All, health);
			Debug.Log ("You were hit by a boulder! Health: " + health);
		}

		if(other.gameObject.CompareTag("fireball")) {
			if (other.gameObject.GetComponent<FireballBehavior> ().playerID != cameraID) {
				health -= 25f;
				PhotonView.Get (this).RPC ("UpdateHealth", PhotonTargets.All, health);
				Debug.Log ("You were hit by a fireball! Health: " + health);
			}
		}
	}

	// Listen for collision with particle systems that have collision messages enabled:
	void OnParticleCollision(GameObject particles) {
		if (particles.CompareTag ("flamethrower")) {
			if (particles.gameObject.GetComponent<FlamethrowerBehavior> ().playerID != cameraID) {
				Debug.Log (particles);
				health -= 10f * Time.deltaTime; 
				PhotonView.Get (this).RPC ("UpdateHealth", PhotonTargets.All, health);
				Debug.Log ("You are hit by a flamethrower!");
			}
		}
	}

	[PunRPC]
	public void UpdateHealth(float newHealth) {
		health = newHealth;
		//		if (PhotonNetwork.isMasterClient) {
		//			PhotonNetwork.Destroy (gameObject);
		//			// This will have to do more, accounting for resetting the camera and all...
		//		}
	}
		
}