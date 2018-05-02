using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamesManager : MonoBehaviour {

	public GameObject yellowThrower, blueThrower, greenThrower, purpleThrower, whiteThrower;

	public Dictionary<string, GameObject> throwers = new Dictionary<string, GameObject>();

	// Use this for initialization
	void Start () {
		throwers.Add ("Yellow", yellowThrower);
		throwers.Add ("Blue", blueThrower);
		throwers.Add ("Green", greenThrower);
		throwers.Add ("Purple", purpleThrower);
		throwers.Add ("White", whiteThrower);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
