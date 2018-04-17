using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OG_InputManager : MonoBehaviour
{

	//This script should be attached to each controller (Controller Left or Controller Right)

	// Getting a reference to the controller GameObject
	private SteamVR_TrackedObject trackedObj;
	// Getting a reference to the controller Interface
	private SteamVR_Controller.Device Controller;

	public LayerMask layerMask;
	public float raycastDistance;

	public GameObject cam;
	public Material myMat;

	private LineRenderer lr;
	private Vector3[] positions;

	public Light lt;
	public GameObject headset;

	private bool reset = true;

	void Awake()
	{
		// initialize the trackedObj to the component of the controller to which the script is attached
		trackedObj = GetComponent<SteamVR_TrackedObject>();

		//----
		lr = GetComponent<LineRenderer> ();
		lr.material = new Material(Shader.Find("Sprites/Default"));

		//a simple 2 color gradient with a fixed alpha of 1.0f
		float alpha = 1f;
		Gradient gradient = new Gradient ();
		gradient.SetKeys (
			new GradientColorKey[] { new GradientColorKey (Color.green, 0f), new GradientColorKey (Color.green, 1f) },
			new GradientAlphaKey[] { new GradientAlphaKey (alpha, 0f), new GradientAlphaKey (alpha, 1f) }
		);
		lr.colorGradient = gradient;
		lr.startWidth = 0.002f;
		lr.endWidth = 0.002f;
		positions = new Vector3[2];


	}

	// Update is called once per frame
	void Update()
	{

			float alpha = 1f;
			Gradient gradient = new Gradient ();
			gradient.SetKeys (
				new GradientColorKey[] { new GradientColorKey (Color.green, 0f), new GradientColorKey (Color.green, 1f) },
				new GradientAlphaKey[] { new GradientAlphaKey (alpha, 0f), new GradientAlphaKey (alpha, 1f) }
			);
			lr.colorGradient = gradient;

			Controller = SteamVR_Controller.Input ((int)trackedObj.index);

			positions [0] = transform.position;//new Vector3 (-2f,0.2f,0f);
			positions [1] = transform.position;//new Vector3 (0f,0.2f,0f);
			//positions [2] = new Vector3 (2f,-2f,0f);
			lr.positionCount = positions.Length;
			lr.SetPositions (positions);
		
		Raycasting ();

		// Getting the Touchpad Axis
		if (Controller.GetAxis() != Vector2.zero)
		{
			Debug.Log(gameObject.name + Controller.GetAxis());

			if(transform.childCount > 2 && transform.GetChild(2).tag == "Light"){
				lt.intensity = Controller.GetAxis().x + 1;
			}
//			Transform go = gameObject.transform.GetChild (1);
//			go.parent = null;
//			go.transform.localScale = Vector3.one;
		}

		// Getting the Trigger press
		if (Controller.GetHairTriggerDown())
		{
			Debug.Log(gameObject.name + " Trigger Press");

		}

		// Getting the Trigger Release
		if (Controller.GetHairTriggerUp())
		{

			Transform go = gameObject.transform.GetChild (2);
			go.GetComponent<PhotonView> ().RequestOwnership ();
            go.GetComponent<TransformManager>().MakeVisible();
            go.GetComponent<TransformManager> ().DetachParent ();
            //
			//go.parent = null;
			//go.transform.localScale = Vector3.one;

		}

		// Getting the Grip Press
		if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
		{
//			Debug.Log(gameObject.name + " Grip Press");
//			float alpha = 1f;
//			Gradient gradient = new Gradient ();
//			gradient.SetKeys (
//				new GradientColorKey[] { new GradientColorKey (Color.yellow, 0f), new GradientColorKey (Color.yellow, 1f) },
//				new GradientAlphaKey[] { new GradientAlphaKey (alpha, 0f), new GradientAlphaKey (alpha, 1f) }
//			);
//			lr.colorGradient = gradient;
//
//			//set some positions
//
//			positions [0] = transform.position;//new Vector3 (-2f,0.2f,0f);
////			var localDirection = transform.InverseTransformPoint (transform.forward);
//			//var localDirection = transform.InverseTransformDirection(transform.forward);
//			positions [1] = transform.TransformDirection(transform.forward);//hit.collider.gameObject.transform.position;//new Vector3 (0f,0.2f,0f);
//			//positions [2] = new Vector3 (2f,-2f,0f);
//			Debug.Log("fwd on yellow is: " + Vector3.forward);
//			lr.positionCount = positions.Length;
//			lr.SetPositions (positions);
//			reset = false;
		}

		// Getting the Grip Release
		if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
		{
//			Debug.Log(gameObject.name + " Grip Release");
//			reset = true;
		}
	}


	void Raycasting(){
		Vector3 fwd = transform.TransformDirection (Vector3.forward); //what is the direction in front of us
		RaycastHit hit = new RaycastHit ();

		if (Physics.Raycast (transform.position, fwd, out hit, raycastDistance, layerMask)) {
			//Debug.Log ("hit object: " + hit.collider.gameObject.name);
			//Debug.Log ("I hit: " + hit.collider.transform);
			//Debug.Log ("hitting: " + hit.collider);

	
			if ((hit.collider.gameObject.tag == "Light" || hit.collider.gameObject.tag == "Painting") && Controller.GetHairTriggerDown ()) {
				//Debug.Log ("setting parent of: " + hit.collider.gameObject);
		
				GameObject goHit = hit.collider.gameObject;
				goHit.GetComponent<PhotonView> ().RequestOwnership ();
                goHit.GetComponent<TransformManager>().MakeTransparent();
                goHit.GetComponent<TransformManager> ().SetNewParent (transform);
                //goHit.transform.position = transform.position;
                //goHit.GetComponent<TransformManager>().MakeTransparent();
				//hit.collider.gameObject.transform.SetParent (transform);
				//hit.collider.gameObject.transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
	
			} else if (hit.collider.gameObject.tag == "Light" || hit.collider.gameObject.tag == "Painting") {
				Debug.Log ("drawing line now...");
				//


				//set some positions

				positions [0] = transform.position;//new Vector3 (-2f,0.2f,0f);
				positions [1] = hit.collider.gameObject.transform.position;//new Vector3 (0f,0.2f,0f);
				//positions [2] = new Vector3 (2f,-2f,0f);
				lr.positionCount = positions.Length;
				lr.SetPositions (positions);

			}
		}
//		if(Physics.Raycast(transform.position, fwd, out hit, 100, layerMask)){
//			if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip)) {
//				Debug.Log ("drawing orange line now...");
//				//
//				float alpha = 1f;
//				Gradient gradient = new Gradient ();
//				gradient.SetKeys (
//					new GradientColorKey[] { new GradientColorKey (Color.yellow, 0f), new GradientColorKey (Color.yellow, 1f) },
//					new GradientAlphaKey[] { new GradientAlphaKey (alpha, 0f), new GradientAlphaKey (alpha, 1f) }
//				);
//				lr.colorGradient = gradient;
//
//				//set some positions
//
//				positions [0] = transform.position;//new Vector3 (-2f,0.2f,0f);
//				var localDirection = transform.InverseTransformPoint (transform.forward);
//				//var localDirection = transform.InverseTransformDirection(transform.forward);
//				positions [1] = localDirection;//hit.collider.gameObject.transform.position;//new Vector3 (0f,0.2f,0f);
//				//positions [2] = new Vector3 (2f,-2f,0f);
//				Debug.Log("fwd on yellow is: " + Vector3.forward);
//				lr.positionCount = positions.Length;
//				lr.SetPositions (positions);
//				reset = false;
//
//			}
//		}
	}

//	void OnCollisionEnter(Collision collision){
//
//		Debug.Log ("My chuildCount is: " + transform.childCount);
//		if (transform.childCount > 2) {
//
//			Transform go = gameObject.transform.GetChild (2);
//			go.parent = null;
//			Transform wallToSnapTo = collision.gameObject.transform;
//			Debug.Log (wallToSnapTo);
//			Debug.Log ("go child: " + go);
//
//			//go.transform.localScale = Vector3.one;
//			//			go.transform.eulerAngles = Vector3.zero;
//			//go.transform.rotation = Quaternion.Euler(0f,headset.transform.rotation.y,0f);//LookAt(headset.transform);
//
//			switch (collision.gameObject.name) {
//
//			case "NorthWall":
//				//go.transform.localScale = Vector3.one;
//				go.transform.eulerAngles = Vector3.zero;
//				go.SetParent(wallToSnapTo);
//
//				break;
//
//			case "SouthWall":
//				//go.transform.localScale = Vector3.one;
//				go.transform.eulerAngles = new Vector3(0f,180f,0f);
//				go.SetParent(wallToSnapTo);
//
//				break;
//
//			case "EastWall":
//				//go.transform.localScale = Vector3.one;
//				go.transform.eulerAngles = new Vector3(0f,90f,0f);
//				go.SetParent(wallToSnapTo);
//
//				break;
//
//			case "WestWall":
//				//go.transform.localScale = Vector3.one;
//				go.transform.eulerAngles = new Vector3(0f,-90f,0f);
//				go.SetParent(wallToSnapTo);
//
//				break;
//
//
//			}
//
//		}
//
//	}

}



