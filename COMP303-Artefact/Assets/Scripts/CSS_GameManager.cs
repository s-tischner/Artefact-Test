using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSS_GameManager : MonoBehaviour
{
    public GameObject selectedPiece;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(selectedPiece != null)
        {
            selectedPiece.SetActive(false);
        }
    }
}
