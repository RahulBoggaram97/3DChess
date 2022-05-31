using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.impactional.chess
{
	public class gridPuzzle : MonoBehaviour
	{

		[SerializeField]
		private GameObject tilePrefab;
		[SerializeField]
		private GameObject tilePrefab2;


		[Header("matrials")]
		[SerializeField] Material p1Mat;
		[SerializeField] Material p2Mat;

		[SerializeField]
		private int rows;
		[SerializeField]
		private int cols;


		private Node[,] grid;

		private Vector3 size;
		private Vector3 tileSize;


		public puzzleApi puzzleApi;

		public string fenPostion;



		[Header("chessPiecePrefabs")]
		public GameObject pawnPrefab;
		public GameObject rookPrefab;
		public GameObject nightPrefab;
		public GameObject bishopPrefab;
		public GameObject queenPrefab;
		public GameObject kingPrefab;

		public Node GetNodeAt(int row, int col)
		{
			if (row < 0 || row >= rows || col < 0 || col >= cols) return null;
			return grid[row, col];
		}

		void Awake()
		{
			grid = new Node[rows, cols];
			tileSize = tilePrefab.GetComponent<Renderer>().bounds.size;
			size = new Vector3(tileSize.x * cols, tileSize.y, tileSize.z * rows);
			StartCoroutine(CreateGrid());
			Debug.Log("awake got called");
		}

		IEnumerator CreateGrid()
		{
			Debug.Log("creating grid");
			Vector3 bottomLeft = new Vector3(
					transform.position.x - size.x / 2 + tileSize.x / 2,
					transform.position.y,
					transform.position.z - size.z / 2 + tileSize.z / 2);
			Vector3 startPosition = bottomLeft;

			GameObject tile = tilePrefab;

			for (int row = 0; row < rows; row++)
			{
				for (int col = 0; col < cols; col++)
				{
					startPosition.z = bottomLeft.z + tileSize.z * row;
					startPosition.x = bottomLeft.x + tileSize.x * col;
					GameObject go = Instantiate(tile, startPosition, tile.transform.rotation) as GameObject;
					Node dn = go.AddComponent<Node>();
					dn.row = row;
					dn.col = col;
					dn.rowChess = Converter.ToChessRow(row);
					dn.colChess = Converter.ToChessCol(col);
					grid[row, col] = dn;
					go.transform.parent = transform;
					go.transform.localScale = Vector3.zero;

					dn.ScaleIn(Random.Range(0f, 1f), Random.Range(1f, 2f), tile.transform.localScale);
					tile = SwapTilePrefab(tile);
				}
				tile = SwapTilePrefab(tile);
			}

			yield return new WaitForSeconds(3);
			callPuzzleApi();



		}


		void callPuzzleApi()
		{
			puzzleApi.fetchPuzzle();
		}

		GameObject SwapTilePrefab(GameObject go)
		{
			if (tilePrefab == go) return tilePrefab2;

			return tilePrefab;
		}

		public void SpawnPiece(GridCoords coords, GameObject piece, float yRotation, PlayerType playerType)
		{
			Node pieceNode = GetNodeAt(coords.row, coords.col);
			Debug.Log("piece is to be spawn at " + coords.row + coords.col);

			Vector3 pRotation = piece.transform.rotation.eulerAngles;
			Debug.Log(piece.name + " has rotation of :" + piece.transform.rotation.eulerAngles);
			Quaternion newPRotation = Quaternion.Euler(pRotation.x, yRotation, pRotation.z);

			//Debug.Log(pieceNode.name);
			//Debug.Log(newPRotation);
			//Debug.Log(piece.name);

			GameObject pieceObject = Instantiate(piece, pieceNode.transform.position + Vector3.up * 1.2f, newPRotation) as GameObject;
			/*pieceObject.transform.localScale = Vector3.zero;*/ //for scaling in start from zero

		
			//assign mat and player type
			Material mat = null;

			GCPlayer player = null;
			switch (playerType)
            {
                case PlayerType.P1:
					mat = p1Mat;
                   
                    break;
                case PlayerType.P2:
					mat = p2Mat;
                    break;
            }
            pieceObject.GetComponent<Renderer>().material = mat;

			
			

			


			






		}



		public void completePuzzle()
		{
			int lastspawnIndex = 0;

			
			int r = 7;
			int c = 0;
			
				for ( int i = 0; fenPostion[i] != ' '; i++)
				{
				
					switch (fenPostion[i])
					{
						

						case 'p':
							spawnPawn(new GridCoords(r, c), PlayerType.P2);
							c++;
							break;
						case 'P':
							spawnPawn(new GridCoords(r, c), PlayerType.P1);
							c++;
							break;
						case 'n':
							spawnNight(new GridCoords(r, c), PlayerType.P2);
							c++;
							break;
						case 'N':
							spawnNight(new GridCoords(r, c), PlayerType.P1);
							c++;
							break;
						case 'r':
							spawnRook(new GridCoords(r, c), PlayerType.P2);
							c++;
							break;
						case 'R':
							spawnRook(new GridCoords(r, c), PlayerType.P1);
							c++;
							break;
						case 'b':
							spawnBishop(new GridCoords(r, c), PlayerType.P2);
							c++;
							break;
						case 'B':
							spawnBishop(new GridCoords(r, c), PlayerType.P1);
							c++;
							break;
						case 'q':
							spawnQueen(new GridCoords(r, c), PlayerType.P2);
							c++;
							break;
						case 'Q':
							spawnQueen(new GridCoords(r, c), PlayerType.P1);
							c++;
							break;
						case 'k':
							spawnKing(new GridCoords(r, c), PlayerType.P2);
							c++;
							break;
						case 'K':
							spawnKing(new GridCoords(r, c), PlayerType.P1);
							c++;
							break;
						case '/':
							r--;
							c = 0;
							break;
						case ' ':
							
							break;
						default:

							Debug.Log(" value of c before :" + c);

							Debug.Log("value of the char :" + fenPostion[i]);
							if (char.IsDigit(fenPostion[i]))
								c = c + int.Parse(fenPostion[i].ToString());
							Debug.Log(c);
							break;

					}
				lastspawnIndex = i;

				}

			lastspawnIndex = lastspawnIndex + 2;

			Debug.Log(fenPostion[lastspawnIndex]);

			setTurn(fenPostion[lastspawnIndex]);

			int castleindex = lastspawnIndex + 2;

			Debug.Log(fenPostion.Substring(castleindex));
		}

		//SPAWN PIECE HELPER METHODS
		void spawnPawn(GridCoords cords, PlayerType type)
		{
			SpawnPiece(cords, pawnPrefab, 0, type);
		}

		void spawnNight(GridCoords cords, PlayerType type)
		{
			SpawnPiece(cords, nightPrefab, 0, type);
		}

		void spawnRook(GridCoords cords, PlayerType type)
		{
			SpawnPiece(cords, rookPrefab, 0, type);
		}

		void spawnBishop(GridCoords cords, PlayerType type)
		{
			SpawnPiece(cords, bishopPrefab, 0, type);
		}

		void spawnQueen(GridCoords cords, PlayerType type)
		{
			SpawnPiece(cords, queenPrefab, 0, type);
		}

		void spawnKing(GridCoords cords, PlayerType type)
		{
			SpawnPiece(cords, kingPrefab, 0, type);
		}

		//TURNDECIDER
		void setTurn(char playerType)
        {
            switch (playerType)
            {
				case 'w':
					break;
				case 'b':
					break;
            }
        }



	}
}
