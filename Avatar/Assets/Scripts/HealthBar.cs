using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon;

public class HealthBar : Photon.MonoBehaviour {

	public GameObject player;
	public Vector3 greenHealthBar;
	public float playerHP;

	void Start() {
		// Get the green portion of the health bar to change it's width:
		greenHealthBar = GameObject.Find ("Healthy").GetComponent<RectTransform> ().localScale;
	}

	void Update () {
		// Get this player's health. HealthBar script should be a child of the Player prefab.
		float newHP = player.GetComponent<PlayerBehavior> ().health;
		if (playerHP != newHP) {
			playerHP = newHP;
			PhotonView.Get (this).RPC ("UpdateHealthBar", PhotonTargets.AllBufferedViaServer, newHP);
		}
	}

	[PunRPC]
	public void UpdateHealthBar(float newHP) {
		// Calculate the width of the bar based on the current health:
		greenHealthBar.x = newHP / 100;

		// Keep the bar from "displaying" a less-than-zero amount:
		if (greenHealthBar.x <= 0f) {
			greenHealthBar.x = 0;
		}

		// Apply the width adjustment based on the current player health:
		GameObject.Find ("Healthy").GetComponent<RectTransform> ().localScale = new Vector3 (greenHealthBar.x, greenHealthBar.y, greenHealthBar.z);
	}

//	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
//		if (stream.isWriting) {
//			stream.SendNext (greenHealthBar.x);
//		} else {
//			greenHealthBar.x = (float)stream.ReceiveNext ();
//		}
//	}
	
}
