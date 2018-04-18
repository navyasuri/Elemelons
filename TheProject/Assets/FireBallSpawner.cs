using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallSpawner : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SpawnFireball() {
		//Vector3 attackVector = controller.transform.position + (transform.forward * 1f);
		Vector3 attackVector = Vector3.zero;
		//attackVector.y += 0.25f; // Move it up a tad for the aesthetics.
		var newAttack = PhotonNetwork.Instantiate ("Attack", attackVector, Quaternion.identity, 0); // Create the attack.
		//newAttack.GetComponent<AttackBehavior>().attackerID = gameObject.GetInstanceID (); // Note ID of attacking player. (To avoid self-damage)
	}
}
