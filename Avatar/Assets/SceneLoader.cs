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

		Debug.Log("Button being clicked");
		SceneManager.LoadScene(sceneName);
		// Application.LoadLevel("about-us");
	}

	public void goToScene (int sceneNumber)
	{

		Debug.Log("Button being clicked");
		SceneManager.LoadScene(sceneNumber);
	}

	public void LoadScene(int level)
	{ 
		Application.LoadLevel(level);
	}

}
