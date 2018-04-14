using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script allows moving any game object (when controllers trigger & release)
public class TransformManager : Photon.MonoBehaviour {

	public float speed = 10f;

	PhotonView photonView;

	void Start(){
		photonView = PhotonView.Get (this);
	}

	// Update is called once per frame
	void Update () {
		//Update the movement
		if (!photonView.isMine) {
			SyncedMovement ();
		}
	}

	private float lastSynchronizationTime = 0f;
	private float syncDelay = 0f;
	private float syncTime = 0f;
	private Vector3 syncStartPosition = Vector3.zero;
	private Vector3 syncEndPosition = Vector3.zero;

	//Here if we are writing to the stream we send position and velocity
	//otherwise we are reading the position and the velocity from the stream to get the update information
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		Rigidbody rb = GetComponent<Rigidbody> ();
		if (stream.isWriting)
		{
			stream.SendNext(rb.position);
			stream.SendNext(rb.velocity);
		}
		else
		{
			Vector3 syncPosition = (Vector3)stream.ReceiveNext();
			Vector3 syncVelocity = (Vector3)stream.ReceiveNext();

			syncTime = 0f;
			syncDelay = Time.time - lastSynchronizationTime;
			lastSynchronizationTime = Time.time;

			syncEndPosition = syncPosition + syncVelocity * syncDelay;
			syncStartPosition = rb.position;
		}
	}

	private void SyncedMovement()
	{
		Rigidbody rb = GetComponent<Rigidbody> ();
		syncTime += Time.deltaTime;
		rb.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
	}

	public void StartColorChange(Vector3 color){
		photonView.RPC("ChangeColorTo",PhotonTargets.All, color);
	}

	public void StartMoveTo(Vector3 direction){
		photonView.RPC("MoveTo",PhotonTargets.All, direction);
	}

	//Change the color
	[PunRPC] void ChangeColorTo(Vector3 color)
	{
		GetComponent<Renderer>().material.color = new Color(color.x, color.y, color.z, 1f);
		if (photonView.isMine)
			photonView.RPC("ChangeColorTo", PhotonTargets.OthersBuffered, color);
	}

	//Move the object
	[PunRPC] void MoveTo(Vector3 direction)
	{
		GetComponent<Transform>().position = direction;
		if (photonView.isMine)
			photonView.RPC("MoveTo", PhotonTargets.OthersBuffered, direction);
	}

	//set a new parent
	[PunRPC] public void SetNewParent(Transform tr){
		transform.SetParent (tr);
		if (photonView.isMine)
			photonView.RPC("SetNewParent", PhotonTargets.OthersBuffered,tr);
	}

	//detach the parent
	[PunRPC] public void DetachParent(){
		transform.parent = null;
		Debug.Log("Detached all parents");

		if (photonView.isMine)
			photonView.RPC("DetachParent", PhotonTargets.OthersBuffered,photonView.viewID);

	}
}
