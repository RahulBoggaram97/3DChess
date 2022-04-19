using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pawnPuzzle : chessPiecePuzzle
{
    


    public override List<Vector2Int> GetAvailMoves(ref chessPiecePuzzle[,] board, int tilecountx, int tilecounty)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        int direction = (team == 0) ? 1 : -1;

        // one in front
        if(board[currentX, currentY + direction] == null)
            r.Add(new Vector2Int(currentX, currentY + direction));

        //two in fron
        if (board[currentX, currentY + direction] == null)
        {
            if (team == 0 && currentY == 1 && board[currentX, currentY + (direction * 2)] == null)

                r.Add(new Vector2Int(currentX, currentY + (direction * 2)));
            if (team == 1 && currentY == 6 && board[currentX, currentY + (direction * 2)] == null)

                r.Add(new Vector2Int(currentX, currentY + (direction * 2)));

        }

        //kill move
        if (currentX != tilecountx - 1)
            if (board[currentX + 1, currentY + direction] != null && board[currentX + 1, currentY + direction].team != team)
                r.Add(new Vector2Int(currentX + 1, currentY + direction));
        if (currentX != 0)
            if (board[currentX - 1, currentY + direction] != null && board[currentX - 1, currentY + direction].team != team)
                r.Add(new Vector2Int(currentX - 1, currentY + direction));

        return r;
    }

    public override SpecialMoves GetSpecialMoves(ref chessPiecePuzzle[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availMoves)
    {
        int direction = (team == 0) ? 1 : -1;

        if ((team == 0 && currentY == 6) || (team == 1 && currentY == 1))
            return SpecialMoves.Promotion;

        if(moveList.Count > 0)
        {
            Vector2Int[] lastMove = moveList[moveList.Count - 1];
            if (board[lastMove[1].x, lastMove[1].y].type == chessPieceTypePuzzle.Pawn)//if the last piece was a paw
            {
                if (Mathf.Abs(lastMove[0].y - lastMove[1].y) == 2)// if the last move was +2 for pawn
                {
                    if(board[lastMove[1].x, lastMove[1].y].team != team)//if the move was from the other team
                    {
                        if(lastMove[1].y == currentY)
                        {
                            if(lastMove[1].x == currentX - 1)
                            {
                                availMoves.Add(new Vector2Int(currentX - 1, currentY + direction));
                                    return SpecialMoves.Enpassant;
                            }
                            if (lastMove[1].x == currentX + 1)
                            {
                                availMoves.Add(new Vector2Int(currentX + 1, currentY + direction));
                                return SpecialMoves.Enpassant;
                            }
                        }
                    }
                }
            }
        }


        return SpecialMoves.None;
    }
}
