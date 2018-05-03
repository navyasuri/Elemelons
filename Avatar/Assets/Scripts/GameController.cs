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

	}
	
	// Update is called once per frame
	void Update () {

		if (boulderCount>= boulderThreshold)
        {
            endLevel(level);

        }
		
	}


    private void endLevel(int level)
    {
		Debug.Log ("level " + level);
        // turn off boulder spawner
//        BoulderSpawner1.SetActive(false);
//        BoulderSpawner2.SetActive(false);
        remainingBoulders = GameObject.FindGameObjectsWithTag("boulder");

        if (remainingBoulders.Length < 1)
        {
            GameObject light = SkillStone.transform.Find("Spotlight").gameObject;
            light.SetActive(true);
			SkillStone.GetComponent<SphereCollider> ().enabled = true;

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
	}


	[PunRPC]
	public void BoulderCountUpdate() {
		Debug.Log ("boulder count " + boulderCount);
		boulderCount++;
	}
}
