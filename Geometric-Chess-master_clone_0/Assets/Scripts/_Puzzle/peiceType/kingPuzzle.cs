using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kingPuzzle : chessPiecePuzzle
{
    public override List<Vector2Int> GetAvailMoves(ref chessPiecePuzzle[,] board, int tilecountx, int tilecounty)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        //right
        if(currentX + 1 < tilecountx)
        {
            //right
            if(board[currentX + 1, currentY] == null)
                r.Add(new Vector2Int(currentX + 1 , currentY));
            else if (board[currentX + 1, currentY].team != team)
                r.Add(new Vector2Int(currentX + 1, currentY));

            //top right
            if(currentY+ 1 < tilecounty)
                if (board[currentX + 1, currentY + 1] == null)
                    r.Add(new Vector2Int(currentX + 1, currentY +1));
                else if (board[currentX + 1, currentY + 1].team != team)
                    r.Add(new Vector2Int(currentX + 1, currentY +1));

            //bottom right
            if (currentY - 1 >= 0)
                if (board[currentX + 1, currentY - 1] == null)
                    r.Add(new Vector2Int(currentX + 1, currentY - 1));
                else if (board[currentX + 1, currentY - 1].team != team)
                    r.Add(new Vector2Int(currentX + 1, currentY - 1));
        }

        //left

        if (currentX - 1 >= 0)
        {
            //right
            if (board[currentX - 1, currentY] == null)
                r.Add(new Vector2Int(currentX - 1, currentY));
            else if (board[currentX - 1, currentY].team != team)
                r.Add(new Vector2Int(currentX - 1, currentY));

            //top right
            if (currentY + 1 < tilecounty)
                if (board[currentX - 1, currentY + 1] == null)
                    r.Add(new Vector2Int(currentX - 1, currentY + 1));
                else if (board[currentX - 1, currentY + 1].team != team)
                    r.Add(new Vector2Int(currentX - 1, currentY + 1));

            //bottom right
            if (currentY - 1 >= 0)
                if (board[currentX - 1, currentY - 1] == null)
                    r.Add(new Vector2Int(currentX - 1, currentY - 1));
                else if (board[currentX - 1, currentY - 1].team != team)
                    r.Add(new Vector2Int(currentX - 1, currentY - 1));
        }

        //up

        if (currentY + 1 < tilecounty)
         if(board[currentX, currentY + 1] == null || board[currentX, currentY + 1].team != team)
                r.Add(new Vector2Int(currentX, currentY+ 1));


        //down
        if (currentY - 1 >=0)
            if (board[currentX, currentY - 1] == null || board[currentX, currentY - 1].team != team)
                r.Add(new Vector2Int(currentX, currentY - 1));


        return r;
    }

    public override SpecialMoves GetSpecialMoves(ref chessPiecePuzzle[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availMoves)
    {
        SpecialMoves r = SpecialMoves.None;

        var kingMove = moveList.Find(m => m[0].x == 4 && m[0].y == ((team == 0) ? 0 : 7));
        var leftRook = moveList.Find(m => m[0].x == 0 && m[0].y == ((team == 0) ? 0 : 7));
        var rightRook = moveList.Find(m => m[0].x == 7 && m[0].y == ((team == 0) ? 0 : 7));

        if(kingMove == null && currentX == 4)
        {
            //white team only
            if(team == 0)
            {
                //left rook
                if(leftRook == null)
                 if(board[0, 0].type == chessPieceTypePuzzle.Rook)
                        if(board[0, 0].team == 0)
                            if(board[3, 0]== null)
                                if (board[2, 0] == null)
                                    if (board[1, 0] == null)
                                    {
                                        availMoves.Add(new Vector2Int(2, 0));
                                        r = SpecialMoves.Castling;
                                    }
                //right rook
                if (rightRook == null)
                    if (board[7, 0].type == chessPieceTypePuzzle.Rook)
                        if (board[7, 0].team == 0)
                            if (board[5, 0] == null)
                                if (board[6, 0] == null)
                                    {
                                        availMoves.Add(new Vector2Int(6, 0));
                                        r = SpecialMoves.Castling;
                                    }
            }
            else
            {
                //black team
                //left rook
                if (leftRook == null)
                    if (board[0, 7].type == chessPieceTypePuzzle.Rook)
                        if (board[0, 7].team == 1)
                            if (board[3, 7] == null)
                                if (board[2, 7] == null)
                                    if (board[1, 7] == null)
                                    {
                                        availMoves.Add(new Vector2Int(2, 7));
                                        r = SpecialMoves.Castling;
                                    }

                //right rook
                if (rightRook == null)
                    if (board[7, 7].type == chessPieceTypePuzzle.Rook)
                        if (board[7, 7].team == 1)
                            if (board[5, 7] == null)
                                if (board[6, 7] == null)
                                {
                                    availMoves.Add(new Vector2Int(6, 7));
                                    r = SpecialMoves.Castling;
                                }
            }
        }


        return r;
    }
}
