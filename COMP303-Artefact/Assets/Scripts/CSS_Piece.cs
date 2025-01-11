using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

// Piece script
// handles all internal piece logic
// authored by Stefanie Tischner

public class CSS_Piece : MonoBehaviour
{
    #region vars
    // keeps track of piece ID
    public bool isWhite;
    public bool isKing;

    // refs
    private CSS_GameManager gameManager;
    private CSS_Board board;

    private SpriteRenderer spriteRenderer;

    // houses the sprites
    [SerializeField] Sprite Base;
    [SerializeField] Sprite BaseSelect;
    [SerializeField] Sprite King;
    [SerializeField] Sprite KingSelect;

    #endregion

    #region start/update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<CSS_GameManager>();
        board = GameObject.Find("Board").GetComponent<CSS_Board>();

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // select/deselcts on click
    private void OnMouseDown()
    {
        if(gameManager.selectedPiece == gameObject)
        {
            Deselect();
        }
        else
        {
            Select();
        }
    }
    #endregion

    #region select logic
    // deselect
    // turns piece into idle state
    public void Deselect()
    {
        gameManager.selectedPiece = null;
        if(!isKing)spriteRenderer.sprite = Base;
        else spriteRenderer.sprite = King;
        gameManager.boardCollider.enabled = false;
    }

    //select
    // turns piece into active state
    private void Select()
    {
        if (gameManager.selectedPiece != null) gameManager.selectedPiece.GetComponent<CSS_Piece>().Deselect();
        if (!isKing)spriteRenderer.sprite = BaseSelect;
        else spriteRenderer.sprite = KingSelect;

        gameManager.selectedPiece = gameObject;
        gameManager.boardCollider.enabled = true;
    }
    #endregion

    #region valid move checks
    // valid move bool
    public bool validMove(CSS_Piece[,] board, Vector2 cell)
    {
        if (blackBase(board, cell) || whiteBase(board, cell))
        {
            return true;
        }
        return false;
    }

    // move logic for black pieces
    public bool blackBase(CSS_Piece[,] board, Vector2 cell)
    {
        if (isWhite && !isKing)return false;
        if (gameManager.whiteTurn && !isWhite) return false;
        else
        {
            Vector2 pos = FindPlace(board);
            if(pos.x > 7) return false;
            if (board[(int)cell.x, (int)cell.y] != null) return false;
            if (cell.y == pos.y - 1 && (cell.x == pos.x + 1 || cell.x == pos.x - 1))
            {
                gameManager.whiteTurn = !gameManager.whiteTurn;
                checkForKing(cell);
                return true;
            }

            //right capture check
            if (cell.y == pos.y - 2 && cell.x == pos.x + 2 && board[(int)pos.x + 1, (int)pos.y - 1] != null)
            {
                // if piece's color doesnt match own
                if (board[(int)pos.x + 1, (int)pos.y - 1].GetComponent<CSS_Piece>().isWhite != isWhite)
                {
                    board[(int)pos.x + 1, (int)pos.y - 1].gameObject.SetActive(false); // temp
                    board[(int)pos.x + 1, (int)pos.y - 1] = null;
                    gameManager.whiteTurn = !gameManager.whiteTurn;
                    checkForKing(cell);
                    return true;
                }
            }
            // left capture check
            if (cell.y == pos.y - 2 && cell.x == pos.x - 2 && board[(int)pos.x - 1, (int)pos.y - 1] != null)
            {
                // if piece's color doesnt match own
                if (board[(int)pos.x - 1, (int)pos.y - 1].GetComponent<CSS_Piece>().isWhite != isWhite)
                {
                    board[(int)pos.x - 1, (int)pos.y - 1].gameObject.SetActive(false); // temp
                    board[(int)pos.x - 1, (int)pos.y - 1] = null;
                    gameManager.whiteTurn = !gameManager.whiteTurn;
                    checkForKing(cell);
                    return true;
                }
            }

            return false;


        }
    }

