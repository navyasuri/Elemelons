using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour {

	public GameObject attack;

	// Use this for initialization
	void Start () {
		// Pick a random, saturated and not-too-dark color
		GetComponent<Renderer> ().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnMouseDown()
	{
		GameObject.Instantiate (attack, new Vector3 (transform.position.x+2*attack.GetComponent<Transform>().localScale.x, 
			1f, transform.position.z), transform.rotation);

	}
}
