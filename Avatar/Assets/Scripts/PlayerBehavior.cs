using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class PlayerBehavior : Photon.MonoBehaviour {

	public string flameType;
	public float health;
	public int cameraID;
	public PhotonPlayer thisPlayer;
	public int raycastDistance;
	public LayerMask layers;
	private float totalPoints;

    void Start () {
		// Initialize health and healthbar:
		health = 300f;
		raycastDistance = 3;
		//PhotonView.Get (this).RPC ("", PhotonTargets.AllBufferedViaServer, health);
		//Debug.Log ("FlameChoice is" + flameType);
		cameraID = GameObject.Find("Camera (eye)").GetInstanceID();
		thisPlayer = gameObject.GetComponent<PhotonView> ().owner;
		totalPoints = 0;
    }

	void Update() {
		// Broken respawn code:
		if (health <= 0) {
			GameObject.Find ("Camera (eye)").transform.GetChild (2).gameObject.GetPhotonView ().RPC ("GameOver", PhotonTargets.All);
		}

		if (health <= 40f){
			GameObject.Find ("AudioPlayer").GetComponent<AudioSource> ().volume = 0.19f;
		}

		RayCast ();
	}

	[PunRPC]
	public void TakeDamage(float damage) {
		health -= damage;
		Debug.Log ("TakeDamage RPC recieved for " + damage + " damage.");
		PhotonView [] playerViews = gameObject.GetPhotonViewsInChildren ();
		playerViews [2].RPC("UpdateHealthBar", PhotonTargets.AllBufferedViaServer, health);
	}

    [PunRPC]
    public void AddHealth(float healthUp)
    {
        health += healthUp;
        PhotonView[] playerViews = gameObject.GetPhotonViewsInChildren();
        playerViews[2].RPC("UpdateHealthBar", PhotonTargets.AllBufferedViaServer, health);
    }

    [PunRPC]
	public void increasePoints(float points, int networkID){
		if (networkID == PhotonNetwork.player.ID) {
			totalPoints += points;
			Debug.Log ("Total points: " + totalPoints);
			PhotonView [] playerViews = gameObject.GetPhotonViewsInChildren ();
			playerViews [3].RPC("UpdatePlayerScore", PhotonTargets.AllBufferedViaServer, totalPoints);
		}
	}

	[PunRPC]
	public void GameOver() {
		if (health > 0) {
			gameObject.transform.GetChild (5).gameObject.SetActive (true);
		} else {//player is dead
			gameObject.transform.GetChild (4).gameObject.SetActive (true);
			totalPoints = -1f;
		}
		StartCoroutine (RestartGame ());
	}

	IEnumerator RestartGame() {
		yield return new WaitForSeconds(10f);
		GameObject.Find ("NetworkManager").GetPhotonView ().RPC ("SceneChange", PhotonTargets.AllBufferedViaServer);
	}

	void RayCast() {
		Vector3 forward = GameObject.Find ("Camera (eye)").transform.forward;
		RaycastHit hit;

		if (Physics.Raycast (GameObject.Find ("Camera (eye)").transform.position, forward, out hit, raycastDistance, layers)) {
			if (hit.collider.gameObject.tag == "SkillStone") {
				Debug.Log ("SkillStone detected");
				
				//inform skillstone that player sees it
				GameObject.Find ("SkillStonePrefab").GetComponent<SkillStoneBehavior> ().playerSees = true;
			} else {
				GameObject.Find ("SkillStonePrefab").GetComponent<SkillStoneBehavior> ().playerSees = false;
			}
		}
	}

	public float getPoints() {
		return totalPoints;
	}
		
}

