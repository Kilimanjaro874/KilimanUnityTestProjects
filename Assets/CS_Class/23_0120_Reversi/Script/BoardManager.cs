using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class BoardManager : MonoBehaviour, IPointerClickHandler
{
    // ---- Member variables ---- //
    // --- Define --- //
    const int col_ = 8;
    const int row_ = 8;
    const float gridSpan_ = 1.0f;
    public enum State
    {
        White, Black, None,
    }
    private State[,] boardState_;
    private State[,] boardStateForGameOverCheck_;
    public enum GameState
    {
        None, PlayerTurn, EnemyTurn, GameSet,
    }
    private GameState gameState_ = GameState.None;
    private int[,] whiteCanPlacePosAndNum_;
    private int[,] blackCanPlacePosAndNum_;
    private int[,] whiteCanPlacePosAndNumForGameOver_;
    private int[,] blackCanPlacePosAndNumForGameOver_;
    // --- Objects --- //
    [SerializeField] private GameObject gridBox_;
    [SerializeField] private PieceStocker pieceStockerPlayer_;
    [SerializeField] private PieceStocker pieceStockerEnemy_;
    [SerializeField] private Transform playerPieceWaitPos_;
    [SerializeField] private Transform enemyPieceWaitPos_;
    private GridBoxScript[,] gridBoxScript_;
    // --- flags --- //
    private bool isCoroutineTaskNow_ = false;       // while coroutine task : true;
    private bool isTurnInit = false;
    private bool isPiecePlaced = false;
    private bool isUpdatePermission = false;
    // --- Selected Objects --- //
    private PieceScript selectedPieceScript_ = null;
    private GridBoxScript selectedGridBoxScript_ = null;
    // --- UI --- //
    [SerializeField]
    private TextMeshProUGUI playerTurn_;
    [SerializeField]
    private TextMeshProUGUI enemyTurn_;
    // --- AI --- //
    [SerializeField]
    private int thinkDepth = 3;

    // -------------------------- //

    private void Start()
    {
        // --- create reversi board --- //
        BoardInit();
        // --- initialize PieceStocker --- //
        pieceStockerPlayer_.Init(State.White);
        pieceStockerEnemy_.Init(State.Black);
        StartCoroutine(PiecePlaceInit());
        gameState_ = GameState.PlayerTurn;
        // ---- UI settings ---- //
        playerTurn_.text = "";
        enemyTurn_.text = "";

        // ---- AI test ---- //
        //scoreAndDepth_ = new scoreAndDepth[200];
    }

    private void FixedUpdate()
    {
        isUpdatePermission= false;      // use for other func.
        // --- if coroutine task updating : return --- //
        if (isCoroutineTaskNow_) { return; }
        // --- if pieces is moving : return (wait for task to finish) --- //
        if (pieceStockerPlayer_.IsPiecesMovingCheck() || pieceStockerEnemy_.IsPiecesMovingCheck()) { return; }
        isUpdatePermission = true;

        // ---- 0. Game over check ---- //
        if (GameOverCheck()) { gameState_ = GameState.GameSet; }

        // ---- 1. Player turn ---- //
        if(GameState.PlayerTurn == gameState_)
        {
           Debug.Log("player");
            // --- 1.1. Initialize --- //
            if (!isTurnInit)
            {
                if (PieceCanBePlacedCheck(State.White, ref whiteCanPlacePosAndNum_))
                {   // can place a piece : init.
                    TurnInitialize(playerPieceWaitPos_.position, pieceStockerPlayer_, pieceStockerEnemy_);
                    isTurnInit = true;
                } else
                {   // cann't place a piece 
                    gameState_ = GameState.EnemyTurn; 
                }
                
                return;
            }
            // --- 1.2. update --- //
            playerTurn_.text = "Player Turn";
            enemyTurn_.text = "";
            if (!isPiecePlaced)
            {
                if (!TurnUpdate(State.White)) { return; }  // wait : place a piece. (else : return )
                isPiecePlaced= true;
                
                return;
            } else
            {
                TurnEndAndFlipPieces(State.White);
                isTurnInit = false;
                isPiecePlaced = false;
                gameState_= GameState.EnemyTurn;
                
                return;
            }
        }

        // ---- 2. Enemy turn ---- //
        if(GameState.EnemyTurn == gameState_)
        {
            Debug.Log("enemy turn");
            // --- 2.1. Initialize --- //
            if (!isTurnInit)
            {
                if(PieceCanBePlacedCheck(State.Black, ref blackCanPlacePosAndNum_))
                {   // can place a piece : init.
                    TurnInitialize(enemyPieceWaitPos_.position, pieceStockerEnemy_, pieceStockerPlayer_);
                    isTurnInit = true;
                } else
                {
                    gameState_ = GameState.PlayerTurn;
                }
                return;
            }
            // --- 2.2. update --- //
            playerTurn_.text = "";
            enemyTurn_.text = "Enemy Turn";
            if (!isPiecePlaced)
            {
                if (!TurnUpdate(State.Black)) { return; }   // wait : place a piece. (else : return )
                isPiecePlaced = true;

                return;
            } else
            {
                TurnEndAndFlipPieces(State.Black);
                isTurnInit = false;
                isPiecePlaced = false;
                gameState_ = GameState.PlayerTurn;

                return;
            }
        }

        // ---- 3. GameOver ---- //
        if(GameState.GameSet== gameState_)
        {
            Debug.Log("Game over");
            int whiteNum, blackNum;
            Result(out whiteNum, out blackNum);
            if(whiteNum > blackNum)
            {
                playerTurn_.text = "Player Win!\n" + "Piece Num\n => " + whiteNum.ToString();
                enemyTurn_.text = "Loose...\n" + "Piece Num\n => " + blackNum.ToString();
            }
            if(blackNum > whiteNum)
            {
                playerTurn_.text = "Loose...\n" + "Piece Num\n => " + whiteNum.ToString();
                enemyTurn_.text = "Enemy Win!\n" + "Piece Num\n => " + blackNum.ToString(); 
            }
            if(blackNum == whiteNum)
            {
                playerTurn_.text = "Draw...\n" + "Piece Num\n => " + whiteNum.ToString(); 
                enemyTurn_.text = "Draw...\n" + "Piece Num\n => " + blackNum.ToString(); 
            }
        }
    }

    // -------------------------- //
    // ---- Member functions ---- //
    private void BoardInit()
    {
        // --- init object state --- //
        gridBoxScript_ = new GridBoxScript[col_, row_];
        // --- init array state --- //
        boardState_ = new State[col_, row_];
        boardStateForGameOverCheck_ = new State[col_, row_];
        whiteCanPlacePosAndNum_= new int[col_, row_];
        blackCanPlacePosAndNum_= new int[col_, row_];
        whiteCanPlacePosAndNumForGameOver_= new int[col_, row_];
        blackCanPlacePosAndNumForGameOver_= new int[col_, row_];
        // --- create reversi board --- //
        for (var c = -4; c < col_ - 4; c++)
        {
            for (var r = -4; r < row_ - 4; r++)
            {
                var box = Instantiate(gridBox_);
                box.transform.SetParent(transform);
                box.transform.position = new Vector3(gridSpan_ / 2 + c * gridSpan_,
                    box.transform.position.y,
                    gridSpan_ / 2 + r * gridSpan_
                    );
                var boxScript = box.GetComponent<GridBoxScript>();
                if (!boxScript) { Debug.Log("error : null of GridBoxScript"); return; }
                // -- get boxScript info. -- //
                gridBoxScript_[c + 4, r + 4] = boxScript;
                // -- set boxScript info. -- //
                boxScript.SetColAndRow(c + 4, r + 4);
            }
        }
    }

    private void TurnInitialize(Vector3 WaitPos, PieceStocker stocker, PieceStocker otherStocker)
    {
        // --- pull out piece, put piece --- //
        StartCoroutine(PullOutPieceToWaitPos(WaitPos, stocker, otherStocker));
        // --- permit : box Highlight --- //
        for (var c = 0; c < col_; c++)
        {
            for (var r = 0; r < row_; r++)
            {
                gridBoxScript_[c, r].IsHilightPermit= true;
            }
        }
    }

    private bool TurnUpdate(State state)
    {
        // --- put piece --- //
        if (!selectedPieceScript_) { return false; }
        if (!selectedGridBoxScript_) { return false; }    // in control : OnPointerClick();
        if (selectedPieceScript_.State != state)
        {
            selectedPieceScript_.State = state;
            StartCoroutine(PieceTurn(state, selectedPieceScript_));
        }
        StartCoroutine(PutPieceToBox(selectedGridBoxScript_.PiecePutPosition.position, selectedPieceScript_));
        // -- update box info. -- //
        selectedGridBoxScript_.PlacedPieceScript = selectedPieceScript_;
        return true;
    }

    private bool EnemyAITurnUpdate(State state)
    {
        // --- AI thinking time --- //


        // --- put piece --- //
        if (!selectedPieceScript_) { return false; }
        if (!selectedGridBoxScript_) { return false; }    // in control : OnPointerClick();
        if (selectedPieceScript_.State != state)
        {
            selectedPieceScript_.State = state;
            StartCoroutine(PieceTurn(state, selectedPieceScript_));
        }
        StartCoroutine(PutPieceToBox(selectedGridBoxScript_.PiecePutPosition.position, selectedPieceScript_));
        // -- update box info. -- //
        selectedGridBoxScript_.PlacedPieceScript = selectedPieceScript_;

        return true;
    }

    private struct scoreAndDepth
    {
        public int thinkDepth;
        public int score;
        public int currentCol;
        public int currentRow;
        public State state;
        public State[,] preState;
        public int[,] preCanGetNum;
        public State[,] nowState;
        public int[,] nowCanGetNum;
        public int row;
        public int col;
    }
    private scoreAndDepth[] scoreAndDepth_;

    //private void AIThinkColRow(out int col, out int row, State state, ref State[,] boardState, ref int[,] canGetNum)
    //{
    //    for(var c = 0; c < col_; c++)
    //    {
    //        for(var r = 0; r < row_; r++)
    //        {
    //            if (canGetNum[c, r] <= 0) { continue; }

    //            AIThinkColRow(c, r, state, 0, ref boardState, ref canGetNum, 0);
    //        }
    //    }

    //    // return result
    //    col= 0; row = 0;
    //}

    //private void AIThinkColRow(int col, int row, State state, int count, ref State[,] boardState, ref int[,] canGetNum, int depth)
    //{
    //    // get : current status.
    //    scoreAndDepth_[count].thinkDepth = depth;
    //    scoreAndDepth_[count].currentCol = col;
    //    scoreAndDepth_[count].currentRow = row;
    //    scoreAndDepth_[count].preState = boardState;
    //    scoreAndDepth_[count].preCanGetNum= canGetNum;
    //    // process:


    //    // next 

    //}

    //private bool ThinkPieceCanBePlacedCheck(State state, ref State[,] boardState, ref int[,] canGetNum, ref scoreAndDepth sAD)
    //{
    //    // --- 1. init --- //
    //    for(var c = 0; c < col_; c++)
    //    {
    //        for (var r = 0; r < row_; r++)
    //        {
    //            // -- init --- //
    //            sAD.nowCanGetNum[c, r] = 0;
    //        }
    //    }

    //    // 2. check :
    //    for(var c = 0; c < col_; c++) {
    //        for(var r = 0; r < row_; r++)
    //        {

    //        }
    //    }


    //    return false;
    //}

    //private int PlacedCheckAndGetNum(State state, int col, int row, int dirC, int dirR, int count, ref scoreAndDepth sAD)
    //{
    //    var cDirC = (dirC >= 0) ? dirC * (count + 1) : (int)(-1 * Mathf.Abs(dirC) * (count + 1));
    //    var cDirR = (dirR >= 0) ? dirR * (count + 1) : (int)(-1 * Mathf.Abs(dirR) * (count + 1));
    //    var tmpC = col + cDirC;
    //    var tmpR = row + cDirR;
    //    if (tmpC < 0 || tmpC >= col_ || tmpR < 0 || tmpR >= row_) { return 0; }
    //    State cState = sAD.preState[tmpC, tmpR];
    //    // 0. missing piece. : end.
    //    if (count == 0 && cState == State.None) { return 0; }
    //}




    private bool GameOverCheck()
    {
        if (PieceCanBePlacedCheckForGameOverCheck(State.White, ref whiteCanPlacePosAndNumForGameOver_)) { return false; }
        if (PieceCanBePlacedCheckForGameOverCheck(State.Black, ref blackCanPlacePosAndNumForGameOver_)) { return false; }
        return true;
    }

    private void Result(out int whiteNum, out int blackNum)
    {
        // --- get whitePiece num & BlackPiece num --- //
        int white = 0;
        int black = 0;
        for(var c = 0; c < col_; c++)
        {
            for(var r = 0; r < row_; r++)
            {
                var boxScript = gridBoxScript_[c, r];
                var pieceScript = boxScript.PlacedPieceScript;
                if (!pieceScript) { continue; }
                if (pieceScript.State == State.White) { white ++; }
                if (pieceScript.State == State.Black) { black ++; }
            }
        }
        whiteNum = white; 
        blackNum = black;
    }

    private void TurnEndAndFlipPieces(State state)
    {
        // --- prohibit : box Highlight --- //
        for (var c = 0; c < col_; c++)
        {
            for (var r = 0; r < row_; r++)
            {
                gridBoxScript_[c, r].IsHilightPermit = false;
            }
        }
        // --- turn Pieces --- //
        int col, row;
        selectedGridBoxScript_.GetColAndRow(out col, out row);
        FlipPiecesRecursive(state, col, row, -1, -1);
        FlipPiecesRecursive(state, col, row, -0, -1);
        FlipPiecesRecursive(state, col, row, 1, -1);
        FlipPiecesRecursive(state, col, row, -1, 0);
        //FlipPiecesRecursive(state, col, row, 0, 0);
        FlipPiecesRecursive(state, col, row, 1, 0);
        FlipPiecesRecursive(state, col, row, -1, 1);
        FlipPiecesRecursive(state, col, row, 0, 1);
        FlipPiecesRecursive(state, col, row, 1, 1);
        // --- fold flags --- //
        selectedPieceScript_ = null;
        selectedGridBoxScript_= null;
    }

    private void FlipPiecesRecursive(State state, int col, int row, int dirC, int dirR)
    {
        var tmpC = col + dirC;
        var tmpR = row + dirR;
        //var eState = (state == State.White) ? State.Black : State.White;
        while (0 <= tmpC && tmpC < col_ && 0 <= tmpR && tmpR < row_)
        {
            var boxScript = gridBoxScript_[tmpC, tmpR];
            var piceScript = boxScript.PlacedPieceScript;

            // --- 1. check : piece exist --- //
            if (!piceScript) { return; }         
            // --- 2. check : different color --- //
            if(piceScript.State == state) { break; }
            tmpC += dirC;       // advance the scan.
            tmpR += dirR;       // 
        }
        // -- 3. check : in an array -- //
        if(0 <= tmpC && tmpC < col_ && 0 <= tmpR && tmpR < row_)
        {
            // -- 4. scan returning & turn pieces -- //
            while(tmpC != col || tmpR != row)
            {
                tmpC -= dirC;
                tmpR -= dirR;
                var boxScript = gridBoxScript_[tmpC, tmpR];
                var pieceScript = boxScript.PlacedPieceScript;
                // -- turn -- //
                if (tmpC != col || tmpR != row)
                {
                    pieceScript.State = state;
                    StartCoroutine(PieceTurnNormal(state, pieceScript));
                }
            }
        }
    }

    private bool PieceCanBePlacedCheck(State state, ref int[,] CanPlacePosAndNum)
    {
        // --- 1. init array status & info. update --- //
        for(var c = 0; c < col_; c++)
        {
            for(var r = 0; r < row_; r++)
            {
                // -- init -- //
                CanPlacePosAndNum[c, r] = 0;
                // -- update -- //
                var box = gridBoxScript_[c, r];
                if (box.PlacedPieceScript)
                {
                    // -- have a piece : cann't place a piece  -- //
                    var piece = box.PlacedPieceScript;
                    boardState_[c, r] = (piece.State == State.White) ? State.White : State.Black;
                } else
                {
                    // -- don't have a piece  : possibility to place piece -- //
                    boardState_[c, r] = State.None;
                }
            }
        }
        // --- 2. check : possibility to place piece & get enemy piece num --- //
        for(var c = 0; c < col_; c++)
        {
            for(var r = 0; r < row_; r++)
            {
                if (boardState_[c,r] != State.None) { continue; }   // cann't place 
                CanPlacePosAndNum[c, r] += PieceCanBePlacedCheckAndGetNum(state, c, r, -1, -1, 0);
                CanPlacePosAndNum[c, r] += PieceCanBePlacedCheckAndGetNum(state, c, r, 0, -1, 0);
                CanPlacePosAndNum[c, r] += PieceCanBePlacedCheckAndGetNum(state, c, r, 1, -1, 0);
                CanPlacePosAndNum[c, r] += PieceCanBePlacedCheckAndGetNum(state, c, r, -1, 0, 0);
                //CanPlacePosAndNum[c, r] += PieceCanBePlacedCheckAndGetNum(state, c, r, 0, 0, 0);
                CanPlacePosAndNum[c, r] += PieceCanBePlacedCheckAndGetNum(state, c, r, 1, 0, 0);
                CanPlacePosAndNum[c, r] += PieceCanBePlacedCheckAndGetNum(state, c, r, -1, 1, 0);
                CanPlacePosAndNum[c, r] += PieceCanBePlacedCheckAndGetNum(state, c, r, 0, 1, 0);
                CanPlacePosAndNum[c, r] += PieceCanBePlacedCheckAndGetNum(state, c, r, 1, 1, 0);
            }
        }
        // --- 3. check : can place piece ? --- //
        int canPlacePosNum = 0;
        for (var c = 0; c < col_; c++)
        {
            for(var r = 0; r < row_; r++)
            {
                var boxScript = gridBoxScript_[c, r];
                if (CanPlacePosAndNum[c, r] != 0)
                {
                    canPlacePosNum++;
                    // -- flag change : GridBoxScript -- //
                    boxScript.IsCanPiecePlace = true;
                } else
                {
                    // -- flag change : GridBoxScript -- //
                    boxScript.IsCanPiecePlace = false;
                }
            }
        }
        if(canPlacePosNum > 0) { return true; }

        return false;
    }

    private bool PieceCanBePlacedCheckForGameOverCheck(State state, ref int[,] CanPlacePosAndNum)
    {
        // --- 1. init array status & info. update --- //
        for (var c = 0; c < col_; c++)
        {
            for (var r = 0; r < row_; r++)
            {
                // -- init -- //
                CanPlacePosAndNum[c, r] = 0;
                // -- update -- //
                var box = gridBoxScript_[c, r];
                if (box.PlacedPieceScript)
                {
                    // -- have a piece : cann't place a piece  -- //
                    var piece = box.PlacedPieceScript;
                    boardStateForGameOverCheck_[c, r] = (piece.State == State.White) ? State.White : State.Black;
                }
                else
                {
                    // -- don't have a piece  : possibility to place piece -- //
                    boardStateForGameOverCheck_[c, r] = State.None;
                }
            }
        }
        // --- 2. check : possibility to place piece & get enemy piece num --- //
        for (var c = 0; c < col_; c++)
        {
            for (var r = 0; r < row_; r++)
            {
                if (boardStateForGameOverCheck_[c, r] != State.None) { continue; }   // cann't place 
                CanPlacePosAndNum[c, r] += PieceCanBePlacedCheckAndGetNumForGameOverCheck(state, c, r, -1, -1, 0);
                CanPlacePosAndNum[c, r] += PieceCanBePlacedCheckAndGetNumForGameOverCheck(state, c, r, 0, -1, 0);
                CanPlacePosAndNum[c, r] += PieceCanBePlacedCheckAndGetNumForGameOverCheck(state, c, r, 1, -1, 0);
                CanPlacePosAndNum[c, r] += PieceCanBePlacedCheckAndGetNumForGameOverCheck(state, c, r, -1, 0, 0);
                //CanPlacePosAndNum[c, r] += PieceCanBePlacedCheckAndGetNumForGameOverCheck(state, c, r, 0, 0, 0);
                CanPlacePosAndNum[c, r] += PieceCanBePlacedCheckAndGetNumForGameOverCheck(state, c, r, 1, 0, 0);
                CanPlacePosAndNum[c, r] += PieceCanBePlacedCheckAndGetNumForGameOverCheck(state, c, r, -1, 1, 0);
                CanPlacePosAndNum[c, r] += PieceCanBePlacedCheckAndGetNumForGameOverCheck(state, c, r, 0, 1, 0);
                CanPlacePosAndNum[c, r] += PieceCanBePlacedCheckAndGetNumForGameOverCheck(state, c, r, 1, 1, 0);
            }
        }
        // --- 3. check : can place piece ? --- //
        int canPlacePosNum = 0;
        for (var c = 0; c < col_; c++)
        {
            for (var r = 0; r < row_; r++)
            {
                var boxScript = gridBoxScript_[c, r];
                if (CanPlacePosAndNum[c, r] != 0)
                {
                    canPlacePosNum++;
                    // -- flag change : GridBoxScript -- //
                    //boxScript.IsCanPiecePlace = true;
                }
                else
                {
                    // -- flag change : GridBoxScript -- //
                    //boxScript.IsCanPiecePlace = false;
                }
            }
        }
        if (canPlacePosNum > 0) { return true; }

        return false;
    }

    private int PieceCanBePlacedCheckAndGetNum(State state, int col, int row, int dirC, int dirR, int count)
    {
        var cDirC = (dirC >= 0) ? dirC * (count + 1) : (int)(-1 * Mathf.Abs(dirC) * (count + 1));
        var cDirR = (dirR >= 0) ? dirR * (count + 1) : (int)(-1 * Mathf.Abs(dirR) * (count + 1));
        var tmpC = col + cDirC;
        var tmpR = row + cDirR;
        if (tmpC < 0 || tmpC >= col_ || tmpR < 0 || tmpR >= row_) { return 0; }
        State cState = boardState_[tmpC, tmpR];
        // 0. missing piece. : end.
        if (count == 0 && cState == State.None) { return 0; }
        // 1. enemy piece exists. : continue.
        State eState = (state == State.White) ? State.Black : State.White;
        if(cState == eState)
        {
            count++;
            return PieceCanBePlacedCheckAndGetNum(state, col, row, dirC, dirR, count);
        }
        // 2. reached state piece. : retun num of capture enemy piece.
        if(count > 0 && cState == state)
        {
            return (count);
        }
        return 0;
    }

    private int PieceCanBePlacedCheckAndGetNumForGameOverCheck(State state, int col, int row, int dirC, int dirR, int count)
    {
        var cDirC = (dirC >= 0) ? dirC * (count + 1) : (int)(-1 * Mathf.Abs(dirC) * (count + 1));
        var cDirR = (dirR >= 0) ? dirR * (count + 1) : (int)(-1 * Mathf.Abs(dirR) * (count + 1));
        var tmpC = col + cDirC;
        var tmpR = row + cDirR;
        if (tmpC < 0 || tmpC >= col_ || tmpR < 0 || tmpR >= row_) { return 0; }
        State cState = boardStateForGameOverCheck_[tmpC, tmpR];
        // 0. missing piece. : end.
        if (count == 0 && cState == State.None) { return 0; }
        // 1. enemy piece exists. : continue.
        State eState = (state == State.White) ? State.Black : State.White;
        if (cState == eState)
        {
            count++;
            return PieceCanBePlacedCheckAndGetNumForGameOverCheck(state, col, row, dirC, dirR, count);
        }
        // 2. reached state piece. : retun num of capture enemy piece.
        if (count > 0 && cState == state)
        {
            return (count);
        }
        return 0;
    }

    // ----------------- //
    // --- Coroutine --- //
    private IEnumerator PiecePlaceInit()
    {
        // --- set 4 pieces --- //
        isCoroutineTaskNow_ = true;
        var p1 = pieceStockerPlayer_.PopPiece();
        var b1 = gridBoxScript_[3, 3];
        yield return p1.pull(b1.PiecePutPosition.position);
        var p2 = pieceStockerEnemy_.PopPiece();
        var b2 = gridBoxScript_[3, 4];
        yield return p2.pull(b2.PiecePutPosition.position);
        var p3 = pieceStockerPlayer_.PopPiece();
        var b3 = gridBoxScript_[4, 4];
        yield return p3.pull(b3.PiecePutPosition.position);
        var p4 = pieceStockerEnemy_.PopPiece();
        var b4 = gridBoxScript_[4, 3];
        yield return p4.pull(b4.PiecePutPosition.position);
        isCoroutineTaskNow_ = false;
        
        yield return null;
    }

    private IEnumerator PullOutPieceToWaitPos(Vector3 WaitPos, PieceStocker stocker, PieceStocker otherStocker)
    {
        // --- Pice pullout from PieceStocker & move to wait position --- //
        isCoroutineTaskNow_ = true;
        var piece = stocker.PopPiece();
        if (!piece)
        {
            // if player / enemy Stocker is void : get from other Stocker.
            piece = otherStocker.PopPiece();
            if(!piece) { yield return null; }
        }
        if (piece)
        {
            yield return piece.pull(WaitPos);
            selectedPieceScript_ = piece;
        }
        isCoroutineTaskNow_ = false;

        yield return null;
    }

    private IEnumerator PutPieceToBox(Vector3 putPos, PieceScript piece)
    {
        isCoroutineTaskNow_ = true;
        yield return piece.put(putPos);
        isCoroutineTaskNow_ = false;

        yield return null;
    }

    private IEnumerator PieceTurn(State state, PieceScript piece)
    {
        isCoroutineTaskNow_ = true;
        yield return piece.turnPieceInCurrentPos(state);
        isCoroutineTaskNow_ = false;

        yield return null;
    }

    private IEnumerator PieceTurnNormal(State state, PieceScript piece)
    {
        isCoroutineTaskNow_= true;
        yield return piece.turnPiece(state);
        isCoroutineTaskNow_ = false;

        yield return null;
    }

    // ---------------------------- //
    // --- IPointerClickHandler --- //
    public void OnPointerClick(PointerEventData eventData)
    {
        // --- select a box -> status update --- //
        if (!isUpdatePermission) { return; }
        if (gameState_ != GameState.PlayerTurn && gameState_ != GameState.EnemyTurn) { return; }
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            var box = eventData.pointerCurrentRaycast.gameObject;
            var boxScript = box.GetComponent<GridBoxScript>();
            if (!boxScript) { return; }
            if (!boxScript.IsCanPiecePlace)
            {
                Debug.Log("Cann't place!");
                return;
            }
            selectedGridBoxScript_ = boxScript;
        }

    }

}
