using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using Unity.VisualScripting;

public class DisconectManager : MonoBehaviourPunCallbacks
{
    public static DisconectManager disconectInstance;
    private int sceneID;
    private bool isLeaving = false;
    [SerializeField] private GameObject fixebleCamera;
    private void Awake()
    {
        if (disconectInstance == null)
        {
            disconectInstance = this;
        }
        else
        {
            Destroy(disconectInstance);
        }
    }
    private void Start()
    {
        fixebleCamera.SetActive(false);
    }

    public void ChangingScenes(int sceneID) 
    {
        fixebleCamera.SetActive(true);
        isLeaving = true;        
        if(FindObjectOfType<DurabilityDefenseDatabase>()) FindObjectOfType<DurabilityDefenseDatabase>().DestroySelf();
        if (PhotonNetwork.IsConnected)
        {
            this.sceneID = sceneID;
            PhotonNetwork.Disconnect();
        }
        else
        {
            SceneManager.LoadScene(sceneID);
        }
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        if (isLeaving) 
        {
            SceneManager.LoadScene(sceneID);   
        }       
    }
}
