using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

// NOTE: MonoBehaviour -> Photon.MonoBehaviour
public class NetworkCommunicator : Photon.MonoBehaviour
{ 
    // To store the position and rotation of our GameObject as received from the network:
    Vector3 targetPosition;
    Quaternion targetRotation;
	Vector3 targetVelocity;
	Rigidbody rb;

	void Start() {
		rb = gameObject.GetComponent<Rigidbody> ();
	}

    void Update() {
        // Check if we control this GameObject. If not, deal with data that doesn't update smoothly
        // by constantly Lerping between the position of the GameObject as we see it,
        // and the position that we get from stream.RecieveNext of OnPhotonSerializeView.
        if (!photonView.isMine) {
			gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, targetPosition, Time.deltaTime * 5f);
			gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, targetRotation, Time.deltaTime * 5f);
			rb.velocity = Vector3.Lerp (rb.velocity, targetVelocity, Time.deltaTime * 5f);
		}
    }

    // This method allows us to serialize data and send/receive it over the network,
    // This is what updates the position of this object for everybody:
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) { 
			// If we own this GameObject, send the data to other players (Use: float/ints/vectors/quaternions)
            stream.SendNext(transform.position); // Send the position.
            stream.SendNext(transform.rotation); // Send the rotation.
			stream.SendNext(rb.velocity); // Send the velocity.
        }
        else
        { // If we don't own this object, update its data according to what we receive:
            targetPosition = (Vector3)stream.ReceiveNext(); // Store the received position in our targetPosition.
            targetRotation = (Quaternion)stream.ReceiveNext(); // Store the received rotation in our targetRotation.
			targetVelocity = (Vector3)stream.ReceiveNext(); // Store the receivec velocity in our targetVelocity.
        }
    }
}
