using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon;

public class SkillStoneBehavior : Photon.MonoBehaviour {

	private bool playerReady;
	public bool levelOver;
	public bool playerSees;

	void Start () {
		levelOver = false;
		playerSees = false;
		playerReady = false;
	}
	
	void Update () {
		// Check for the level to be over (unless this is the lobby scene):
		if (levelOver || SceneManager.GetActiveScene ().name == "Lobby") {
			// Once the player raycasts onto the SkillStone, set the call flag to false and unlock a skill:
			if (playerSees) {
				levelOver = false;
				unlockSkill ();
			}
		}
	}

	private void unlockSkill(){
		// If this is the lobby scene, set this player to ready and call the network to update the ready count:
		if (!playerReady && SceneManager.GetActiveScene ().name == "Lobby") {
			Debug.Log ("[Lobby SkillStone] Player is now ready!");
			playerReady = true;
			Debug.Log ("[Lobby SkillStone] Are you master client? " + PhotonNetwork.isMasterClient);
			GameObject.Find ("NetworkManager").GetPhotonView ().RPC ("UpdateReadyCount", PhotonTargets.All);
		} 
		// If this is the main scene, call the GameManager to unlock the next skill based on the current level:
		else {
			GameObject.Find ("GameManager").GetComponent<GameController> ().addSkill ();
			StartCoroutine ("resetter");
		}
	}

	// Pause. Then turn off the SkillStone, and start the next level!
	IEnumerator resetter(){
		yield return new WaitForSeconds (5f);
		gameObject.transform.Find("Spotlight").gameObject.SetActive(false);
		gameObject.transform.Find ("Flames").GetComponent<ParticleSystem> ().Stop ();
		GameObject.Find ("AudioPlayer").GetComponent<AudioSource> ().Play ();
		GameObject.Find ("GameManager").GetComponent<GameController> ().StartNextLevel ();
	}
}
