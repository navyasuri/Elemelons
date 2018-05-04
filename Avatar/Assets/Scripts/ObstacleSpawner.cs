using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class ObstacleSpawner : Photon.MonoBehaviour {

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
	
	// FixedUpdate is called by seconds, so processing speed across networks is mitigated
	void FixedUpdate () {
		if (Time.time - startTime > delay) {
			if (PhotonNetwork.isMasterClient) {
				newObstacle = PhotonNetwork.Instantiate (obstacle.name, currPos, Quaternion.identity, 0);
				startTime = Time.time;
				newObstacle.GetComponent<Rigidbody> ().AddForce (mForce, ForceMode.Force);
			}
		}
	}
		
}
