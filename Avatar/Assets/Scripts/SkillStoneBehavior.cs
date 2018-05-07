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
//			if (!gameObject.GetComponent<AudioSource> ().isPlaying) {
//				gameObject.GetComponent<AudioSource> ().Play ();
//			}
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
			Debug.Log ("[Lobby SkillStone] Player is now ready! Master client? " + PhotonNetwork.isMasterClient);
			playerReady = true;
			GameObject.Find ("Camera (eye)").transform.GetChild (2).gameObject.GetComponent<DeveloperDefined>().UnlockNext(0);

            //
            // RUN FIREBALL TRAINING HERE
            GameObject.Find("Camera (eye)").transform.GetChild(2).GetChild(7).gameObject.SetActive(true);
            GameObject.Find("Camera (eye)").transform.GetChild(2).GetChild(7).gameObject.GetComponent<StatusUpdate>().UpdateStatus("fireball unlocked!");

			//
			// RUN FIREBALL TRAINING HERE
			//GameObject.Find("Player").transform.GetChild(7).gameObject.GetComponent<StatusUpdate>().UpdateStatus("right fire punch unlocked!");

			//
		} 

		// If this is the main scene, call the GameManager to unlock the next skill based on the current level:
		if (!SceneManager.GetActiveScene().name.Equals("Lobby")) {
            GameObject.Find("GameManager").GetComponent<GameController>().addSkill();
        }
    }

    [PunRPC]
    public void NextLevel()
    {
        StartCoroutine("LoadNext");
    }

    IEnumerator LoadNext()
    {
        yield return new WaitForSeconds(4f);
        gameObject.GetComponent<AudioSource>().Stop();
        gameObject.transform.Find("Spotlight").gameObject.SetActive(false);
        gameObject.transform.Find("Flames").GetComponent<ParticleSystem>().Stop();
        GameObject.Find("GameManager").GetComponent<GameController>().StartNextLevel();
    }
}
