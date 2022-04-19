using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RookPuzzle : chessPiecePuzzle
{
    public override List<Vector2Int> GetAvailMoves(ref chessPiecePuzzle[,] board, int tilecountx, int tilecounty)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        //down
        for(int i = currentY -1 ; i >= 0; i--)
        {
            if(board[currentX, i] == null)
                r.Add(new Vector2Int(currentX, i));

            if(board[currentX, i] != null)
            {
                if (board[currentX, i].team != team)
                    r.Add(new Vector2Int(currentX, i));
                break;
            }
        }

        //up
        for (int i = currentY + 1; i < tilecounty; i++)
        {
            if (board[currentX, i] == null)
                r.Add(new Vector2Int(currentX, i));

            if (board[currentX, i] != null)
            {
                if (board[currentX, i].team != team)
                    r.Add(new Vector2Int(currentX, i));
                break;
            }
        }

        //left
        for (int i = currentX - 1; i >= 0; i--)
        {
            if (board[i, currentY] == null)
                r.Add(new Vector2Int(i, currentY));

            if (board[i, currentY] != null)
            {
                if (board[i, currentY].team != team)
                    r.Add(new Vector2Int(i, currentY));
                break;
            }
        }

        //right
        
        for (int i = currentX + 1; i < tilecountx; i++)
        {
            if (board[i, currentY] == null)
                r.Add(new Vector2Int(i, currentY));

            if (board[i, currentY] != null)
            {
                if (board[i, currentY].team != team)
                    r.Add(new Vector2Int(i, currentY));
                break;
            }
        }

        return r;
    }
}
