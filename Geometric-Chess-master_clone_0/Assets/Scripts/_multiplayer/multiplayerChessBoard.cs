using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.SceneManagement;

namespace com.impactional.chess
{



    public class multiplayerChessBoard : MonoBehaviourPunCallbacks
    {
        

        [Header("Art Stuff")]
        [SerializeField] private float tileSize = 1.0f;
        [SerializeField] Material tileMat1;
        [SerializeField] Material tileMat2;
        Material tileMat;
        [SerializeField] private float yOffset = 0.2f;
        [SerializeField] private float deathSize = 0.3f;
        [SerializeField] private float deathSpacing = 0.3f;
        [SerializeField] private float draggOffset = 1f;
        [SerializeField] private GameObject victoryScene;

        [Header("Prefabs and Materials")]
        [SerializeField] private GameObject[] prefabs;
        [SerializeField] private Material[] teamMaterials;


        [Header("UI")]
        [SerializeField] private GameObject piecePromotionSelectionMenuPanel;
        

        [Header("Timer")]
        public const string PLAYER_TIMER = "PLAYER_TIMER";
        public const int DEFAULT_PLAYER_TIMER_MIN = 10; //minutes
        public Text whiteTimerText;
        public Text blackTimerText;

      
        private float whiteTimer;
        private float blackTimer;



        //LOGIC

        private chessPiecePuzzle[,] chessPieces;
        private chessPiecePuzzle currentDragging;
        private List<chessPiecePuzzle> deadwhites = new List<chessPiecePuzzle>();
        private List<chessPiecePuzzle> deadBlacks = new List<chessPiecePuzzle>();

        private List<Vector2Int> availableMoves = new List<Vector2Int>();


        private const int TILE_COUNT_X = 8;
        private const int TILE_COUNT_Y = 8;
        private GameObject[,] tiles;
        private Camera currentCamera;
        private Vector2Int currentHover;
        private Vector3 bounds;
        private bool isWhiteTurn;

        private List<Vector2Int[]> moveList = new List<Vector2Int[]>();
        private SpecialMoves specialMoves;

        bool valid;

        [Header("otherScripts")]
        public endGameApiCAller matchEnderApiCalls;

        

        private void Awake()
        {


            currentCamera = Camera.main;
            tileMat = tileMat1;

            GenrateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
            
           

            isWhiteTurn = true;
        }



