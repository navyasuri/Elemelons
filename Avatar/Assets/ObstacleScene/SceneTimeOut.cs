using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTimeOut : MonoBehaviour {

	public float timer;
	public float currTime;
	public GameObject skillStone;
	private Vector3[] spawnPoints;
	private int counter;

	// Use this for initialization
	void Start () {
		currTime = Time.time;
		timer = 10f;
		spawnPoints = new Vector3[]{new Vector3(0,0,0),new Vector3(5f,0,5f)};
		counter = 0;

	}

	// Update is called once per frame
	void Update () {

		if (counter<spawnPoints.Length) {
			if (Time.time - currTime > timer*(counter+1)) {
				initializeSkillStone (skillStone, counter);
				counter++;
				currTime = Time.time;
			}
		}
	}

	protected void initializeSkillStone(GameObject o, int i){
		GameObject.Instantiate (o, spawnPoints[i], Quaternion.identity);
	}
}
