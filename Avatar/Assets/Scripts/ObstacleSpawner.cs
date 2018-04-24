using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour {

	private float startTime;
	private float delay;
	public GameObject obstacle;
	private Vector3 mForce;
	private GameObject newObstacle;
	private Vector3 currPos;

	// Use this for initialization
	void Start () {
		delay = 5.0f;
		startTime = Time.time;
		currPos = this.transform.position;
		float zForce;
		if (currPos.z < 0) {
			zForce = 5f;
		} else {
			zForce = -5f;
		}
		mForce = new Vector3 (0.0f, 0.0f, zForce);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Time.time - startTime > delay) {

			newObstacle = PhotonNetwork.Instantiate (obstacle.name, currPos, Quaternion.identity, 0);
			startTime = Time.time;
			newObstacle.GetComponent<Rigidbody> ().AddForce (mForce, ForceMode.Force);
		}

		//if (newObstacle != null) {
		//	newObstacle.GetComponent<Rigidbody> ().AddForce (mForce, ForceMode.Force);
		//}

			
	}
		
}