        private void Update()
        {

            //if (!currentCamera)
            //{
            //    currentCamera = Camera.main;
            //    return;
            //}

            RaycastHit info;
            Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover", "Highlight", "checkedking")))
            {
                Vector2Int hitPos = lookUpTileIndex(info.transform.gameObject);

                //if we are hovering a tile after hovering a no tile
                if (currentHover == -Vector2Int.one)
                {
                    currentHover = hitPos;
                    tiles[hitPos.x, hitPos.y].layer = LayerMask.NameToLayer("Hover");
                }

                //if we alredy have hovered a tile, change the previos one
                if (currentHover != hitPos)
                {
                    tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                    currentHover = hitPos;
                    tiles[hitPos.x, hitPos.y].layer = LayerMask.NameToLayer("Hover");
                }

                //if we press down on the mouse
                if (Input.GetMouseButtonDown(0))
                {
                    RemoveHighlightTiles();
                    if (chessPieces[hitPos.x, hitPos.y] != null )
                    {
                        if (chessPieces[hitPos.x, hitPos.y].gameObject.GetPhotonView().IsMine)
                        {
                            //is it our turn?
                            if ((chessPieces[hitPos.x, hitPos.y].team == 0 && isWhiteTurn) || (chessPieces[hitPos.x, hitPos.y].team == 1 && !isWhiteTurn))
                            {

                                currentDragging = chessPieces[hitPos.x, hitPos.y];

                                //get a list where i can go, highlight tiles as well
                                availableMoves = currentDragging.GetAvailMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                                // get a list of special moves;
                                specialMoves = currentDragging.GetSpecialMoves(ref chessPieces, ref moveList, ref availableMoves);

                                PreventCheck();
                                HighlightTiles();
                            }

                        }
                    }
                }

                //if we are realsing the mouse button
                if (currentDragging != null && Input.GetMouseButtonUp(0))
                {
                    Vector2Int prevPos = new Vector2Int(currentDragging.currentX, currentDragging.currentY);

                    photonView.RPC("MoveTo", RpcTarget.All, currentDragging.currentX, currentDragging.currentY, hitPos.x, hitPos.y);







                    if (!valid)
                    {

                        currentDragging.setPostion(getTileCenter(prevPos.x, prevPos.y));

                    }


                   // currentDragging = null;
                    //RemoveHighlightTiles();


                }

            }
            else
            {
                if (currentHover != -Vector2Int.one)
                {
                    tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                    currentHover = -Vector2Int.one;
                }
                if (currentDragging && Input.GetMouseButtonDown(0))
                {
                    currentDragging.setPostion(getTileCenter(currentDragging.currentX, currentDragging.currentY));
                    currentDragging = null;
                    RemoveHighlightTiles();
                }
            }

            //if we are draggin a piece
            if (currentDragging)
            {
                if (Input.GetMouseButton(0))
                {
                     Plane horizontalPlane = new Plane(Vector3.up, Vector3.up * yOffset);
                     float distance = 0.0f;
                     if (horizontalPlane.Raycast(ray, out distance))
                     {
                            currentDragging.setPostion(ray.GetPoint(distance) + Vector3.up * draggOffset);

                     }
                }
                else
                {
                    PositionSinglePieces(currentDragging.currentX, currentDragging.currentY);
                }
            }



            //TIMER
            ///////
            if (isWhiteTurn)
            {
                whiteTimer -= Time.deltaTime;
                if (whiteTimer < 0)
                {
                    whiteTimer = 0;
                    //endTheGame();
                }
                UpdateWhiteTimer();
            }
            else
            {
                blackTimer -= Time.deltaTime;
                if (blackTimer < 0)
                {
                    blackTimer = 0;
                    //endTheGame();
                }
                UpdateBlackTimer();
            }

            

        }



        //GENRATE CHESSBOARD
        void GenrateAllTiles(float tileSize, int tilecountx, int tileCounty)
        {
            yOffset += transform.position.y;
            bounds = new Vector3((tilecountx / 2) * tileSize, 0, (tileCounty / 2) * tileSize);

            tiles = new GameObject[tilecountx, tileCounty];
            for (int x = 0; x < tilecountx; x++)
            {
                for (int y = 0; y < tileCounty; y++)
                {
                    tiles[x, y] = GenrateSingleTile(tileSize, x, y);
                }
                tileMat = swapTileMaterial(tileMat);
            }
        }
        GameObject GenrateSingleTile(float tileSize, int x, int y)
        {
            tileMat = swapTileMaterial(tileMat);

            GameObject tileObject = new GameObject(string.Format("X:{0}, Y:{1}", x, y));

            tileObject.transform.parent = transform;

            Mesh mesh = new Mesh();
            tileObject.AddComponent<MeshFilter>().mesh = mesh;
            tileObject.AddComponent<MeshRenderer>().material = tileMat;

            Vector3[] vertices = new Vector3[4];
            vertices[0] = new Vector3(x * tileSize, yOffset, y * tileSize) - bounds;
            vertices[1] = new Vector3(x * tileSize, yOffset, (y + 1) * tileSize) - bounds;
            vertices[2] = new Vector3((x + 1) * tileSize, yOffset, y * tileSize) - bounds;
            vertices[3] = new Vector3((x + 1) * tileSize, yOffset, (y + 1) * tileSize) - bounds;


            int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };
            mesh.vertices = vertices;
            mesh.triangles = tris;

            mesh.RecalculateNormals();

            tileObject.layer = LayerMask.NameToLayer("Tile");
            tileObject.AddComponent<BoxCollider>();

