using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nightPuzzle : chessPiecePuzzle
{

    public override List<Vector2Int> GetAvailMoves(ref chessPiecePuzzle[,] board, int tilecountx, int tilecounty)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        //top right
        int x = currentX + 1;
        int y = currentY + 2;

        if(x < tilecountx && y < tilecounty)
            if(board[x, y] == null || board[x, y].team != team)
                r.Add(new Vector2Int(x, y));

         x = currentX + 2;
         y = currentY + 1;

        if (x < tilecountx && y < tilecounty)
            if (board[x, y] == null || board[x, y].team != team)
                r.Add(new Vector2Int(x, y));

        //topleft
        x = currentX - 1;
        y = currentY + 2;

        if (x >= 0 && y < tilecounty)
            if (board[x, y] == null || board[x, y].team != team)
                r.Add(new Vector2Int(x, y));

        x = currentX - 2;
        y = currentY + 1;

        if (x >= 0 && y < tilecounty)
            if (board[x, y] == null || board[x, y].team != team)
                r.Add(new Vector2Int(x, y));

        //botton right
        x = currentX + 2;
        y = currentY - 1;

        if (x < tilecountx && y >= 0)
            if (board[x, y] == null || board[x, y].team != team)
                r.Add(new Vector2Int(x, y));

        x = currentX + 1;
        y = currentY - 2;

        if (x < tilecountx && y >= 0)
            if (board[x, y] == null || board[x, y].team != team)
                r.Add(new Vector2Int(x, y));


        //bottom left
        x = currentX - 2;
        y = currentY - 1;

        if (x >= 0 && y >= 0)
            if (board[x, y] == null || board[x, y].team != team)
                r.Add(new Vector2Int(x, y));

        x = currentX - 1;
        y = currentY - 2;

        if (x >= 0 && y >= 0)
            if (board[x, y] == null || board[x, y].team != team)
                r.Add(new Vector2Int(x, y));



        return r;
    }
}
