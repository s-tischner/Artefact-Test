using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSS_GameManager : MonoBehaviour
{
    public GameObject selectedPiece;
    public Collider2D boardCollider;
    // Start is called before the first frame update
    void Start()
    {
        boardCollider = GameObject.Find("Board").GetComponent<Collider2D>();
        boardCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
