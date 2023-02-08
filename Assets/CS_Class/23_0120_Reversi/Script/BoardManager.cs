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
    enum GameState{
        None, Init, PlayerTurn, EnemyTurn, GameSet, Result, End,
    }
    private GameState gameState_ = GameState.None;
    
    public enum State
    {
        White, Black, None, End,
    }
    State state_ = State.None;

    // --- Objects --- //
    [SerializeField]
    private GameObject gridBox_;
    private GridBoxScript[,] cellBoxes_;
    [SerializeField]
    private PiceManager playerPiceBox_;
    [SerializeField]
    private PiceManager enemyPiceBox_;
  

    private void Start()
    {
        cellBoxes_ = new GridBoxScript[grid_col_, grid_row_];
        // ---- Create gridBox ---- //
        // --- get gridBox info. --- //     
        for(var c = -4; c < grid_col_ - 4; c++)
        {
            for(var r = -4; r < grid_row_ - 4; r++)
            {
                var cell = Instantiate(gridBox_);
                cell.transform.SetParent(transform);
                cell.transform.position = new Vector3(c * gridBoxSpan_, cell.transform.position.y, r * gridBoxSpan_);
                var cellBoxScript = cell.GetComponent<GridBoxScript>();
                if(cellBoxScript == null) { Debug.Log("null error : cellBoxScript."); }
                // -- get cellbox script -- //
                cellBoxes_[c + 4, r + 4] = cellBoxScript;
            }   
        }
        // ---- Create Pices & set init ---- //
        if (!playerPiceBox_.GeneratePices() || !enemyPiceBox_.GeneratePices())
        {
            Debug.Log("error : Generate Pices");
            return;
        }
        // ---- State Change ---- //
        gameState_++;
    }

    private void Update()
    {
        if(GameState.Init == gameState_)
        {
            // --- set 4 pices --- //

            gameState_++;
        }
        
    }





}
