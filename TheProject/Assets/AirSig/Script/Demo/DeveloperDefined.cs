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
	public SteamVR_TrackedObject rightController;
	protected SteamVR_Controller.Device rightDevice;
	protected ParticleSystem rightParticles;

	public SteamVR_TrackedObject leftController;
	protected SteamVR_Controller.Device leftDevice;
	protected ParticleSystem leftParticles;

	// UI for displaying current status and operation results 
	public Text textMode;
	public Text textResult;
	public GameObject instruction;
	public GameObject cHeartDown;

	protected string textToUpdate;

	protected readonly string DEFAULT_INSTRUCTION_TEXT = "Pressing trigger and write in the air\nReleasing trigger when finish";
	protected string defaultResultText;

	// Set by the callback function to run this action in the next UI call
	protected Action nextUiAction;
	protected IEnumerator uiFeedback;

    // Callback for receiving signature/gesture progression or identification results
    AirSigManager.OnDeveloperDefinedMatch developerDefined;

	public string Attack = "Attack";

	bool shooting = false;



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
		//Debug.Log ("You drew: " + gesture);
//		Debug.Log(string.Format("<color=cyan>Gesture Match: {0} Score: {1}</color>", gesture.Trim (), score));
		if (score > 1.2) {
			Debug.Log(string.Format ("<color=green>Gesture Match: {0} Score: {1}</color>", gesture.Trim (), score));

			if (gesture.Trim().Equals ("AttackPunch")) {
				//SpawnFireball (leftController);
			}

			if (gesture.Trim ().Equals ("DefenseShieldCross")) {
				Debug.Log ("A wild fire shield appeared!");
			}
		} else {
			Debug.Log(string.Format ("<color=red>Gesture Match: {0} Score: {1}</color>", gesture.Trim (), score));
		}
		//SpawnFireball(leftController);

		//GameObject.Find ("FireballSpawner").GetComponent<FireBallSpawner> ().SpawnFireball ();
		shooting = true;
    }

    // Use this for initialization
    void Awake() {
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);

        // Update the display text
        textMode.text = string.Format("Mode: {0}", AirSigManager.Mode.DeveloperDefined.ToString());
        textResult.text = defaultResultText = "Pressing trigger and write symbol in the air\nReleasing trigger when finish";
        textResult.alignment = TextAnchor.UpperCenter;
        instruction.SetActive(false);
        //ToggleGestureImage("All");

		airsigManager.SetMode(AirSigManager.Mode.DeveloperDefined);
		airsigManager.SetClassifier("AttackDefenseGestureProfile", "");
		airsigManager.SetDeveloperDefinedTarget(new List<string> { "AttackPunch", "DefenseShieldCross" });

        // Configure AirSig by specifying target 
        developerDefined = new AirSigManager.OnDeveloperDefinedMatch(HandleOnDeveloperDefinedMatch);
        airsigManager.onDeveloperDefinedMatch += developerDefined;
        

        

        checkDbExist();

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
		Debug.Log (leftController);

		if (shooting) {
			SpawnFireball (leftController);
			shooting = false;
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

	protected void ToggleGestureImage(string target) {
		if ("All".Equals(target)) {
			cHeartDown.SetActive(true);
			foreach (Transform child in cHeartDown.transform) {
				child.gameObject.SetActive(true);
			}
		} else if ("Heart".Equals(target)) {
			cHeartDown.SetActive(true);
			foreach (Transform child in cHeartDown.transform) {
				if (child.name == "Heart") {
					child.gameObject.SetActive(true);
				} else {
					child.gameObject.SetActive(false);
				}
			}
		} else if ("C".Equals(target)) {
			cHeartDown.SetActive(true);
			foreach (Transform child in cHeartDown.transform) {
				if (child.name == "C") {
					child.gameObject.SetActive(true);
				} else {
					child.gameObject.SetActive(false);
				}
			}
		} else if ("Down".Equals(target)) {
			cHeartDown.SetActive(true);
			foreach (Transform child in cHeartDown.transform) {
				if (child.name == "Down") {
					child.gameObject.SetActive(true);
				} else {
					child.gameObject.SetActive(false);
				}
			}
		} else {
			cHeartDown.SetActive(false);
		}
	}

	protected IEnumerator setResultTextForSeconds(string text, float seconds, string defaultText = "") {
		string temp = textResult.text;
		textResult.text = text;
		yield return new WaitForSeconds(seconds);
		textResult.text = defaultText;
	}

	protected void checkDbExist() {
		bool isDbExist = airsigManager.IsDbExist;
		if (!isDbExist) {
			textResult.text = "<color=red>Cannot find DB files!\nMake sure\n'Assets/AirSig/StreamingAssets'\nis copied to\n'Assets/StreamingAssets'</color>";
			textMode.text = "";
			instruction.SetActive(false);
			cHeartDown.SetActive(false);
		}
	}

	protected void UpdateUIandHandleControl() {
		if (Input.GetKeyUp(KeyCode.Escape)) {
			Application.Quit();
		}

		//Debug.Log (textToUpdate);

		if (null != textToUpdate) {
			if(uiFeedback != null) StopCoroutine(uiFeedback);
			uiFeedback = setResultTextForSeconds(textToUpdate, 5.0f, defaultResultText);
			StartCoroutine(uiFeedback);
			textToUpdate = null;
		}

		//Debug.Log ("Left Hand Index is " + (int)leftController.index);
		//Debug.Log ("Right hand Index is " + (int)rightController.index);
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

		if (nextUiAction != null) {
			nextUiAction();
			nextUiAction = null;
		}
	}



	/* =================================================================
	 * 
	 *  Lastly, here are any necessary custom scripts to network AirSig:
	 * 
	 * =================================================================
	 */

	public void AirSigControlUpdate(GameObject leftPassedIn, GameObject rightPassedIn) {
		rightController = rightPassedIn.GetComponent<SteamVR_TrackedObject>();
		rightDevice = SteamVR_Controller.Input((int)rightController.index);
		rightParticles = rightPassedIn.GetComponent<ParticleSystem>();

		leftController = leftPassedIn.GetComponent<SteamVR_TrackedObject>();
		leftDevice = SteamVR_Controller.Input((int)leftController.index);
		leftParticles = leftPassedIn.GetComponent<ParticleSystem>();

		Debug.Log ("AirSig left controller is: " + leftController);
	}

	public void SpawnFireball(SteamVR_TrackedObject controller) {
		Vector3 attackVector = controller.transform.position + (transform.forward * 1f);
		//Vector3 attackVector = Vector3.zero;
		//attackVector.y += 0.25f; // Move it up a tad for the aesthetics.
		var newAttack = PhotonNetwork.Instantiate (Attack, attackVector, controller.transform.rotation, 0); // Create the attack.
		//newAttack.GetComponent<AttackBehavior>().attackerID = gameObject.GetInstanceID (); // Note ID of attacking player. (To avoid self-damage)
	}
}