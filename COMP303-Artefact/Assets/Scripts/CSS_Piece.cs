using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSS_Piece : MonoBehaviour
{
    public bool isWhite;
    public bool isKing;

    private CSS_GameManager gameManager;
    private CSS_Board board;

    private SpriteRenderer spriteRenderer;

    [SerializeField] Sprite Base;
    [SerializeField] Sprite BaseSelect;
    [SerializeField] Sprite King;
    [SerializeField] Sprite KingSelect;


    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<CSS_GameManager>();
        board = GameObject.Find("Board").GetComponent<CSS_Board>();

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

    public void Deselect()
    {
        gameManager.selectedPiece = null;
        spriteRenderer.sprite = Base;
        gameManager.boardCollider.enabled = false;
    }

    private void Select()
    {
        if (gameManager.selectedPiece != null) gameManager.selectedPiece.GetComponent<CSS_Piece>().Deselect();
        spriteRenderer.sprite = BaseSelect;
        gameManager.selectedPiece = gameObject;
        gameManager.boardCollider.enabled = true;
    }

    public bool validMove(CSS_Piece[,] board, Vector2 cell)
    {
        if (blackBase(board, cell) || whiteBase(board, cell))
        {
            return true;
        }
        return false;
    }

    public bool blackBase(CSS_Piece[,] board, Vector2 cell)
    {
        if (isWhite) return false;
        else
        {
            Vector2 pos = FindPlace(board);
            if(pos.x > 7) return false;
            if (cell.y == pos.y - 1 && (cell.x == pos.x + 1 || cell.x == pos.x - 1)) return true;
            return false;


        }
    }

    public bool whiteBase(CSS_Piece[,] board, Vector2 cell)
    {
        if (!isWhite) return false;
        else
        {
            print(cell);
            Vector2 pos = FindPlace(board);
            if (pos.x > 7) return false;
            if (cell.y == pos.y + 1 && (cell.x == pos.x + 1 || cell.x == pos.x - 1))return true;
            return false;
        }
    }

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

        return new Vector2(10,10);
    }
}
