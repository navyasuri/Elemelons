using System.Collections.Generic;
using UnityEngine;

using AirSig;

public class DeveloperDefined : BasedGestureHandle {

    // Callback for receiving signature/gesture progression or identification results
    AirSigManager.OnDeveloperDefinedMatch developerDefined;

	public string Attack = "Attack";
	SteamVR_TrackedObject left;
	SteamVR_TrackedObject right;


    // Handling developer defined gesture match callback - This is invoked when the Mode is set to Mode.DeveloperDefined and a gesture is recorded.
    // gestureId - a serial number
    // gesture - gesture matched or null if no match. Only guesture in SetDeveloperDefinedTarget range will be verified against
    // score - the confidence level of this identification. Above 1 is generally considered a match
    void HandleOnDeveloperDefinedMatch(long gestureId, string gesture, float score) {
		//Debug.Log ("You drew: " + gesture);
        textToUpdate = string.Format("<color=cyan>Gesture Match: {0} Score: {1}</color>", gesture.Trim(), score);
		Vector3 attackVector = left.transform.position + (transform.forward * 1f);
		attackVector.y += 0.25f; // Move it up a tad for the aesthetics.
		var newAttack = PhotonNetwork.Instantiate (Attack, attackVector, left.transform.rotation, 0); // Create the attack.
		newAttack.GetComponent<AttackBehavior> ().attackerID = gameObject.GetInstanceID (); // Note ID of attacking player. (To avoid self-damage)
		//newAttack.GetComponent<AttackBehavior> ().attackerColor = playerColor;
    }

    // Use this for initialization
    void Awake() {
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);

		left = leftController;
		right = rightController;

        // Update the display text
        textMode.text = string.Format("Mode: {0}", AirSigManager.Mode.DeveloperDefined.ToString());
        textResult.text = defaultResultText = "Pressing trigger and write symbol in the air\nReleasing trigger when finish";
        textResult.alignment = TextAnchor.UpperCenter;
        instruction.SetActive(false);
        //ToggleGestureImage("All");

        // Configure AirSig by specifying target 
        developerDefined = new AirSigManager.OnDeveloperDefinedMatch(HandleOnDeveloperDefinedMatch);
        airsigManager.onDeveloperDefinedMatch += developerDefined;
        airsigManager.SetMode(AirSigManager.Mode.DeveloperDefined);
		airsigManager.SetDeveloperDefinedTarget(new List<string> { "AttackPunch", "DefenseShieldCross" });
        airsigManager.SetClassifier("AttackDefenseGestureProfile", "");

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
    }
}