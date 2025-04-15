using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private PhotonView photonView;

    [SerializeField] private List<PhotonView> playersPhotonView;
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView.IsMine)
        {
            CreateController();
        }
    }

    private void CreateController()
    {
        PhotonView player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"),
            new Vector3(Random.Range(-1.5f, 1.5f), 1.1f, Random.Range(-1.5f, 1.5f)), Quaternion.identity).GetPhotonView();
        
        PlayerViewManager.Instance.photonView.RPC("AddToList", RpcTarget.All, player.ViewID);
    }
    [PunRPC]
    void UpdateList(int id) 
    {
        playersPhotonView.Add(PhotonView.Find(id));
    }
}
