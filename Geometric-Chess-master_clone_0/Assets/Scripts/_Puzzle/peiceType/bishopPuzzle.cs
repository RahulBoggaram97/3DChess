using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bishopPuzzle : chessPiecePuzzle
{
    public override List<Vector2Int> GetAvailMoves(ref chessPiecePuzzle[,] board, int tilecountx, int tilecounty)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        //top right
        for (int x = currentX + 1, y = currentY + 1; x < tilecountx && y < tilecounty; x++, y++)
        {
            if (board[x, y] == null)
                r.Add(new Vector2Int(x, y));
            else 
            {
                if (board[x, y].team != team)
                    r.Add(new Vector2Int(x, y));

                break;
            }
        }


        //top left
        for (int x = currentX - 1, y = currentY + 1; x >=0 && y < tilecounty; x--, y++)
        {
            if (board[x, y] == null)
                r.Add(new Vector2Int(x, y));
            else
            {
                if (board[x, y].team != team)
                    r.Add(new Vector2Int(x, y));

                break;
            }
        }

        //bottom right
        for (int x = currentX + 1, y = currentY - 1; x < tilecountx && y >= 0; x++, y--)
        {
            if (board[x, y] == null)
                r.Add(new Vector2Int(x, y));
            else
            {
                if (board[x, y].team != team)
                    r.Add(new Vector2Int(x, y));

                break;
            }
        }

        //bottom left
        for (int x = currentX - 1, y = currentY - 1; x >= 0 && y >= 0; x--, y--)
        {
            if (board[x, y] == null)
                r.Add(new Vector2Int(x, y));
            else
            {
                if (board[x, y].team != team)
                    r.Add(new Vector2Int(x, y));

                break;
            }
        }

        return r;

    }
}
