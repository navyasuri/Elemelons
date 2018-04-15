using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class PlayerBehavior : Photon.MonoBehaviour {

	public GameObject attack; // For the drag-and-drop of the Component on the Player Prefab.
	string Attack = "Attack (1)";
    public Color playerColor;

    void Awake()
    {

    }

    void Start () {
        // Pick a random, saturated and not-too-dark color
        playerColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f); // Results in an RGBA value.
        // Create a Vector3 to package the RGB values to be send serialized:
        Vector3 serializedColor;
        serializedColor.x = playerColor.r;
        serializedColor.y = playerColor.g;
        serializedColor.z = playerColor.b;
        // Run a Remote Procedure Call for only currently connected users, sending the values to SetColor:
        PhotonView.Get(this).RPC("SetColor", PhotonTargets.All, serializedColor);
    }
    // On Start goals:
    // set your color
    // tell everyone else what color you are
    // get the colors of the other players.

    [PunRPC] // As a photon serialized view, only send floats/ints/vectors/quaternions.
    public void SetColor(Vector3 color)
    {
        // Sets this Player to the color sent with the call:
        GetComponent<Renderer>().material.color = new Color(color.x, color.y, color.z, 1f);
        playerColor = GetComponent<Renderer>().material.color;
        if (PhotonView.Get(this).isMine)
        { // If this user called the function, send this call to the other users, including those who join:
            photonView.RPC("SetColor", PhotonTargets.OthersBuffered, color);
        }
    }
	
	void Update () {
        if (photonView.isMine) // Is this part of this application which called 'PhotonNetwork.Instantiate()'?
            Movement();
    }

    float speed = 5f;

    void Movement()
    {
        if (Input.GetKey(KeyCode.UpArrow)) // Move forward.
            transform.position += transform.forward * Time.deltaTime * speed;
        if (Input.GetKey(KeyCode.DownArrow)) // Move back.
            transform.position -= transform.forward * Time.deltaTime * speed;
        if (Input.GetKey(KeyCode.RightArrow)) // Turn right.
            transform.Rotate(Vector3.up * (Time.deltaTime + speed)); // Rotate at 5 degrees per second
        if (Input.GetKey(KeyCode.LeftArrow)) // Turn left.
            transform.Rotate(Vector3.up * -1 * (Time.deltaTime + speed));
        if (Input.GetKeyDown(KeyCode.Space)) // Attack
        {
            // Set the Vector at which to instantiate the attack GameObject, at 1 unit forward of the Player:
            Vector3 attackVector = transform.position + (transform.forward * 1f);
            attackVector.y += 0.25f; // Move it up a tad for the aesthetics.
            var newAttack = PhotonNetwork.Instantiate(Attack, attackVector, transform.rotation, 0); // Create the attack.
            newAttack.GetComponent<AttackBehavior>().attackerID = gameObject.GetInstanceID(); // Note ID of attacking player. (To avoid self-damage)
            newAttack.GetComponent<AttackBehavior>().attackerColor = playerColor;
        }
    }
}