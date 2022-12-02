using UnityEngine;
using TMPro;

public class Cell : MonoBehaviour
{
    // ----- �����o�ϐ� ----- //
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
    private int _mineNum = 0;           // �Z�������̒n����
    public int mineNum { set => _mineNum = value; } 
    private bool _isOpened = false;     // �Z�����J���ꂽ�Ftrue
    public bool isOpened { set => _isOpened = value; }

    // ----- �����o�֐� ----- //
    private void OnValidate()
    {
        // ---- Inspector�X�V���ďo ---- //
        if (!_isOpened) { return; }     // �v���C���[��Open����ĂȂ�
        OnCellStateChanged();
    }

    private void OnCellStateChanged()
    {
        // ---- Cell��Ԃɉ����\�������ω� ---- //
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

