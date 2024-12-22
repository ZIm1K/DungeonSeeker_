using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DoorButton : MonoBehaviourPun
{
    public DoorController door;
    private bool isInteracting = false;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isInteracting)
        {
            if (Input.GetKey(KeyCode.E)) 
            { 
                isInteracting = true;
                bool newDoorState = !door.isOpen;
                photonView.RPC("InteractWithButton",RpcTarget.All,newDoorState);
            }
        }
    }
    [PunRPC]
    public void InteractWithButton(bool newDoorState) 
    {
        door.photonView.RPC("SetDoorState", RpcTarget.All,newDoorState);
        StartCoroutine(ResetInteraction());
    }
    private IEnumerator ResetInteraction() 
    { 
        yield return new WaitForSeconds(1f);
        isInteracting = false;
    }
}
