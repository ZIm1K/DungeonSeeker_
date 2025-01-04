using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerCallbacks : MonoBehaviourPunCallbacks    
{
    [SerializeField] private PlayerViewManager playerViewManager;
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PhotonView viewToRemove = playerViewManager.playersPhotonViews.Find(view => view.Owner == otherPlayer);

        if (viewToRemove != null)
        {
            playerViewManager.photonView.RPC("RemoveFromList",RpcTarget.All,viewToRemove.ViewID);
        }
    }
}
