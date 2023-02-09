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
    bool isPiecePlacementComplete_ = false;

    // --- State --- // 
    enum GameState{
        None, Init, PlayerTurn, EnemyTurn, GameSet, Result, End,
    }
    private GameState gameState_ = GameState.None;
    
    public enum State
    {
        White, Black, None, End,
    }
    //State state_ = State.None;

    // --- Objects --- //
    [SerializeField]
    private GameObject gridBox_;
    private GridBoxScript[,] cellBoxes_;
    private bool[,] isPlayerCanPutPiecePlace_;
    private bool[,] isEnemyCanPutPiecePlace_;
    [SerializeField]
    private PieceManager playerPiceBox_;
    [SerializeField]
    private PieceManager enemyPiceBox_;
    [SerializeField]
    private Transform playerPiceWaitPos_;
    [SerializeField]
    private Transform enemyPiceWaitPos_;
  
    private void Start()
    {
        cellBoxes_ = new GridBoxScript[grid_col_, grid_row_];
        isPlayerCanPutPiecePlace_ = new bool[grid_col_, grid_row_];
        isEnemyCanPutPiecePlace_ = new bool[grid_col_, grid_row_];
        // ---- Create gridBox ---- //
        // --- get gridBox info. --- //     
        for(var c = -4; c < grid_col_ - 4; c++)
        {
            for(var r = -4; r < grid_row_ - 4; r++)
            {
                var cell = Instantiate(gridBox_);
                cell.transform.SetParent(transform);
                cell.transform.position = new Vector3(gridBoxSpan_/2 + c * gridBoxSpan_, cell.transform.position.y, gridBoxSpan_/2 + r * gridBoxSpan_);
                var cellBoxScript = cell.GetComponent<GridBoxScript>();
                if(cellBoxScript == null) { Debug.Log("null error : cellBoxScript."); }
                // -- get cellbox script -- //
                cellBoxes_[c + 4, r + 4] = cellBoxScript;
                // -- reset is-CanPutPiecePlace -- //
                isPlayerCanPutPiecePlace_[c + 4, r + 4] = false;
                isEnemyCanPutPiecePlace_[c + 4, r + 4] = false;
            }   
        }
        // ---- Create Pices & set init ---- //
        if (!playerPiceBox_.GeneratePices(BoardManager.State.White) || !enemyPiceBox_.GeneratePices(BoardManager.State.Black))
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
            StartCoroutine(BoardInit());            // set 4 pieces.
            gameState_ = GameState.PlayerTurn;
        }

        if(GameState.PlayerTurn == gameState_)
        {
            if (!isPiecePlacementComplete_) { return; }
            StartCoroutine(WaitInit(playerPiceBox_, playerPiceWaitPos_));   // set piece on wait pos


        }
        
    }

    private void IsCanPutPiecePlaceUpdate(ref bool[,] isXCanPutPiecePlace, State state)
    {

    }

    private IEnumerator BoardInit()
    {
        isPiecePlacementComplete_= false;
        var piece1 = playerPiceBox_.PopPiceScript();
        var box1 = cellBoxes_[3, 3];
        yield return piece1.pull(box1.PicePutPos.position);
        var piece2 = enemyPiceBox_.PopPiceScript();
        var box2 = cellBoxes_[3, 4];
        yield return piece2.pull(box2.PicePutPos.position);
        var piece3 = playerPiceBox_.PopPiceScript();
        var box3 = cellBoxes_[4, 4];
        yield return piece3.pull(box3.PicePutPos.position);
        var piece4 = enemyPiceBox_.PopPiceScript();
        var box4 = cellBoxes_[4, 3];
        yield return piece4.pull(box4.PicePutPos.position);
        isPiecePlacementComplete_ = true;
    }

    private IEnumerator WaitInit(PieceManager pieceMgr, Transform waitPos)
    {
        isPiecePlacementComplete_ = false;
        var piece = pieceMgr.PopPiceScript();
        yield return piece.pull(waitPos.position);
        isPiecePlacementComplete_ = true;
    }


}
