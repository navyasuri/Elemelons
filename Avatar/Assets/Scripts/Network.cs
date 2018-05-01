using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public GameObject spawnPoint1;
    public GameObject spawnPoint2;
	protected static Dictionary<string, GameObject> attacks;
	protected static Dictionary<string, GameObject> throwers;

	public class SpawnPoint{
		public GameObject spawnPoint;
		public string flameType;
		public GameObject attack, thrower;

		public SpawnPoint(string FlameType){
			flameType = FlameType;
			attack = attacks[flameType];
			thrower = throwers[flameType];
		}
	}

	SpawnPoint sp1; 
	SpawnPoint sp2;

	public bool offlineMode = false;

    // Arrays to track spawn points locations and 'taken' status:
	public SpawnPoint[] spawnPoints;
    public bool[] spawnPointTaken;

    void Start()
    {
		attacks = GameObject.Find("FlamesManager").GetComponent<FlamesManager>().attacks;
		throwers = GameObject.Find("FlamesManager").GetComponent<FlamesManager>().throwers;
		if (offlineMode) {
			PhotonNetwork.offlineMode = true;
		} else {
			PhotonNetwork.ConnectUsingSettings("0.2.1");
		}
        // Allows us to connect to the network. The only argument is the version of this application.

        PhotonNetwork.autoJoinLobby = true;
		PhotonNetwork.automaticallySyncScene = true;

		sp1 = new SpawnPoint ("purple");
		sp2 = new SpawnPoint ("green");

		if (sp1.spawnPoint && sp2.spawnPoint)
        {
			spawnPoints = new SpawnPoint[2];
            spawnPoints[0] = sp1;
            spawnPoints[1] = sp2;
        }

        spawnPointTaken = new bool[2];
    }

    void Update()
    {

    }

    // This is a simple way to display the connection state on the screen itself, instead of in the Console:
    void OnGUI()
    {
        if (!PhotonNetwork.connected)
        { // If the player isn't currently connected to the Photon Cloud:
            GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString()); // Display log messages.
        }
        else if (PhotonNetwork.room == null)
        { // Else, if you're connected and not yet in a room, display a clickable button to create a room:
            if (GUI.Button(new Rect(100, 100, 250, 100), "Create a Room")) // This line creates a GUI Button.
                PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 6, IsVisible = true }, null); // This line is called on click.

            if (roomsList != null)
            { // If we have some rooms to display:
                for (int i = 0; i < roomsList.Length; i++)
                { // Loop and create buttons for each available room:
                    if (GUI.Button(new Rect(100, 250 + (110 * i), 250, 100), "Join " + roomsList[i].Name))
                        PhotonNetwork.JoinRoom(roomsList[i].Name); // Join the room that the user clicked on!
                }
            }
        }
    }

    // Once we've joined our room, we want to instantiate an object for our player to control:
    public override void OnJoinedRoom()
    {
        // Get the spawn point location to place the rig
        Vector3 spawnLocation;
		GameObject thrower;
        if (spawnPoints.Length > 0)
        {
            if (!spawnPointTaken[0])
            {
				spawnLocation = spawnPoints[0].spawnPoint.transform.position;
                spawnPointTaken[0] = true;
				thrower = sp1.thrower;
            }
            else
            {
				spawnLocation = spawnPoints[1].spawnPoint.transform.position;
                spawnPointTaken[1] = true;
            }
        }
        else
        {
            Debug.Log("No Spawn points assigned! Instantiating at 0, 0, 0");
            spawnLocation = Vector3.zero;
        }

        //Waiting for rig to come into the network and connect the player:
        StartCoroutine(WaitForRig());

        //Debug.Log("Creating new player and spawn position is " + spawnLocation);

		GameObject.Instantiate(teleportRig, spawnLocation, Quaternion.identity);
        // NOTE: Prefab should be located in the Assets/Resources folder.
    }

    // Photon automatically calls this function when a room is created or removed:
    public override void OnReceivedRoomListUpdate()
    {
        roomsList = PhotonNetwork.GetRoomList();
    }

    // Function to free up spawn points on disconnect
    void onLeftRoom()
    {
        GameObject playerRemaining = GameObject.FindGameObjectWithTag("Player");
        if (playerRemaining.transform.position == sp1.spawnPoint.transform.position)
        {
            spawnPointTaken[1] = false;
        }
        else
        {
            spawnPointTaken[0] = false;
        }
    }

    // By Photon default, we join a lobby. This will join a room right away:
    public override void OnJoinedLobby()
    {
        // Once we've joined the lobby, (tick the Auto-Join Lobby setting in Assets>Resources>PhotonServerSettings)
        // tell Photon to join a random room inside our application.
        // Essentially, that means that if a room is available, we will join it.
        // If that fails, Photon will call OnPhotonRandomJoinFailed() below, creating a room.

        PhotonNetwork.JoinRandomRoom();
    }

    // If no room is available, then create a new one (so at least one room will be available for future users to join):
    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 8 }, null);
    }

    // Helper function to find the rig
    IEnumerator WaitForRig()
    {
        yield return new WaitForSeconds(1);

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

			GameObject leftFix = GameObject.Find ("LeftHandTransformFixer");
			GameObject playerHandLeft = PhotonNetwork.Instantiate(leftHandPrefab.name, leftController.transform.position, Quaternion.identity, 0);
			playerHandLeft.transform.SetParent(leftController.transform);
			playerHandLeft.transform.localPosition = leftFix.transform.position;
			playerHandLeft.transform.localRotation = leftFix.transform.localRotation;

			GameObject rightFix = GameObject.Find ("RightHandTransformFixer");
			GameObject playerHandRight = PhotonNetwork.Instantiate(rightHandPrefab.name, rightController.transform.position, Quaternion.identity, 0);
			playerHandRight.transform.SetParent(rightController.transform);
			playerHandRight.transform.localPosition = rightFix.transform.position;
			playerHandRight.transform.localRotation = rightFix.transform.localRotation;

			// Once the player is instantiated in the game room, update the controller references for AirSig:
			GameObject.Find("GameManager").gameObject.GetComponent<DeveloperDefined>().AirSigControlUpdate(leftController, rightController, headset);
		}
    }

}