            return tileObject;
        }
        Material swapTileMaterial(Material tileMat)
        {
            if (tileMat == tileMat1) return tileMat2;
            return tileMat1;
        }




        //SPAWING OF THE PIECES
        [PunRPC]
        public void SpawnAllPieces()
        {
            chessPieces = new chessPiecePuzzle[TILE_COUNT_X, TILE_COUNT_Y];

            int whiteTeam = 0, blackTeam = 1;



            for (int i = 0; i < TILE_COUNT_X; i++)
            {
                chessPieces[i, 1] = spawnSinglePiece(chessPieceTypePuzzle.Pawn, 0);
                chessPieces[i, 6] = spawnSinglePiece(chessPieceTypePuzzle.Pawn, 1);

            }

            //rooks
            chessPieces[0, 0] = spawnSinglePiece(chessPieceTypePuzzle.Rook, 0);
            chessPieces[7, 0] = spawnSinglePiece(chessPieceTypePuzzle.Rook, 0);
            chessPieces[0, 7] = spawnSinglePiece(chessPieceTypePuzzle.Rook, 1);
            chessPieces[7, 7] = spawnSinglePiece(chessPieceTypePuzzle.Rook, 1);

            //nights
            chessPieces[1, 0] = spawnSinglePiece(chessPieceTypePuzzle.Knight, 0);
            chessPieces[6, 0] = spawnSinglePiece(chessPieceTypePuzzle.Knight, 0);
            chessPieces[1, 7] = spawnSinglePiece(chessPieceTypePuzzle.Knight, 1);
            chessPieces[6, 7] = spawnSinglePiece(chessPieceTypePuzzle.Knight, 1);

            //bishop
            chessPieces[2, 0] = spawnSinglePiece(chessPieceTypePuzzle.Bishop, 0);
            chessPieces[5, 0] = spawnSinglePiece(chessPieceTypePuzzle.Bishop, 0);
            chessPieces[2, 7] = spawnSinglePiece(chessPieceTypePuzzle.Bishop, 1);
            chessPieces[5, 7] = spawnSinglePiece(chessPieceTypePuzzle.Bishop, 1);

            //queens
            chessPieces[3, 0] = spawnSinglePiece(chessPieceTypePuzzle.Queen, 0);
            chessPieces[3, 7] = spawnSinglePiece(chessPieceTypePuzzle.Queen, 1);

            //king
            chessPieces[4, 0] = spawnSinglePiece(chessPieceTypePuzzle.King, 0);
            chessPieces[4, 7] = spawnSinglePiece(chessPieceTypePuzzle.King, 1);





            PositionAllPieces();

            SetTimers();

        }
        private chessPiecePuzzle spawnSinglePiece(chessPieceTypePuzzle type, int team)
        {
            chessPiecePuzzle cp = Instantiate(prefabs[(int)type - 1], transform).GetComponent<chessPiecePuzzle>();

            cp.type = type;
            cp.team = team;
            cp.GetComponent<MeshRenderer>().material = teamMaterials[team];

            lobbyManager.transferOwnership(cp.gameObject, team);

            return cp;
        }



