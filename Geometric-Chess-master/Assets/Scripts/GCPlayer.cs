using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerType {
		P1, P2
}

public class GCPlayer : IClicker, IInputReceiver {

	private PlayerType type;

	private List<Piece> pieces;
	private List<Piece> eatenPieces;

	private Piece piece;
	private Piece checkedBy; //Experimental

	//Experimental
	public bool IsChecked {
		get {return checkedBy != null;}
	}

	public List<Piece> Pieces {
		get {return pieces;}
	}

	public List<Piece> EatenPieces {
		get {return eatenPieces;}
	}

	//Experimental
	public Piece CheckedBy {
		get {return checkedBy;}
		set {
			checkedBy = value;
		}
	}

	public Piece HoldingPiece {
		get {return piece;}
	}

	public bool IsReady {
		get {
			for (int i = 0; i < pieces.Count; i++) {
				if (!pieces[i].IsReady) return false;
			}

			return true;
		}
	}

	public PlayerType Type {
		get {return type;}
	}

	public GCPlayer(PlayerType type) {
		this.type = type;
		pieces = new List<Piece>();
		eatenPieces = new List<Piece>();
	}

	public void EnableInput() {
		InputManager.InputEvent += OnInputEvent;
	}

	public void DisableInput() {
		InputManager.InputEvent -= OnInputEvent;
	}

	void OnDisable() {
		DisableInput();
	}

	public void OnInputEvent(InputActionType action) {
		switch (action) {
			case InputActionType.GRAB_PIECE:
				Node gNode = Finder.RayHitFromScreen<Node>(Input.mousePosition);
				if (gNode == null) break;
				piece = gNode.Piece;
				if (piece == null) break;
				if (!piece.IsReady) break;
				if (Click(gNode) && piece && Has(piece) && Click(piece)) {
					piece.Pickup();
					piece.Compute();
					piece.HighlightPossibleMoves();
					piece.HighlightPossibleEats();
					GameManager.Instance.GameState.Grab();
				} 

				//check clickable for tile and piece then pass Player
				//check if player has piece - PIECE 
				//check if player has piece if not empty - NODE 
				break;
			case InputActionType.CANCEL_PIECE:
					if (piece != null) {
						//if (!piece.IsReady) break;
						piece.Drop();
						piece = null;
						GameManager.Instance.GameState.Cancel();
					}
				break;
			case InputActionType.PLACE_PIECE:
				Node tNode = Finder.RayHitFromScreen<Node>(Input.mousePosition);
				if (tNode == null) break;
				Piece tPiece = tNode.Piece;
				if (tPiece == null) {
					if (piece.IsPossibleMove(tNode)) {
						if (Rules.IsCheckMove(this,piece,tNode, true)) {
							Debug.Log("Move checked"); // do nothing
						} else {
							piece.MoveToXZ(tNode, Drop);
							GameManager.Instance.GameState.Place();
						}
					}
				} else {
					if (piece.IsPossibleEat(tNode)) {
						if (Rules.IsCheckEat(this,piece,tNode, true)) {
							Debug.Log("Eat checked"); // do nothing
						} else {
							GCPlayer oppPlayer = GameManager.Instance.Opponent(this);
							oppPlayer.RemovePiece(tPiece);
							AddEatenPieces(tPiece);
							setEatenPiecesAside(tPiece.gameObject);
							GameObject.Destroy(tPiece.gameObject);
							
							piece.MoveToXZ(tNode, Drop);
							GameManager.Instance.GameState.Place();
						}
					}
				}
				break;
		}
	}

	public void ClearPiecesPossibles() {
		for (int i = 0; i < pieces.Count; i++) {
			pieces[i].ClearPossibleEats();
			pieces[i].ClearPossibleMoves();
		}
	}

	public void ClearCheck() {
		if (checkedBy == null) return;
		checkedBy = null;
		//checkedBy.ClearCheck(this);
	}

	//the methods inside must be in order
	private void Drop() {
		piece.Drop();
		piece.Compute();
		GameManager.Instance.GameState.Release();
		piece = null;
	}

	public bool Has(Piece piece) {
		return pieces.Contains(piece);
	}

	public bool Click(IClickable clickable) {
		if (clickable == null) return false;
		return clickable.Inform<GCPlayer>(this); 
	}

	public void AddPieces(params Piece[] pieces) {
		for (int i = 0; i < pieces.Length; i++) {
			this.pieces.Add(pieces[i]);
		}
	}

	public void AddEatenPieces(params Piece[] pieces) {
		for (int i = 0; i < pieces.Length; i++) {
			this.eatenPieces.Add(pieces[i]);
		}
	}

	public bool RemovePiece(Piece piece) {
		return pieces.Remove(piece);
	}

	public void ComputePieces() {
		for (int i = 0; i < pieces.Count; i++) {
			pieces[i].Compute();
		}
	}

	Vector3 whiteEatPosIntial = new Vector3(4.5f, 0,  1.5f);
	float whiteRowNo;
	float whiteColNo;

	Vector3 blackEatPosIntial = new Vector3(-4.5f, 0, -1.5f);
	float blackRowNo;
	float blackColNo;

	void setEatenPiecesAside(GameObject piece)
	{
	     if(GameManager.Instance.CurrentPlayer.type == PlayerType.P1)
        {
			whiteRowNo = eatenPieces.Count % 4;
			whiteColNo = eatenPieces.Count / 4;

			Vector3 newPosToSpawn = new Vector3(whiteEatPosIntial.x + whiteColNo, 0, whiteEatPosIntial.z - whiteRowNo);
			Vector3 pRotation = piece.transform.rotation.eulerAngles;
			Quaternion newProt = Quaternion.Euler(pRotation.x, pRotation.y, pRotation.z);

			GameObject.Instantiate(piece, newPosToSpawn, newProt);

        }
		if (GameManager.Instance.CurrentPlayer.type == PlayerType.P2)
		{
			whiteRowNo = eatenPieces.Count % 4;
			whiteColNo = eatenPieces.Count / 4;

			Vector3 newPosToSpawn = new Vector3(blackEatPosIntial.x + blackColNo, 0, blackEatPosIntial.z - blackRowNo);
			Vector3 pRotation = piece.transform.rotation.eulerAngles;
			Quaternion newProt = Quaternion.Euler(pRotation.x, pRotation.y, pRotation.z);

			GameObject.Instantiate(piece, newPosToSpawn, newProt);

		}
	}
}
