using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
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
        }
        else
        {
            Destroy(Instance);
        }
    }
    void Start()
    {
        photonView = GetComponent<PhotonView>();       
    }    
    [PunRPC]
    void AddToList(int id)
    {
        playersPhotonViews.Add(PhotonView.Find(id));
    }
    [PunRPC]
    void RemoveFromList(int id)
    {
        playersPhotonViews.Remove(PhotonView.Find(id));
    }
    public void SavePlayerInventory() 
    {
        foreach (var player in playersPhotonViews) 
        {
            if (player.Owner == PhotonNetwork.LocalPlayer) 
            {
                player.GetComponent<InventorySaver>().SaveInventory();
            }
        }
    }
}
