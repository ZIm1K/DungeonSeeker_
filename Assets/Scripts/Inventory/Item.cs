using Photon.Pun;
using UnityEngine;

namespace Inventory
{
    public class Item : MonoBehaviourPun
    {
        public ItemScriptableObject item;
        public int amount;       
        [PunRPC]
        protected virtual void RPC_RequestDestroy(int viewID)
        {
            PhotonView targetView = PhotonView.Find(viewID);
            if (targetView != null && PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(targetView.gameObject);
            }
        }       
    }
}
