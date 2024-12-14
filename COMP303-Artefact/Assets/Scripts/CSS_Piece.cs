using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSS_Piece : MonoBehaviour
{
    private CSS_GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<CSS_GameManager>();
        if (gameManager == null)
        {
            print("error");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        gameManager.selectedPiece = gameObject;
    }
}
