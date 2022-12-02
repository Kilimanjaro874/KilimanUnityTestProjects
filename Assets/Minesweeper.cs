using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    private Cell[,] _cells;                 // cell2次元配列用意
    private bool _isFirstToched = false;    // 初手地雷防止用：ワンタッチ目はゲームオーバーにならない
    private bool _isToched = false;         // ゲーム中、セルクリックの度 : true
   
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

        //for (var i = 0; i < _mineCount; i++)
        //{
        //    var r = Random.Range(0, _rows -1);
        //    var c = Random.Range(0, _columns - 1);
        //    var cell = _cells[r, c];
        //    cell.cellState = G_Manager.CellState.Mine;
        //}
        //SetMineNum(ref _cells);
    }

    private void Update()
    {
        if (!_isToched) { return; }     // セルクリック無し：終了

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // ----- UI上のゲームオブジェクトクリック時に発動 ----- //
        var cell = eventData.pointerCurrentRaycast.gameObject;
        if(cell.GetComponent<Cell>() == null) { return; }   // クリックオブジェクトがセル以外：return
        var scr = cell.GetComponent<Cell>();
        _isToched = true;                                   // セルクリックフラグtrue;
        // ---- 始めてクリックした時始めて地雷をマスにセットする ----- //
        if (_isFirstToched) { return; }
        SetMine(ref _cells, scr.row, scr.col);              // クリックした地点以外のセルに地雷状態を与える
        _isFirstToched = true;                              // 始めてクリックした時のフラグ立ち上げ
    }


    void SetMine(ref Cell[,] cells, int void_row, int void_col)
    {
        // ----- 地雷の状態をセルに与える(void_row, void_col以外のセル) ----- //
        if(_mineCount > cells.GetLength(0) * cells.GetLength(1)) { Debug.Log("Error"); return; } 
        for(var i = 0; i < _mineCount; i++)
        {
            int r = 0; 
            int c = 0;
            while (true)
            {
                r = Random.Range(0, cells.GetLength(0));
                c = Random.Range(0, cells.GetLength(1));
                if(r == void_row) { continue; }
                if(c == void_col) { continue; }
                var cell = _cells[r, c];
                if(cell.cellState == G_Manager.CellState.Mine) { continue; }
                cell.cellState = G_Manager.CellState.Mine;
                break;
            }
        }
    }


    void SetMineNum(ref Cell[,] cells)
    {
        // ----- Cell周辺に存在する地雷数を全Cell表示 ----- //
        for(var r = 0; r < cells.GetLength(0); r++)
        {
            for(var c = 0; c < cells.GetLength(1); c++)
            {
                 cells[r, c].mineNum = SerchMineNum(ref cells, r, c);
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
}