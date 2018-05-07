using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUpBehavior : MonoBehaviour {


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetPhotonView() != null)
        { // if the colliding game object is networked (has a photon view)
            if (collision.gameObject.CompareTag("Player"))
            { // if the collision is with a player
                collision.gameObject.GetPhotonView().RPC("AddHealth", collision.gameObject.GetPhotonView().owner, 30f);
                PhotonView.Get(this).RPC("NetworkDestroy", PhotonTargets.All);
            }
        }
    }

    [PunRPC]
    void NetworkDestroy()
    {
        Destroy(gameObject);
        if (gameObject.GetPhotonView().isMine)
        {
            PhotonNetwork.RemoveRPCs(gameObject.GetPhotonView());
            PhotonNetwork.Destroy(gameObject.GetPhotonView());
        }
    }
}
