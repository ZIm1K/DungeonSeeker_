using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class PlayerViewManager : MonoBehaviour
{
    public static PlayerViewManager Instance;
    
    public PhotonView photonView;

    [SerializeField] public List<PhotonView> playersPhotonViews;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            photonView = GetComponent<PhotonView>();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        if (photonView == null)
        {
            photonView = GetComponent<PhotonView>();
        }
    }    
    [PunRPC]
    void AddToList(int id)
    {
        PhotonView playerView = PhotonView.Find(id);
        if (playerView != null && !playersPhotonViews.Contains(playerView))
        {
            playersPhotonViews.Add(playerView);
        }
    }
    [PunRPC]
    void RemoveFromList(int id)
    {
        PhotonView playerView = PhotonView.Find(id);
        if (playerView != null)
        {
            playersPhotonViews.Remove(playerView);
        }
    }
    public void SavePlayerInventory() 
    {
        foreach (var player in playersPhotonViews) 
        {
            if (player != null && player.Owner == PhotonNetwork.LocalPlayer)
            {
                InventorySaver saver = player.GetComponent<InventorySaver>();
                if (saver != null)
                {
                    saver.SaveInventory();
                }
            }
        }
    }
}
