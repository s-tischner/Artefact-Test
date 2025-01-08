using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Game Manager script
// This script is mainly used to house communication between scripts and hold certain global variables
// authored by Stefanie Tischner

public class CSS_GameManager : MonoBehaviour
{
    //selectied piece holder
    public GameObject selectedPiece;

    //keeps track of turn
    public bool whiteTurn = true;

    public Collider2D boardCollider;


    void Start()
    {
        //turns off board collider to start game
        boardCollider = GameObject.Find("Board").GetComponent<Collider2D>();
        boardCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

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
}
