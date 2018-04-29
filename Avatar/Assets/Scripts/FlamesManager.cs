using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamesManager : MonoBehaviour {

	public GameObject yellowAttack, blueAttack, greenAttack, purpleAttack, whiteAttack;
	public GameObject yellowThrower, blueThrower, greenThrower, purpleThrower, whiteThrower;

	Dictionary<string, GameObject> attacks;
	Dictionary<string, GameObject> throwers;

	// Use this for initialization
	void Start () {
		attacks.Add ("yellow", yellowAttack);
		attacks.Add ("blue", blueAttack);
		attacks.Add ("green", greenAttack);
		attacks.Add ("purple", purpleAttack);
		attacks.Add ("white", whiteAttack);

		throwers.Add ("yellow", yellowThrower);
		throwers.Add ("blue", blueThrower);
		throwers.Add ("green", greenThrower);
		throwers.Add ("purple", purpleThrower);
		throwers.Add ("white", whiteThrower);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
