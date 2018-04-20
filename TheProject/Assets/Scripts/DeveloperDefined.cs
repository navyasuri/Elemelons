using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using AirSig;

public class DeveloperDefined : MonoBehaviour {

	// Reference to AirSigManager for setting operation mode and registering listener
	public AirSigManager airsigManager;

	// Reference to the vive controllers
	protected SteamVR_TrackedObject rightController;
	protected SteamVR_Controller.Device rightDevice;
	protected ParticleSystem rightParticles;

	protected SteamVR_TrackedObject leftController;
	protected SteamVR_Controller.Device leftDevice;
	protected ParticleSystem leftParticles;

	protected GameObject headset;

	// UI for displaying current status and operation results 
	//public Text textMode;
	//public Text textResult;
	//public GameObject instruction;
	//public GameObject cHeartDown;

	protected string textToUpdate;

	protected readonly string DEFAULT_INSTRUCTION_TEXT = "Pressing trigger and write in the air\nReleasing trigger when finish";
	protected string defaultResultText;

	// Set by the callback function to run this action in the next UI call
	protected Action nextUiAction;
	protected IEnumerator uiFeedback;

    // Callback for receiving signature/gesture progression or identification results
    AirSigManager.OnDeveloperDefinedMatch developerDefined;

	public string attack = "Attack";
	public string defenseWall = "DefenseWall";

	public bool gestureTriggered = false;
	public bool attackTriggered = false;
	public bool defenseTriggered = false;



	/* ================================================================
	 * 
	 *  The following are the pre-scriptmerge DeveloperDefined methods:
	 * 
	 * ================================================================
	 */

    // Handling developer defined gesture match callback - This is invoked when the Mode is set to Mode.DeveloperDefined and a gesture is recorded.
    // gestureId - a serial number
    // gesture - gesture matched or null if no match. Only guesture in SetDeveloperDefinedTarget range will be verified against
    // score - the confidence level of this identification. Above 1 is generally considered a match
    public void HandleOnDeveloperDefinedMatch(long gestureId, string gesture, float score) {
		// Good Match!
		if (score > 0.8) {
			Debug.Log(string.Format ("<color=cyan>Gesture Match: {0} Score: {1}</color>", gesture.Trim (), score));
			//textToUpdate = string.Format ("<color=cyan>Gesture Match: {0} Score: {1}</color>", gesture.Trim (), score);

			// Launch fireball
			if (gesture.Trim().Equals ("AttackPunchSimple")) {
				attackTriggered = true;
				gestureTriggered = true;
				// Actions are then triggered by Update()
			}

			// Defend yo self
			if (gesture.Trim ().Equals ("DefenseShieldCross")) {
				defenseTriggered = true;
				gestureTriggered = true;
			}
		
		// Try again...
		} else {
			Debug.Log(string.Format ("<color=red>Gesture Match: {0} Score: {1}</color>", gesture.Trim (), score));
		}
    }

    // Use this for initialization
    void Awake() {
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);

        // Update the display text
//        textMode.text = string.Format("Mode: {0}", AirSigManager.Mode.DeveloperDefined.ToString());
//        textResult.text = defaultResultText = "Pressing grip and write symbol in the air\nReleasing grip when finished.";
//        textResult.alignment = TextAnchor.UpperCenter;
        //instruction.SetActive(false);

		// Configure AirSig by specifying target 
		airsigManager.SetMode(AirSigManager.Mode.DeveloperDefined);
		airsigManager.SetClassifier("NewAttackDefense", "");
		airsigManager.SetDeveloperDefinedTarget(new List<string> { "AttackPunchSimple", "DefenseShieldCross" });
        developerDefined = new AirSigManager.OnDeveloperDefinedMatch(HandleOnDeveloperDefinedMatch);
        airsigManager.onDeveloperDefinedMatch += developerDefined;
        
        checkDbExist();

		// Set each controller as an AirSig gesture trigger, and which button activates the recording
        airsigManager.SetTriggerStartKeys(
            AirSigManager.Controller.RIGHT_HAND,
			SteamVR_Controller.ButtonMask.Grip,
            AirSigManager.PressOrTouch.PRESS);

        airsigManager.SetTriggerStartKeys(
            AirSigManager.Controller.LEFT_HAND,
			SteamVR_Controller.ButtonMask.Grip,
            AirSigManager.PressOrTouch.PRESS);
    }
		
    void OnDestroy() {
        // Unregistering callback
        airsigManager.onDeveloperDefinedMatch -= developerDefined;
    }

    void Update() {
        Vector3 attackVec = UpdateUIandHandleControl();

		if (gestureTriggered) {
			GestureResponse (headset, attackVec);
			gestureTriggered = false;
		}
    }



	/* =============================================
	 * 
	 *  And now for the BasedGestureHandle scripts:
	 * 
	 * =============================================
	 */

//	protected string GetDefaultIntructionText() {
//		return DEFAULT_INSTRUCTION_TEXT;
//	}

	// All for updating the AirSig UI gesture match results, from the Demo scene:

