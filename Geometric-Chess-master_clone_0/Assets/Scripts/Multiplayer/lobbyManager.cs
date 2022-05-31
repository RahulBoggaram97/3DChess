using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

namespace com.impactional.chess
{
    public class lobbyManager : MonoBehaviourPun, IPunOwnershipCallbacks
    {


        static int viewIdCounter = 3;

        public GameObject lobbyCanvas;
        public GameObject chessBoard;

        public GameObject blackTimer;
        public GameObject whiteTimer;

        private void Start()
        {
            updateLobbyData();
        }


        public void updateLobbyData()
        {
            photonView.RPC("updateLobbyDataRPC", RpcTarget.AllBufferedViaServer);
        }

        [PunRPC]
        public void updateLobbyDataRPC()
        {

            if (PhotonNetwork.PlayerList.Length == 2)
            {
                lobbyCanvas.SetActive(false);
                PhotonNetwork.CurrentRoom.IsOpen = false;
                blackTimer.SetActive(true);
                whiteTimer.SetActive(true);
                if(chessBoard.GetComponent<multiplayerChessBoard>() != null)
                chessBoard.GetComponent<multiplayerChessBoard>().setCamera();

                if(chessBoard.GetComponent<tournamentChessBoard>() != null)
                {
                    chessBoard.GetComponent<tournamentChessBoard>().setCamera();
                }
                

               if(PhotonNetwork.IsMasterClient)
                chessBoard.GetPhotonView().RPC("SpawnAllPieces", RpcTarget.All);

            }

        }







        public static void transferOwnership(GameObject piece, int team)
        {

            piece.GetPhotonView().ViewID = viewIdCounter;
            viewIdCounter++;

            if (PhotonNetwork.IsMasterClient)
            {

                Debug.Log("after the id was : " + piece.GetPhotonView().ViewID);

                switch (team)
                {
                    case 0:
                        piece.GetPhotonView().TransferOwnership(PhotonNetwork.PlayerList[0]);
                        break;
                    case 1:
                        piece.GetPhotonView().TransferOwnership(PhotonNetwork.PlayerList[1]);
                        break;
                }
            }
        }

        public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
        {
            throw new System.NotImplementedException();
        }

        public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
        {
            
        }

        public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
        {
            throw new System.NotImplementedException();
        }
    }
}
