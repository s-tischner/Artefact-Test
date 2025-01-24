using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSS_MiniMax : MonoBehaviour
{
    private CSS_GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<CSS_GameManager>();
    }

    // Update is called once per frame
    void Update()
    {

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
            float maxEval = float.NegativeInfinity;
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
            float minEval = float.PositiveInfinity;
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

    //private KeyValuePair<float, CSS_Piece[,]> Minmax(CSS_Piece[,] board, int depth, bool whiteTurn, bool maxPlayer)
    //{
    //    if(depth == 0)
    //    {
    //        return new KeyValuePair<float, CSS_Piece[,]>(MaxEval, bestMove);
    //    }
    //    if (maxPlayer)
    //    {
    //        // max score
    //        float MaxEval = -100000;

    //        // best node
    //        CSS_Piece[,] bestMove = null;

    //        // gets all possible moves and stores them in a board

    //        List<CSS_Piece[,]> allMoves = gameManager.findAllMoves(true, board);

    //        // for every move runs a minmax eval
    //       for (int i = 0; i<allMoves.Count; i++)
    //       {

    //            float evaluation = Minmax(allMoves[i], depth - 1, false, false).Key;


    //        // compares board against current max


    //            MaxEval = Mathf.Max(MaxEval, evaluation);

    //            if (MaxEval == evaluation)
    //            {
    //                bestMove = allMoves[i];
    //            }
    //       }

    //        // returns best board

    //        return new KeyValuePair<float, CSS_Piece[,]>(MaxEval, bestMove);
    //    }
    //    else
    //    {
    //        // max score
    //        float MaxEval = 100000;

    //        // best node
    //        CSS_Piece[,] bestMove = null;

    //        // gets all possible moves and stores them in a board

    //        List<CSS_Piece[,]> allMoves = gameManager.findAllMoves(false, board);

    //        // for every move runs a minmax eval
    //        for (int i = 0; i < allMoves.Count; i++)
    //        {

    //            float evaluation = Minmax(allMoves[i], depth - 1, true, true).Key;

    //            MaxEval = Mathf.Min(MaxEval, evaluation);

    //            if (MaxEval == evaluation)
    //            {
    //                bestMove = allMoves[i];
    //            }
    //        }

    //        // returns best board

    //        return new KeyValuePair<float, CSS_Piece[,]>(MaxEval, bestMove);
    //    }
    //}
}