//Hair Trigger Up commented out code



//			//Here snap the paintintg to the wall:
//			//1. find where we are looking's position
//			//2. set the paitnings position to that x and y
//			//3. set the parent to the wall
//			Transform go = gameObject.transform.GetChild (1);
//			go.parent = null;
//			Transform wallToSnapTo = cam.GetComponent<HeadsetRaycast> ().myWall.transform;
//			//4. set the z to the wall's z
//			Debug.Log("You looked at the: " + wallToSnapTo.name);
//
//
//			switch (wallToSnapTo.name) {
//			case "SouthWall": //worried about x&y and snap to wall's z position
//				if(wallToSnapTo.childCount != 0){
//
//					go.transform.position = new Vector3(go.transform.position.x,wallToSnapTo.GetChild(0).transform.position.y,(wallToSnapTo.position.z+(wallToSnapTo.localScale.z/2)+.01f));
//				}
//				else{
//					go.transform.position = new Vector3(go.transform.position.x,go.transform.position.y,(wallToSnapTo.position.z+(wallToSnapTo.localScale.z/2)+.01f));
//				}
//				break;
//
//
//			case "NorthWall": //worried about x&y and snap to wall's z position
//				if(wallToSnapTo.childCount != 0){
//
//					go.transform.position = new Vector3(go.transform.position.x,wallToSnapTo.GetChild(0).transform.position.y,(wallToSnapTo.position.z-(wallToSnapTo.localScale.z/2)-.01f));
//				}
//				else{
//					go.transform.position = new Vector3(go.transform.position.x,go.transform.position.y,(wallToSnapTo.position.z-(wallToSnapTo.localScale.z/2)-.01f));
//				}
//				break;
//
//
//			case "EastWall": //worried about y&z and snap to wall's x position 
//				if(wallToSnapTo.childCount != 0){ 
//
//					go.transform.position = new Vector3 ((wallToSnapTo.position.x-(wallToSnapTo.localScale.z/2)-.01f),wallToSnapTo.GetChild(0).transform.position.y,go.transform.position.z);
//				}
//				else{
//					go.transform.position = new Vector3((wallToSnapTo.position.x-(wallToSnapTo.localScale.z/2)-.01f),go.transform.position.y,go.transform.position.z);
//				}
//				break;
//
//
//			case "WestWall": //worried about y&z and snap to wall's x position 
//				if(wallToSnapTo.childCount != 0){ 
//
//					go.transform.position = new Vector3 ((wallToSnapTo.position.x+(wallToSnapTo.localScale.z/2)+.01f),wallToSnapTo.GetChild(0).transform.position.y,go.transform.position.z);
//				}
//				else{
//					go.transform.position = new Vector3((wallToSnapTo.position.x+(wallToSnapTo.localScale.z/2)+.01f),go.transform.position.y,go.transform.position.z);
//				}
//				break;
//
//
//			}
//			//This is if walltosnapto is SouthWall
//
//
//
//
//
//
//			go.transform.localScale = Vector3.one;
//			go.SetParent(wallToSnapTo);
//
//			Debug.Log(gameObject.name + " Trigger Release");