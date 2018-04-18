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

	//protected string textToUpdate;

	protected readonly string DEFAULT_INSTRUCTION_TEXT = "Pressing trigger and write in the air\nReleasing trigger when finish";
	protected string defaultResultText;

	// Set by the callback function to run this action in the next UI call
	//protected Action nextUiAction;
	//protected IEnumerator uiFeedback;

    // Callback for receiving signature/gesture progression or identification results
    AirSigManager.OnDeveloperDefinedMatch developerDefined;

	public string Attack = "Attack";

	bool ShootFireball = false;



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
			Debug.Log(string.Format ("<color=green>Gesture Match: {0} Score: {1}</color>", gesture.Trim (), score));

			// Launch fireball
			if (gesture.Trim().Equals ("AttackPunch")) {
				// Actions triggered by Update()
				ShootFireball = true;
			}

			// Defend yo self
			if (gesture.Trim ().Equals ("DefenseShieldCross")) {
				Debug.Log ("A wild fire shield appeared!");
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
        //textMode.text = string.Format("Mode: {0}", AirSigManager.Mode.DeveloperDefined.ToString());
        //textResult.text = defaultResultText = "Pressing trigger and write symbol in the air\nReleasing trigger when finish";
        //textResult.alignment = TextAnchor.UpperCenter;
        //instruction.SetActive(false);

		// Configure AirSig by specifying target 
		airsigManager.SetMode(AirSigManager.Mode.DeveloperDefined);
		airsigManager.SetClassifier("AttackDefenseGestureProfile", "");
		airsigManager.SetDeveloperDefinedTarget(new List<string> { "AttackPunch", "DefenseShieldCross" });
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
        UpdateUIandHandleControl();

		if (ShootFireball) {
			SpawnFireball (headset);
			ShootFireball = false;
		}
    }



	/* =============================================
	 * 
	 *  And now for the BasedGestureHandle scripts:
	 * 
	 * =============================================
	 */

	protected string GetDefaultIntructionText() {
		return DEFAULT_INSTRUCTION_TEXT;
	}

	// All for updating the AirSig UI gestuer match results, from the Demo scene:
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

	protected void UpdateUIandHandleControl() {
		if (Input.GetKeyUp(KeyCode.Escape)) {
			Application.Quit();
		}

		if (rightDevice != null && leftDevice != null) {
			if (rightDevice.GetPressDown (SteamVR_Controller.ButtonMask.Grip)) {
				rightParticles.Clear ();
				rightParticles.Play ();
			} else if (rightDevice.GetPressUp (SteamVR_Controller.ButtonMask.Grip)) {
				rightParticles.Stop ();
			}

			if (leftDevice.GetPressDown (SteamVR_Controller.ButtonMask.Grip)) {
				leftParticles.Clear ();
				leftParticles.Play ();
			} else if (leftDevice.GetPressUp (SteamVR_Controller.ButtonMask.Grip)) {
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

	public void SpawnFireball(GameObject headset) {
		Vector3 attackVector = headset.transform.position + (headset.transform.forward * 1f);
		//Vector3 attackVector = headset.transform.position;

		//attackVector.y += 0.25f; // Move it up a tad for the aesthetics.
		var newAttack = PhotonNetwork.Instantiate (Attack, attackVector, Quaternion.identity, 0); // Create the attack.
		newAttack.GetComponent<AttackBehavior>().Launch(headset.transform.forward);

//		newAttack.GetComponent<AttackBehavior>().attackerID = gameObject.GetInstanceID (); // Note ID of attacking player. (To avoid self-damage)
	}
}