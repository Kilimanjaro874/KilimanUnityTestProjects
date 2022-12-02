using UnityEngine;
using TMPro;

public class Cell : MonoBehaviour
{
    // ----- メンバ変数 ----- //
    [SerializeField]
    private G_Manager.CellState _cellState = G_Manager.CellState.None;
    public G_Manager.CellState cellState
    {
        get => _cellState;
        set
        {
            _cellState = value;
            OnCellStateChanged();
        }
    }
    [SerializeField]
    private TextMeshProUGUI _view = null;

    private int _row;
    public int row { get => _row; set { _row = value; } }
    private int _col;
    public int col { get => _col; set { _col = value; } }
    private int _mineNum = 0;           // セル八方の地雷数
    public int mineNum { set => _mineNum = value; } 
    private bool _isOpened = false;     // セルが開かれた：true
    public bool isOpened { set => _isOpened = value; }

    // ----- メンバ関数 ----- //
    private void OnValidate()
    {
        // ---- Inspector更新時呼出 ---- //
        if (!_isOpened) { return; }     // プレイヤーにOpenされてない
        OnCellStateChanged();
    }

    private void OnCellStateChanged()
    {
        // ---- Cell状態に応じ表示文字変化 ---- //
        if (_view == null) { return; }

        if (_cellState == G_Manager.CellState.None)
        {
            _view.text = "";
        }
        else if (_cellState == G_Manager.CellState.Mine)
        {
            _view.text = "X";
            _view.color = Color.red;
        }
        else
        {
            _view.text = ((int)_cellState).ToString();
            _view.color = Color.blue;
        }
    }

   
}

