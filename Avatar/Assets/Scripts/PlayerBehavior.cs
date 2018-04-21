using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class PlayerBehavior : Photon.MonoBehaviour {

    protected Color playerColor;
	public float health;
	public GameObject overheadHealth;

	void Awake() {
		DontDestroyOnLoad (this.gameObject);
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

		health = 3f;
		GameObject healthUI = Instantiate (overheadHealth) as GameObject;
		healthUI.SendMessage ("SetTarget", this, SendMessageOptions.RequireReceiver);
    }

	void Update() {
		if (health <= 0f) {
			Destroy (gameObject.GetComponentInParent<Camera>());
			GameObject.Find ("NetworkManager").GetComponent<Network> ().OnJoinedRoom ();
			health = 3f;
		}
	}

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

	public void OnPhotonSerializedView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			stream.SendNext (health);
		} else {
			this.health = (float)stream.ReceiveNext ();
		}
	}

	void OnTriggerEnter(Collider other) {
		// Ignore the collision if the trigger is happening to somebody else:
		if (!photonView.isMine) {
			return;
		}

		// Also ignore if the collision is not an attack:
		if(!other.gameObject.CompareTag("attack")) {
			return;
		}

		if (other.gameObject.CompareTag ("boulder")) {
			health -= 1.5f;
			Debug.Log ("You were hit by a boulder! Health: " + health);
		}

		if(other.gameObject.CompareTag("fireball")) {
			health -= 1f;
			Debug.Log ("You were hit by a fireball! Health: " + health);
		}
	}

	void OnTriggerStay(Collider other) 
	{
		if (!photonView.isMine) {
			return;
		}
		// Slow damage if sitting still in flames.
		if(other.gameObject.CompareTag("flames")) {
			health -= 0.1f*Time.deltaTime; 
		}
	
	}
		
}