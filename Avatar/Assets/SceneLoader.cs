using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour {

	// void Start ()
	// {
	// 	Debug.Log("Working");
	// }


	public void goToScene (string sceneName)
	{

		Debug.Log("Loading a new scene");
		SceneManager.LoadSceneAsync(sceneName);
		// Application.LoadLevel("about-us");
	}

	public void goToScene (int sceneNumber)
	{

		Debug.Log("Loading a new scene");
		SceneManager.LoadScene(sceneNumber);
	}

	public void LoadScene(int level)
	{ 
		Application.LoadLevel(level);
	}

}
