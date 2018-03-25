using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        PhotonNetwork.ConnectUsingSettings("0.1")
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    string room_name = "Atrium";
    RoomInfo[] all_rooms;
    string networkedObject_name = "MyNetworkedAvatar";

    void OnGUI()
    {   //This is called to easily display GUI elements
        if (!PhotonNetwork.connected)
        {   //if the player isn't currently connected to the Photon Cloud
            GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString()); //display log messages
        }
        else if (PhotonNetwork.room == null)
        { //else, if you're connected and not yet in a room
            if (GUI.Button(new Rect(100, 100, 250, 100, "Create a Room"){ //display a clickable button to create a room
			PhotonNetwork.CreateRoom(room_name + Guid.NewGuid().ToString("N"), true, true, 5);
        }

        if (all_rooms != null)
        { //if we have some rooms to display
            for (int i = 0; i < all_rooms.Length; i++)
            {
                if (GUI.Button(new Rect(100, 250 + (110 * i), 250, 100), "Join " + all_rooms[i].name)) //create buttons for each available room
                    PhotonNetwork.JoinRoom(all_rooms[i].name); //join the room that the user clicked on!
            }
        }
    }

    void OnReceiveRoomListUpdate()
    { //this function is automatically called when you get new rooms (e.g. when a room is created or removed
        all_rooms = PhotonNetwork.GetRoomList();
    }

    void OnJoinedRoom()
    { //this is automatically called when you join a room
        Debug.Log("Joined new room!");

        PhotonNetwork.Instantiate(networkedObject_name, Vector3.zero, Quaternion.identity) //this is where you want to create your avatar
    }
}
