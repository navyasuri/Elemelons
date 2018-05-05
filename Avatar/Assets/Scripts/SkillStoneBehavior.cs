using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon;

public class SkillStoneBehavior : Photon.MonoBehaviour {

	float startTime;
	bool playerReady = false;
	public float range;
	private float dist;
	public bool levelEnd;
	public bool playerSees;


	// Use this for initialization
	void Start () {
		//start particle system
		startTime = Time.time;
		range = 1f;
		levelEnd = false;
		playerSees = false;

		Debug.Log ("Skill stone says active scene is: " + SceneManager.GetActiveScene ().name);
		if(SceneManager.GetActiveScene().name.Equals("Lobby")) {
			gameObject.GetComponent<SphereCollider> ().enabled = true;
		}

	}
	
	// Update is called once per frame
	void Update () {

		if (levelEnd) {
			dist = Vector3.Distance (this.transform.position, GameObject.FindGameObjectWithTag ("Player").transform.position);
			if (dist < range) {
				unlockSkill ();
			}
		}

	}

	void OnTriggerEnter(Collider col){
		Debug.Log ("Collision with Skill Stone by: " + col.gameObject.tag);
		if (col.gameObject.CompareTag("Player")) {
			//start UI tutorial
			//when tutorial ends, stop particle system
			if (!playerReady && SceneManager.GetActiveScene ().name == "Lobby") {
				Debug.Log ("[Skill stone] Player is now ready!");
				playerReady = true;
				Debug.Log ("[Skill stone] Are you master client? " + PhotonNetwork.isMasterClient);
				GameObject.Find("NetworkManager").GetPhotonView().RPC("UpdateReadyCount", PhotonTargets.All);
			}
			else {
				
				StartCoroutine ("resetter");
			}

		}


	}

	private void unlockSkill(){
		Debug.Log ("Collision by player!");
		StartCoroutine ("resetter");
	}

	IEnumerator resetter(){
		
		yield return new WaitForSeconds (5f);
		GameObject.Find ("GameManager").GetComponent<GameController> ().increaseLevel ();
		GameObject light = gameObject.transform.Find("Spotlight").gameObject;
		light.SetActive(false);
		gameObject.transform.Find ("Flames").GetComponent<ParticleSystem> ().Stop ();
		playerSees = false;

	}


}
