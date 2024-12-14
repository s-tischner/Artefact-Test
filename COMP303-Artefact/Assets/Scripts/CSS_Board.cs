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


    void Start()
    {
        GenerateBoard();
        UpdateBoard();
    }

    private void Update()
    {

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
