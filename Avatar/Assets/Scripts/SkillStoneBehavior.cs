using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon;

public class SkillStoneBehavior : Photon.MonoBehaviour {
	float startTime;
	bool playerReady = false;

	// Use this for initialization
	void Start () {
		//start particle system
		startTime = Time.time;
		gameObject.GetComponent<SphereCollider> ().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
//		if (Time.time - startTime > 5)
//			GameObject.Find("SceneManager").GetComponent<SceneLoader>().goToScene("VRPunScene");
	}

	void OnCollisionEnter(Collision col){
		if (col.gameObject.CompareTag("Player")) {
			//start UI tutorial
			//when tutorial ends, stop particle system

			if (!playerReady && SceneManager.GetActiveScene ().name == "Lobby") {
				playerReady = true;
				GameObject.Find("NetworkManager").GetPhotonView().RPC("UpdateReadyCount", PhotonTargets.MasterClient);
			}
			else {
				
				StartCoroutine ("resetter");
			}

		}


	}

	IEnumerator resetter(){
		
		yield return new WaitForSeconds (5f);
		gameObject.GetComponent<SphereCollider> ().enabled = false;
		GameObject.Find ("GameManager").GetComponent<GameController> ().increaseLevel ();
		GameObject light = gameObject.transform.Find("Spotlight").gameObject;
		light.SetActive(false);
	}
}
