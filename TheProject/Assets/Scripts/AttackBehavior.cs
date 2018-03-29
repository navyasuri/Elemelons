using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class AttackBehavior : Photon.MonoBehaviour {

//	public Transform target;
//	public float speed;
	Vector3 newPos;
	GameObject player;
	Rigidbody rb;
    public int attackerID;
    public Color attackerColor;
	float starterTime;
	float curTime;

	void Start() {
		GetComponent<Renderer>().material.color = attackerColor;
		rb = gameObject.GetComponent<Rigidbody>();
        // Needed to be transform.forward, not transform.position.
        // Moved to Start to avoid acceleration.
        rb.AddForce(transform.forward * 500f);
		starterTime = Time.time;

        Vector3 serializedColor;
        serializedColor.x = attackerColor.r;
        serializedColor.y = attackerColor.g;
        serializedColor.z = attackerColor.b;
        // Run a Remote Procedure Call for only currently connected users, sending the values to SetColor:
        PhotonView.Get(this).RPC("SetColor", PhotonTargets.All, serializedColor);
    }

    [PunRPC] // As a photon serialized view, only send floats/ints/vectors/quaternions.
    public void SetColor(Vector3 color)
    {
        // Sets this Player to the color sent with the call:
        GetComponent<Renderer>().material.color = new Color(color.x, color.y, color.z, 1f);
        attackerColor = GetComponent<Renderer>().material.color;
        if (PhotonView.Get(this).isMine)
        { // If this user called the function, send this call to the other users, including those who join:
            photonView.RPC("SetColor", PhotonTargets.OthersBuffered, color);
        }
    }

    void Update() {
		curTime = Time.time;
		if (curTime - starterTime > 2f) {
			Destroy (gameObject);
		}
	}

	void OnCollisionEnter(Collision collision)
	{
        if (collision.gameObject.CompareTag("Player")) // Check for a Player.
        {
            // If the attack is not colliding with the Player who sent it, destroy them:
            if (collision.gameObject.GetInstanceID() != attackerID)
                Destroy(collision.gameObject);
            Destroy(gameObject); // Destroy the attack on any collision.
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
