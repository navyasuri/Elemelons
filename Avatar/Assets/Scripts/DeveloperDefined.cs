using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using AirSig;
using Photon;

public class DeveloperDefined : Photon.MonoBehaviour {

	// Reference to AirSigManager for setting operation mode and registering listener
	public AirSigManager airsigManager;

	// Vive objects
	protected GameObject headset;
	protected SteamVR_TrackedObject rightController;
	protected SteamVR_Controller.Device rightDevice;
	protected ParticleSystem rightParticles;

	protected SteamVR_TrackedObject leftController;
	protected SteamVR_Controller.Device leftDevice;
	protected ParticleSystem leftParticles;

	// Gesture control
	Vector3 rightStart;
	Vector3 rightEnd;
	Vector3 rightDir;
	Vector3 leftStart;
	Vector3 leftEnd;
	Vector3 leftDir;
	bool rightAttackReady = false;
	bool leftAttackReady = false;
	bool gestureTriggered = false;
	bool attackTriggered = false;
	bool defenseTriggered = false;
	bool throwerTriggered = false;

	//ENABLED bools
	bool leftAttackEnabled = true, rightAttackEnabled = true, defenseEnabled = true, throwerEnabled = true;

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



    // Handling developer defined gesture match callback - This is invoked when the Mode is set to Mode.DeveloperDefined and a gesture is recorded.
    // gestureId - a serial number
    // gesture - gesture matched or null if no match. Only gesture in SetDeveloperDefinedTarget list will be verified against
    // score - the confidence level of this identification. Above 1 is generally considered a match
    public void HandleOnDeveloperDefinedMatch(long gestureId, string gesture, float score) {
		// Good Match!
		if (score > 0.8) {
			Debug.Log(string.Format ("<color=cyan>Gesture Match: {0} Score: {1}</color>", gesture.Trim (), score));
			//textToUpdate = string.Format ("<color=cyan>Gesture Match: {0} Score: {1}</color>", gesture.Trim (), score);

			// Launch fireball
			if (gesture.Trim().Equals ("AttackPunchSimple")) {
				// Actions are then triggered by Update() based on these flags:
				attackTriggered = true;
				gestureTriggered = true;
			}

			// Defend yo self
			if (gesture.Trim ().Equals ("DefenseShieldCross")) {
				defenseTriggered = true;
				gestureTriggered = true;
			}

			if (gesture.Trim ().Equals ("C")) {
				throwerTriggered = true;
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
//        instruction.SetActive(false);

		// Configure AirSig by specifying target 
		airsigManager.SetMode(AirSigManager.Mode.DeveloperDefined);
		airsigManager.SetClassifier("AtDefThrow", "");
		airsigManager.SetDeveloperDefinedTarget(new List<string> { "AttackPunchSimple", "DefenseShieldCross", "C" });
        developerDefined = new AirSigManager.OnDeveloperDefinedMatch(HandleOnDeveloperDefinedMatch);
        airsigManager.onDeveloperDefinedMatch += developerDefined;
        checkDbExist();

		// Set each controller as an AirSig gesture trigger, and which button activates the recording
        airsigManager.SetTriggerStartKeys(
            AirSigManager.Controller.RIGHT_HAND,
			SteamVR_Controller.ButtonMask.Trigger,
            AirSigManager.PressOrTouch.PRESS);

        airsigManager.SetTriggerStartKeys(
            AirSigManager.Controller.LEFT_HAND,
			SteamVR_Controller.ButtonMask.Trigger,
            AirSigManager.PressOrTouch.PRESS);
    }
		
    void OnDestroy() {
        // Unregistering callback
        airsigManager.onDeveloperDefinedMatch -= developerDefined;
    }

    void Update() {
		if (gestureTriggered) {
			GestureResponse ();
			gestureTriggered = false;
		}
		UpdateUIandHandleControl();
    }

	protected void checkDbExist() {
		bool isDbExist = airsigManager.IsDbExist;
		if (!isDbExist) {
			Debug.Log("<color=red>Cannot find DB files!\nMake sure\n'Assets/AirSig/StreamingAssets'\nis copied to\n'Assets/StreamingAssets'</color>");
		}
	}

	protected void UpdateUIandHandleControl() {
		
		if (leftAttackReady || rightAttackReady) {
			leftAttackReady = false;
			rightAttackReady = false;
		}

		if (rightDevice != null && leftDevice != null) {
			
			if (rightDevice.GetPressDown (SteamVR_Controller.ButtonMask.Trigger)) {
				rightStart = headset.transform.position;
				rightStart.y -= 0.2f;
				rightParticles.Clear ();
				rightParticles.Play ();

			} else if (rightDevice.GetPressUp (SteamVR_Controller.ButtonMask.Trigger)) {
				rightEnd = rightController.transform.position;
				rightAttackReady = true;
				rightDir = rightEnd - rightStart;
				rightParticles.Stop ();
			}

			if (leftDevice.GetPressDown (SteamVR_Controller.ButtonMask.Trigger)) {
				leftStart = headset.transform.position;
				leftStart.y -= 0.2f;
				leftParticles.Clear ();
				leftParticles.Play ();
			} else if (leftDevice.GetPressUp (SteamVR_Controller.ButtonMask.Trigger)) {
				leftEnd = leftController.transform.position;
				leftAttackReady = true;
				leftDir = leftEnd - leftStart;
				leftParticles.Stop ();
			}

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
	



	// Called by Network.cs once WaitForRig() has gound all the pieces:
	public void AirSigControlUpdate(GameObject leftPassedIn, GameObject rightPassedIn, GameObject headsetPassedIn) {
		rightController = rightPassedIn.GetComponent<SteamVR_TrackedObject>();
		rightDevice = SteamVR_Controller.Input((int)rightController.index);
		rightParticles = GameObject.Find("RightFlames").GetComponent<ParticleSystem> ();

		leftController = leftPassedIn.GetComponent<SteamVR_TrackedObject>();
		leftDevice = SteamVR_Controller.Input((int)leftController.index);
		leftParticles = GameObject.Find("LeftFlames").GetComponent<ParticleSystem> ();

		headset = headsetPassedIn;
	}

	// Spawns gesture-based prefabs relative to the player's headset (camera eye)
	public void GestureResponse() {
		// Get the position one unit in front of the headset:
		Vector3 spawnVector = headset.transform.position + (headset.transform.forward * 2f);

		if (attackTriggered) { // Attack!
			// Instantiate the prefab GameObject on network, at the calling controller:

			if (rightAttackReady) {
				if (rightAttackEnabled) {
					GameObject gestureResult = PhotonNetwork.Instantiate ("AttackBlue", rightController.transform.position, Quaternion.identity, 0);
					// Give the GameObject traits to be handled by GestureBehavior:
					gestureResult.GetComponent<AllFireBehavior> ().fireball = true; // Is an attack.
					gestureResult.GetComponent<AllFireBehavior> ().playerID = headset.GetInstanceID (); // Launched by this player.
					gestureResult.GetComponent<AllFireBehavior> ().DoAfterStart (rightDir); // Do this, from the launching hand's position.
				}
			} else if (leftAttackReady) {
				if (leftAttackEnabled) {
					GameObject gestureResult = PhotonNetwork.Instantiate ("Attack", leftController.transform.position, Quaternion.identity, 0);
					// Give the GameObject traits to be handled by GestureBehavior:
					gestureResult.GetComponent<AllFireBehavior> ().fireball = true; // Is an attack.
					gestureResult.GetComponent<AllFireBehavior> ().playerID = headset.GetInstanceID (); // Launched by this player.
					gestureResult.GetComponent<AllFireBehavior> ().DoAfterStart (leftDir);
				}
			}
			attackTriggered = false;
		} else if (defenseTriggered) {// Defend!
			if (defenseEnabled) {
				GameObject gestureResult = PhotonNetwork.Instantiate ("DefenseWall", spawnVector, Quaternion.identity, 0);
				gestureResult.GetComponent<AllFireBehavior> ().defense = true;
				gestureResult.GetComponent<AllFireBehavior> ().playerID = headset.GetInstanceID (); // Pass the ID of this player's headset.
				gestureResult.GetComponent<AllFireBehavior> ().DoAfterStart (headset.transform.forward);
			}
			defenseTriggered = false;
		} else if (throwerTriggered) {
			if (throwerEnabled){
			GameObject gestureResult = PhotonNetwork.Instantiate ("AttackBlue", rightController.transform.position, Quaternion.identity, 0);
			// Give the GameObject traits to be handled by GestureBehavior:
			gestureResult.GetComponent<AllFireBehavior> ().flamethrower = true; // Shooting flames now
			gestureResult.GetComponent<AllFireBehavior> ().playerID = headset.GetInstanceID (); // Launched by this player.
//			gestureResult.GetComponent<AllFireBehavior> ().DoAfterStart (rightDir);
			}

			throwerTriggered = false;
		}

		// Set a timer with something of .2 seconds to avoid double attacking using both controllers.


		// =======================================
		// TODO: Turn this bug into a feature:
		//       When shoot fireballs is not set to false right away,
		//       the player spawns a fire storm.
		//       Hook this up to a for(loop) with a counter up to, say 20, before setting to false.
	}
}