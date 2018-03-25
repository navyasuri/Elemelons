using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehavior : MonoBehaviour {

//	public Transform target;
//	public float speed;
	Vector3 newPos;
	GameObject player;
	Rigidbody rb;

	// Use this for initialization
	void Start () {
		GetComponent<Renderer> ().material.color = Color.red;
		GameObject.Find ("Player");
		rb = gameObject.GetComponent<Rigidbody> ();

	}
	
	// Update is called once per frame
	void Update () {
//		float step = speed * Time.deltaTime;
//		transform.position = Vector3.MoveTowards(transform.position, target.position, step);

//		newPos=transform.position;
//		newPos.x += 0.1f;
//		transform.position = newPos;
		rb.AddForce(transform.position*1f);
		
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject == GameObject.Find("Player1"))
			Debug.Log ("Attacked player");
			Destroy(gameObject);
	}
}
