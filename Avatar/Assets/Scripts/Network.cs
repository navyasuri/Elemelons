﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon; // To use Photon-specific methods

/*
    Network should only run once, called by the NetworkManager GameObject in the scene!
*/


// MonoBehaviour -> Photon.PunBehaviour
// Now we can override Photon methods.
// Photon.PunBehavior vs Photon.MonoBehavior also inherits IPunCallbacks
public class Network : Photon.PunBehaviour
{
    private const string roomName = "ElemelonsPublicBeta";
    private RoomInfo[] roomsList;

    // Prefab references for objects to be Instantiated by this script:
	public GameObject teleportRig;
	public GameObject playerHeadPrefab;
    public GameObject leftHandPrefab;
    public GameObject rightHandPrefab;
    public GameObject yellowSpawn;
	public GameObject blueSpawn;
	public GameObject greenSpawn;
	public GameObject purpleSpawn;
	public GameObject whiteSpawn;
	string playerColor;
	int playerCount;
	int playersReady;
	protected static Dictionary<string, GameObject> throwers;

	public bool offlineMode = false;
	bool nowChanging;

    // Arrays to track spawn points locations and 'taken' status:
	public Transform[] spawnPoints;
    public bool[] spawnPointTaken;

    void Start() {
		if (offlineMode) {
			PhotonNetwork.offlineMode = true;
		} else {
			PhotonNetwork.ConnectUsingSettings("0.3.1");
		}
        // Allows us to connect to the network. The only argument is the version of this application.

        PhotonNetwork.autoJoinLobby = true;
		PhotonNetwork.automaticallySyncScene = true;

		throwers = GameObject.Find("FlamesManager").GetComponent<FlamesManager>().throwers;

		spawnPoints = new Transform[5];
		spawnPoints[0] = yellowSpawn.transform;
		spawnPoints[1] = blueSpawn.transform;
		spawnPoints[2] = greenSpawn.transform;
		spawnPoints[3] = purpleSpawn.transform;
		spawnPoints[4] = whiteSpawn.transform;
    }

    void Update() {
		// Get the current scene, if lobby, check for ready count
		Debug.Log("[Network Update()] Players: " + playerCount + ". Ready: " + playersReady);
		if(SceneManager.GetActiveScene().name.Equals("Lobby")) {
			DontDestroyOnLoad(GameObject.Find("SelectionPad(Clone)"));
			DontDestroyOnLoad(GameObject.Find("InvalidSelectionPad(Clone)"));
			if(playerCount > 0 && playersReady == playerCount) { // Playercount is updated by OnJoinedRoom()
				if(PhotonNetwork.isMasterClient && !nowChanging) {
					// Tell everyone to begin loading the next scene, once, via server to keep things in sync:
					nowChanging = true;
					PhotonView.Get (this).RPC ("SceneChange", PhotonTargets.AllViaServer);
				}
			}
		}
    }

	[PunRPC]
	public void UpdateReadyCount() {
		Debug.Log ("[Network UpdateReadyCount RPC] called, yep");
		if(PhotonNetwork.isMasterClient) {
			playersReady++;
			Debug.Log ("[Network UpdateReadyCount RPC] A new player is ready! " + playersReady + " players are ready");
		}
	}

	[PunRPC]
	void SceneChange() {
		GameObject blankScreen = GameObject.Find("Camera (eye)").transform.GetChild(2).GetChild(5).gameObject;
		StartCoroutine (fadeSceneIn (blankScreen, 3f));
	}

	IEnumerator fadeSceneIn(GameObject blankScreen, float fadeTime) {
		for(float t = 0; t < fadeTime; t += 0.05f) {
			blankScreen.GetComponent<Renderer> ().material.SetColor("_Color", new Color(0, 0, 0, t/fadeTime));
			yield return new WaitForSeconds(0.05f);
		}
		yield return new WaitForSeconds (2f);
		if (PhotonNetwork.isMasterClient) {
			PhotonNetwork.LoadLevel ("VRPUNScene");
			Debug.Log ("Scene change called");
		}
	}

