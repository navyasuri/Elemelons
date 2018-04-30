using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class switchController : MonoBehaviour {

	private SteamVR_TrackedController device;
	public GameObject unclenched;
	public GameObject clenched;

	// Use this for initialization
	void Start () {
		device = GetComponent<SteamVR_TrackedController> ();
		device.TriggerClicked += Trigger;

//		this.gameObject.GetComponent<MeshRenderer> ().material = unclenched.gameObject.GetComponent<MeshRenderer> ().material;
//		this.gameObject.GetComponent<MeshFilter> ().mesh = unclenched.gameObject.GetComponent<MeshFilter> ().mesh;
	}
	
	// Update is called once per frame
	void Update () {

		//if trigger pressed
//		(device.OnTriggerClicked(ClickedEventArgs)
//		device.TriggerClicked += Trigger;
//		if (device.gripped) {
//			this.gameObject.GetComponent<MeshRenderer> ().material = clenched.gameObject.GetComponent<MeshRenderer> ().material;
//			this.gameObject.GetComponent<MeshFilter> ().mesh = clenched.gameObject.GetComponent<MeshFilter> ().mesh;
//		} else {
//			this.gameObject.GetComponent<MeshRenderer> ().material = unclenched.gameObject.GetComponent<MeshRenderer> ().material;
//			this.gameObject.GetComponent<MeshFilter> ().mesh = unclenched.gameObject.GetComponent<MeshFilter> ().mesh;
//		}
		
	}

	void Trigger (object sender, ClickedEventArgs e) {
//		this.gameObject.GetComponent<MeshRenderer> ().material = clenched.gameObject.GetComponent<MeshRenderer> ().material;
//		this.gameObject.GetComponent<MeshFilter> ().mesh = clenched.gameObject.GetComponent<MeshFilter> ().mesh;
	}
	
}
