﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon; // To use Photon-specific methods

/*
    Network should only run once, called by the NetworkManager GameObject in the scene!
    All objects to be displayed over Photon
*/


// MonoBehaviour -> Photon.PunBehaviour
// Now we can override Photon methods.
public class Network : Photon.PunBehaviour
{
    private const string roomName = "ElemelonsClosedBeta";
    private RoomInfo[] roomsList;

    // Prefab references for objects to be Instantiated by this script:
    public GameObject playerPrefab;
    public GameObject leftHandPrefab;
    public GameObject rightHandPrefab;
    public GameObject spawnPoint1;
    public GameObject spawnPoint2;

    // Arrays to track spawn points locations and 'taken' status:
    public Transform[] spawnPoints;
    public bool[] spawnPointTaken;

    void Start()
    {
        // Allows us to connect to the network. The only argument is the version of this application.
        PhotonNetwork.ConnectUsingSettings("0.1.1");

        PhotonNetwork.autoJoinLobby = true;

        if (spawnPoint1 && spawnPoint2)
        {
            spawnPoints = new Transform[2];
            spawnPoints[0] = spawnPoint1.transform;
            spawnPoints[1] = spawnPoint2.transform;
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
        //Debug.Log("Current number of players is: " + PhotonNetwork.countOfPlayers);

        //Place the rig at a spawn point
        Vector3 spawnLocation;
        if (spawnPoints.Length > 0)
        {
            if (!spawnPointTaken[0])
            {
                spawnLocation = spawnPoints[0].position;
                spawnPointTaken[0] = true;
            }
            else
            {
                spawnLocation = spawnPoints[1].position;
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


        Debug.Log("Creating new player and spawn position is " + spawnLocation);
        // Use PhotonNetwork.Instantiate instead of GameObject.Instantiate to make sure that the player prefab GameObject we
        // instantiate will be kept track of by Photon, in order to update its information in NetworkCommunicator.cs, 
        // which must be attached to the object.

        // Not necessary, because in this case, the "player" we need is just the SteamVR rig, which exists upon game startup.
        // NOTE: But then how do spawn locations work???   

        //PhotonNetwork.Instantiate(player.name, spawnLocation, Quaternion.identity, 0);
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
        if (playerRemaining.transform.position == spawnPoint1.transform.position)
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

        //Debug.Log(PhotonNetwork.countOfPlayers);

        yield return new WaitForSeconds(1);

        //Find headset and instaniate player prefab ON NETWORK — set headset as player head's parent
        GameObject headset = GameObject.Find("Camera (eye)");
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, headset.transform.position, headset.transform.rotation, 0);
        player.transform.SetParent(headset.transform);

        //Find the controllers and instantiate capsules ON NETOWRK — set controllers as the parents of the capsules

        //Find left controller
        GameObject controllerLeft = GameObject.Find("Controller (left)/Model");
        Debug.Log(controllerLeft);
        GameObject capsuleHandLeft = PhotonNetwork.Instantiate(leftHandPrefab.name, controllerLeft.transform.position, Quaternion.identity, 0);
        capsuleHandLeft.transform.SetParent(controllerLeft.transform);

        //Now for right controller

        GameObject controllerRight = GameObject.Find("Controller (right)/Model");
        GameObject capsuleHandRight = PhotonNetwork.Instantiate(rightHandPrefab.name, controllerRight.transform.position, Quaternion.identity, 0);
        capsuleHandRight.transform.SetParent(controllerRight.transform);

    }
}