    // This is a simple way to display the connection state on the screen itself, instead of in the Console:
    void OnGUI() {
		// If the player isn't currently connected to the Photon Cloud:
        if (!PhotonNetwork.connected) { 
            GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString()); // Display log messages.
        }
		// Else, if you're connected and not yet in a room, display a clickable button to create a room:
        else if (PhotonNetwork.room == null) { 
            if (GUI.Button(new Rect(100, 100, 250, 100), "Create a Room")) // This line creates a GUI Button.
                PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 5, IsVisible = true }, null); // This line is called on click.

			// If we have some rooms to display:
            if (roomsList != null) { 
				// Loop and create buttons for each available room:
                for (int i = 0; i < roomsList.Length; i++) { 
                    if (GUI.Button(new Rect(100, 250 + (110 * i), 250, 100), "Join " + roomsList[i].Name))
                        PhotonNetwork.JoinRoom(roomsList[i].Name); // Join the room that the user clicked on!
                }
            }
        }
    }

    // Once we've joined our room, we want to instantiate an object for our player to control:
    public override void OnJoinedRoom() {
        // Get the spawn point location to place the rig, assign player color based on that location:
        Vector3 spawnLocation;
		playerCount = PhotonNetwork.playerList.Length;

        if (spawnPoints.Length > 0) {
			if (playerCount == 1) {
				spawnLocation = spawnPoints[0].position;
                //spawnPointTaken[0] = true;
				playerColor = "Yellow";
            }
			else if (playerCount == 2) {
				spawnLocation = spawnPoints[1].position;
				//spawnPointTaken[1] = true;
				playerColor = "Blue";
			}
			else if (playerCount == 3) {
				spawnLocation = spawnPoints[2].position;
				//spawnPointTaken[2] = true;
				playerColor = "Green";
			}
			else if (playerCount == 4) {
				spawnLocation = spawnPoints[3].position;
				//spawnPointTaken[3] = true;
				playerColor = "Purple";
			}
            else {
				spawnLocation = spawnPoints[4].position;
                //spawnPointTaken[4] = true;
				playerColor = "White";
            }
        }
        else {
            Debug.Log("No Spawn points assigned! Instantiating at 0, 0, 0");
            spawnLocation = Vector3.zero;
			playerColor = "Yellow";
        }

        //Waiting for rig to come into the network and connect the player:
        StartCoroutine(WaitForRig());

		Debug.Log("Creating new " + playerColor + " player at " + spawnLocation);

		GameObject.Instantiate(teleportRig, spawnLocation, Quaternion.identity);
		DontDestroyOnLoad (GameObject.Find ("TeleportingRig(Clone)"));
    }

    // Photon automatically calls this function when a room is created or removed:
    public override void OnReceivedRoomListUpdate() {
        roomsList = PhotonNetwork.GetRoomList();
    }

    // Function to free up spawn points on disconnect
    void onLeftRoom() {
        GameObject playerRemaining = GameObject.FindGameObjectWithTag("Player");
		if (playerRemaining.transform.position == blueSpawn.transform.position) {
            spawnPointTaken[1] = false;
        }
		if (playerRemaining.transform.position == greenSpawn.transform.position) {
			spawnPointTaken[2] = false;
		}
		if (playerRemaining.transform.position == purpleSpawn.transform.position) {
			spawnPointTaken[3] = false;
		}
		if (playerRemaining.transform.position == whiteSpawn.transform.position) {
			spawnPointTaken[4] = false;
		}
        else {
            spawnPointTaken[0] = false;
        }
    }

    // By Photon default, we join a lobby. This will join a room right away:
    public override void OnJoinedLobby() {
        // Once we've joined the lobby, (tick the Auto-Join Lobby setting in Assets>Resources>PhotonServerSettings)
        // tell Photon to join a random room inside our application.
        // Essentially, that means that if a room is available, we will join it.
        // If that fails, Photon will call OnPhotonRandomJoinFailed() below, creating a room.
		PhotonNetwork.JoinRandomRoom();
    }

    // If no room is available, then create a new one (so at least one room will be available for future users to join):
    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg) {
        PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 8 }, null);
    }

    // Helper function to find the rig
    IEnumerator WaitForRig() {
        yield return new WaitForSeconds(0.2f);

        //Find headset and instaniate player prefab ON NETWORK — set the Camera Rig headset as the parent of the player head's prefab:
        GameObject headset = GameObject.Find("Camera (eye)");
		GameObject leftController = GameObject.Find("Controller (left)");
		GameObject rightController = GameObject.Find("Controller (right)");

		if (headset == null || leftController == null || rightController == null) {
			Debug.Log ("Rig not tracking, waiting for all components to track.");
			StartCoroutine (WaitForRig ());
		} else {
			// Instantiate player hand/body prefabs ON NETWORK, setting rig items as parents of the prefabs:
			GameObject player = PhotonNetwork.Instantiate(playerHeadPrefab.name, headset.transform.position, headset.transform.rotation, 0);
			player.transform.SetParent(headset.transform);
			player.GetComponent<PlayerBehavior> ().flameType = playerColor;

			// Instantiate hand prefab and set as child of controller:
			GameObject playerHandLeft = PhotonNetwork.Instantiate(leftHandPrefab.name, leftController.transform.position, Quaternion.identity, 0);
			playerHandLeft.transform.SetParent(leftController.transform);
			// Fix the orientation by resetting it's local space relative to the helper GameObject in the scene:
			GameObject leftFix = GameObject.Find ("LeftHandTransformFixer");
			playerHandLeft.transform.localPosition = leftFix.transform.position;
			playerHandLeft.transform.localRotation = leftFix.transform.localRotation;
			// Add the flamethrower to the hand, using the orientations of the calibrated flamethrower already on the hand prefab:
			Transform leftFlameFix = playerHandLeft.transform.Find ("Flamethrower");
			GameObject throwerLeft = PhotonNetwork.Instantiate(GameObject.Find ("FlamesManager").GetComponent<FlamesManager> ().throwers [playerColor].name, leftFlameFix.position, leftFlameFix.rotation, 0);
			throwerLeft.transform.SetParent(playerHandLeft.transform);
			// Remove the extra flamethrower from the prefab:
			Destroy (leftFlameFix.gameObject);
			// Finally, give the new flamethrower the playerID to avoid self-damage:
			throwerLeft.GetComponent<FlamethrowerBehavior> ().playerID = headset.GetInstanceID();

			// Same as above, but for the right hand:
			GameObject playerHandRight = PhotonNetwork.Instantiate(rightHandPrefab.name, rightController.transform.position, Quaternion.identity, 0);
			playerHandRight.transform.SetParent(rightController.transform);
			GameObject rightFix = GameObject.Find ("RightHandTransformFixer");
			playerHandRight.transform.localPosition = rightFix.transform.position;
			playerHandRight.transform.localRotation = rightFix.transform.localRotation;
			Transform rightFlameFix = playerHandRight.transform.Find ("Flamethrower");
			GameObject throwerRight = PhotonNetwork.Instantiate(GameObject.Find ("FlamesManager").GetComponent<FlamesManager> ().throwers [playerColor].name, rightFlameFix.position, rightFlameFix.rotation, 0);
			throwerRight.transform.SetParent(playerHandRight.transform);
			Destroy (rightFlameFix.gameObject);
			throwerRight.GetComponent<FlamethrowerBehavior> ().playerID = headset.GetInstanceID();

			// Once the player is instantiated in the game room, update the controller references for AirSig:
			player.gameObject.GetComponent<DeveloperDefined>().AirSigControlUpdate(leftController, rightController, headset, playerColor);
		}
    }

}
