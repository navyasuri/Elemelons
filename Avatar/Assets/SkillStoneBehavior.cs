using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkillStoneBehavior : MonoBehaviour {
	float startTime;


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
		if (col.gameObject.tag == "player") {
			//start UI tutorial
			//when tutorial ends, stop particle system

			if (SceneManager.GetActiveScene ().name == "Lobby")
				GameObject.Find ("SceneManager").GetComponent<SceneLoader> ().goToScene ("VRPunScene");
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
