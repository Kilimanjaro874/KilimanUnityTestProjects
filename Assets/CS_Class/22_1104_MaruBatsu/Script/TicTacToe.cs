using UnityEngine;
using UnityEngine.UI;

public class TicTacToe : MonoBehaviour
{
    // --- 五目並べ等でも通用するよう、一般性を持たせて開発した --- //
    [SerializeField]
    private const int _Size = 5;
    //private GameObject[,] _cells;     // 構造体で管理するよう変更
    [SerializeField]
    private const int _lineUpNum = 4;   // 駒を本変数以上に並べたとき：勝利

    [SerializeField]
    private Color _nomalCell = Color.white;
    [SerializeField]
    private Color _selectedCell = Color.cyan;

    private int _selectedRow;
    private int _selectedColumn;

    [SerializeField]
    private Sprite _circle = null;

    [SerializeField]
    private Sprite _cross = null;

    enum turn
    {
        // 先攻、後攻を列挙型で管理  
        _circleTurn, _crossTurn, end                    
    }
    [SerializeField, Tooltip("先行ターンを指定")]
    private turn _turn = turn._circleTurn;

    enum state
    {
        // 駒の状態(無し、〇、×)を列挙型で管理
        _blank, _circle, _cross
    }

    struct cellAndState
    {
        // マスを管理するゲームオブジェクト＆駒の状態(列挙型)を管理
        public GameObject _cell;
        public state _state;
    }
    private cellAndState[,] _cellSt;    

    struct scan_dir
    {
        // 駒を置き、その八方に同じ駒がいくつ存在するか、走査する方向を定義しておく構造体
        public Vector2 _dir1;
        public Vector2 _dir2;
    }
    private scan_dir[] _scanDir;


    private void Start()
    {
        // --- マス目の数変更に対応 --- //
        var glg = GetComponent<GridLayoutGroup>();
        glg.constraintCount = _Size;
        // --- デフォルト --- //
        _cellSt = new cellAndState[_Size, _Size];
        for (var r = 0; r < _cellSt.GetLength(0); r++)
        {
            for (var c = 0; c < _cellSt.GetLength(1); c++)
            {
                var cell = new GameObject($"Cell({r},{c})");
                cell.transform.parent = transform;
                cell.AddComponent<Image>();
                _cellSt[r, c]._cell = cell;
                _cellSt[r, c]._state = state._blank;    // 空の状態を登録
            }
        }
        // --- 駒の状態を走査する構造体を定義しておく --- //
        // もっと簡単に記述できる方法が知りたい...
        _scanDir = new scan_dir[4];         
        _scanDir[0]._dir1 = new Vector2(0, -1);     // 横方向走査
        _scanDir[0]._dir2 = new Vector2(0, 1);
        _scanDir[1]._dir1 = new Vector2(-1, 0);     // 縦方向走査
        _scanDir[1]._dir2 = new Vector2(1, 0);
        _scanDir[2]._dir1 = new Vector2(-1, 1);     // 右斜め上、左斜め下走査
        _scanDir[2]._dir2 = new Vector2(1, -1);
        _scanDir[3]._dir1 = new Vector2(-1, -1);    // 左斜め上、右斜め下走査
        _scanDir[3]._dir2 = new Vector2(1, 1);      

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { _selectedColumn--; }
        if (Input.GetKeyDown(KeyCode.RightArrow)) { _selectedColumn++; }
        if (Input.GetKeyDown(KeyCode.UpArrow)) { _selectedRow--; }
        if (Input.GetKeyDown(KeyCode.DownArrow)) { _selectedRow++; }

        if (_selectedColumn < 0) { _selectedColumn = 0; }
        if (_selectedColumn >= _Size) { _selectedColumn = _Size - 1; }
        if (_selectedRow < 0) { _selectedRow = 0; }
        if (_selectedRow >= _Size) { _selectedRow = _Size - 1; }

        for (var r = 0; r < _cellSt.GetLength(0); r++)
        {
            for (var c = 0; c < _cellSt.GetLength(1); c++)
            {
                var cell = _cellSt[r, c]._cell;
                var image = cell.GetComponent<Image>();
                image.color =
                    (r == _selectedRow && c == _selectedColumn) ? _selectedCell : _nomalCell;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            var cell = _cellSt[_selectedRow, _selectedColumn]._cell;
            var image = cell.GetComponent<Image>();
            // --- ターンに応じて表示画像変更 --- //
            if (turn._circleTurn == _turn) {
                image.sprite = _circle;
                _cellSt[_selectedRow, _selectedColumn]._state = state._circle;
            }
            else if(turn._crossTurn == _turn) {
                image.sprite = _cross; 
                _cellSt[_selectedRow, _selectedColumn]._state = state._cross;
            }
            // --- マークが揃っているかの判定＆処理 --- //
            if(CheckBoxes(ref _cellSt, _selectedRow, _selectedColumn, _turn)){
                // -- 揃った時の処理 -- //
                Debug.Log("Complete!!!!, Win : " + _turn);
            } else
            {
                // -- 揃って無い時の処理 -- //
                Debug.Log("not Complete");
            }
            // --- ターンの管理 --- //
            _turn++;
            if(_turn == turn.end) { _turn = turn._circleTurn; }
        }
    }

    private bool CheckBoxes(ref cellAndState[,] cs, int s_row, int s_col, turn tu)
    {
        // --- 盤面が 〇 or × で三連取っているかの判定結果を返す --- //
        state check_state = state._circle;
        if (turn._crossTurn == tu) { check_state = state._cross; }
        // -- 八方に走査開始 --
        for (var i = 0; i < _scanDir.GetLength(0); i++)
        {
            // -- 置いた駒が、移動先（Vector2)の駒と揃っているかを再帰的にカウント --
            int count1 = 0;
            int count2 = 0;
            ScanBoxes(ref cs, ref count1, s_row, s_col, ref check_state, ref _scanDir[i]._dir1);
            ScanBoxes(ref cs, ref count2, s_row, s_col, ref check_state, ref _scanDir[i]._dir2);
            if (1 + count1 + count2 >= _lineUpNum)    // 自身の駒含めて、_lineUpNum以上並べていればtrue
            {
                return true;
            }
        }

        return false;
    }

    private void ScanBoxes(ref cellAndState[,] cs, ref int count, int s_row, int s_col, ref state st, ref Vector2 dir)
    {   // --- 盤面csの状態を走査、再帰的に使用 --- //
        int scan_row = s_row + (int)dir.x;
        int scan_col = s_col + (int)dir.y;

        if(scan_row < 0) { return; }
        if(scan_col < 0) { return; }
        if(scan_row >= _Size) { return; }
        if(scan_col >= _Size) { return; }

        if(cs[scan_row, scan_col]._cell)
        {
            if(st == cs[scan_row, scan_col]._state)
            {
                count++;
                ScanBoxes(ref cs, ref count, scan_row, scan_col, ref st, ref dir);
            }
        } else { return;}
    }
}
