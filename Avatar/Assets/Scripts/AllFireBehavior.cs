using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class AllFireBehavior : Photon.MonoBehaviour {

	GameObject player;
	Rigidbody rb;
    public int playerID;
	public AudioSource fireballWhoosh;
	public AudioSource fireballImpact;
	public AudioSource defenseWoosh;
	public AudioSource flamethrowerWhoosh;
	public ParticleSystem fireballPoof;
	public ParticleSystem fire;
	public float lowPitch = 0.45f;
	public float highPitch = 0.85f;
	float randomPitch;

    //public Color attackerColor;
	float startTime;
	float currentTime;
	float timeSinceDestruct;

	public bool attack = false;
	public bool defense = false;
	public bool isLive = true;

	void Start() {
		//GetComponent<Renderer>().material.color = attackerColor;
		rb = gameObject.GetComponent<Rigidbody>();
		startTime = Time.time;
		randomPitch = Random.Range (lowPitch, highPitch);

	    // Old code for colorizing, kept here for RPC example:
//        Vector3 serializedColor;
//        serializedColor.x = attackerColor.r;
//        serializedColor.y = attackerColor.g;
//        serializedColor.z = attackerColor.b;
//        // Run a Remote Procedure Call for only currently connected users, sending the values to SetColor:
//        PhotonView.Get(this).RPC("SetColor", PhotonTargets.All, serializedColor);
    }

//    [PunRPC] // As a photon serialize view, only send floats/ints/vectors/quaternions.
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
				isLive = false;
				SelfDestruct ();
			}
		}

		if (defense) {
			if (currentTime - startTime > 2f) {
				isLive = false;
				PhotonView.Get(this).RPC("NetworkDestroy", PhotonTargets.All);
			}
		}
	}

	// For attack:
	void OnCollisionEnter(Collision collision)
	{
		//Debug.Log("Fireball destroyed itself on " + collision.gameObject.tag);
		isLive = false;
		SelfDestruct (); // Destroy the attack, with effects, on any collision.
	}

	// For defense:
	void OnTriggerEnter(Collider collision)
	{
		if (collision.gameObject.CompareTag("attack")) // Check for attacking objects
		{
			// Destroy (ON NETWORK) any attacks that are not this player's:
			if (collision.gameObject.GetComponent<AllFireBehavior> ().playerID != playerID) {
				PhotonView.Get(collision.gameObject).RPC("NetworkDestroy", PhotonTargets.All);
				fireballImpact.Play ();
			}
		}
	}

	// Called by DeveloperDefined gesture triggers and networked prefab instantiation:
	public void DoAfterStart(Vector3 direction) {
		// Orient the new object:
		Quaternion rotationForTrails = Quaternion.FromToRotation (Vector3.back, direction);
		transform.rotation = rotationForTrails;

		// Send attacks flying!
		if (attack) {
			rb = gameObject.GetComponent<Rigidbody> ();
			rb.AddForce (direction * 1250f);
		}
	}

	// Handler for fireball destruction effects:
	public void SelfDestruct() {
		// If the explosion clip has finished playing, destroy the fireball prefab:
		timeSinceDestruct += Time.deltaTime;
		if (timeSinceDestruct > fireballImpact.clip.length + 0.1f) {
			PhotonView.Get(this).RPC("NetworkDestroy", PhotonTargets.All);
		}

		// If the clip is not playing (this is SelfDestruct's first call), play it,
		// turn off the Renderer/Collider, and turn on the explosion particle effect:
		if (!fireballImpact.isPlaying) {
			//gameObject.GetComponent<MeshRenderer> ().enabled = false;
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			gameObject.GetComponent<SphereCollider> ().enabled = false;
			fire.Stop();
			//fireballPoof.Play;
			fireballImpact.pitch = randomPitch;
			Debug.Log ("Exploding!");
			fireballImpact.Play ();
		}
	}

	[PunRPC] // Flag this function as a special indirectly callable network script.
	void NetworkDestroy() {
		if (GetComponent<PhotonView> ().instantiationId == 0) {
			Destroy (gameObject);
		}
		if (PhotonNetwork.isMasterClient) {
			PhotonNetwork.Destroy (gameObject);
		}
	}

	// Burn down the network:
//	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
//		if (stream.isWriting) {
//			stream.SendNext (isLive);
//		} else {
//			this.isLive = (bool)stream.ReceiveNext ();
//		}
//	}

}
