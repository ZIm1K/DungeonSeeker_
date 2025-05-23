using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using Photon.Pun;

public class NameChangeManager : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private GameObject namePanel;
    [SerializeField] private GameObject reWritePanel;
    private void Awake()
    {
        playerNameText.text = PlayerPrefs.GetString("PlayerName", "Player123456789");
        PhotonNetwork.NickName = playerNameText.text;
    }
    public void OpenClosePanels(bool isOpen) 
    {
        namePanel.SetActive(!isOpen);
        reWritePanel.SetActive(isOpen);
    }
    public void ApplyNewName() 
    {
        if (!playerNameInput.text.IsNullOrEmpty()) 
        {
            playerNameText.text = playerNameInput.text;
            PhotonNetwork.NickName = playerNameText.text;
            PlayerPrefs.SetString("PlayerName", playerNameText.text);
        }
    }
}
