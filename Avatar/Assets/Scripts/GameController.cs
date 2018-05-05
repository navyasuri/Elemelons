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

	// Use this for initialization
	void Start () {
       
        level = 1;
		boulderCount = 0;
		boulderThreshold = 5;
//		BoulderSpawner1.gameObject.SetActive(false);
//		BoulderSpawner2.SetActive(false);

		// Reset players once the scene loads:
		PhotonView.Get(this).RPC("ResetPlayerLocations", PhotonTargets.All);
	}
	
	// Update is called once per frame
	void Update () {
		if (boulderCount> boulderThreshold) {
            endLevel(level);
			Debug.Log ("In update: level " + level);
        }
	}


    private void endLevel(int level) {
		Debug.Log ("level " + level);

		// turn off boulder spawner
		BoulderSpawner1.gameObject.SetActive(false);
        BoulderSpawner2.SetActive(false);
		StartCoroutine ("MusicFadeOut");

		Debug.Log (BoulderSpawner1.activeInHierarchy);
        remainingBoulders = GameObject.FindGameObjectsWithTag("boulder");

        if (remainingBoulders.Length < 1)
        {
            GameObject light = SkillStone.transform.Find("Spotlight").gameObject;
            light.SetActive(true);
			SkillStone.transform.Find ("Flames").GetComponent<ParticleSystem>().Play();
			SkillStone.GetComponent<SkillStoneBehavior> ().levelEnd = true;

            switch (level)
            {
                case 1:
                    // do code for level 1
					GameObject.Find("GameManager").GetPhotonView().RPC("UnlockNext", PhotonTargets.All, 1);
					boulderThreshold = 10;
                    break;
				case 2:
					GameObject.Find("GameManager").GetPhotonView().RPC("UnlockNext", PhotonTargets.All, 2);
					boulderThreshold = 15;
	                    // do code for level 2
	                break;
				case 3:
					GameObject.Find("GameManager").GetPhotonView().RPC("UnlockNext", PhotonTargets.All, 3);
					boulderThreshold = 25;
	                    //do code for level 3
	                break;
				case 4:
					//game over
					GameObject.Find ("GameManager").GetPhotonView ().RPC ("GameOver", PhotonTargets.All);
					boulderThreshold = 999;
					break;
            }
        }    
    }

	public void increaseLevel(){
		level++;
		BoulderSpawner1.SetActive(true);
		BoulderSpawner2.SetActive(true);
		SkillStone.GetComponent<SkillStoneBehavior> ().levelEnd = false;
		GameObject.Find ("AudioPlayer").GetComponent<AudioSource> ().time = 0f;
		GameObject.Find ("AudioPlayer").GetComponent<AudioSource> ().Play ();
	}

	IEnumerator MusicFadeOut() {
		for(float t = 3f; t > 0f; t -= 0.05f) {
			GameObject.Find ("AudioPlayer").GetComponent<AudioSource> ().volume = t/4f;
			yield return new WaitForSeconds(0.05f);
		}
		GameObject.Find ("AudioPlayer").GetComponent<AudioSource> ().Stop();
	}

	[PunRPC]
	public void BoulderCountUpdate() {
		Debug.Log ("boulder count " + boulderCount);
		boulderCount++;
	}

	[PunRPC]
	void ResetPlayerLocations() {
		// Send RPC to all clients, telling them to use their local playerColor to move to spawn
		GameObject blankScreen = GameObject.Find("Camera (eye)").transform.GetChild(2).GetChild(5).gameObject;
		string playerColor =  blankScreen.GetComponentInParent<DeveloperDefined> ().playerColor;
		GameObject.Find ("TeleportingRig(Clone)").transform.position = GameObject.Find ("Spawn" + playerColor).transform.position;
		blankScreen.GetComponent<Renderer> ().material.SetColor("_Color", new Color(0, 0, 0, 0));
	}
}
