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
}
