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
        Debug.LogWarning("Leaving");
        if(FindObjectOfType<DurabilityDefenseDatabase>()) FindObjectOfType<DurabilityDefenseDatabase>().DestroySelf();
        Debug.LogWarning("Destroying DDD");
        if (PhotonNetwork.IsConnected)
        {
            Debug.LogWarning("You are connected waiting....");
            this.sceneID = sceneID;
            PhotonNetwork.Disconnect();
        }
        else
        {
            Debug.LogWarning("You are disconected and left, cool");
            SceneManager.LoadScene(sceneID);
        }
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning("Final pros of leaving");
        if (isLeaving) 
        {
            Debug.LogWarning("Loading scene");
            SceneManager.LoadScene(sceneID);   
        }       
    }
}
