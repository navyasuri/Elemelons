using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class GameController : Photon.MonoBehaviour {

    public GameObject BoulderSpawner1;
    public GameObject BoulderSpawner2;
    public GameObject SkillStone;
    private GameObject[] remainingBoulders;
    private int level;
	public int boulderCount;
	private int boulderThreshold;

	void Start () {
		// Set starting vars:
        level = 1;
		boulderCount = 0;
		boulderThreshold = 5;

		// Reset players once the scene loads:
		//PhotonView.Get(this).RPC("ResetPlayerLocations", PhotonTargets.All);
	}
	
	void Update () {
		// If players have cleared the level by destroying the minimum number of boulders:
		if (boulderCount > boulderThreshold) {
			boulderCount = boulderThreshold;
            endLevel(level);
			Debug.Log ("[GameController Update()] End level was called. Level is now: " + level);
		}
	}

    private void endLevel(int level) {
		Debug.Log ("level " + level);

		// Turn off boulder spawner, fade music out:
		BoulderSpawner1.gameObject.SetActive(false);
        BoulderSpawner2.SetActive(false);
		Debug.Log ("Audio stopped, should now destroy boulders");
		// Get the list of remining boulders and remove them from the scene:
        remainingBoulders = GameObject.FindGameObjectsWithTag("boulder");

		for (int i = 0; i < remainingBoulders.Length; i++) {
			remainingBoulders[i].gameObject.GetPhotonView().RPC("NetworkDestroyNoCount", PhotonTargets.All);
		}

		Debug.Log ("RemainingBoulders: " + remainingBoulders.Length);

		// Once all boulders are gone, light up the SkillStone (via RPC, for all players), tell it that the level is over:
		PhotonView.Get(this).RPC("SkillStoneOn", PhotonTargets.AllBufferedViaServer);
    }

	[PunRPC]
	void SkillStoneOn() {
		StartCoroutine ("MusicFadeOut");
		SkillStone.transform.Find ("Spotlight").gameObject.SetActive (true);
		SkillStone.transform.Find ("Flames").GetComponent<ParticleSystem> ().Play ();
		SkillStone.GetComponent<SkillStoneBehavior> ().levelOver = true;
		SkillStone.GetComponent<AudioSource> ().Play ();
    }

	// Calls DeveloperDefined via RPC on the client's player prefab to enable gesture booleans.
	// Also sets the boulderCount to the previous threshold before updating to avoid jumping levels:
	public void addSkill(){
		switch (level)
		{
		case 1:
			Debug.Log ("[GameController addSkill()] Defense enabled");
			//
			// UI for skill unlock should happen here
			//
			GameObject.Find ("Camera (eye)").transform.GetChild (2).gameObject.GetComponent<DeveloperDefined> ().UnlockNext (1);
			boulderThreshold = 10;
			break;
		case 2:
			Debug.Log ("[GameController addSkill()] Left enabled");
			//
			// UI for skill unlock should happen here
			//
			GameObject.Find("Camera (eye)").transform.GetChild(2).gameObject.GetComponent<DeveloperDefined> ().UnlockNext (2);
			boulderThreshold = 15;
			break;
		case 3:
			Debug.Log ("[GameController addSkill()] Flame throwere enabled");
			//
			// UI for skill unlock should happen here
			//
			GameObject.Find("Camera (eye)").transform.GetChild(2).gameObject.GetComponent<DeveloperDefined> ().UnlockNext (3);
			boulderThreshold = 25;
			break;
		case 4:
			//game over
			GameObject.FindGameObjectWithTag("Player").GetPhotonView().RPC ("GameOver", PhotonTargets.AllViaServer);
			boulderThreshold = 999;
			break;
		default:
			Debug.Log ("[GameController addSkill()] Switch case called for unrecognized level case: " + level);
			break;
		}
	}

	public void StartNextLevel(){
		if (level == 4) {
			SkillStone.transform.Find ("Spotlight").gameObject.SetActive (true);
			SkillStone.transform.Find ("Flames").GetComponent<ParticleSystem> ().Play ();
		} else {
			level++;
			BoulderSpawner1.SetActive (true);
			BoulderSpawner2.SetActive (true);
			GameObject.Find ("AudioPlayer").GetComponent<AudioSource> ().Play ();
			GameObject.Find ("AudioPlayer").GetComponent<AudioSource> ().volume = 0.75f;
		}
	}

	IEnumerator MusicFadeOut() {
		for(float t = 0.163f; t > 0f; t -= 0.002f) {
			GameObject.Find ("AudioPlayer").GetComponent<AudioSource> ().volume = t/1f;
			yield return new WaitForSeconds(0.05f);
		}
		Debug.Log ("Stopping audio");
		GameObject.Find ("AudioPlayer").GetComponent<AudioSource> ().Stop();
	}
		
	[PunRPC]
	public void BoulderCountUpdate() {
		Debug.Log ("boulder count " + boulderCount);
		boulderCount++;
	}

}
