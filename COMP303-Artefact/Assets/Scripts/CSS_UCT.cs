using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//MinMax algorithm script
//Code was derived from a previous project I've completed and then altered to fit the current artefact
//The original code was made using the following project as a base https://github.com/bagnalla/mcts_checkers/blob/master/src/mcts.cc
//the following code was used to help improve the initial one https://github.com/joelthelion/uct/blob/master/uct.cpp
//Not much of the original versions are left 
//authored by Student Number: 2105232

public class CSS_UCT : MonoBehaviour
{
    #region vars and start
    //game manager ref
    private CSS_GameManager gameManager;

    //controls exploration vs exploitation
    private const float ExplorationThreshold = 1.41f;

    void Start()
    {
        //gets game manager
        gameManager = GameObject.Find("GameManager").GetComponent<CSS_GameManager>();
    }
    #endregion

    #region main logic loop

    //main logic loop
    public CSS_Piece[,] UCTMainLoop(CSS_Piece[,] board, bool whiteTurn, float maxThinkTime)
    {
        //sets root node
        UCTNode root = new UCTNode(null, gameManager.copyBoard(board), whiteTurn);

        //gets starting time
        float startTime = Time.realtimeSinceStartup;

        //while its under the given computing time
        while (Time.realtimeSinceStartup - startTime < maxThinkTime)
        {
            //sets start node
            UCTNode node = Select(root);

            //expands all children and picks a random one
            if (gameManager.findAllMoves(node.whiteTurn, node.board).Count != 0) Expand(node);
            UCTNode childNode = node.children.Count > 0? node.children[Random.Range(0, node.children.Count)]: node;

            // updates nodes with info
            int result = NodeEval(childNode);
            while (childNode != null)
            {
                childNode.visits++;
                if ((result == 1 && childNode.whiteTurn) || (result == -1 && !childNode.whiteTurn)) childNode.wins++;
                childNode = childNode.parent;
            }
        }

        //gets the best performing child
        UCTNode bestChild = root.children.OrderByDescending(child => child.visits).FirstOrDefault();
        return bestChild != null ? bestChild.board : null;
    }

    #endregion

    #region functions

    //selects a node
    private UCTNode Select(UCTNode node)
    {
        //gets node from children based on balancing function
        while (node.children.Count > 0)
        {
            node = node.children.OrderByDescending(child => UCTBalancing(child)).First();
        }
        return node;
    }

    // decides what nodes get picked based on how many wins they have and how many times theyve been visited
    private float UCTBalancing(UCTNode node)
    {
        // explores nodes with no visits
        if (node.visits == 0) return 10000000000f;

        // equation to determine priority exploration order based on visits, parent visits, and wins
        // equation taken from above linked code
        return (float)node.wins / (node.visits + .0001f) + ExplorationThreshold * Mathf.Sqrt(Mathf.Log(node.parent.visits) / (node.visits + .0001f));
    }

    // gets all children and adds them to the list in the node
    private void Expand(UCTNode node)
    {
        List<CSS_Piece[,]> possibleMoves = gameManager.findAllMoves(node.whiteTurn, node.board);
        foreach (var move in possibleMoves)
        {
            node.children.Add(new UCTNode(node, move, !node.whiteTurn));
        }
    }

    //simulates moves and returns an eval based on wins/losses
    private int NodeEval(UCTNode node)
    {
        CSS_Piece[,] tempBoard = gameManager.copyBoard(node.board);
        bool WT = node.whiteTurn;

        while (true)
        {
            List<CSS_Piece[,]> moves = gameManager.findAllMoves(WT, tempBoard);

            //when no moves possible returns a winner/loser based on the last move made
            if (moves.Count == 0)
            {
                return WT ? -1 : 1;
            }

            //loads in the next random move
            tempBoard = gameManager.copyBoard(moves[Random.Range(0, moves.Count)]);
            //changes color turn
            WT = !WT;
        }
    }
    #endregion

    #region classes

    //node class
    private class UCTNode
    {
        public UCTNode(UCTNode p, CSS_Piece[,] b, bool wT)
        {
            parent = p;
            board = b;
            whiteTurn = wT;
        }

        public UCTNode parent;
        public List<UCTNode> children = new List<UCTNode>();
        public CSS_Piece[,] board;
        public bool whiteTurn;
        public int visits = 0;
        public int wins = 0;
    }
    #endregion

}