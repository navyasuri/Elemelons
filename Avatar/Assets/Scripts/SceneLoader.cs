using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

	void Awake() {
	}

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (gameObject);
		SceneManager.sceneLoaded += OnSceneFinishedLoading;
	}

	private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode) {
		if (scene.name == "VRPUNScene") {
			Debug.Log ("Scene loaded callback from SceneManager");
			// Call OnJoinedRoom again to reinstantiate the player in the new scene:
			GameObject.Find ("NetworkManager").GetComponent<Network> ().OnJoinedRoom ();
		}
	}
}