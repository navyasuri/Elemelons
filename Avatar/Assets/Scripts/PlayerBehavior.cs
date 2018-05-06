using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon;

public class PlayerBehavior : Photon.MonoBehaviour {

	public string flameType;
	public float health;
	public int cameraID;
	public PhotonPlayer thisPlayer;
	public int raycastDistance;
	public LayerMask layers;
	private float totalPoints;

	void Awake() {
		SceneManager.sceneLoaded += OnSceneFinishedLoading;
	}

	private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode) {
		if (scene.name == "VRPUNScene") {
			if (PhotonNetwork.isMasterClient) {
				Debug.Log ("Scene loaded callback from master player, " + PhotonNetwork.playerList.Length + " players in scene");
			}
		}
	}

    void Start () {
		// Initialize health and healthbar:
		health = 100f;
		raycastDistance = 3;
		//PhotonView.Get (this).RPC ("", PhotonTargets.AllBufferedViaServer, health);
		//Debug.Log ("FlameChoice is" + flameType);
		cameraID = GameObject.Find("Camera (eye)").GetInstanceID();
		thisPlayer = gameObject.GetComponent<PhotonView> ().owner;
		totalPoints = 0;

    }

	void Update() {
		// Broken respawn code:
		if (health <= 0f) {
			//Destroy (gameObject.GetComponentInParent<Camera>());
			//GameObject.Find ("NetworkManager").GetComponent<Network> ().OnJoinedRoom ();
			// Delete all the old player stuff
			health = 100f;
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
	public void GameOver() {
		if (photonView.isMine) {
			Debug.Log("[PlayerBehavior GameOver RPC] Game Over. Trigger some UI, boss!");
			// Trigger some UI, boss.
		}
	}


	[PunRPC]
	public void increasePoints(float points){
		totalPoints += points;
		Debug.Log ("total points:" + totalPoints);
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
		
}

