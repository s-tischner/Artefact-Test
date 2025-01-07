using UnityEngine;

// Board script
// generates board and pieces
// houses update board function that refreshes the visual representation of the game
// authored by Stefanie Tischner with code referenced from https://www.youtube.com/watch?v=93o_Ad5C5Ds&t=679s
public class CSS_Board : MonoBehaviour
{

    //houses the pieces
    public CSS_Piece[,] Pieces = new CSS_Piece[8,8];

    //refs to prefabs
    [SerializeField] GameObject White;
    [SerializeField] GameObject Black;

    CSS_GameManager gameManager;


    void Start()
    {
        //finds game manager
        gameManager = GameObject.Find("GameManager").GetComponent<CSS_GameManager>();

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
            if(gameManager.selectedPiece.GetComponent<CSS_Piece>().validMove(Pieces,new Vector2(((float)x), ((float)y))))
            {
                // finds piece in the array
                Vector2 ogPlace = gameManager.selectedPiece.GetComponent<CSS_Piece>().FindPlace(Pieces);

                int x1 = (int)ogPlace.x;
                int y1 = (int)ogPlace.y;

                int x2 = (int)x;
                int y2 = (int)y;

                // gets rid of old spot
                Pieces[x1, y1] = null;

                //puts the piece into the new place
                Pieces[x2, y2] = gameManager.selectedPiece.GetComponent<CSS_Piece>();

                //updates board
                UpdateBoard();

                //unselects piece
                gameManager.selectedPiece.GetComponent<CSS_Piece>().Deselect();
            }

            
        }

    }

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
        GameObject piece = Instantiate(color);
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
}
