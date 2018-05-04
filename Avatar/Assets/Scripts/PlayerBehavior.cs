using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class PlayerBehavior : Photon.MonoBehaviour {

	public string flameType;
	public float health;
	public int cameraID;
	public PhotonPlayer thisPlayer;

    void Start () {
		// Initialize health and healthbar:
		health = 100f;
		//PhotonView.Get (this).RPC ("", PhotonTargets.AllBufferedViaServer, health);
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

	[PunRPC]
	public void TakeDamage(float damage) {
		health -= damage;
		Debug.Log ("TakeDamage RPC recieved for " + damage + " damage.");
		PhotonView [] playerViews = gameObject.GetPhotonViewsInChildren ();
		playerViews [2].RPC("UpdateHealthBar", PhotonTargets.AllBufferedViaServer, health);
	}
		
}