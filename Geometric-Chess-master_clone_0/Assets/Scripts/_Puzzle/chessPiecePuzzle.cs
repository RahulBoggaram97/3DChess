using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum chessPieceTypePuzzle{

    None = 0,
    Pawn = 1,
    Rook = 2,
    Knight = 3,
    Bishop = 4,
    Queen = 5,
    King = 6

}


public class chessPiecePuzzle : MonoBehaviour
{
    public int team;
    public int currentX;
    public int currentY;
    public chessPieceTypePuzzle type;

    private Vector3 desiredPos;
    private Vector3 desiredScale = Vector3.one;

    bool test = false;

    private void Awake()
    {
        //Debug.Log("chess puzzle script is ready");
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * 10);
        //transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 10);
    }


    public virtual List<Vector2Int> GetAvailMoves(ref chessPiecePuzzle[,]  board, int tilecountx, int tilecounty)
    {
        List<Vector2Int>  r = new List<Vector2Int>();

        r.Add(new Vector2Int(3, 3));
        r.Add(new Vector2Int(4, 3));
        r.Add(new Vector2Int(3, 4));
        r.Add(new Vector2Int(4, 4));


        return r;
    }

    public virtual SpecialMoves GetSpecialMoves(ref chessPiecePuzzle[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availMoves)
    {
        return SpecialMoves.None;
    }
    

    public virtual void setPostion(Vector3 position, bool force = false)
    {
        test = true;
       
        
        desiredPos = position;
       
        if (force)
            transform.position = desiredPos;
        
    }

    public virtual void setScale(Vector3 scale, bool force = false)
    {
        desiredScale = scale;
        if (force)
            transform.position = desiredScale;
    }
}