        //Position
        private void PositionAllPieces()
        {
            for (int x = 0; x < TILE_COUNT_X; x++)
                for (int y = 0; y < TILE_COUNT_Y; y++)
                    if (chessPieces[x, y] != null)
                        PositionSinglePieces(x, y, true);

        }
        private void PositionSinglePieces(int x, int y, bool force = false)
        {
            chessPieces[x, y].currentX = x;
            chessPieces[x, y].currentY = y;
            chessPieces[x, y].setPostion(getTileCenter(x, y), force);
        }
        private Vector3 getTileCenter(int x, int y)
        {
            return new Vector3(x * tileSize, yOffset, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
        }




        //HIGHLIGHT TILES
        private void HighlightTiles()
        {
            for (int i = 0; i < availableMoves.Count; i++)
            {
                tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Highlight");
                if (chessPieces[availableMoves[i].x, availableMoves[i].y] != null)
                    tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("checkedking");
            }
        }
        private void RemoveHighlightTiles()
        {
            for (int i = 0; i < availableMoves.Count; i++)
            {
                tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Tile");
            }

         
        }




        //CHECKMATE
        private void checkMate(int team)
        {
            dispalyVicotry(team);
           
            sendMatchData(team);
        }

        void sendMatchData(int team)
        {
            if (PhotonNetwork.LocalPlayer == PhotonNetwork.PlayerList[0])
            {
                if (team == 0)
                    matchEnderApiCalls.sendOnlineMatchStat(matchEnderApiCalls.MatchWonConstString);
                else
                    matchEnderApiCalls.sendOnlineMatchStat(matchEnderApiCalls.MatchLossConstString);
            }
            if (PhotonNetwork.LocalPlayer == PhotonNetwork.PlayerList[1])
            {
                if (team == 1)
                    matchEnderApiCalls.sendOnlineMatchStat(matchEnderApiCalls.MatchWonConstString);
                else
                    matchEnderApiCalls.sendOnlineMatchStat(matchEnderApiCalls.MatchLossConstString);
            }
        }
        private void dispalyVicotry(int winningTeam)
        {
            victoryScene.SetActive(true);
            victoryScene.transform.GetChild(winningTeam).gameObject.SetActive(true);
        }
        public void onResetButton()
        {
            //fields reset
            currentDragging = null;
            availableMoves.Clear();
            moveList.Clear();




            //cleanUp

            for (int x = 0; x < TILE_COUNT_X; x++)
            {
                for (int y = 0; y < TILE_COUNT_Y; y++)
                {
                    if (chessPieces[x, y] != null)
                        Destroy(chessPieces[x, y].gameObject);

                    chessPieces[x, y] = null;
                }

            }

            for (int i = 0; i < deadwhites.Count; i++)
                Destroy(deadwhites[i].gameObject);
            for (int i = 0; i < deadBlacks.Count; i++)
                Destroy(deadBlacks[i].gameObject);

            deadwhites.Clear();
            deadBlacks.Clear();



        }
        public void OnExitButton()
        {            
            PhotonNetwork.Disconnect();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            SceneManager.LoadScene("Main Menu");
        }
       



        // SPECIAL MOVES
        private void ProccessSpecialMOves()
        {

            if (specialMoves == SpecialMoves.Enpassant)
            {
                var newMove = moveList[moveList.Count - 1];
                chessPiecePuzzle myPawn = chessPieces[newMove[1].x, newMove[1].y];
                var targetPawnPos = moveList[moveList.Count - 2];
                chessPiecePuzzle enemyPawn = chessPieces[targetPawnPos[1].x, targetPawnPos[1].y];

                if (myPawn.currentX == enemyPawn.currentX)
                {
                    if (myPawn.currentY == enemyPawn.currentY - 1 || myPawn.currentY == enemyPawn.currentY + 1)
                    {
                        if (enemyPawn.team == 0)
                        {
                            deadwhites.Add(enemyPawn);
                            enemyPawn.setScale(Vector3.one * deathSize);
                            enemyPawn.setPostion(new Vector3(8 * tileSize, yOffset, -1 * tileSize)
                                - bounds
                                + new Vector3(tileSize / 2, 0, tileSize / 2)
                                + (Vector3.forward * deathSpacing) * deadwhites.Count);
                        }
                        else
                        {
                            deadBlacks.Add(enemyPawn);
                            enemyPawn.setScale(Vector3.one * deathSize);
                            enemyPawn.setPostion(new Vector3(-1 * tileSize, yOffset, 8 * tileSize)
                                - bounds
                                + new Vector3(tileSize / 2, 0, tileSize / 2)
                                + (Vector3.back * deathSpacing) * deadBlacks.Count);

                        }
                        chessPieces[enemyPawn.currentX, enemyPawn.currentY] = null;
                    }
                }

            }

            if (specialMoves == SpecialMoves.Promotion)
            {
                Vector2Int[] lastMove = moveList[moveList.Count - 1];
                chessPiecePuzzle targetePawn = chessPieces[lastMove[1].x, lastMove[1].y];

                if (targetePawn.type == chessPieceTypePuzzle.Pawn)
                {
                    if (targetePawn.team == 0 && lastMove[1].y == 7)
                    {

                        //show menu

                        showPromotionMenu();

                        //spawnTheSelected


                        //addd a pause state untill the player chooses


                        //chessPiecePuzzle newQueen = spawnSinglePiece(chessPieceTypePuzzle.Queen, 0);
                        //newQueen.transform.position = chessPieces[lastMove[1].x, lastMove[1].y].transform.position;
                        //Destroy((chessPieces[lastMove[1].x, lastMove[1].y]).gameObject);
                        //chessPieces[lastMove[1].x, lastMove[1].y] = newQueen;
                        //PositionSinglePieces(lastMove[1].x, lastMove[1].y);
                    }
                    if (targetePawn.team == 1 && lastMove[1].y == 0)
                    {
                        chessPiecePuzzle newQueen = spawnSinglePiece(chessPieceTypePuzzle.Queen, 1);
                        newQueen.transform.position = chessPieces[lastMove[1].x, lastMove[1].y].transform.position;
                        Destroy((chessPieces[lastMove[1].x, lastMove[1].y]).gameObject);
                        chessPieces[lastMove[1].x, lastMove[1].y] = newQueen;
                        PositionSinglePieces(lastMove[1].x, lastMove[1].y);
                    }
                }


            }

            if (specialMoves == SpecialMoves.Castling)
            {
                Vector2Int[] lastMove = moveList[moveList.Count - 1];

                //left rook
                if (lastMove[1].x == 2)
                {
                    if (lastMove[1].y == 0)//white side
                    {
                        chessPiecePuzzle rook = chessPieces[0, 0];
                        chessPieces[3, 0] = rook;
                        PositionSinglePieces(3, 0);
                        chessPieces[0, 0] = null;
                    }
                    else if (lastMove[1].y == 7)// black side
                    {
                        chessPiecePuzzle rook = chessPieces[0, 7];
                        chessPieces[3, 7] = rook;
                        PositionSinglePieces(3, 7);
                        chessPieces[0, 7] = null;

                    }
                }

                //left rook
                else if (lastMove[1].x == 6)
                {
                    if (lastMove[1].y == 0)//white side
                    {
                        chessPiecePuzzle rook = chessPieces[7, 0];
                        chessPieces[5, 0] = rook;
                        PositionSinglePieces(5, 0);
                        chessPieces[7, 0] = null;
                    }
                    else if (lastMove[1].y == 7)// black side
                    {
                        chessPiecePuzzle rook = chessPieces[7, 7];
                        chessPieces[5, 7] = rook;
                        PositionSinglePieces(5, 7);
                        chessPieces[7, 7] = null;

                    }
                }
            }


        }
        private void PreventCheck()
        {
            chessPiecePuzzle targetKing = null;
            for (int x = 0; x < TILE_COUNT_X; x++)
            {
                for (int y = 0; y < TILE_COUNT_Y; y++)
                {
                    if (chessPieces[x, y] != null)
                        if (chessPieces[x, y].type == chessPieceTypePuzzle.King)
                            if (chessPieces[x, y].team == currentDragging.team)
                                targetKing = chessPieces[x, y];
                }
            }
            //since we are sending ref available moves, we will be deleting moves that are putting us in the check
            SimulateMovesForSinglePiece(currentDragging, ref availableMoves, targetKing);
        }
        private void SimulateMovesForSinglePiece(chessPiecePuzzle cp, ref List<Vector2Int> moves, chessPiecePuzzle targetKing)
        {
            // save the current values, to reset after the funtion call
            int actualX = cp.currentX;
            int actualY = cp.currentY;

            List<Vector2Int> movesToRemove = new List<Vector2Int>();

            //going through all the moves, simulate them and check if we are in check
            for (int i = 0; i < moves.Count; i++)
            {
                int simX = moves[i].x;
                int simY = moves[i].y;

                Vector2Int kingPosThisSim = new Vector2Int(targetKing.currentX, targetKing.currentY);
                //did we simulate the king's move
                if (cp.type == chessPieceTypePuzzle.King)
                    kingPosThisSim = new Vector2Int(simX, simY);

                //copy the [,] and not a referecne
                chessPiecePuzzle[,] simulation = new chessPiecePuzzle[TILE_COUNT_X, TILE_COUNT_Y];
                List<chessPiecePuzzle> simAttackingPiece = new List<chessPiecePuzzle>();
                for (int x = 0; x < TILE_COUNT_X; x++)
                {
                    for (int y = 0; y < TILE_COUNT_Y; y++)
                    {
                        if (chessPieces[x, y] != null)
                        {
                            simulation[x, y] = chessPieces[x, y];
                            if (simulation[x, y].team != cp.team)
                                simAttackingPiece.Add(simulation[x, y]);
                        }
                    }

                }

                //simulate that move
                simulation[actualX, actualY] = null;
                cp.currentX = simX;
                cp.currentY = simY;
                simulation[simX, simY] = cp;

                //did one of the piece got taken down during our simulation
                var deadPiece = simAttackingPiece.Find(c => c.currentX == simX && c.currentY == simY);
                if (deadPiece != null)
                    simAttackingPiece.Remove(deadPiece);

                //get all the simulated attacking pieces moves
                List<Vector2Int> simMoves = new List<Vector2Int>();
                for (int a = 0; a < simAttackingPiece.Count; a++)
                {
                    var pieceMoves = simAttackingPiece[a].GetAvailMoves(ref simulation, TILE_COUNT_X, TILE_COUNT_Y);

                    for (int b = 0; b < pieceMoves.Count; b++)
                        simMoves.Add(pieceMoves[b]);
                }

                //is the king in trouble? if so, remove the move
                if (ContainsValidMove(ref simMoves, kingPosThisSim))
                {
                    movesToRemove.Add(moves[i]);
                }

                //restore the actual cp data;
                cp.currentX = actualX;
                cp.currentY = actualY;

            }

            //remove from the current available move list
            for (int i = 0; i < movesToRemove.Count; i++)
            {
                moves.Remove(movesToRemove[i]);
            }
        }
        private bool checkForCheckMate()
        {
            var lastMove = moveList[moveList.Count - 1];
            int targetTeam = (chessPieces[lastMove[1].x, lastMove[1].y]).team == 0 ? 1 : 0;

            List<chessPiecePuzzle> attackingPieces = new List<chessPiecePuzzle>();
            List<chessPiecePuzzle>  deffendingPieces = new List<chessPiecePuzzle>();
            chessPiecePuzzle targetKing = null;
            for (int x = 0; x < TILE_COUNT_X; x++)
            {
                for (int y = 0; y < TILE_COUNT_Y; y++)
                
                    if (chessPieces[x, y] != null)
                    {


                        if (chessPieces[x, y].team == targetTeam)
                        {
                            deffendingPieces.Add(chessPieces[x, y]);
                            if(chessPieces[x,y].type == chessPieceTypePuzzle.King)
                                targetKing = chessPieces[x, y];
                        }
                        else
                        {
                             attackingPieces.Add(chessPieces[x, y]);
                        }
                    }
            }

            //is the king attacked right now?
            List<Vector2Int> currentlyAvailableMoves = new List<Vector2Int>();
            for (int x = 0; x < attackingPieces.Count; x++)
            {
                var pieceMoves = attackingPieces[x].GetAvailMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);

                for (int b = 0; b < pieceMoves.Count; b++)
                    currentlyAvailableMoves.Add(pieceMoves[b]);
            }

            //Are we in check right now?
            if (ContainsValidMove(ref currentlyAvailableMoves, new Vector2Int(targetKing.currentX, targetKing.currentY)))
            {
                tiles[targetKing.currentX, targetKing.currentY].layer = LayerMask.NameToLayer("checkedking");
                //king is under attack, can we move something to help him?
                for (int i = 0; i < deffendingPieces.Count; i++)
                {
                    List<Vector2Int> defendingMoves = deffendingPieces[i].GetAvailMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                    //since we're sending ref available moevs. we will be deleting moves that puts under check
                    SimulateMovesForSinglePiece(deffendingPieces[i], ref defendingMoves, targetKing);

                    if (defendingMoves.Count != 0)
                        return false;
                }

                return true;//checkMate exit
            }

            return false;
        }
        

