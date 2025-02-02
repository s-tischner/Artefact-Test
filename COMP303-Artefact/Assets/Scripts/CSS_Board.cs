using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using static CSS_GameManager;
using static UnityEditor.PlayerSettings;

// Board script
// generates board and pieces
// houses update board function that refreshes the visual representation of the game
// authored by Student Number: 2105232 with code referenced from https://www.youtube.com/watch?v=93o_Ad5C5Ds&t=679s

[System.Serializable]
public class CSS_Board : MonoBehaviour
{
    #region vars
    //houses the pieces
    public CSS_Piece[,] Pieces = new CSS_Piece[8,8];
    public CSS_MiniMax minmax;
    public CSS_UCT UCT;

    public List<forcedMoves> forcedMoves = new List<forcedMoves>();

    //refs to prefabs
    [SerializeField] GameObject White;
    [SerializeField] GameObject Black;

    CSS_GameManager gameManager;
    #endregion

    #region start/update
    void Start()
    {
        //finds game manager
        gameManager = GameObject.Find("GameManager").GetComponent<CSS_GameManager>();
        minmax = GetComponent<CSS_MiniMax>();
        UCT = GetComponent<CSS_UCT>();

        //initializes board
        GenerateBoard();
        UpdateBoard();
    }

    // manual play script
    private void OnMouseDown()
    {
        if(gameManager.selectedPiece != null)
        {

            // code to convert world space into game space
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            Vector3 cellPos = Vector3.zero;

            //averages click into a cell on board
            cellPos.x = worldPosition.x - (worldPosition.x % .5f) + .25f;
            cellPos.y = worldPosition.y - (worldPosition.y % .5f) + .25f;

            //turns cell pos into the board array index
            var x = (cellPos.x / .5)-.5f;
            var y = (cellPos.y / .5)-.5f;

            // if the move is valid
            if (gameManager.selectedPiece.GetComponent<CSS_Piece>().validMove(Pieces,new Vector2(((float)x), ((float)y))))
            {
                // finds piece in the array
                Vector2 ogPlace = gameManager.selectedPiece.GetComponent<CSS_Piece>().FindPlace(Pieces);

                int x1 = (int)ogPlace.x;
                int y1 = (int)ogPlace.y;

                int x2 = (int)x;
                int y2 = (int)y;

                //checks if theres any forced moves
                if (forcedMoves.Count != 0)
                {
                    // goes through list of moves and checks if the one made is in the list
                    for (int i = 0; i < forcedMoves.Count; i++)
                    {
                        if (forcedMoves[i].piece == gameManager.selectedPiece.GetComponent<CSS_Piece>() && forcedMoves[i].cell == new Vector2(x2, y2))
                        {
                            // gets rid of old spot
                            Pieces[x1, y1] = null;

                            //puts the piece into the new place
                            Pieces[x2, y2] = gameManager.selectedPiece.GetComponent<CSS_Piece>();

                            //checks for game over
                            if (gameManager.boardEvaluation(Pieces).whiteCount == 0 || gameManager.boardEvaluation(Pieces).blackCount == 0) print("game over");

                            gameManager.whiteTurn = !gameManager.whiteTurn;

                            //updates board
                            refreshBoard(Pieces);
                            UpdateBoard();
                        }
                    }

                    //unselects piece
                    gameManager.selectedPiece.GetComponent<CSS_Piece>().Deselect();
                }
                // same code but if there isnt a forced move
                else
                {
                    // gets rid of old spot
                    Pieces[x1, y1] = null;

                    //puts the piece into the new place
                    Pieces[x2, y2] = gameManager.selectedPiece.GetComponent<CSS_Piece>();

                    //checks if game over
                    if (gameManager.boardEvaluation(Pieces).whiteCount == 0 || gameManager.boardEvaluation(Pieces).blackCount == 0) print("game over");

                    gameManager.whiteTurn = !gameManager.whiteTurn;

                    //updates board
                    refreshBoard(Pieces);
                    UpdateBoard();

                    //unselects piece
                    gameManager.selectedPiece.GetComponent<CSS_Piece>().Deselect();
                }
            }

            
        }

    }
    #endregion

