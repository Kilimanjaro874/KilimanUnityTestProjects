using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Cell : MonoBehaviour
{
    // ----- メンバ変数 ----- //
    [SerializeField]
    private G_Manager.CellState _cellState = G_Manager.CellState.None;          // 地雷等ゲームクリアにまつわる状態
    public G_Manager.CellState cellState
    {
        get => _cellState;
        set
        {
            _cellState = value;
            OnCellStateChanged();
        }
    }
    private G_Manager.OptionState _optionState = G_Manager.OptionState.None;    // 旗等視覚的な状態
    public G_Manager.OptionState optionState
    {
        get => _optionState;
        set
        {
            _optionState = value;
            OnCellStateChanged();
        }
    }
    [SerializeField]
    private TextMeshProUGUI _view = null;
    [SerializeField]
    private Color _onColor = Color.white;
    [SerializeField]
    private Color _offColor = Color.blue;
    [SerializeField]
    private Color _flagColor = Color.yellow;
    [SerializeField]
    private Color _questionColor = Color.green;
    [SerializeField]
    private Image _cellImage = null;
    
    private int _row;
    public int row { get => _row; set { _row = value; } }
    private int _col;
    public int col { get => _col; set { _col = value; } }
    private bool _isOpened = false;         // セルが開かれた：true
    public bool isOpened { get => _isOpened; set => _isOpened = value; }
    private bool _oneFlameActive = false;   // 1フレームのみUpdateを有効化：true
    public bool oneFlameActive { set => _oneFlameActive = value; }
   

    // ----- メンバ関数 ----- //
    private void OnValidate()
    {
        // ---- Inspector更新時呼出 ---- //
        // Sceneからセルの状態変化とそのViewを参照するため用意
        OnCellStateChanged();
    }

    private void Update()
    {
        // ---- アップデート処理 ---- //

        // ---- 1フレーム間のみ有効 ---- //
        if (!_oneFlameActive) { return; }
        _oneFlameActive = false;
        OnCellStateChanged();
    }

    private void OnCellStateChanged()
    {
        // ---- Cell状態に応じ色, 表示文字変化 ---- //
        if (_view == null) { return; }

        // --- 条件分岐 --- //
        // オープンしてない場合
        if (!_isOpened)
        {
            _cellImage.color = _offColor;
            if(_optionState == G_Manager.OptionState.None)
            {
                _view.text = "";
            }
            else if(_optionState == G_Manager.OptionState.Flag)
            {
                _view.text = "!";
                _view.color = Color.red;
                _cellImage.color = _flagColor;
            }
            else if (_optionState == G_Manager.OptionState.Question)
            {
                _view.text = "?";
                _view.color = Color.black;
                _cellImage.color = _questionColor;
            }
            return;
        }
        // オープン済みの場合
        if (_isOpened)
        {
            _cellImage.color = _onColor;
            if (_cellState == G_Manager.CellState.None)
            {
                _view.text = "";
                if (_cellImage == null) { Debug.Log("null : cell image"); return; }

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
    
   
}

