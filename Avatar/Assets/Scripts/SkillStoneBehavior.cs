using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon;

public class SkillStoneBehavior : Photon.MonoBehaviour {

	private bool playerReady;
	public bool levelEnd;
	public bool playerSees;
	public float range;
	private float dist;


	// Use this for initialization
	void Start () {
		//start particle system

		range = 2f;
		levelEnd = false;
		playerSees = false;
		playerReady = false;

		Debug.Log ("Skill stone says active scene is: " + SceneManager.GetActiveScene ().name);
//		if(SceneManager.GetActiveScene().name.Equals("Lobby")) {
//			gameObject.GetComponent<SphereCollider> ().enabled = true;
//		}

	}
	
	// Update is called once per frame
	void Update () {

		if (SceneManager.GetActiveScene ().name == "Lobby" || levelEnd) {
//			dist = Vector3.Distance (this.transform.position, GameObject.Find ("[CameraRig]").transform.position);
			if (playerSees) {
				unlockSkill ();
			}
		}

	}

//	void OnTriggerEnter(Collider col){
//		Debug.Log ("Collision with Skill Stone by: " + col.gameObject.tag);
//		if (col.gameObject.CompareTag("Player")) {
//			//start UI tutorial
//			//when tutorial ends, stop particle system
//			if (!playerReady && SceneManager.GetActiveScene ().name == "Lobby") {
//				Debug.Log ("[Skill stone] Player is now ready!");
//				playerReady = true;
//				Debug.Log ("[Skill stone] Are you master client? " + PhotonNetwork.isMasterClient);
//				GameObject.Find("NetworkManager").GetPhotonView().RPC("UpdateReadyCount", PhotonTargets.All);
//			}
//			else {
//				
//				StartCoroutine ("resetter");
//			}
//
//		}
//
//
//	}

	private void unlockSkill(){
		Debug.Log ("Collision by player!");

		if (!playerReady && SceneManager.GetActiveScene ().name == "Lobby") {
			Debug.Log ("[Skill stone] Player is now ready!");
			playerReady = true;
			Debug.Log ("[Skill stone] Are you master client? " + PhotonNetwork.isMasterClient);
			GameObject.Find ("NetworkManager").GetPhotonView ().RPC ("UpdateReadyCount", PhotonTargets.All);
		} 

		else {

			StartCoroutine ("resetter");
		}

	}


	IEnumerator resetter(){
		
		yield return new WaitForSeconds (10f);

		GameObject.Find ("GameManager").GetComponent<GameController> ().increaseLevel ();
		gameObject.transform.Find("Spotlight").gameObject.SetActive(false);
		gameObject.transform.Find ("Flames").GetComponent<ParticleSystem> ().Stop ();
		playerSees = false;

	}


}
