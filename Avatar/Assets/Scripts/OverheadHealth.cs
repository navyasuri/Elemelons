using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverheadHealth : MonoBehaviour {

	public GameObject target;
	public GameObject childHealthBar;
	public float playerHealth;
	public float headOffset;

	void Start () {
		
	}
	
	void Update () {
		if (target == null) {
			Destroy (this.gameObject);
			return;
		}

		playerHealth = target.GetComponent<PlayerBehavior>().health;

		childHealthBar.transform.position = target.transform.position;
		childHealthBar.transform.position = new Vector3(transform.position.x, transform.position.y + headOffset, transform.position.z);
	}

	public void SetTarget(GameObject target) {
		this.target = target;
	}
	
}
