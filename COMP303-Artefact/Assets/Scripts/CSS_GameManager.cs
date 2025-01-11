using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Game Manager script
// This script is mainly used to house communication between scripts and hold certain global variables
// authored by Stefanie Tischner

public class CSS_GameManager : MonoBehaviour
{
    #region variables
    //selectied piece holder
    public GameObject selectedPiece;

    //keeps track of turn
    public bool whiteTurn = true;

    public Collider2D boardCollider;
    #endregion

    #region start/update
    void Start()
    {
        //turns off board collider to start game
        boardCollider = GameObject.Find("Board").GetComponent<Collider2D>();
        boardCollider.enabled = false;
    }
    #endregion

    #region classes

    public class boardVal
    {
        public boardVal(int v, int w, int b)
        {
            value = v;
            whiteCount = w;
            blackCount = b;
        }

        public int value;
        public int whiteCount;
        public int blackCount;
    }

    public class forcedMoves
    {
        public forcedMoves(CSS_Piece p, Vector2 v)
        {
            piece = p;
            cell = v;
        }

        public CSS_Piece piece;
        public Vector2 cell;
    }

    #endregion

    #region functions
    public boardVal boardEvaluation(CSS_Piece[,] board)
    {
        boardVal val = new boardVal(0, 0, 0);
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (board[y,x] != null)
                {
                    if (board[y, x].isWhite)
                    {
                        if (board[y, x].isKing) val.value += 20;
                        val.value += 10;
                        val.whiteCount++;
                    }

                    else
                    {
                        if (board[y, x].isKing) val.value -= 20;
                        val.value -= 10;
                        val.blackCount++;
                    }
                }
            }
        }

        return val;
    }

    public List<forcedMoves> searchForForcedMoves(CSS_Piece[,] board)
    {
        List<forcedMoves> result = new List<forcedMoves>();
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (board[x, y] != null && board[x, y].checkForMove(board[x, y], board).Count != 0)
                {
                    for (int i = 0; i < board[x, y].checkForMove(board[x, y], board).Count; i++)
                    {
                        new forcedMoves(board[x, y], board[x, y].checkForMove(board[x, y], board)[i]);
                    }
                }
            }
        }

        return result;
    }

    #endregion
}
