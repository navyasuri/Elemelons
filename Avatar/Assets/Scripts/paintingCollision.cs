using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class paintingCollision : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision collision){

		if(transform.parent.name == "Controller (left)" || transform.parent.name == "Controller (right)"){
//		if (transform.childCount > 2) {

//			Transform go = gameObject.transform.GetChild (2);
			//transform.position = transform.parent.position;

			GetComponent<PhotonView> ().RequestOwnership ();
			GetComponent<TransformManager>().DetachParent();
			Transform wallToSnapTo = collision.gameObject.transform;
			Debug.Log (wallToSnapTo);
//			Debug.Log ("go child: " + go);

			//go.transform.localScale = Vector3.one;
			//			go.transform.eulerAngles = Vector3.zero;
			//go.transform.rotation = Quaternion.Euler(0f,headset.transform.rotation.y,0f);//LookAt(headset.transform);

			switch (collision.gameObject.name) {

			case "NorthWall":
				//go.transform.localScale = Vector3.one;
				gameObject.transform.eulerAngles = Vector3.zero;
				gameObject.transform.SetParent(wallToSnapTo);

				break;

			case "SouthWall":
				//go.transform.localScale = Vector3.one;
				gameObject.transform.eulerAngles = new Vector3(0f,180f,0f);
				gameObject.transform.SetParent(wallToSnapTo);

				break;

			case "EastWall":
				//go.transform.localScale = Vector3.one;
				gameObject.transform.eulerAngles = new Vector3(0f,90f,0f);
				gameObject.transform.SetParent(wallToSnapTo);

				break;

			case "WestWall":
				//go.transform.localScale = Vector3.one;
				gameObject.transform.eulerAngles = new Vector3(0f,-90f,0f);
				gameObject.transform.SetParent(wallToSnapTo);

				break;


			}

		}

	}
}
