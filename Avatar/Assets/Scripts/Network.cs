using System.Collections;
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
	public int playersReady;
    public int playersSeen;
	protected static Dictionary<string, GameObject> throwers;

	public bool offlineMode = false;
	bool nowChanging;

    // Arrays to track spawn points locations and 'taken' status:
	public Transform[] spawnPoints;
	public string[] playerColors;

    void Start() {
		throwers = GameObject.Find("FlamesManager").GetComponent<FlamesManager>().throwers;

		if (offlineMode) {
			PhotonNetwork.offlineMode = true;
		} else {
			PhotonNetwork.ConnectUsingSettings("0.3.1");
		}
        // Allows us to connect to the network. The only argument is the version of this application.

		PhotonNetwork.automaticallySyncScene = true;
        PhotonNetwork.autoJoinLobby = true;
    }

    void Update() {
		// Get the current scene, if lobby, check for ready count
		playerCount = PhotonNetwork.playerList.Length;
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
        // In the main scene, update the level only once all players have seen the stone:
        if(SceneManager.GetActiveScene().name.Equals("VRPUNScene"))
        {
            Debug.Log("[Network Update()] Players: " + playerCount + ". Ready: " + playersSeen);
            if (playerCount > 0 && playersSeen == playerCount)
            { // Playercount is updated by seeing the skill stone
                if (PhotonNetwork.isMasterClient)
                {
                    GameObject.Find("SkillStonePrefab").GetPhotonView().RPC("NextLevel", PhotonTargets.AllViaServer);
                    playersSeen = 0;
                }
            }
        }
    }

	[PunRPC]
	public void UpdateReadyCount() {
		if(PhotonNetwork.isMasterClient) {
			playersReady++;
			Debug.Log ("[Network UpdateReadyCount RPC] A new player is ready! " + playersReady + " players are ready");
		}
    }

    [PunRPC]
    public void UpdateSeenCount()
    {
        if (PhotonNetwork.isMasterClient)
        {
            playersSeen++;
            Debug.Log("[Network UpdateSeenCount RPC] A new player is seeing skill stone! " + playersSeen + " players are ready");
        }
    }

    [PunRPC]
	void SceneChange() {
		GameObject blankScreen = GameObject.Find("Camera (eye)").transform.GetChild(2).GetChild(6).gameObject;
		StartCoroutine (fadeSceneIn (blankScreen, 3f));
	}

	IEnumerator fadeSceneIn(GameObject blankScreen, float fadeTime) {
		for(float t = 0; t < fadeTime; t += 0.05f) {
			blankScreen.GetComponent<Renderer> ().material.SetColor("_Color", new Color(0, 0, 0, t/fadeTime));
			yield return new WaitForSeconds(0.05f);
		}
		yield return new WaitForSeconds (2f);
		if (PhotonNetwork.isMasterClient) {
			PhotonNetwork.automaticallySyncScene = true;
			if (SceneManager.GetActiveScene ().name.Equals ("Lobby")) {				
				PhotonNetwork.LoadLevel (1);
			} else {
				PhotonNetwork.LoadLevel (0);
			}
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
		Quaternion spawnRotation;

		spawnPoints = new Transform[5];
		spawnPoints[0] = yellowSpawn.transform;
		spawnPoints[1] = blueSpawn.transform;
		spawnPoints[2] = greenSpawn.transform;
		spawnPoints[3] = purpleSpawn.transform;
		spawnPoints[4] = whiteSpawn.transform;

		playerColors = new string[5];
		playerColors[0] = "Yellow";
		playerColors[1] = "Blue";
		playerColors[2] = "Green";
		playerColors[3] = "Purple";
		playerColors[4] = "White";

		playerColor = GameObject.Find ("SceneLoader").GetComponent<SceneLoader> ().playerColor;

		if (playerColor.Equals("") || SceneManager.GetActiveScene().name.Equals("Lobby")) {
			// Returns 0-4
			int randomColor = Random.Range (0, 5);
			playerColor = playerColors [randomColor];
		}

		switch (playerColor)
		{
		case "Yellow":
			spawnLocation = spawnPoints[0].position;
			spawnRotation = spawnPoints [0].rotation;
			break;
		case "Blue":
			spawnLocation = spawnPoints[1].position;
			spawnRotation = spawnPoints [1].rotation;
			break;
		case "Green":
			spawnLocation = spawnPoints[2].position;
			spawnRotation = spawnPoints [2].rotation;
			break;
		case "Purple":
			spawnLocation = spawnPoints[3].position;
			spawnRotation = spawnPoints [3].rotation;
			break;
		case "White":
			spawnLocation = spawnPoints[4].position;
			spawnRotation = spawnPoints [4].rotation;
			break;
		default:
			Debug.Log("No playerColor assigned! Something broke. Instantiating at 0, 0, 0");
			spawnLocation = Vector3.zero;
			spawnRotation = Quaternion.identity;
			playerColor = "Yellow";
			break;
		}

		GameObject.Find ("SceneLoader").GetComponent<SceneLoader> ().playerColor = playerColor;

        //Waiting for rig to come into the network and connect the player:
        StartCoroutine(WaitForRig());
		Debug.Log("New " + playerColor + " player at " + spawnLocation);
		GameObject.Instantiate(teleportRig, spawnLocation, spawnRotation);
    }

    // Photon automatically calls this function when a room is created or removed:
    public override void OnReceivedRoomListUpdate() {
        roomsList = PhotonNetwork.GetRoomList();
    }

    // Function to free up spawn points on disconnect
    void onLeftRoom() {

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

			// Once the player is instantiated in the game room, update the controller references for AirSig:
			player.gameObject.GetComponent<DeveloperDefined>().AirSigControlUpdate(leftController, rightController, headset, playerColor);
		}
    }

}
