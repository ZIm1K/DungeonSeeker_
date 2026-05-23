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
            new Vector3(Random.Range(-1.5f, 1.5f), 2f, Random.Range(-1.5f, 1.5f)), Quaternion.identity).GetPhotonView();

        if (PlayerViewManager.Instance != null && PlayerViewManager.Instance.photonView != null)
        {
            PlayerViewManager.Instance.photonView.RPC("AddToList", RpcTarget.All, player.ViewID);
        }
        else
        {
            Debug.LogWarning("PlayerViewManager is not ready to register player.");
        }
    }
    [PunRPC]
    void UpdateList(int id) 
    {
        playersPhotonView.Add(PhotonView.Find(id));
    }
}