    //move logic for white pieces
    public bool whiteBase(CSS_Piece[,] board, Vector2 cell)
    {
        //making sure its a valid move based on piece type
        if (!isWhite && !isKing) return false;
        if(!gameManager.whiteTurn && isWhite) return false;

        else
        {
            // making sure the move is legal
            Vector2 pos = FindPlace(board);
            if (pos.x > 7) return false;
            if (board[(int)cell.x, (int)cell.y] != null) return false;

            //simple move check
            if (cell.y == pos.y + 1 && (cell.x == pos.x + 1 || cell.x == pos.x - 1))
            {
                gameManager.whiteTurn = !gameManager.whiteTurn;
                checkForKing(cell);
                return true;
            }

            //right capture check
            if (cell.y == pos.y + 2 && cell.x == pos.x + 2 && board[(int)pos.x+1, (int)pos.y+1] != null)
            {
                // if piece's color doesnt match own
                if(board[(int)pos.x + 1, (int)pos.y + 1].GetComponent<CSS_Piece>().isWhite != isWhite)
                {
                    board[(int)pos.x + 1, (int)pos.y + 1].gameObject.SetActive(false); // temp
                    board[(int)pos.x + 1, (int)pos.y + 1] = null;
                    gameManager.whiteTurn = !gameManager.whiteTurn;
                    checkForKing(cell);
                    return true;
                }
            }
            // left capture check
            if (cell.y == pos.y + 2 && cell.x == pos.x - 2 && board[(int)pos.x - 1, (int)pos.y + 1] != null)
            {
                // if piece's color doesnt match own
                if (board[(int)pos.x - 1, (int)pos.y + 1].GetComponent<CSS_Piece>().isWhite != isWhite)
                {
                    board[(int)pos.x - 1, (int)pos.y + 1].gameObject.SetActive(false); // temp
                    board[(int)pos.x - 1, (int)pos.y + 1] = null;
                    gameManager.whiteTurn = !gameManager.whiteTurn;
                    checkForKing(cell);
                    return true;
                }
            }
            return false;
        }
    }
    #endregion

    #region MISC functions
    //function to find a piece within the array
    public Vector2 FindPlace(CSS_Piece[,] board)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if(board[i, j] == this)
                {
                    return new Vector2(i, j);
                }
                
            }
        }

        // if its not valid it returns a vector thats out of bounds of the board
        return new Vector2(10,10);
    }

    private void checkForKing(Vector2 pos)
    {
        if(pos.y == 0 && !isKing && !isWhite)
        {
            spriteRenderer.sprite = King;
            isKing = true;
        }

        if (pos.y == 7 && !isKing && isWhite)
        {
            spriteRenderer.sprite = King;
            isKing = true;
        }
    }

    public List<Vector2> checkForMove(CSS_Piece piece, CSS_Piece[,] board)
    {
        List<Vector2> moves = new List<Vector2>();

        Vector2 place = piece.FindPlace(board);

        // top check
        if(piece.isWhite || piece.isKing)
        {
            if (place.x + 2 < 8 && place.y + 2 < 8 && board[(int)place.x + 1, (int)place.y + 1] != null)
            {
                if (board[(int)place.x + 1, (int)place.y + 1].isWhite != piece.isWhite && board[(int)place.x + 2, (int)place.y + 2] == null)
                {
                    moves.Add(new Vector2(place.x + 2, place.y + 2));
                }
            }
            if (place.x - 2 > -1 && place.y + 2 < 8 && board[(int)place.x - 1, (int)place.y + 1] != null)
            {
                if (board[(int)place.x - 1, (int)place.y + 1].isWhite != piece.isWhite && board[(int)place.x - 2, (int)place.y + 2] == null)
                {
                    moves.Add(new Vector2(place.x - 2, place.y + 2));
                }
            }
        }

        // bottom check
        if (!piece.isWhite || piece.isKing)
        {
            if (place.x + 2 < 8 && place.y - 2 > -1 && board[(int)place.x + 1, (int)place.y - 1] != null)
            {
                if (board[(int)place.x + 1, (int)place.y - 1].isWhite != piece.isWhite && board[(int)place.x + 2, (int)place.y - 2] == null)
                {
                    moves.Add(new Vector2(place.x + 2, place.y - 2));
                }
            }
            if (place.x - 2 > -1 && place.y - 2 > -1 && board[(int)place.x - 1, (int)place.y - 1] != null)
            {
                if (board[(int)place.x - 1, (int)place.y - 1].isWhite != piece.isWhite && board[(int)place.x - 2, (int)place.y - 2] == null)
                {
                    moves.Add(new Vector2(place.x - 2, place.y - 2));
                }
            }
        }
        return moves;
    }
    #endregion
}
