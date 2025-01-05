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
        gameManager = GameObject.Find("GameManager").GetComponent<CSS_GameManager>();

        GenerateBoard();
        UpdateBoard();
    }

    private void OnMouseDown()
    {
        if(gameManager.selectedPiece != null)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            print(worldPosition);
            Vector3 cellPos = Vector3.zero;

            cellPos.x = (worldPosition.x);

            cellPos.x = worldPosition.x - (worldPosition.x % .5f) + .25f;
            cellPos.y = worldPosition.y - (worldPosition.y % .5f) + .25f;

            //print("x: " + cellPos.x + " // y: " + cellPos.y);


            var x = (cellPos.x / .5)-.5f;
            var y = (cellPos.y / .5)-.5f;

            if(gameManager.selectedPiece.GetComponent<CSS_Piece>().validMove(Pieces,new Vector2(((float)x), ((float)y))))
            {
                print("x: " + x + " // y: " + y);

                gameManager.selectedPiece.transform.position = cellPos;

                Vector2 ogPlace = gameManager.selectedPiece.GetComponent<CSS_Piece>().FindPlace(Pieces);

                int x1 = (int)ogPlace.x;
                int y1 = (int)ogPlace.y;

                int x2 = (int)x;
                int y2 = (int)y;

                Pieces[x1, y1] = null;

                Pieces[x2, y2] = gameManager.selectedPiece.GetComponent<CSS_Piece>();

                gameManager.selectedPiece.GetComponent<CSS_Piece>().Deselect();
            }

            
        }

    }

    private void GenerateBoard()
    {
        for (int y = 0; y < 3; y++)
        {
            for(int x = 0; x < 8; x+=2)
            {
                GeneratePiece((y%2 ==0)? x : x+1, y, White);
            }
        }

        for (int y = 7; y > 4; y--)
        {
            for (int x = 0; x < 8; x += 2)
            {
                GeneratePiece((y % 2 == 0) ? x : x + 1, y, Black);
            }
        }
    }

    private void GeneratePiece(int x, int y, GameObject color)
    {
        GameObject piece = Instantiate(color);
        CSS_Piece p = piece.GetComponent<CSS_Piece>();
        Pieces[x,y] = p;
    }

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
