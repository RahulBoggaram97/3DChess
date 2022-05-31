using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class tournamentPhotonManager : MonoBehaviourPunCallbacks
{
 

   

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        connect();
    }



    public void connect()
    {

        if (PhotonNetwork.IsConnected)
        {
            Debug.Log(PhotonNetwork.CurrentLobby.Name + " is the lobby and  the room is " + PhotonNetwork.CurrentRoom.Name);
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();          
        }

    }

    public override void OnConnectedToMaster()
    {
        //have to wait for this message in order to use the create button
        Debug.Log("the server has made or connected, now we can create room");

    }

    public void MatchGame()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    
   
}
