using System.Collections.Generic;
using UnityEngine;

//OLETS algorithm script
//Code was derived from a previous project I've completed and then altered to fit the current artefact
//The original code was made using the following project as a base https://github.com/GAIGResearch/GVGAI/tree/master/src/tracks/singlePlayer/advanced/olets
//This was heavily based on the code written above
//authored by Student Number: 2105232

public class CSS_OLETS : MonoBehaviour
{
    // vars
    const int depth = 10;
    const float ExplorationThreshold = 1.41f;
    const int ThinkTime = 500;

    private static CSS_GameManager gameManager;

    //gets game manager
    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<CSS_GameManager>();
    }

    //main logic loop
    public static CSS_Piece[,] OLETSMainLoop(CSS_Piece[,] board, bool whiteTurn)
    {
        int runs = 0;
        //makes root node and expands it
        OLETSTNode root = new OLETSTNode(null, board, whiteTurn, 0);
        root.Expand();

        //saves time at start of computation
        float startTime = Time.realtimeSinceStartup * 1000f;

        //while under time
        while ((Time.realtimeSinceStartup * 1000f - startTime) < ThinkTime)
        {
            runs++;
            // pick a node and run random role outs
            OLETSTNode selected = root.SelectNode();
            float value = selected.Rollout();

            //load gathered info into parents
            selected.Backpropagate(value);
        }

        gameManager.OLETS.Add(new Vector2(gameManager.OLETS.Count, runs));

        //returns best child
        OLETSTNode bestChild = root.GetBestChild();

        if (bestChild != null) return bestChild.self;
        else return null;
    }

    // tree node class
    //most of the functions are stored within the class bc the main loop is static
    private class OLETSTNode
    {
        public OLETSTNode parent;
        public List<OLETSTNode> children = new List<OLETSTNode>();
        public CSS_Piece[,] self;
        public bool isWhiteTurn;
        public float wins = 0;
        public int visits = 0;
        public int depth;

        public OLETSTNode(OLETSTNode parent, CSS_Piece[,] self, bool isWhiteTurn, int depth)
        {
            this.parent = parent;
            this.self = self;
            this.isWhiteTurn = isWhiteTurn;
            this.depth = depth;
        }

        //expands a node into its children
        public void Expand()
        {
            List<CSS_Piece[,]> NodeChildren = gameManager.findAllMoves(isWhiteTurn, self);
            foreach (var nextNode in NodeChildren)
            {
                children.Add(new OLETSTNode(this, nextNode, !isWhiteTurn, depth + 1));
            }
        }

        //selects best child and returns it
        public OLETSTNode SelectNode()
        {
            OLETSTNode node = this;

            while (node.children.Count > 0)
            {
                node = node.GetBestChildUCT();
            }

            // Expands node in case not already and returns first child
            if (node.visits > 0 && node.depth < CSS_OLETS.depth)
            {
                node.Expand();
                if (node.children.Count > 0)
                {
                    node = node.children[0];
                }
            }

            return node;
        }

        //uses the uct formula to calculate best child
        //replace with order by descending
        public OLETSTNode GetBestChildUCT()
        {
            OLETSTNode best = null;
            float bestScore = -10000000000;

            foreach (OLETSTNode child in children)
            {
                float uct = child.wins / (child.visits + .0001f) + 1.41f + Mathf.Sqrt(Mathf.Log(visits + 1) / (child.visits + .0001f));
                if (uct > bestScore)
                {
                    bestScore = uct;
                    best = child;
                }
            }
            return best;
        }

        //does a random move for all of the depth
        public float Rollout()
        {
            CSS_Piece[,] tempBoard = gameManager.copyBoard(self);
            bool WT = isWhiteTurn;

            for (int i = 0; i < CSS_OLETS.depth; i++)
            {
                List<CSS_Piece[,]> moves = gameManager.findAllMoves(WT, tempBoard);
                if (moves.Count == 0) break;

                tempBoard = moves[Random.Range(0, moves.Count)];
                WT = !WT;
            }

            //evaluates board
            CSS_GameManager.boardVal val = gameManager.boardEvaluation(tempBoard);

            // depending on if its min or max player returns value
            if (isWhiteTurn) return val.value;
            else return -val.value;
        }

        // feeds info back up the family tree
        public void Backpropagate(float result)
        {
            OLETSTNode node = this;
            while (node != null)
            {
                node.visits++;
                node.wins += result;
                node = node.parent;
            }
        }

        // finds best child by dividing value by the visits
        // replace with order by descending
        public OLETSTNode GetBestChild()
        {
            OLETSTNode best = null;
            float bestVal = -10000000000;

            foreach (var child in children)
            {
                float avg = child.wins / (child.visits + 0.0001f); //cant divide by zero so the .0001 is here
                if (avg > bestVal)
                {
                    bestVal = avg;
                    best = child;
                }
            }

            return best;
        }
    }
}