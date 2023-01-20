using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    //// ---- Memver variables ---- ////
    // --- Define --- //
    const int grid_row_ = 8;
    const int grid_col_ = 8;
    const float gridBoxSpan_ = 1.0f;

    // --- State --- // 
    private enum GameState{
        Init, Playing, GameSet, End,
    }

    // --- Objects --- //
    [SerializeField]
    private GameObject gridBox_;
    private GridBoxScript[,] cells_;
  

    private void Start()
    {
        // --- get gridBox info. --- //     
        for(var c = -4; c < grid_col_ - 4; c++)
        {
            for(var r = -4; r < grid_row_ - 4; r++)
            {
                
                var cell = Instantiate(gridBox_);
                cell.transform.SetParent(transform);
                cell.transform.position = new Vector3(c * gridBoxSpan_, cell.transform.position.y, r * gridBoxSpan_);

            }   
        }
    }



}
