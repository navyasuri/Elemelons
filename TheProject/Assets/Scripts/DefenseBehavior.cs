using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class DefenseBehavior : Photon.MonoBehaviour {

	GameObject player;
	Rigidbody rb;
    protected int playerID;
    //public Color attackerColor;
	float startTime;
	float currentTime;

	void Start() {
		//GetComponent<Renderer>().material.color = attackerColor;
		rb = gameObject.GetComponent<Rigidbody>();
        // Needed to be transform.forward, not transform.position.
        // Moved to Start to avoid acceleration.
		startTime = Time.time;
    }

    void Update() {
		currentTime = Time.time;
		if (currentTime - startTime > 5f) {
			Destroy (gameObject);
		}
	}

	void OnTriggerEnter(Collider collision)
	{
        if (collision.gameObject.CompareTag("attack"))
        {
			// Destroy any 'attack' tagged GameObjects that hit the wall
            Destroy(collision.gameObject);
        }
	}

	public void Spawn(Vector3 launch) {
		Quaternion rotation = Quaternion.FromToRotation (Vector3.back, launch);
		transform.rotation = rotation;
	}

}
