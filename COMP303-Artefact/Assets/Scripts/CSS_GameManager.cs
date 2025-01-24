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

    // class that stores basic info about the board
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

    // class that stores a piece and a cell it should move to
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

    // stores all data needed to restore a piece
    [System.Serializable]
    public class pieceData
    {
        public pieceData(bool w, bool k, Vector2 p)
        {
            isWhite = w;
            isKing = k;
            pos = p;
        }
        public bool isWhite;
        public bool isKing;
        public Vector2 pos;
    }

    // serializable array
    [System.Serializable]
    public class SArray
    {
        public SArray(pieceData[] i)
        {
            items = i;
        }
        public pieceData[] items;
    }

    #endregion

    #region functions

    //evaluates board
    public boardVal boardEvaluation(CSS_Piece[,] board)
    {
        boardVal val = new boardVal(0, 0, 0);

        // iterates board and takes note of how many of each color and kings there are
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

    public List<CSS_Piece[,]> findAllMoves(bool whiteTurn, CSS_Piece[,] board)
    {
        List<CSS_Piece[,]> moves = new List<CSS_Piece[,]>();

        List<forcedMoves> forced = searchForForcedMoves(board);

        if (forced.Count != 0)
        {
            for (int i = 0; i < forced.Count; i++)
            {
                if(forced[i].piece.isWhite == whiteTurn)
                {
                    //convert forced move into board
                    CSS_Piece[,] temp = copyBoard(board);
                    Vector2 pos = forced[i].piece.FindPlace(board);

                    temp[(int)forced[i].cell.x, (int)forced[i].cell.y] = forced[i].piece;
                    temp[(int)pos.x, (int)pos.y] = null;
                    moves.Add(temp);
                }
            }
        }

        if (moves.Count != 0) return moves;
        else
        {
            //goes through every piece
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (board[x,y] != null && board[x, y].isWhite == whiteTurn)
                    {
                        print("cum");
                        for (int i = 0; i < 8; i++)
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                if (board[x, y].validMove(board, new Vector2(i, j)))
                                {
                                    CSS_Piece[,] temp = copyBoard(board);
                                    Vector2 pos = board[x, y].FindPlace(board);

                                    temp[i, j] = board[x, y];
                                    temp[(int)pos.x, (int)pos.y] = null;
                                    moves.Add(temp);
                                }
                            }
                        }
                    }  
                }
            }

            return moves;
        }
    }

    //finds all moves every piece is forced to make and adds them to a list
    public List<forcedMoves> searchForForcedMoves(CSS_Piece[,] board)
    {
        List<forcedMoves> result = new List<forcedMoves>();

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (board[x, y] != null)
                {
                    for (int i = 0; i < board[x, y].checkForMove(board[x, y], board).Count; i++)
                    {
                        result.Add(new forcedMoves(board[x, y], board[x, y].checkForMove(board[x, y], board)[i]));
                    }
                }
            }
        }

        return result;
    }

    //turns all the pieces into the data structure that gets passed into the JSON file
    public pieceData[] boardToJSONPrep(CSS_Piece[,] board)
    {
        int j = 0;
        int i = 0;

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (board[x, y] != null)
                {
                    j++;
                }
            }
        }

        pieceData[] newBoard = new pieceData[j];

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (board[x, y] != null)
                {
                    newBoard[i] = new pieceData(board[x, y].isWhite, board[x, y].isKing, new Vector2(x,y));
                    i++;
                }
            }
        }

        return newBoard;
    }

    public CSS_Piece[,] copyBoard(CSS_Piece[,] original)
    {
        CSS_Piece[,] copy = new CSS_Piece[8, 8];
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (original[i, j] != null) copy[i,j] = original[i, j];
            }
        }
        return copy;
    }

    #endregion
}