        //OPERATION
        private bool ContainsValidMove(ref List<Vector2Int> moves, Vector2 pos)
        {
            for (int i = 0; (i < moves.Count); i++)
                if (moves[i].x == pos.x && moves[i].y == pos.y)
                    return true;

            return false;
        }
        [PunRPC]
        private void MoveTo(int prevX, int prevY, int x, int y)
        {
            chessPiecePuzzle cp = chessPieces[prevX, prevY];

            availableMoves = cp.GetAvailMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
            // get a list of special moves;
            specialMoves = cp.GetSpecialMoves(ref chessPieces, ref moveList, ref availableMoves);

            

            if (!ContainsValidMove(ref availableMoves, new Vector2(x, y)))
            {
                valid = false;
                return;
            }

            Vector2Int previousPosition = new Vector2Int(cp.currentX, cp.currentY);


            //is there another piece on the target postion ?
            if (chessPieces[x, y] != null)
            {
                chessPiecePuzzle ocp = chessPieces[x, y];

                if (cp.team == ocp.team) 
                { 
                    valid = false;
                    return;
                }

                //if its the enemy team
                if (ocp.team == 0)
                {
                    deadwhites.Add(ocp);
                    ocp.setScale(Vector3.one * deathSize);
                    ocp.setPostion(new Vector3(8 * tileSize, yOffset, -1 * tileSize)
                        - bounds
                        + new Vector3(tileSize / 2, 0, tileSize / 2)
                        + (Vector3.forward * deathSpacing) * deadwhites.Count);
                }
                else
                {
                    deadBlacks.Add(ocp);
                    ocp.setScale(Vector3.one * deathSize);
                    ocp.setPostion(new Vector3(-1 * tileSize, yOffset, 8 * tileSize)
                        - bounds
                        + new Vector3(tileSize / 2, 0, tileSize / 2)
                        + (Vector3.back * deathSpacing) * deadBlacks.Count);
                }

            }

            chessPieces[x, y] = cp;
            chessPieces[previousPosition.x, previousPosition.y] = null;



            PositionSinglePieces(x, y);

            isWhiteTurn = !isWhiteTurn;

            moveList.Add(new Vector2Int[] { previousPosition, new Vector2Int(x, y) });



            ProccessSpecialMOves();

            if (checkForCheckMate())
            {
                checkMate(cp.team);
            }

            valid = true;
        }
        private Vector2Int lookUpTileIndex(GameObject hitinfo)
        {
            for (int x = 0; x < TILE_COUNT_X; x++)
                for (int y = 0; y < TILE_COUNT_Y; y++)
                    if (tiles[x, y] == hitinfo)
                        return new Vector2Int(x, y);


            return -Vector2Int.one;


        }

       
       

