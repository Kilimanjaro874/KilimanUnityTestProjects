using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class BoardManager : MonoBehaviour, IPointerClickHandler
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
    //State state_ = State.None;
    // --- Flags --- //
    bool isPiecePlacementComplete_ = false;
    bool IsPlayerPutPieceWait_ = false;
    bool IsEnemyPutPieceWait_ = false;
    // --- Objects --- //
    [SerializeField]
    private GameObject gridBox_;                    // 1 square on the board.
    private GridBoxScript[,] cellBoxes_;            // 8x8 -> BoxScript.
    private GridBoxScript selectedBox_ = null;      // player/enemy choiced box.
    private PieceScript selectedPiece_ = null;      // player/enemy pulled out piece.
    private bool[,] isPlayerCanPutPiecePlace_;
    private bool[,] isEnemyCanPutPiecePlace_;
    [SerializeField]
    private PieceManager playerPiceBox_;    // Piece Stocker for player.
    [SerializeField]
    private PieceManager enemyPiceBox_;     // Piece Stocker for enemy.
    [SerializeField]
    private Transform playerPiceWaitPos_;   // Piece Stock pos for player.
    [SerializeField]
    private Transform enemyPiceWaitPos_;    // Piece Stock pos for enemy.
    // --- UI --- //
    [SerializeField]
    private TextMeshProUGUI playerTurn_;
    [SerializeField]
    private TextMeshProUGUI enemyTurn_;
  
    private void Start()
    {
        cellBoxes_ = new GridBoxScript[grid_col_, grid_row_];
        isPlayerCanPutPiecePlace_ = new bool[grid_col_, grid_row_];
        isEnemyCanPutPiecePlace_ = new bool[grid_col_, grid_row_];
        // ---- UI settings ---- //
        playerTurn_.text = "";
        enemyTurn_.text = "";
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
                // -- set col & row info. -- //
                cellBoxScript.SetBoxColAndRow(c + 4, r + 4);
                // -- reset is-CanPutPiecePlace -- //
                isPlayerCanPutPiecePlace_[c + 4, r + 4] = false;
                isEnemyCanPutPiecePlace_[c + 4, r + 4] = false;
            }   
        }
        // ---- Create Pices & set init ---- //
        if (!playerPiceBox_.GeneratePices(BoardManager.State.White) || !enemyPiceBox_.GeneratePices(BoardManager.State.Black))
        {
            Debug.Log("error : Generate Pieces");
            return;
        }
        // ---- State Change ---- //
        gameState_ = GameState.Init;
        
    }

    private void Update()
    {
        // check game over

        if (isPiecePlacementComplete_)
        {
            if (IsFlippingCheck()) { return; }
            if (CheckGameOver()) { 
                gameState_ = GameState.GameSet;
            }
        }


        // --- Initialize --- //
        if (GameState.Init == gameState_)
        {
            StartCoroutine(BoardInit());            // set 4 pieces.
            gameState_ = GameState.PlayerTurn;
        }

        // --- Player Turn --- //
        if(GameState.PlayerTurn == gameState_)
        {
            if (IsFlippingCheck()) { return; }
           

            if (!isPiecePlacementComplete_) { return; }
            if (!IsPlayerPutPieceWait_)
            {
                // 1. Player turn init. : pull out piece from PieceManager.
                if (IsFlippingCheck()) { return; }
                StartCoroutine(WaitInit(playerPiceBox_, playerPiceWaitPos_));   // set piece on wait pos
                ResetIsCanPutPiecePlace();
                IsCanPutPiecePlaceUpdate(State.White);
                if (!IsCanPiecePlaceCheck())
                {
                    // check game over
                    if (CheckGameOver())
                    {
                        gameState_ = GameState.GameSet;
                    }
                    // not game over -> can't place a piece.
                    IsEnemyPutPieceWait_ = true;
                    gameState_ = GameState.EnemyTurn;
                }
                IsPlayerPutPieceWait_ = true;
            } else
            {

                // 2. Player turn.
                // -- UI setting -- //
                playerTurn_.text = "Player turn";
                enemyTurn_.text = "";
                Debug.Log("Player turn.");
                // -- wait for determination of piece position -- //
                if (!selectedBox_) { return; }      
                if (selectedPiece_)
                {
                    // 2.1. place a piece
                    selectedPiece_.State= State.White;
                    selectedPiece_.putPiece(selectedBox_.PicePutPos.position);
                    selectedPiece_ = null;
                }   
                if (selectedBox_.State != State.None)
                {
                    // 2.2. wait for piece placement comp.
                    FlipPieces(selectedBox_);
                    selectedBox_ = null;
                    IsPlayerPutPieceWait_ = false;
                    IsEnemyPutPieceWait_= false;
                    gameState_ = GameState.EnemyTurn;
                }
            }
        }

        // --- Enemy Turn --- //
        if(GameState.EnemyTurn == gameState_)
        {
            if (IsFlippingCheck()) { return; }
            
            Debug.Log("enemy turn");
            if (!isPiecePlacementComplete_) { return; }
            if (!IsEnemyPutPieceWait_)
            {
                // 1. Player turn init. : pull out piece from PieceManager.
                if (IsFlippingCheck()) { return; }
                StartCoroutine(WaitInit(enemyPiceBox_, enemyPiceWaitPos_));   // set piece on wait pos
                ResetIsCanPutPiecePlace();
                IsCanPutPiecePlaceUpdate(State.Black);
                if (!IsCanPiecePlaceCheck())
                {
                    // check game over
                    if (CheckGameOver())
                    {
                        gameState_ = GameState.GameSet;
                    }
                    // can't place a piece.

                    IsEnemyPutPieceWait_ = true;
                    gameState_ = GameState.PlayerTurn;
                }
                IsEnemyPutPieceWait_ = true;
            }
            else
            {
                // 2. Player turn.
                // -- UI setting -- //
                playerTurn_.text = "";
                enemyTurn_.text = "Enemy turn";
                
                // -- wait for determination of piece position -- //
                if (!selectedBox_) { return; }
                if (selectedPiece_)
                {
                    // 2.1. place a piece
                    selectedPiece_.State = State.Black;
                    selectedPiece_.putPiece(selectedBox_.PicePutPos.position);
                    selectedPiece_ = null;
                }
                if (selectedBox_.State != State.None)
                {
                    // 2.2. wait for piece placement comp.
                    FlipPieces(selectedBox_);
                    selectedBox_ = null;
                    IsEnemyPutPieceWait_= false;
                    IsPlayerPutPieceWait_ = false;
                    gameState_ = GameState.PlayerTurn;
                }
            }
        }

        // --- Game Over --- //
        if(GameState.GameSet == gameState_)
        {
            Debug.Log("End Game");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
         
        //if(GameState.PlayerTurn != gameState_) { return; }
        if (IsFlippingCheck()) { return; }
        if(eventData.button == PointerEventData.InputButton.Left) {
            var box = eventData.pointerCurrentRaycast.gameObject;
            var boxScript = box.GetComponent<GridBoxScript>();
            if (!boxScript) { return; }
            var choicedBox = boxScript;
            if (!choicedBox.IsCanPlacePiece) {
                Debug.Log("Can't place!");
                return; 
            }
            selectedBox_ = choicedBox;      // Determining the placement box.
        }
    }

    private void IsCanPutPiecePlaceUpdate(State state)
    {
        for(var c = 0; c < cellBoxes_.GetLength(0); c++)
        {
            for(var r = 0; r < cellBoxes_.GetLength(1); r++)
            {
                var cellBox = cellBoxes_[c, r];
                if (!cellBox) continue;
                if (cellBox.State != BoardManager.State.None) { continue; }     // already exists : ignore
                // check
                if (PiecePlaceRecursiveCheck(state, c, r, -1, -1, 0)) {  }
                if (PiecePlaceRecursiveCheck(state, c, r, 0, -1, 0 )) {  }
                if (PiecePlaceRecursiveCheck(state, c, r, 1, -1, 0 )) {  }

                if (PiecePlaceRecursiveCheck(state, c, r, -1, 0, 0)) {  }
                //if (PiecePlaceRecursiveCheck(state, c, r, 0, 0, 0)) { cellBox.IsCanPlacePiece = true; }
                if(c == 3 && r == 1)
                {
                    Debug.Log("deb");
                }
                if (PiecePlaceRecursiveCheck(state, c, r, 1, 0, 0)) { }

                if (PiecePlaceRecursiveCheck(state, c, r, -1, 1, 0)) {  }
                if (PiecePlaceRecursiveCheck(state, c, r, 0, 1, 0)) {  }
                if (PiecePlaceRecursiveCheck(state, c, r, 1, 1, 0)) {  }
            }
        }
    }

    private bool IsCanPiecePlaceCheck()
    {
        for (var c = 0; c < cellBoxes_.GetLength(0); c++)
        {
            for (var r = 0; r < cellBoxes_.GetLength(1); r++)
            {
                var cellBox = cellBoxes_[c, r];
                if (cellBox.IsCanPlacePiece)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void ResetIsCanPutPiecePlace()
    {
        for(var c = 0; c < cellBoxes_.GetLength(0); c++)
        {
            for(var r = 0; r < cellBoxes_.GetLength(1); r++)
            {
                var cellBox = cellBoxes_[c, r];
                cellBox.IsCanPlacePiece = false;
            }
        }
    }

    private bool IsFlippingCheck()
    {

        for (var c = 0; c < cellBoxes_.GetLength(0); c++)
        {
            for (var r = 0; r < cellBoxes_.GetLength(1); r++)
            {
                var cellBox = cellBoxes_[c, r];
                if(cellBox.PlacedPieceScript!= null)
                {
                    var pice = cellBox.PlacedPieceScript;
                    if (pice.IsFlipping)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private bool PiecePlaceRecursiveCheck(State state, int current_col, int current_row, int dirC, int dirR, int count)
    {
        if (count < 0 || count > grid_row_ - 1) { return false; }
        var currentDirC = (dirC >= 0) ? dirC * (count + 1) : (int)(-1 * Mathf.Abs(dirC) * (count + 1));
        var currentDirR = (dirR >= 0) ? dirR * (count + 1) : (int)(-1 * Mathf.Abs(dirR) * (count + 1));
        var tmpC = current_col + currentDirC;
        var tmpR = current_row + currentDirR;
        if (tmpC < 0 || tmpC >= cellBoxes_.GetLength(0)) { return false; }
        if (tmpR < 0 || tmpR >= cellBoxes_.GetLength(1)) { return false; }
        var cellBox = cellBoxes_[tmpC, tmpR];
        if (count == 0 && cellBox.State == State.None) { return false; }
        if (count != 0 && cellBox.State == state) {
            var currentBox = cellBoxes_[current_col, current_row];
            currentBox.IsCanPlacePiece = true;
            return true;
        }
        var invState = (state == State.White) ? State.Black : State.White;
        if (cellBox.State == invState) {
            count++;
            PiecePlaceRecursiveCheck(state, current_col, current_row, dirC, dirR, count);
        }
        return false;
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

    private void FlipPieces(GridBoxScript boxScript)
    {
        int col, row;
        boxScript.GetBoxColAndRow(out col, out row);


        FlipPiecesRecursive(boxScript.State, col, row, -1, -1);
        FlipPiecesRecursive(boxScript.State, col, row, 0, -1);
        FlipPiecesRecursive(boxScript.State, col, row, 1, -1);
        FlipPiecesRecursive(boxScript.State, col, row, -1, 0);
        //FlipPiecesRecursive(boxScript.State, col, row, 0, 0);
        FlipPiecesRecursive(boxScript.State, col, row, 1, 0);
        FlipPiecesRecursive(boxScript.State, col, row, -1, 1);
        FlipPiecesRecursive(boxScript.State, col, row, 0, 1);
        FlipPiecesRecursive(boxScript.State, col, row, 1, 1);
    }

    private bool FlipPiecesRecursive(State state, int current_col, int current_row, int dirC, int dirR)
    {
        var tmpC = current_col + dirC;
        var tmpR = current_row + dirR;
        var invState = (state == State.White) ? State.Black : State.White;
        while (0 <= tmpC && tmpC < cellBoxes_.GetLength(0) && 0 <= tmpR && tmpR < cellBoxes_.GetLength(1))
        {
            var boxScript = cellBoxes_[tmpC, tmpR];
            if (boxScript.State == state) { break; }
            if (boxScript.State == State.None) { return false; }
            tmpC += dirC;
            tmpR += dirR;
        }

        if(0 <= tmpC && tmpC < cellBoxes_.GetLength(0) && 0 <= tmpR && tmpR < cellBoxes_.GetLength(1))
        {
            while(tmpC != current_col || tmpR != current_row)
            {
                tmpC -= dirC;
                tmpR -= dirR;
                var boxScript = cellBoxes_[tmpC, tmpR];
                if (tmpC != current_col || tmpR != current_row)
                {
                    boxScript.TurnPiece();
                }
            }
        }
        return true;
       
    }

    private bool CheckGameOver()
    {
        int noneCount = 0;
        int whiteCount = 0;
        int blackCount = 0;
        for (var c = 0; c < cellBoxes_.GetLength(0); c++)
        {
            for (var r = 0; r < cellBoxes_.GetLength(1); r++)
            {
                var boxScript = cellBoxes_[c, r];
                if(boxScript.State == State.None) { noneCount++; }
                if(boxScript.State == State.White) { whiteCount++; }
                if(boxScript.State == State.Black) { blackCount++; }
            }
        }
        if(noneCount== 0) { return true; }
        if(whiteCount==0) { return true; }
        if(blackCount==0) { return true; }
        return false;
    }


    private IEnumerator WaitInit(PieceManager pieceMgr, Transform waitPos)
    {
        isPiecePlacementComplete_ = false;
        var piece = pieceMgr.PopPiceScript();
        if (piece)
        {
            yield return piece.pull(waitPos.position);
            selectedPiece_ = piece;
            isPiecePlacementComplete_ = true;
        }
    }
}
