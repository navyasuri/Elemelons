using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using Photon;

public class BoulderBehavior : MonoBehaviour {

	//public AudioSource rumbling;
	AudioSource rumbling;
	public float lowPitch;
	public float highPitch;
	public GameObject explosionPrefab;
	float startTime;


	// Use this for initialization
	void Start () {
//		this.gameObject.GetComponent<MeshRenderer> ().material.color = Color.red;
		rumbling = GetComponent<AudioSource>();
		rumbling.time = 3f;
		float randomScale = Random.Range (0.01f, 0.03f);
		gameObject.transform.localScale += new Vector3 (randomScale, randomScale, randomScale);
		float startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {

		if (rumbling.time > 4.0f) {
			rumbling.Stop ();
			rumbling.time = 3f;
		}

		if (gameObject.transform.position.y < -10f) {
			Destroy (this.gameObject);
		}


//		if (Time.time - startTime > 2f) {
//			GameObject.Instantiate(parentPrefab, transform.position, Quaternion.identity);
//			Destroy (this.gameObject);
//		}
	}

	void OnCollisionEnter(Collision col){
		//float randomPitch = Random.Range (lowPitch, highPitch);
		//rumbling.pitch = randomPitch;
		//rumbling.time = 0f;
		rumbling.Play();

		Debug.Log (col.gameObject.tag);
		// Subtract health from player on impact:
//		if (col.gameObject.CompareTag("Player")) {
//			col.gameObject.GetComponent<PlayerBehavior> ().hit();
//		}
		// If the boulder hits anything not tagged 'environment':
		if (!col.gameObject.CompareTag("Environment")) {
			// get parent, play audio source
			//PhotonNetwork.Instantiate(parentPrefab.name, transform.position, Quaternion.identity, 0);
			GameObject.Instantiate(explosionPrefab, transform.position, Quaternion.identity);
			Destroy (this.gameObject);
		}
	}

}