        //Promotion pieces
        private void showPromotionMenu()
        {
            piecePromotionSelectionMenuPanel.SetActive(true);
        }
        public void PromoteToQueen()
        {

            Vector2Int[] lastMove = moveList[moveList.Count - 1];


            chessPiecePuzzle newQueen = spawnSinglePiece(chessPieceTypePuzzle.Queen, 0);
            newQueen.transform.position = chessPieces[lastMove[1].x, lastMove[1].y].transform.position;
            Destroy((chessPieces[lastMove[1].x, lastMove[1].y]).gameObject);
            chessPieces[lastMove[1].x, lastMove[1].y] = newQueen;
            PositionSinglePieces(lastMove[1].x, lastMove[1].y);

            piecePromotionSelectionMenuPanel.SetActive(false);
        }
        public void PromoteToBishop()
        {
            Vector2Int[] lastMove = moveList[moveList.Count - 1];


            chessPiecePuzzle newQueen = spawnSinglePiece(chessPieceTypePuzzle.Bishop, 0);
            newQueen.transform.position = chessPieces[lastMove[1].x, lastMove[1].y].transform.position;
            Destroy((chessPieces[lastMove[1].x, lastMove[1].y]).gameObject);
            chessPieces[lastMove[1].x, lastMove[1].y] = newQueen;
            PositionSinglePieces(lastMove[1].x, lastMove[1].y);

            piecePromotionSelectionMenuPanel.SetActive(false);
        }
        public void PromoteToNight()
        {
            Vector2Int[] lastMove = moveList[moveList.Count - 1];


            chessPiecePuzzle newQueen = spawnSinglePiece(chessPieceTypePuzzle.Knight, 0);
            newQueen.transform.position = chessPieces[lastMove[1].x, lastMove[1].y].transform.position;
            Destroy((chessPieces[lastMove[1].x, lastMove[1].y]).gameObject);
            chessPieces[lastMove[1].x, lastMove[1].y] = newQueen;
            PositionSinglePieces(lastMove[1].x, lastMove[1].y);

            piecePromotionSelectionMenuPanel.SetActive(false);
        }
        public void PromoteToRook()
        {
            Vector2Int[] lastMove = moveList[moveList.Count - 1];


            chessPiecePuzzle newQueen = spawnSinglePiece(chessPieceTypePuzzle.Rook, 0);
            newQueen.transform.position = chessPieces[lastMove[1].x, lastMove[1].y].transform.position;
            Destroy((chessPieces[lastMove[1].x, lastMove[1].y]).gameObject);
            chessPieces[lastMove[1].x, lastMove[1].y] = newQueen;
            PositionSinglePieces(lastMove[1].x, lastMove[1].y);

            piecePromotionSelectionMenuPanel.SetActive(false);
        }


