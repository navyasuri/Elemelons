using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public GameObject BoulderSpawner1;
    public GameObject BoulderSpawner2;
    public GameObject SkillStone;
    private GameObject[] remainingBoulders;
    private float timer;
    private float currTime;
    private float timeOflevel;
    private float startTime;
    private int level;

	// Use this for initialization
	void Start () {
        startTime = Time.time;
        currTime = startTime;
        timer = 0f;
        timeOflevel = 20.0f;
        level = 1;

	}
	
	// Update is called once per frame
	void Update () {

        if ((Time.time - currTime) > timeOflevel)
        {
            endLevel(level);
            level++;

        }
		
	}


    private void endLevel(int level)
    {
        // turn off boulder spawner
        BoulderSpawner1.SetActive(false);
        BoulderSpawner2.SetActive(false);
        remainingBoulders = GameObject.FindGameObjectsWithTag("boulder");

        if (remainingBoulders.Length < 1)
        {
            GameObject light = SkillStone.transform.Find("Spotlight").gameObject;
            light.SetActive(true);

            switch (level)
            {
                case 1:
                    // do code for level 1
                    break;
                case 2:
                    // do code for level 2
                    break;
                case 3:
                    //do code for level 3
                    break;

            }
        }

        
        
    }
}
