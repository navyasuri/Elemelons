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
	bool gestureTriggered = false, rightTriggered = false, leftTriggered = false;
	bool fireballTriggered = false;
	bool defenseTriggered = false;
	bool throwerTriggered = false;
	public string playerColor;

	// Bools for skill stone progression (public for debugging)
	public bool leftEnabled = true, fireballEnabled = true, defenseEnabled = true, throwerEnabled = true;

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


	// Use this for initialization
	void Awake() {
		Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);

		// Configure AirSig by specifying target 
		airsigManager.SetMode(AirSigManager.Mode.DeveloperDefined);
		airsigManager.SetClassifier("AtDefThrow", "");
		airsigManager.SetDeveloperDefinedTarget(new List<string> { "C", "AttackPunchSimple", "DefenseShieldCross" }); // Just in case the order here matters, list them in the order they were added to the pack on the AirSig website.
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
		
	// Necessary function for AirSig. Called by Network.cs once its WaitForRig() has found all the pieces:
	public void AirSigControlUpdate(GameObject leftPassedIn, GameObject rightPassedIn, GameObject headsetPassedIn, string playerColorPassedIn) {
		rightController = rightPassedIn.GetComponent<SteamVR_TrackedObject>(); // Get SteamVR script from the "controller (left)" GameObject.
		rightDevice = SteamVR_Controller.Input((int)rightController.index); // Automatically (safely) track whatever index SteamVR has assigned to the rightController today.
		rightParticles = rightPassedIn.transform.GetChild(0).Find("RightFlames").gameObject.GetComponent<ParticleSystem> (); // Note that you must go down one layer first, into the Hand prefab, to get the RightFlames child.

		leftController = leftPassedIn.GetComponent<SteamVR_TrackedObject>();
		leftDevice = SteamVR_Controller.Input((int)leftController.index);
		leftParticles = leftPassedIn.transform.GetChild(0).Find("LeftFlames").gameObject.GetComponent<ParticleSystem> ();

		headset = headsetPassedIn;
		playerColor = playerColorPassedIn;
	}

	void Update() {
		// If a gesture matched above the threshold, run the response and reset:
		if (gestureTriggered) {
			gestureTriggered = false;
			GestureResponse ();
		}
		UpdateUIandHandleControl();
	}
		
	// Handling developer defined gesture match callback - This is invoked when the Mode is set to Mode.DeveloperDefined and a gesture is recorded.
	// gestureId - a serial number
	// gesture - gesture matched or null if no match. Only gesture in SetDeveloperDefinedTarget list will be verified against
	// score - the confidence level of this identification. Above 1 is generally considered a match
	public void HandleOnDeveloperDefinedMatch(long gestureId, string gesture, float score) {
		// Good Match!
		// Actions are then triggered by Update() based on these flags:

		// Launch fireball above 0.85 match:
		if (score > 0.85 && gesture.Trim ().Equals ("AttackPunchSimple")) {
			Debug.Log (string.Format ("<color=cyan>Gesture Match: {0} Score: {1}</color>", gesture.Trim (), score));
			gestureTriggered = true;
			fireballTriggered = true;
		}

		// Active defense above 1 match:
		else if(score > 0.9 && gesture.Trim ().Equals ("DefenseShieldCross")) {
			Debug.Log (string.Format ("<color=cyan>Gesture Match: {0} Score: {1}</color>", gesture.Trim (), score));
			gestureTriggered = true;
			defenseTriggered = true;
		}

		// Flamethrower! (if above 0.9 match)
		else if (score > 1 && gesture.Trim ().Equals ("C")) {
			Debug.Log (string.Format ("<color=cyan>Gesture Match: {0} Score: {1}</color>", gesture.Trim (), score));
			gestureTriggered = true;
			throwerTriggered = true;
		}

		// Try again...
		else {
			Debug.Log(string.Format ("<color=red>Gesture Match: {0} Score: {1}</color>", gesture.Trim (), score));
		}
	}

	protected void checkDbExist() {
		bool isDbExist = airsigManager.IsDbExist;
		if (!isDbExist) {
			Debug.Log("<color=red>Cannot find DB files!\nMake sure\n'Assets/AirSig/StreamingAssets'\nis copied to\n'Assets/StreamingAssets'</color>");
		}
	}

	void OnDestroy() {
		// Unregistering callback
		airsigManager.onDeveloperDefinedMatch -= developerDefined;
	}
		
	protected void UpdateUIandHandleControl() {
		// Check that devices are found before trying to trigger gestures:
		if (rightDevice != null && leftDevice != null) {

			// If the right controller trigger is held down:
			if (rightDevice.GetPressDown (SteamVR_Controller.ButtonMask.Trigger)) {
				rightStart = headset.transform.position;
				rightStart.y -= 0.2f;  // Log the starting position as a little below the headset. (May need tweaks)
				rightParticles.Clear ();
				rightParticles.Play ();  // Play the gesture tracking particles.
			} else if (rightDevice.GetPressUp (SteamVR_Controller.ButtonMask.Trigger)) {
				rightEnd = rightController.transform.position; // Log where the right controller lets go of the trigger.
				rightDir = rightEnd - rightStart; // Determine vector to launch fireballs along.
				rightParticles.Stop (); // Stop gesture particles.
				rightTriggered = true;
			}

			// If the left controller trigger is held down:
			if (leftEnabled) {
				if (leftDevice.GetPressDown (SteamVR_Controller.ButtonMask.Trigger)) {
					leftStart = headset.transform.position;
					leftStart.y -= 0.2f;
					leftParticles.Clear ();
					leftParticles.Play ();
				} else if (leftDevice.GetPressUp (SteamVR_Controller.ButtonMask.Trigger)) {
					leftEnd = leftController.transform.position;
					leftDir = leftEnd - leftStart;
					leftParticles.Stop ();
					leftTriggered = true;
				}
			}
		}
	}

	// Spawns gesture-based prefabs relative to the player's headset (camera eye)
	public void GestureResponse() {
		// Get the position two units in front of the headset:
		Vector3 inFrontOfPlayer = headset.transform.position + (headset.transform.forward * 1f);

		// Attack!
		if (fireballTriggered && fireballEnabled) {
			// Instantiate the prefab GameObject on network, at the calling controller:
			string playerFireball = "Fireball" + playerColor;
			if (rightTriggered) {
				GameObject gestureResult = PhotonNetwork.Instantiate (playerFireball, rightController.transform.position, Quaternion.identity, 0);
				// Give the GameObject traits to be handled by GestureBehavior:
				//gestureResult.GetComponent<FireballBehavior> ().playerID = headset.GetInstanceID(); // Launched by this player.
				//PhotonView[] playerViews = headset.GetPhotonViewsInChildren();
				//Debug.Log(playerViews[0]);
				gestureResult.GetComponent<FireballBehavior> ().DoAfterStart (rightDir); // Do this, from the launching hand's position.
			} else if (leftEnabled && leftTriggered) {
				GameObject gestureResult = PhotonNetwork.Instantiate (playerFireball, leftController.transform.position, Quaternion.identity, 0);
				// Give the GameObject traits to be handled by GestureBehavior:
				//gestureResult.GetComponent<FireballBehavior> ().playerID = headset.GetInstanceID (); // Launched by this player.
				gestureResult.GetComponent<FireballBehavior> ().DoAfterStart (leftDir);
			}
			// Fireball has been launched, untrigger until next gesture match:
			fireballTriggered = false;
		}

		// Defend!
		if (defenseTriggered && defenseEnabled) {
			GameObject gestureResult = PhotonNetwork.Instantiate ("DefenseWall", inFrontOfPlayer, Quaternion.identity, 0);
			//gestureResult.GetComponent<DefenseBehavior> ().playerID = headset.GetInstanceID (); // Pass the ID of this player's headset (Camera (eye)).
			gestureResult.GetComponent<DefenseBehavior> ().DoAfterStart (headset.transform.forward);
			// Defense has been activated, untrigger until next gesture match:
			defenseTriggered = false;
		}

		// Flamethrower!
		if (throwerTriggered && throwerEnabled) {
			// Instantiate the prefab GameObject on network, at the calling controller:
			string playerFlameThrower = "FlameThrower" + playerColor + "(Clone)";
			if (rightTriggered) {
				//GameObject.Find("Flamethrower").GetComponent<ParticleSystem> ().Play();
				rightController.gameObject.transform.GetChild(0).Find(playerFlameThrower).gameObject.GetComponent<FlamethrowerBehavior> ().DoAfterStart();
			} else if (leftEnabled && leftTriggered) {
				//GameObject.Find("Flamethrower").GetComponent<ParticleSystem> ().Play();
				leftController.gameObject.transform.GetChild(0).Find(playerFlameThrower).gameObject.GetComponent<FlamethrowerBehavior> ().DoAfterStart();
			}
			// Flamethrower has been activated, untrigger until next gesture match:
			throwerTriggered = false;
		}

		// Reset controller triggers before checking for new gesture updates:
		rightTriggered = false;
		leftTriggered = false;

		// =======================================
		// TODO: Turn this bug into a feature:
		//       When shoot fireballs is not set to false right away,
		//       the player spawns a fire storm.
		//       Hook this up to a for(loop) with a counter up to, say 20, before setting to false.

// Needs work, currently launches too fast and in a strange fan
//		if (firestormTriggered && firestormEnabled) {
//			if (rightTriggered) {
//				for (int i = 0; i < 20; i++) {
//					GameObject gestureResult = PhotonNetwork.Instantiate ("AttackBlue", rightController.transform.position, Quaternion.identity, 0);
//					// Give the GameObject traits to be handled by GestureBehavior:
//					gestureResult.GetComponent<FireballBehavior> ().playerID = headset.GetInstanceID (); // Launched by this player.
//					gestureResult.GetComponent<FireballBehavior> ().DoAfterStart (rightDir); // Do this, from the launching hand's position.
//				}
//			} else if (leftEnabled && leftTriggered) {
//				for (int i = 0; i < 20; i++) {
//					GameObject gestureResult = PhotonNetwork.Instantiate ("Attack", leftController.transform.position, Quaternion.identity, 0);
//					// Give the GameObject traits to be handled by GestureBehavior:
//					gestureResult.GetComponent<FireballBehavior> ().playerID = headset.GetInstanceID (); // Launched by this player.
//					gestureResult.GetComponent<FireballBehavior> ().DoAfterStart (leftDir);
//				}
//			}
//			// Firestorm unleashed, untrigger until next gesture match:
//			firestormTriggered = false;
//		}

	}
}