        //TIMER
        void SetTimers()
        {
            float timer = PlayerPrefs.GetInt(PLAYER_TIMER, DEFAULT_PLAYER_TIMER_MIN); //get in minutes
            timer *= 60f; //convert to seconds
            whiteTimer = blackTimer = timer;
            UpdateWhiteTimer();
            UpdateBlackTimer();
        }
        void UpdateWhiteTimer()
        {
            whiteTimerText.text = GetChessTimeFormat(whiteTimer);
        }

        //EXPERIMENT_TIMER
        void UpdateBlackTimer()
        {
            blackTimerText.text = GetChessTimeFormat(blackTimer);
        }
        string GetChessTimeFormat(float timeInSeconds)
        {
            int seconds = (int)(timeInSeconds % 60);
            int minutes = (int)(timeInSeconds / 60);

            int hours = (int)(minutes / 60);
            minutes = (int)(minutes % 60);
            string d2 = "D2";
            return hours.ToString(d2) + ":" + minutes.ToString(d2) + ":" + seconds.ToString(d2);
        }

        //Camera
        public void setCamera()
        {
            if(PhotonNetwork.LocalPlayer == PhotonNetwork.PlayerList[0])
            {
                currentCamera.transform.SetPositionAndRotation(new Vector3(0f, 5.6f, -5f), Quaternion.Euler(54f, 0f, 0f));
            }
            if(PhotonNetwork.LocalPlayer == PhotonNetwork.PlayerList[1])
            {
                currentCamera.transform.SetPositionAndRotation(new Vector3(0f, 6f, 5f), Quaternion.Euler(54f, 180f, 0f));
            }
        }
    }

}
