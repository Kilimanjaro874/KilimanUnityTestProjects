using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Minesweeper : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private int _rows = 5;

    [SerializeField]
    private int _columns = 5;

    [SerializeField]
    private int _mineCount = 5;

    [SerializeField]
    private GridLayoutGroup _gridLayoutGroup = null;

    [SerializeField]
    private Cell _cellPrefab = null;

    [SerializeField]
    private TextMeshProUGUI _timeText = null;       // 経過時間時間テキスト

    [SerializeField]
    private TextMeshProUGUI _gameClearText = null;  // ゲームクリアテキスト

    [SerializeField]
    private TextMeshProUGUI _gameOverText = null;   // ゲームオーバーテキスト

    private Cell[,] _cells;                 // cell2次元配列用意
    private bool _isFirstToched = false;    // 初手地雷防止用：ワンタッチ目はゲームオーバーにならない
    private bool _isToched = false;         // ゲーム中、セルクリックの度 : true

    private float _time = 0f;               // 経過時間格納
    private bool _isTime = true;            // 時間カウント有効/無効
    private bool _isGameClear = false;          // ゲームクリアフラグ
    private bool _isGameOver = false;       // ゲームオーバーフラグ

    GameObject _activeCell;                 // プレイヤーが選択したセルゲームオブジェクトを都度格納
   
    void Start()
    {
        // ----- Minesweeperステージ初期化 ------ //
        _gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _gridLayoutGroup.constraintCount = _columns;

        _cells = new Cell[_rows, _columns];
        var parent = _gridLayoutGroup.gameObject.transform;
        for (var r = 0; r < _rows; r++)
        {
            for (var c = 0; c < _columns; c++)
            {
                var cell = Instantiate(_cellPrefab);
                cell.transform.SetParent(parent);
                cell.row = r;        // cellにrowセット
                cell.col = c;        // cellにcolセット
                _cells[r,c] = cell;
            }
        }
        // ----- ゲームオーバー / クリアテキスト非表示 ----- //
        _gameClearText.gameObject.SetActive(false);
        _gameOverText.gameObject.SetActive(false);

    }

    private void Update()
    {
        // ----- Ⅰ. 通常処理 ----- //
        if (_isGameOver)    // ゲームオーバー処理
        {
            _gameOverText.gameObject.SetActive(true);
            return;
        }
        if (_isGameClear)   // ゲームクリア処理
        {
            _gameClearText.gameObject.SetActive(true);
            return;
        }
        TimeTextUpdate(Time.deltaTime, ref _timeText, _isTime); // ゲーム時間管理


        // ----- Ⅱ. Cellの処理 ----- //
        if (!_isToched) { return; }     // セルクリック無し：終了
        _isToched = false;              // フラグを折る
        // ---- 0. アクティブなセルを参照(OnPointerClickで取得済み) ---- //
        var cell = _activeCell;                             // アクティブなセルゲームオブジェクト取得
        var scr = cell.GetComponent<Cell>();                // スクリプト参照
        // ---- 1. 初めてプレイヤーがセルに触った時の処理 ---- //
        if (!_isFirstToched)
        {
            _isFirstToched = true;                          // フラグを折る
            SetMine(ref _cells, scr.row, scr.col);          // 地雷をセット
            SetCellState(ref _cells);                       // セルの状態を更新
        }
        // ---- 2. Cell状態更新後の判定処理 ---- //
        scr.isOpened = true;                                // オープン済みとしてフラグを設定
        scr.oneFlameActive = true;                          // 1フレーム間だけセルの外観を更新する関数実行
        // --- 2.1 ゲームオーバー判定 --- //
        _isGameOver = GameOverCheck(ref scr);
        // --- 2.2 クリア判定 --- //
        _isGameClear = GameClearCheck(ref _cells);
        // --- 2.3 自動展開処理 --- //
        OpenCells(ref _cells, scr.row, scr.col);
    }

    private bool GameOverCheck(ref Cell cell)
    {
        // ---- ゲームオーバー判定 ---- //
        if(cell.cellState == G_Manager.CellState.Mine) { return true; }
        return false;
    }

    private bool GameClearCheck(ref Cell[,] cells)
    {
        // ---- ゲームクリア判定 ----- //
        int dugNum = 0;
        for(var r = 0; r < cells.GetLength(0); r++)
        {
            for(var c = 0; c < cells.GetLength(1); c++)
            {
                if(cells[r, c].isOpened) { dugNum++; }
            }
        }
        if(dugNum == cells.GetLength(0) * cells.GetLength(1) - _mineCount) { return true; }
        return false;
    }

    private void OpenCells(ref Cell[,] cells, int row, int col)
    {
        // ---- 何もないセル隣同士の自動展開 ---- //
        // --- セル自動展開処理 --- //
        if(1 <= (int)cells[row, col].cellState || 8 >= (int) cells[row, col].cellState)
        {
            // セルが数字だった場合はそこまでで自動展開ストップ
            cells[row, col].isOpened = true;
            cells[row, col].oneFlameActive = true;
        }
        if(cells[row, col].cellState != G_Manager.CellState.None) { return; }   // セルが空白：再帰を続ける
        cells[row, col].isOpened = true;
        cells[row, col].oneFlameActive = true;
        // --- 再帰処理 --- //
        // -- 配列サイズチェック -- //
        for(int d_r = -1; d_r <= 1; d_r++)
        {
            for(int d_c = -1; d_c <= 1; d_c++)
            {
                int tmp_r = row + d_r;
                int tmp_c = col + d_c;
                if (row == tmp_r && col == tmp_c) { continue; }
                if (tmp_r < 0 || tmp_r >= cells.GetLength(0)) { continue; }
                if (tmp_c < 0 || tmp_c >= cells.GetLength(1)) { continue; }
                // まだ開いてないセルに対し、再帰的に本関数実行
                if(!cells[tmp_r, tmp_c].isOpened) { OpenCells(ref cells, tmp_r, tmp_c); }
            }
        }
        return;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // ----- UI上のゲームオブジェクトクリック時に発動 ----- //
        // ---- 1. 左クリック ---- //
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            var cell = eventData.pointerCurrentRaycast.gameObject;
            if (cell.GetComponent<Cell>() == null) { return; }   // クリックオブジェクトがセル以外：return
            var scr = cell.GetComponent<Cell>();                // スクリプト取得
            if (scr.isOpened == true) { return; }               // 既にオープン済み：return
            _activeCell = cell;                                 // アクティブなセルとしてセット
            _isToched = true;                                   // セルクリックフラグtrue;
        }
        //---- 2. 右クリック ---- //
        else if(eventData.button == PointerEventData.InputButton.Right)
        {
            var cell = eventData.pointerCurrentRaycast.gameObject;
            if (cell.GetComponent<Cell>() == null) { return; }   // クリックオブジェクトがセル以外：return
            var scr = cell.GetComponent<Cell>();                // スクリプト取得
            if (scr.isOpened == true) { return; }               // 既にオープン済み：return
            scr.optionState++;                                  // 旗や？マークに変更
            if(scr.optionState >= G_Manager.OptionState.End) { scr.optionState = G_Manager.OptionState.None; }
        }
    }

    void SetMine(ref Cell[,] cells, int void_row, int void_col)
    {
        // ----- 地雷の状態をセルに与える(void_row, void_col以外のセル) ----- //
        // ---- Cellの数より地雷が多い：エラー ---- //
        if(_mineCount >= cells.GetLength(0) * cells.GetLength(1)) { Debug.Log("Error"); return; } 
        for(var i = 0; i < _mineCount; i++)
        {
            int r = 0; 
            int c = 0;
            while (true)
            {
                r = Random.Range(0, cells.GetLength(0));
                c = Random.Range(0, cells.GetLength(1));
                if(r == void_row && c == void_col) { continue; }
                var cell = _cells[r, c];
                if(cell.cellState == G_Manager.CellState.Mine) { continue; }
                cell.cellState = G_Manager.CellState.Mine;
                break;
            }
        }
    }

    void SetCellState(ref Cell[,] cells)
    {
        // ---- Cellの状態を更新する ---- //
        for(var r = 0; r < cells.GetLength(0); r++)
        {
            for(var c = 0; c < cells.GetLength(1); c++)
            {
                cells[r, c].cellState = (G_Manager.CellState)SerchMineNum(ref cells, r, c);
            }
        }
    }

    int SerchMineNum(ref Cell[,] cells, int row, int col)
    {
        // ----- Cell八方に存在する地雷数を返す。地雷の場合は-1を返す ----- //
        if(cells[row, col] == null) { Debug.Log("Error!"); }      // エラー
        if(cells[row, col].cellState == G_Manager.CellState.Mine) { return -1; }    // 地雷の場合
        int mineNum = 0 ;
        for(int d_r = -1; d_r <= 1; d_r++)
        {
            for(int d_c = -1; d_c <= 1; d_c++)
            {
                int tmp_r = row + d_r;
                int tmp_c = col + d_c;
                if (tmp_r < 0 || tmp_r >= cells.GetLength(0)) { continue; }
                if (tmp_c < 0 || tmp_c >= cells.GetLength(1)) { continue; }
                var cell = cells[tmp_r, tmp_c];
                if(cell.cellState == G_Manager.CellState.Mine) { mineNum++; }
            }
        }
        return mineNum;
    }

    void TimeTextUpdate(float delta_time, ref TextMeshProUGUI t_text, bool active)
    {
        // ----- 時間をカウント＆時間テキスト更新表示 ----- //
        if (!active) { return; }
        _time += delta_time;
        int min = (int)(_time / 60f);
        int sec = (int)(_time - min * 60);
        string time = string.Format("{0:00}:{1:00}",min,sec);
        t_text.text = "Time : " + time;
    }

    public void OpenAnswer(){
        // ----- マインスイーパーの答え表示：ボタンから表示 ----- //
        for(int r = 0; r < _cells.GetLength(0); r++)
        {
            for(int c = 0; c < _cells.GetLength(1); c++)
            {
                _cells[r, c].isOpened = true;
                _cells[r,c].oneFlameActive = true;
            }
        }
        _isToched = true;   // 反映
    }
}