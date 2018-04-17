using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OG_ColliderManager : MonoBehaviour {


	public GameObject NorthWall;
	public GameObject SouthWall;
	public GameObject WestWall;
	public GameObject EastWall;

	public GameObject ControllerLeft;
	public GameObject ControllerRight;

	private GameObject[] walls = new GameObject[4];

	// Use this for initialization
	void Start () {
		walls [0] = NorthWall;
		walls [1] = SouthWall;
		walls [2] = WestWall;
		walls [3] = EastWall;
	}
	
	// Update is called once per frame
	void Update () {
		if(ControllerLeft.transform.childCount > 1 || ControllerRight.transform.childCount > 1){
			for(int i=0; i <walls.Length;i++){
				walls[i].GetComponent<Collider>().enabled = true;
			}
		}else{
			for(int i=0; i <walls.Length;i++){
				walls[i].GetComponent<Collider>().enabled = false;
			}
		}
	}
}
