using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AirSig;

public class CustomAigSig : BasedGestureHandle {

	// Use Unity inspector to drag AirSigManager reference here
//	public AirSigManager airsigManager;

	// Define callback for listening Developer-defined Gesture match event
	AirSigManager.OnDeveloperDefinedMatch developerGesture;

	// Callback method that will handle the event
	void HandleOnDeveloperDefinedMatch(long gestureId, string gesture, float score) {
		// handle match or fail to match
		textToUpdate = string.Format("<color=cyan>Gesture Match: {0} Score: {1}</color>", gesture.Trim(), score);
	}

	void Awake () {
		Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);

		// Update the display text
		textMode.text = string.Format("Mode: {0}", AirSigManager.Mode.DeveloperDefined.ToString());
		textResult.text = defaultResultText = "Pressing trigger and write symbol in the air\nReleasing trigger when finish";
		textResult.alignment = TextAnchor.UpperCenter;
		instruction.SetActive(false);
		ToggleGestureImage("All");

		developerGesture = new AirSigManager.OnDeveloperDefinedMatch(HandleOnDeveloperDefinedMatch);
		airsigManager.SetMode(AirSigManager.Mode.DeveloperDefined);
		airsigManager.SetDeveloperDefinedTarget(new List<string> { "HEART", "Triangle-AC" });
		airsigManager.SetClassifier("TestTriangleHeart", "");
		airsigManager.onDeveloperDefinedMatch += developerGesture;

		checkDbExist();

		airsigManager.SetTriggerStartKeys(
			AirSigManager.Controller.RIGHT_HAND,
			SteamVR_Controller.ButtonMask.Trigger,
			AirSigManager.PressOrTouch.PRESS);


		airsigManager.SetTriggerStartKeys(
			AirSigManager.Controller.LEFT_HAND,
			SteamVR_Controller.ButtonMask.Touchpad,
			AirSigManager.PressOrTouch.PRESS);
	}

	void OnDestroy() {
		// Unregistering callback
		airsigManager.onDeveloperDefinedMatch -= developerGesture;
	}

	void Update() {
		UpdateUIandHandleControl();
	}
}
