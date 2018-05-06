using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon;

public class SkillStoneBehavior : Photon.MonoBehaviour {

	private bool playerReady;
	public bool levelEnd;
	public bool playerSees;
	private bool hasBeenCalled;

	// Use this for initialization
	void Start () {
		//start particle system

		levelEnd = false;
		playerSees = false;
		playerReady = false;

		Debug.Log ("Skill stone says active scene is: " + SceneManager.GetActiveScene ().name);

	}
	
	// Update is called once per frame
	void Update () {
		if (SceneManager.GetActiveScene ().name == "Lobby" || levelEnd) {
			if (playerSees) {
				levelEnd = false;
				unlockSkill ();
			}
		}
	}

	private void unlockSkill(){
		Debug.Log ("Collision by player!");

		if (!playerReady && SceneManager.GetActiveScene ().name == "Lobby") {
			Debug.Log ("[Skill stone] Player is now ready!");
			playerReady = true;
			Debug.Log ("[Skill stone] Are you master client? " + PhotonNetwork.isMasterClient);
			GameObject.Find ("NetworkManager").GetPhotonView ().RPC ("UpdateReadyCount", PhotonTargets.All);
		} 

		else {
			GameObject.Find ("GameManager").GetComponent<GameController> ().addSkill ();
			StartCoroutine ("resetter");
		}
	}

	IEnumerator resetter(){
		yield return new WaitForSeconds (5f);

		gameObject.transform.Find("Spotlight").gameObject.SetActive(false);
		gameObject.transform.Find ("Flames").GetComponent<ParticleSystem> ().Stop ();
		playerSees = false;
		GameObject.Find ("GameManager").GetComponent<GameController> ().increaseLevel ();

	}
		

}
