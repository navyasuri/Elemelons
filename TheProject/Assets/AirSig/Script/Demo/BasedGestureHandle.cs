using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using AirSig;

public class BasedGestureHandle : MonoBehaviour {

    // Reference to AirSigManager for setting operation mode and registering listener
    public AirSigManager airsigManager;

    // Reference to the vive controllers
	protected SteamVR_TrackedObject rightController;
	protected SteamVR_Controller.Device rightDevice;
	protected ParticleSystem rightParticles;

	protected SteamVR_TrackedObject leftController;
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

	public void Start() {
		rightController = GameObject.FindWithTag ("right").GetComponent<SteamVR_TrackedObject> ();
		rightDevice = SteamVR_Controller.Input ((int)rightController.index);
		rightParticles = GameObject.FindWithTag ("rightParticles").GetComponent<ParticleSystem>();

		leftController = GameObject.FindWithTag ("left").GetComponent<SteamVR_TrackedObject> ();
		leftDevice = SteamVR_Controller.Input ((int)leftController.index);
		leftParticles = GameObject.FindWithTag ("leftParticles").GetComponent<ParticleSystem>();
	}

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

		if (rightDevice.GetPressDown(SteamVR_Controller.ButtonMask.Grip)) {
			rightParticles.Clear();
			rightParticles.Play();
		} else if (rightDevice.GetPressUp(SteamVR_Controller.ButtonMask.Grip)) {
			rightParticles.Stop();
		}

		if (leftDevice.GetPressDown(SteamVR_Controller.ButtonMask.Grip)) {
			leftParticles.Clear();
			leftParticles.Play();
		} else if (leftDevice.GetPressUp(SteamVR_Controller.ButtonMask.Grip)) {
			leftParticles.Stop();
		}
			
//		if (-1 != (int)rightHandControl.index) {
//			var device = SteamVR_Controller.Input((int)rightHandControl.index);
//			if (device.GetPressDown(SteamVR_Controller.ButtonMask.Grip)) {
//				track.Clear();
//				track.Play();
//			} else if (device.GetPressUp(SteamVR_Controller.ButtonMask.Grip)) {
//				track.Stop();
//			}
//		}
//
//		if (-1 != (int)leftHandControl.index) {
//			var device = SteamVR_Controller.Input((int)leftHandControl.index);
//			if (device.GetPressDown(SteamVR_Controller.ButtonMask.Grip)) {
//				track.Clear();
//				track.Play();
//			} else if (device.GetPressUp(SteamVR_Controller.ButtonMask.Grip)) {
//				track.Stop();
//			}
//		}

        if (nextUiAction != null) {
            nextUiAction();
            nextUiAction = null;
        }
    }

}
