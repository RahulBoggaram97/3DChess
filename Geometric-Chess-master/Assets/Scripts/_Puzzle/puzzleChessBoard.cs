using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public enum SpecialMoves
{
    None = 0,
    Enpassant,
    Castling,
    Promotion

}
public class puzzleChessBoard : MonoBehaviour
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

    [Header("Prefabs and Materials")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teamMaterials;


    [Header("UI")]
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private GameObject retryButton;
    [SerializeField] private GameObject nextPuzzleButton;



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

    //puzzle Scripts
    public com.impactional.chess.puzzleApi puzzleApiCaller;
    public string fenPosition;
    public string resultPgn;





    private void Awake()
    {
        uiPanel.SetActive(false);
        retryButton.SetActive(false);

        currentCamera = Camera.main;
        tileMat = tileMat1;

        GenrateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
        puzzleApiCaller.fetchPuzzle();

        isWhiteTurn = true;
    }



    private void Update()
    {
      
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            printMove();
        }



        //if (!currentCamera)
        //{
        //    currentCamera = Camera.main;
        //    return;
        //}

        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover", "Highlight")))
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
                if (chessPieces[hitPos.x, hitPos.y] != null)
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

            //if we are realsing the mouse button
            if (currentDragging != null && Input.GetMouseButtonUp(0))
            {
                Vector2Int prevPos = new Vector2Int(currentDragging.currentX, currentDragging.currentY);

                bool valid = MoveTo(currentDragging, hitPos.x, hitPos.y);


                
                



                if (!valid)
                {
                   
                    currentDragging.setPostion(getTileCenter(prevPos.x, prevPos.y));

                }


                currentDragging = null;
                RemoveHighlightTiles();

                //puzzle decide to go or not
                if (valid)
                {
                    if (CompareMoves(new Vector2Int[] { prevPos, new Vector2Int(hitPos.x, hitPos.y) }, matchableMove))
                        ContinuePuzzle();
                    else
                        HandleWrongMoveSatate();
                }
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
            Plane horizontalPlane = new Plane(Vector3.up, Vector3.up * yOffset);
            float distance = 0.0f;
            if (horizontalPlane.Raycast(ray, out distance))
            {
                currentDragging.setPostion(ray.GetPoint(distance) + Vector3.up * draggOffset);

            }
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
    public void SpawnAllPieces()
    {
        chessPieces = new chessPiecePuzzle[TILE_COUNT_X, TILE_COUNT_Y];

        int whiteTeam = 0, blackTeam = 1;

        int lastspawnIndex = 0;


        int r = 0;
        int c = 7;

        for (int i = 0; fenPosition[i] != ' '; i++)
        {

            switch (fenPosition[i])
            {


                case 'p':
                    chessPieces[r, c] = spawnSinglePiece(chessPieceTypePuzzle.Pawn, blackTeam);
                    r++;
                    break;
                case 'P':
                    chessPieces[r, c] = spawnSinglePiece(chessPieceTypePuzzle.Pawn, whiteTeam);
                    r++;
                    break;
                case 'n':
                    chessPieces[r, c] = spawnSinglePiece(chessPieceTypePuzzle.Knight, blackTeam);
                    r++;
                    break;
                case 'N':
                    chessPieces[r, c] = spawnSinglePiece(chessPieceTypePuzzle.Knight, whiteTeam);
                    r++;
                    break;
                case 'r':
                    chessPieces[r, c] = spawnSinglePiece(chessPieceTypePuzzle.Rook, blackTeam);
                    r++;
                    break;
                case 'R':
                    chessPieces[r, c] = spawnSinglePiece(chessPieceTypePuzzle.Rook, whiteTeam);
                    r++;
                    break;
                case 'b':
                    chessPieces[r, c] = spawnSinglePiece(chessPieceTypePuzzle.Bishop, blackTeam);
                    r++;
                    break;
                case 'B':
                    chessPieces[r, c] = spawnSinglePiece(chessPieceTypePuzzle.Bishop, whiteTeam);
                    r++;
                    break;
                case 'q':
                    chessPieces[r, c] = spawnSinglePiece(chessPieceTypePuzzle.Queen, blackTeam);
                    r++;
                    break;
                case 'Q':
                    chessPieces[r, c] = spawnSinglePiece(chessPieceTypePuzzle.Queen, whiteTeam);
                    r++;
                    break;
                case 'k':
                    chessPieces[r, c] = spawnSinglePiece(chessPieceTypePuzzle.King, blackTeam);
                    r++;
                    break;
                case 'K':
                    chessPieces[r, c] = spawnSinglePiece(chessPieceTypePuzzle.King, whiteTeam);
                    r++;
                    break;
                case '/':
                    c--;
                    r = 0;
                    break;
                case ' ':

                    break;
                default:

                    //Debug.Log(" value of r before :" + r);

                    //Debug.Log("value of the char :" + fenPosition[i]);
                    if (char.IsDigit(fenPosition[i]))
                        r = r + int.Parse(fenPosition[i].ToString());
                    //Debug.Log(r);
                    break;

            }
            lastspawnIndex = i;

        }

        PositionAllPieces();

        lastspawnIndex = lastspawnIndex + 2;

        Debug.Log(fenPosition[lastspawnIndex]);

        setTurn(fenPosition[lastspawnIndex]);

        int castleindex = lastspawnIndex + 2;

        Debug.Log(fenPosition.Substring(castleindex));

        ConvertResultStringToPuzzleMoveList();
       
    }

    private chessPiecePuzzle spawnSinglePiece(chessPieceTypePuzzle type, int team)
    {
        chessPiecePuzzle cp = Instantiate(prefabs[(int)type - 1], transform).GetComponent<chessPiecePuzzle>();

        cp.type = type;
        cp.team = team;
        cp.GetComponent<MeshRenderer>().material = teamMaterials[team];

        return cp;
    }

    void setTurn(char playerType)
    {
        switch (playerType)
        {
            case 'w':
                isWhiteTurn = true;
                break;
            case 'b':
                isWhiteTurn = false;
                break;
        }
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
        }
    }

    private void RemoveHighlightTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
        {
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Tile");
        }

        availableMoves.Clear();
    }




    //CHECKMATE

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
                    chessPiecePuzzle newQueen = spawnSinglePiece(chessPieceTypePuzzle.Queen, 0);
                    newQueen.transform.position = chessPieces[lastMove[1].x, lastMove[1].y].transform.position;
                    Destroy((chessPieces[lastMove[1].x, lastMove[1].y]).gameObject);
                    chessPieces[lastMove[1].x, lastMove[1].y] = newQueen;
                    PositionSinglePieces(lastMove[1].x, lastMove[1].y);
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


    //OPERATION
    private bool ContainsValidMove(ref List<Vector2Int> moves, Vector2 pos)
    {
        for (int i = 0; (i < moves.Count); i++)
            if (moves[i].x == pos.x && moves[i].y == pos.y)
                return true;

        return false;
    }


    private bool MoveTo(chessPiecePuzzle cp, int x, int y)
    {

        if (!ContainsValidMove(ref availableMoves, new Vector2(x, y)))
            return false;

        Vector2Int previousPosition = new Vector2Int(cp.currentX, cp.currentY);


        //is there another piece on the target postion ?
        if (chessPieces[x, y] != null)
        {
            chessPiecePuzzle ocp = chessPieces[x, y];

            if (cp.team == ocp.team)
                return false;

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

       

        return true;
    }


    private Vector2Int lookUpTileIndex(GameObject hitinfo)
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (tiles[x, y] == hitinfo)
                    return new Vector2Int(x, y);


        return -Vector2Int.one;


    }


    //Puzzle operation


    int resultPgnIndexer = 0;
    Vector2Int[] matchableMove = new Vector2Int[2];


    public void ConvertResultStringToPuzzleMoveList()
    {
        matchableMove = null;
        string[] brokenResultPgn = resultPgn.Split(null);

        if(resultPgnIndexer >= brokenResultPgn.Length)
        {
            endPuzzle();
            return;
        }
        

        Debug.Log("current string working on " + brokenResultPgn[resultPgnIndexer] + " and its length is :" + brokenResultPgn[resultPgnIndexer].Length);

        if (brokenResultPgn[resultPgnIndexer].Contains("."))
        {
            resultPgnIndexer++;
            ConvertResultStringToPuzzleMoveList();
            return;
        }
        else if(brokenResultPgn[resultPgnIndexer].Length == 2)
        {
            matchableMove = convertTwoToPawnMoves(brokenResultPgn[resultPgnIndexer]);
        }
        else if(brokenResultPgn[resultPgnIndexer].Length == 3)
        {
            matchableMove = convertThreeToMoves(brokenResultPgn[resultPgnIndexer]);
        }
        else if(brokenResultPgn[resultPgnIndexer].Length == 4)
        {
            matchableMove = convertFourToMoves(brokenResultPgn[resultPgnIndexer]);
        }
        else if(brokenResultPgn[resultPgnIndexer].Length == 5)
        {
            matchableMove = convertFiveToMoves(brokenResultPgn[resultPgnIndexer]);
        }
        

    }


   

    //what to do for keep continuing the puzzle
    public void ContinuePuzzle()
    {
        resultPgnIndexer++;
        ConvertResultStringToPuzzleMoveList();
       
        simulateOtherPlayerMoves();

     

    }

    private void simulateOtherPlayerMoves()
    {
        if (matchableMove != null) 
        {
            if (chessPieces[matchableMove[0].x, matchableMove[0].y] != null)
            {
                
                chessPiecePuzzle cp = chessPieces[matchableMove[0].x, matchableMove[0].y];


                //get a list where i can go, highlight tiles as well
                availableMoves = cp.GetAvailMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                // get a list of special moves;
                specialMoves = cp.GetSpecialMoves(ref chessPieces, ref moveList, ref availableMoves);

               



                bool valid = MoveTo(cp, matchableMove[1].x, matchableMove[1].y);

                StartCoroutine(waitforMovestobeDoneThenCalculateNextMove());
            }
            else { Debug.Log("chess piece is null"); }
        }
        else
        {
            Debug.Log("matchable Move is null");
        }
    }

    IEnumerator waitforMovestobeDoneThenCalculateNextMove()
    {
        yield return new WaitForSeconds(1);

        resultPgnIndexer++;
        ConvertResultStringToPuzzleMoveList();
    }

    public void HandleWrongMoveSatate()
    {
        uiPanel.SetActive(true);
        retryButton.SetActive(true);
        nextPuzzleButton.SetActive(false);
    }

    public void endPuzzle()
    {
        uiPanel.SetActive(true);
        retryButton.SetActive(false);
        nextPuzzleButton.SetActive(true);

    }

    ////move converter Funtions

    #region
    //private void ConvertTwoToPawnMoves(string moveNotion, int team)
    //{
    //    if (team == 0)
    //    {
    //        //enter the moveNotion ie some like e4 to the destination
    //        Vector2Int destinationPos = TwoLettertoVecotor2Pos(moveNotion);
    //        Vector2Int intialPos = new Vector2Int();

    //        //look if there is down under
    //        if (chessPieces[destinationPos.x, destinationPos.y - 1] != null)
    //        {
    //            intialPos = new Vector2Int(destinationPos.x, destinationPos.y - 1);
    //        }
    //        //look if ther is 2 down under
    //        if (chessPieces[destinationPos.x, destinationPos.y - 2] != null)
    //        {
    //            intialPos = new Vector2Int(destinationPos.x, destinationPos.y - 2);
    //        }

    //        OurPuzzleMoves.Add(new Vector2Int[] { intialPos, destinationPos });
    //    }
    //    else
    //    {
    //        //for black

    //        //enter the moveNotion ie some like e4 to the destination
    //        Vector2Int destinationPos = TwoLettertoVecotor2Pos(moveNotion);
    //        Vector2Int intialPos = new Vector2Int();

    //        //look if there is a pawn up above
    //        if (chessPieces[destinationPos.x, destinationPos.y + 1] != null)
    //        {
    //            intialPos = new Vector2Int(destinationPos.x, destinationPos.y + 1);
    //        }
    //        //look if there is a pawn two up above
    //        if (chessPieces[destinationPos.x, destinationPos.y + 2] != null)
    //        {
    //            intialPos = new Vector2Int(destinationPos.x, destinationPos.y + 2);
    //        }
    //        OurPuzzleMoves.Add(new Vector2Int[] { intialPos, destinationPos });

    //    }
    //}
    #endregion

    private Vector2Int[] convertTwoToPawnMoves(string moveNotion)
    {
        Vector2Int destinationPos = TwoLettertoVecotor2Pos(moveNotion);     
        if (isWhiteTurn)
        {
            if (chessPieces[destinationPos.x, destinationPos.y - 1] != null)
            {
                return new Vector2Int[] { new Vector2Int(destinationPos.x, destinationPos.y - 1), destinationPos };
            }
            //look if ther is 2 down under
            if (chessPieces[destinationPos.x, destinationPos.y - 2] != null)
            {
                return new Vector2Int[] { new Vector2Int(destinationPos.x, destinationPos.y - 2), destinationPos };
            }
            else
            {
                return null;
            }         
        }
        else
        {
            if (chessPieces[destinationPos.x, destinationPos.y + 1] != null)
            {
                return new Vector2Int[] { new Vector2Int(destinationPos.x, destinationPos.y + 1), destinationPos };
            }
            //look if ther is 2 down under
            if (chessPieces[destinationPos.x, destinationPos.y + 2] != null)
            {
                return new Vector2Int[] { new Vector2Int(destinationPos.x, destinationPos.y + 2), destinationPos };
            }
            else
            {
                return null;
            }
        }      
    }

    private Vector2Int[] convertThreeToMoves(string moveNotion)
    {
            
        Debug.Log("x detected so the string we passed int the funtion is :" + moveNotion.Substring(1, 2));
        if (moveNotion[0] == 'x')
        {

           return convertTwoToPawnMoves(moveNotion.Substring(1, 2));
        }
        if (moveNotion[0] == 'K')
        {
            chessPiecePuzzle king = SearchKing();
            return new Vector2Int[] { new Vector2Int(king.currentX, king.currentY), TwoLettertoVecotor2Pos(moveNotion.Substring(1, 2)) };

        }
        if (moveNotion[0] == 'Q')
        {
            chessPiecePuzzle queen = SearchQueen();
            return new Vector2Int[] { new Vector2Int(queen.currentX, queen.currentY), TwoLettertoVecotor2Pos(moveNotion.Substring(1, 2)) };
        }
        if (moveNotion[0] == 'B')
        {
            chessPiecePuzzle bishop = SearchBishop(moveNotion.Substring(1, 2));
            return new Vector2Int[] { new Vector2Int(bishop.currentX, bishop.currentY), TwoLettertoVecotor2Pos(moveNotion.Substring(1, 2)) };
        }
        if (moveNotion[0] == 'N')
        {
            chessPiecePuzzle night = SearchKnight(moveNotion.Substring(1, 2));
            return new Vector2Int[] { new Vector2Int(night.currentX, night.currentY), TwoLettertoVecotor2Pos(moveNotion.Substring(1, 2)) };
        }
        if (moveNotion[0] == 'R')
        {
            chessPiecePuzzle rook = SearchRook(moveNotion.Substring(1, 2));
            return new Vector2Int[] { new Vector2Int(rook.currentX, rook.currentY), TwoLettertoVecotor2Pos(moveNotion.Substring(1, 2)) };
        }
        if (moveNotion.Contains("+"))
        {
           return convertTwoToPawnMoves(moveNotion.Substring(0, 2));
        }

        return null;
    }

    private Vector2Int[] convertFourToMoves(string moveNotion)
    {
        char[] kar = { moveNotion[0], moveNotion[2], moveNotion[3] };

        Debug.Log("Four string dectected and the notion will be sent as " + new string(kar));


        if (moveNotion.Contains("+"))
        {
            return convertThreeToMoves(moveNotion.Substring(0, 3));
        }
        if (moveNotion.Contains("x"))
        {
            if (moveNotion[0] == 'K' || moveNotion[0] == 'Q' || moveNotion[0] == 'B' || moveNotion[0] == 'N' || moveNotion[0] == 'R')
            {
                char[] shortKar = { moveNotion[0], moveNotion[2], moveNotion[3] };

                return convertThreeToMoves(new string(shortKar));
            }
        }
        if (moveNotion.Contains("#"))
        {
            return convertThreeToMoves(moveNotion.Substring(0, 3));
        }
        
        if (!char.IsDigit(moveNotion[1]))
        {
            return convertFourWithSecondCharToMove(moveNotion);
        }
        if (char.IsDigit(moveNotion[1]))
        {
            //return 
        }

        return null;
    }

    private Vector2Int[] convertFiveToMoves(string moveNotion)
    {
        if (moveNotion.Contains("+"))
        {
            return convertFourToMoves(moveNotion.Substring(0, 4));
        }
        if (moveNotion.Contains("#"))
        {
            return convertFourToMoves(moveNotion.Substring(0, 4));
        }


        return null;
    }




    //ADDITION MOVE CREATIONS

    private Vector2Int[] convertFourWithSecondCharToMove(string moveNotion)
    {
        int x_index = ConvertLetterToDigit(moveNotion[1]);
        chessPieceTypePuzzle typeNeeded = calculateTheTypeNeededByLetter(moveNotion[0]);
        Vector2Int destinationPos = TwoLettertoVecotor2Pos(moveNotion.Substring(2, 2));
       

        for (int i = 0; i < TILE_COUNT_Y; i++)
        {
            if (chessPieces[x_index, i] != null)
            {
                if (chessPieces[x_index, i].type == typeNeeded)
                {
                    chessPiecePuzzle cp = chessPieces[x_index, i];

                    return new Vector2Int[] { new Vector2Int(cp.currentX, cp.currentY), destinationPos };
                }
            }

        }

            return null;
    }

    private Vector2Int[] convertFourWithSecondDIGITtoMove(string moveNotion)
    {
        int x_index = ConvertLetterToDigit(moveNotion[1]);
        chessPieceTypePuzzle typeNeeded = calculateTheTypeNeededByLetter(moveNotion[0]);
        Vector2Int destinationPos = TwoLettertoVecotor2Pos(moveNotion.Substring(2, 2));


        for (int i = 0; i < TILE_COUNT_Y; i++)
        {
            if (chessPieces[x_index, i] != null)
                if (chessPieces[x_index, i].type == typeNeeded)
                {
                    chessPiecePuzzle cp = chessPieces[x_index, i];

                    return new Vector2Int[] { new Vector2Int(cp.currentX, cp.currentY), destinationPos };
                }

        }

        return null;
    }



    private chessPieceTypePuzzle calculateTheTypeNeededByLetter(char type)
    {
        switch (type)
        {
            case 'K':
                return chessPieceTypePuzzle.King;
            case 'Q':
                return chessPieceTypePuzzle.King;
            case 'B':
                return chessPieceTypePuzzle.King;
            case 'N':
                return chessPieceTypePuzzle.King;
            case 'R':
                return chessPieceTypePuzzle.King;
            default:
                return chessPieceTypePuzzle.None;

        }
    }

    //puzzleHelper funtions

    private Vector2Int TwoLettertoVecotor2Pos(string moveNotion)
    {
        int x = ConvertLetterToDigit(moveNotion[0]);
        int y = int.Parse(moveNotion[1].ToString()) - 1;

        return new Vector2Int(x, y);
    }

    private int ConvertLetterToDigit(char letter)
    {
        switch (letter)
        {
            case 'a':
                return 0;

            case 'b':
                return 1;

            case 'c':
                return 2;

            case 'd':
                return 3;

            case 'e':
                return 4;

            case 'f':
                return 5;

            case 'g':
                return 6;

            case 'h':
                return 7;
            default:
                return -1;

        }
    }

    private int SwitchTeamIndex(int currentTeam)
    {
        if (currentTeam == 0)
            return 1;
        return 0;
    }

    private bool CompareMoves(Vector2Int[] MadeMove, Vector2Int[] puzzleMove)
    {
        if (MadeMove[0] == puzzleMove[0] && MadeMove[1] == puzzleMove[1])
            return true;

        return false;
    }


    //piece Search methods
    private chessPiecePuzzle SearchKing()
    {
        int team = 0;

        if (isWhiteTurn)
            team = 0;
        else 
            team = 1;

        chessPiecePuzzle foundKing = null;

        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (chessPieces[x, y] != null)
                {
                    if (chessPieces[x, y].type == chessPieceTypePuzzle.King)
                    {
                        if (chessPieces[x, y].team == team)
                        {
                            foundKing = chessPieces[x, y];
                        }
                    }
                }
            }
        }


        return foundKing;
    }

    private chessPiecePuzzle SearchQueen()
    {
        int team = 0;

        if (isWhiteTurn)
            team = 0;
        else
            team = 1;

        chessPiecePuzzle foundQueen = null;

        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (chessPieces[x, y] != null)
                    if (chessPieces[x, y].type == chessPieceTypePuzzle.Queen)
                        if (chessPieces[x, y].team == team)
                            foundQueen = chessPieces[x, y];
            }
        }


        return foundQueen;
    }

    private chessPiecePuzzle SearchBishop(string destinationMoveNotion)
    {
        int team = 0;

        if (isWhiteTurn)
            team = 0;
        else
            team = 1;

        Vector2Int destinationPos = TwoLettertoVecotor2Pos(destinationMoveNotion);
        List<Vector2Int> availMovesForBishop = new List<Vector2Int>();

        chessPiecePuzzle bishop = null;
        chessPiecePuzzle finalBishp = null;

        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (chessPieces[x, y] != null)
                {
                    if (chessPieces[x, y].type == chessPieceTypePuzzle.Bishop)
                    {
                        if (chessPieces[x, y].team == team)
                        {
                            bishop = chessPieces[x, y];

                            availMovesForBishop = bishop.GetAvailMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                            if (availMovesForBishop.Contains(destinationPos))
                            {

                                finalBishp = bishop;
                            }
                        }
                    }
                }
            }
        }

        return finalBishp;
    }

    private chessPiecePuzzle SearchKnight( string destinationMoveNotion)
    {
        int team = 0;

        if (isWhiteTurn)
            team = 0;
        else
            team = 1;

        Vector2Int destinationPos = TwoLettertoVecotor2Pos(destinationMoveNotion);

        List<Vector2Int> availMovesForKnight = new List<Vector2Int>();

        chessPiecePuzzle Knight = null;
        chessPiecePuzzle finalKingiht = null;

        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (chessPieces[x, y] != null)
                {
                    if (chessPieces[x, y].type == chessPieceTypePuzzle.Knight)
                    {
                        if (chessPieces[x, y].team == team)
                        {
                            Knight = chessPieces[x, y];
                            availMovesForKnight = Knight.GetAvailMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                            if (availMovesForKnight.Contains(destinationPos))
                            {


                                finalKingiht = Knight;
                            }
                        }
                    }
                }
            }
        }

        return finalKingiht;
    }

    private chessPiecePuzzle SearchRook(string destinationMoveNotion)
    {
        int team = 0;

        if (isWhiteTurn)
            team = 0;
        else
            team = 1;

        Vector2Int destinationPos = TwoLettertoVecotor2Pos(destinationMoveNotion);
        List<Vector2Int> availMovesForRook = new List<Vector2Int>();

        chessPiecePuzzle rook = null;
        chessPiecePuzzle finalRook = null;

        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (chessPieces[x, y] != null)
                {
                    if (chessPieces[x, y].type == chessPieceTypePuzzle.Rook)
                    {
                        if (chessPieces[x, y].team == team)
                        {
                            rook = chessPieces[x, y];
                            availMovesForRook = rook.GetAvailMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                            if (availMovesForRook.Contains(destinationPos))
                            {

                                finalRook = rook;
                            }
                        }
                    }
                }
            }
        }

        return finalRook;
    }


    //printing moves
    private void printMove()
    {
        if(matchableMove != null)
        Debug.Log("From : (" + matchableMove[0].x +  "," + matchableMove[0].y + ") to : (" + matchableMove[1].x + "," + matchableMove[1].y + ")"); 
    }

    //ui methods
    public void Retry()
    {
        onResetButton();
        matchableMove = null;
        resultPgnIndexer = 0;
        SpawnAllPieces();
        uiPanel.SetActive(false);
        retryButton.SetActive(false);
        nextPuzzleButton.SetActive(false);

    }

    public void NextPuzzle()
    {
        onResetButton();
        matchableMove = null;
        resultPgnIndexer = 0;
        puzzleApiCaller.fetchPuzzle();
        uiPanel.SetActive(false);
        retryButton.SetActive(false);
        nextPuzzleButton.SetActive(false);

    }
    
}
