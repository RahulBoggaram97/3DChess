using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class tourUiHangler : MonoBehaviourPunCallbacks
{
    [Header("otherScript")]
    public tourApiManager _tourApiManager;

    [Header("Canvases")]
    public GameObject tounamentSwipeCanvas;
    public GameObject createCanvas;
    public GameObject inTournamentCanvas;

    [Header("Scenes")]
    public string onlineMainGameScnename;

    public void joinTournamentOnPhoton(string t_id)
    {
       
        TypedLobby newLobby = new TypedLobby(t_id, LobbyType.Default);
        PhotonNetwork.JoinLobby(newLobby);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined the lobby we can Match if you want, the lobby name is : " + PhotonNetwork.CurrentLobby.Name  );
        tounamentSwipeCanvas.SetActive(false);
        createCanvas.SetActive(false);
        inTournamentCanvas.SetActive(true);
        Debug.Log(playerPermData.getCurrentTournamentId() + "froum tournament ui handler");
       
    }

    public override void OnJoinedRoom()
    {
        SceneManager.LoadScene(onlineMainGameScnename);
    }

    public void leaveTournament()
    {
        PhotonNetwork.LeaveLobby();
        playerPermData.nullTheCurrentTournamentId();
        tounamentSwipeCanvas.SetActive(true );
        createCanvas.SetActive(false );
        inTournamentCanvas.SetActive(false);
    }

   
}
