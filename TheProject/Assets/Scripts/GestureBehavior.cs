using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class GestureBehavior : Photon.MonoBehaviour {

	GameObject player;
	Rigidbody rb;
    public int playerID;
	AudioSource flameWoosh;
    //public Color attackerColor;
	float startTime;
	float currentTime;

	public bool attack = false;
	public bool defense = false;

	void Start() {
		//GetComponent<Renderer>().material.color = attackerColor;
		rb = gameObject.GetComponent<Rigidbody>();
		startTime = Time.time;

	    // Old code for colorizing, kept here for RPC example:
//        Vector3 serializedColor;
//        serializedColor.x = attackerColor.r;
//        serializedColor.y = attackerColor.g;
//        serializedColor.z = attackerColor.b;
//        // Run a Remote Procedure Call for only currently connected users, sending the values to SetColor:
//        PhotonView.Get(this).RPC("SetColor", PhotonTargets.All, serializedColor);
    }

//    [PunRPC] // As a photon serialized view, only send floats/ints/vectors/quaternions.
//    public void SetColor(Vector3 color)
//    {
//        // Sets this Player to the color sent with the call:
//        GetComponent<Renderer>().material.color = new Color(color.x, color.y, color.z, 1f);
//        attackerColor = GetComponent<Renderer>().material.color;
//        if (PhotonView.Get(this).isMine)
//        { // If this user called the function, send this call to the other users, including those who join:
//            photonView.RPC("SetColor", PhotonTargets.OthersBuffered, color);
//        }
//    }

    void Update() {
		currentTime = Time.time;

		if (attack) {
			if (currentTime - startTime > 3f) {
				Destroy (gameObject);
			}
		}

		if (defense) {
			if (currentTime - startTime > 2f) {
				Destroy (gameObject);
			}
		}
	}

	// For attack:
	void OnCollisionEnter(Collision collision)
	{
        if (collision.gameObject.CompareTag("Player")) // Check for a Player.
        {
            // If the attack is not colliding with the Player who sent it, destroy them:
            if (collision.gameObject.GetInstanceID() != playerID)
                Destroy(collision.gameObject);
            Destroy(gameObject); // Destroy the attack on any collision.
			collision.gameObject.GetComponent<PlayerBehavior>().health -= 1;
			Debug.Log ("Player hit! Health remaining: " + collision.gameObject.GetComponent<PlayerBehavior> ().health);
        }
	}

	// For defense:
	void OnTriggerEnter(Collider collision)
	{
		if (collision.gameObject.CompareTag("attack"))
		{
			// Destroy any attacks that are not this players:
			if (collision.gameObject.GetComponent<GestureBehavior> ().playerID != playerID) {
				Destroy (collision.gameObject);
			}
		}
	}

	public void Spawn(Vector3 direction) {
		// Orient the new object:
		Quaternion rotation = Quaternion.FromToRotation (Vector3.back, direction);
		transform.rotation = rotation;

		// Send attacks flying!
		if (attack) {
			rb = gameObject.GetComponent<Rigidbody> ();
			rb.AddForce (direction * 500f);
			flameWoosh = gameObject.GetComponent<AudioSource> ();
			flameWoosh.Play ();
		}
	}

    //[PunRPC] // Used to flag methods as remote-callable.
    //void Attack(Vector3 dir)
    //{

    //    GetComponent<Rigidbody>().AddForce(dir * 8, ForceMode.Impulse);

    //    if (photonView.isMine)
    //        photonView.RPC("ForceJump", PhotonTargets.Others, dir);
    //}

}
