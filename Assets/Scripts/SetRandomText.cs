using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetRandomText : MonoBehaviourPunCallbacks
{
    public TMP_Text gameOverTitleText;
    public List<string> deathMessages;

    private void Start()
    {
        SetRandomDieTitleText();
    }

    public void SetRandomDieTitleText()
    {
        if (deathMessages.Count > 0)
        {
            StartCoroutine(TextTypper(deathMessages[Random.Range(0, deathMessages.Count)]));            
        }        
    }
    private IEnumerator TextTypper(string message) 
    {
        yield return new WaitForSeconds(3f);
        for (int i = 0; i < message.Length; i++) 
        {
            gameOverTitleText.text += message[i];
            yield return new WaitForSeconds(0.1f);            
        }
        yield return new WaitForSeconds(4f);
        DisconectManager.disconectInstance.ChangingScenes(0);
        //if (PhotonNetwork.IsConnected)
        //{
        //    PhotonNetwork.Disconnect();
        //}
        //else
        //{
        //    SceneManager.LoadScene(0);
        //}
    }
    //public override void OnDisconnected(DisconnectCause cause)
    //{
    //    SceneManager.LoadScene(0);        
    //}
}