    #region functions
    //generates the board
    private void GenerateBoard()
    {
        //white
        for (int y = 0; y < 3; y++)
        {
            for(int x = 0; x < 8; x+=2)
            {
                GeneratePiece((y%2 ==0)? x : x+1, y, White);
            }
        }

        //black
        for (int y = 7; y > 4; y--)
        {
            for (int x = 0; x < 8; x += 2)
            {
                GeneratePiece((y % 2 == 0) ? x : x + 1, y, Black);
            }
        }
    }

    //generates the piece based on prefab
    private void GeneratePiece(int x, int y, GameObject color)
    {
        GameObject piece = Instantiate(color, gameObject.transform);
        CSS_Piece p = piece.GetComponent<CSS_Piece>();
        Pieces[x,y] = p;
    }

    //reads the board array and places pieces based on the info
    public void UpdateBoard()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (Pieces[x,y] != null)
                {
                    Pieces[x,y].transform.position = (new Vector2(x*.5f +.25f,y*.5f +.25f));
                }
            }
        }
    }

    //saved the board to a JSON file
    public void SaveBoard()
    {
        // converts board to new array type to be saved
        SArray newBoard = new SArray(gameManager.boardToJSONPrep(Pieces));

        //saves to predetermined location
        string saveBoard = JsonUtility.ToJson(newBoard);
        string filepath = Application.persistentDataPath + "/Board.json";

        System.IO.File.WriteAllText(filepath, saveBoard);
    }

    //loads in the board
    public void LoadBoard()
    {
        //gets file
        SArray LoadedBoard;
        string filepath = Application.persistentDataPath + "/Board.json";

        if (File.Exists(filepath))
        {
            string saveText = File.ReadAllText(filepath);

            LoadedBoard = JsonUtility.FromJson<SArray>(saveText);

            // unscrambles data and updates board
            ReloadBoard(LoadedBoard);

            UpdateBoard();
        }
    }

    // gets rid of old data and replaces it with loaded in data
    public void ReloadBoard(SArray loadFile)
    {
        // kills old board
        DeleteBoardContents(Pieces);

        // gets the data stored in the JSON file and creates new pieces based on it
        for (int i = 0; i < loadFile.items.Length; i++)
        {
            Vector2 pos = loadFile.items[i].pos;
            if (loadFile.items[i].isWhite)
            {
                GeneratePiece((int)pos.x, (int)pos.y, White);
            }
            else
            {
                GeneratePiece((int)pos.x, (int)pos.y, Black);
            }
        }
    }

    // iterates through board and kills all pieces
    public void DeleteBoardContents(CSS_Piece[,] Pieces)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (Pieces[x, y] != null)
                {
                    Destroy(Pieces[x, y].gameObject);
                    Pieces[x,y] = null;
                }
            }
        }
    }

    //test
    public void doRandom()
    {
        print(gameManager.findAllMoves(gameManager.whiteTurn, Pieces).Count);
        Pieces = gameManager.findAllMoves(gameManager.whiteTurn, Pieces)[0];
        refreshBoard(Pieces);
        UpdateBoard();
        gameManager.whiteTurn = !gameManager.whiteTurn;
    }

    public void doMinMax()
    {
        KeyValuePair<float, CSS_Piece[,]> minMaxTest = minmax.Minmax(Pieces, 8, gameManager.whiteTurn);
        Pieces = minMaxTest.Value;
        refreshBoard(Pieces);
        UpdateBoard();
        gameManager.whiteTurn = !gameManager.whiteTurn;
    }

    public void doUCT()
    {
        print(gameManager.whiteTurn);
        Pieces = UCT.UCTMainLoop(Pieces, gameManager.whiteTurn);
        refreshBoard(Pieces);
        UpdateBoard();
        gameManager.whiteTurn = !gameManager.whiteTurn;
    }

    public void refreshBoard(CSS_Piece[,] board)
    {
        Transform[] pieces = gameObject.GetComponentsInChildren<Transform>();
        for (int a = 0; a < pieces.Length; a++)
        {
            bool exists = false;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i,j] == pieces[a].gameObject.GetComponent<CSS_Piece>()) exists = true;
                }
            }
            if (!exists) pieces[a].gameObject.SetActive(false);
        }
    }
    #endregion

}
