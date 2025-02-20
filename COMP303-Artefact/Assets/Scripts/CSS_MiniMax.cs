using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//MinMax algorithm script
//Code was derived from a previous project I've completed and then altered to fit the current artefact
//The original code was made by using the following code as a general guideline: https://github.com/Gualor/checkers-minimax/blob/master/scripts/minimax.py
//authored by Student Number: 2105232

public class CSS_MiniMax : MonoBehaviour
{
    private CSS_GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<CSS_GameManager>();
    }

    public KeyValuePair<float, CSS_Piece[,]> Minmax(CSS_Piece[,] board, int depth, bool maxPlayer)
    {
        // Base case: stop recursion at depth 0 or game over
        if (depth == 0 || gameManager.boardEvaluation(board).whiteCount == 0 || gameManager.boardEvaluation(board).blackCount == 0)
        {
            float score = gameManager.boardEvaluation(board).value; // Make sure this evaluates from white's perspective or as needed
            return new KeyValuePair<float, CSS_Piece[,]>(score, board);
        }

        if (maxPlayer)
        {
            float maxEval = -100000000;
            CSS_Piece[,] bestMove = null;

            List<CSS_Piece[,]> allMoves = gameManager.findAllMoves(true, board); // true = white's turn

            foreach (var move in allMoves)
            {
                float eval = Minmax(move, depth - 1, false).Key;

                if (eval > maxEval)
                {
                    maxEval = eval;
                    bestMove = move;
                }
            }

            return new KeyValuePair<float, CSS_Piece[,]>(maxEval, bestMove);
        }
        else
        {
            float minEval = 1000000000;
            CSS_Piece[,] bestMove = null;

            List<CSS_Piece[,]> allMoves = gameManager.findAllMoves(false, board); // false = black's turn

            foreach (var move in allMoves)
            {
                float eval = Minmax(move, depth - 1, true).Key;

                if (eval < minEval)
                {
                    minEval = eval;
                    bestMove = move;
                }
            }

            return new KeyValuePair<float, CSS_Piece[,]>(minEval, bestMove);
        }
    }
}