//	protected void ToggleGestureImage(string target) {
//		if ("All".Equals(target)) {
//			cHeartDown.SetActive(true);
//			foreach (Transform child in cHeartDown.transform) {
//				child.gameObject.SetActive(true);
//			}
//		} else if ("Heart".Equals(target)) {
//			cHeartDown.SetActive(true);
//			foreach (Transform child in cHeartDown.transform) {
//				if (child.name == "Heart") {
//					child.gameObject.SetActive(true);
//				} else {
//					child.gameObject.SetActive(false);
//				}
//			}
//		} else if ("C".Equals(target)) {
//			cHeartDown.SetActive(true);
//			foreach (Transform child in cHeartDown.transform) {
//				if (child.name == "C") {
//					child.gameObject.SetActive(true);
//				} else {
//					child.gameObject.SetActive(false);
//				}
//			}
//		} else if ("Down".Equals(target)) {
//			cHeartDown.SetActive(true);
//			foreach (Transform child in cHeartDown.transform) {
//				if (child.name == "Down") {
//					child.gameObject.SetActive(true);
//				} else {
//					child.gameObject.SetActive(false);
//				}
//			}
//		} else {
//			cHeartDown.SetActive(false);
//		}
//	}
//
//	protected IEnumerator setResultTextForSeconds(string text, float seconds, string defaultText = "") {
//		string temp = textResult.text;
//		textResult.text = text;
//		yield return new WaitForSeconds(seconds);
//		textResult.text = defaultText;
//	}

	protected void checkDbExist() {
		bool isDbExist = airsigManager.IsDbExist;
		if (!isDbExist) {
			Debug.Log("<color=red>Cannot find DB files!\nMake sure\n'Assets/AirSig/StreamingAssets'\nis copied to\n'Assets/StreamingAssets'</color>");
		}
	}

	protected Vector3 UpdateUIandHandleControl() {
		if (Input.GetKeyUp(KeyCode.Escape)) {
			Application.Quit();
		}


		Vector3 starter = Vector3.one;
		Vector3 end = Vector3.one;
		if (rightDevice != null && leftDevice != null) {
			if (rightDevice.GetPressDown (SteamVR_Controller.ButtonMask.Grip)) {
				starter = rightDevice.transform.pos;
				rightParticles.Clear ();
				rightParticles.Play ();
			} else if (rightDevice.GetPressUp (SteamVR_Controller.ButtonMask.Grip)) {
				end = rightDevice.transform.pos;
				rightParticles.Stop ();
			}

			if (leftDevice.GetPressDown (SteamVR_Controller.ButtonMask.Grip)) {
				starter = leftDevice.transform.pos;
				leftParticles.Clear ();
				leftParticles.Play ();
			} else if (leftDevice.GetPressUp (SteamVR_Controller.ButtonMask.Grip)) {
				end = leftDevice.transform.pos;
				leftParticles.Stop ();
			}

			Vector3 dir = end - starter;
			return dir;
		}

// More script for the AirSig Demo UI:
//		if (null != textToUpdate) {
//			if(uiFeedback != null) StopCoroutine(uiFeedback);
//			uiFeedback = setResultTextForSeconds(textToUpdate, 5.0f, defaultResultText);
//			StartCoroutine(uiFeedback);
//			textToUpdate = null;
//		}
//
//		if (nextUiAction != null) {
//			nextUiAction();
//			nextUiAction = null;
//		}
	}



	/* =================================================================
	 * 
	 *  Lastly, here are any necessary custom scripts to network AirSig:
	 * 
	 * =================================================================
	 */

	public void AirSigControlUpdate(GameObject leftPassedIn, GameObject rightPassedIn, GameObject headsetPassedIn) {
		rightController = rightPassedIn.GetComponent<SteamVR_TrackedObject>();
		rightDevice = SteamVR_Controller.Input((int)rightController.index);
		rightParticles = rightPassedIn.GetComponent<ParticleSystem>();

		leftController = leftPassedIn.GetComponent<SteamVR_TrackedObject>();
		leftDevice = SteamVR_Controller.Input((int)leftController.index);
		leftParticles = leftPassedIn.GetComponent<ParticleSystem>();

		headset = headsetPassedIn;
	}

	// Spawns gesture-based prefabs relative to the player's headset (camera eye)
	public void GestureResponse(GameObject headset, Vector3 attackVec) {
		// Get the position one unit in front of the headset:
		Vector3 Vector = headset.transform.position + (headset.transform.forward * 1f);

		if (attackTriggered) { // Attack!
			GameObject gestureResult = PhotonNetwork.Instantiate (attack, attackVec, Quaternion.identity, 0);
			gestureResult.GetComponent<GestureBehavior> ().attack = true;
			// Pass Camera eye and children (hopefully) as the player here, for special properties
			gestureResult.GetComponent<GestureBehavior> ().playerID = headset.GetInstanceID (); // Pass the ID of this player's headset.
			// 'Activate' the prefab
			gestureResult.GetComponent<GestureBehavior> ().Spawn (headset.transform.forward);
			attackTriggered = false;
		}

		if (defenseTriggered) { // Defend!
			GameObject gestureResult = PhotonNetwork.Instantiate (defenseWall, Vector, Quaternion.identity, 0);
			Debug.Log (headset);
			gestureResult.GetComponent<GestureBehavior> ().defense = true;
			gestureResult.GetComponent<GestureBehavior> ().playerID = headset.GetInstanceID (); // Pass the ID of this player's headset.
			gestureResult.GetComponent<GestureBehavior> ().Spawn (headset.transform.forward);
			defenseTriggered = false;
		}

		// Set a timer with something of .2 seconds to avoid double attacking using both controllers.


		// =======================================
		// TODO: Turn this bug into a feature:
		//       When shoot fireballs is not set to false right away,
		//       the player spawns a fire storm.
		//       Hook this up to a for(loop) with a counter up to, say 20, before setting to false.
	}
}