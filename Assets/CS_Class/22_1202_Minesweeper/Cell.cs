using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Cell : MonoBehaviour
{
    // ----- �����o�ϐ� ----- //
    [SerializeField]
    private G_Manager.CellState _cellState = G_Manager.CellState.None;          // �n�����Q�[���N���A�ɂ܂����
    public G_Manager.CellState cellState
    {
        get => _cellState;
        set
        {
            _cellState = value;
            OnCellStateChanged();
        }
    }
    private G_Manager.OptionState _optionState = G_Manager.OptionState.None;    // �������o�I�ȏ��
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
    private bool _isOpened = false;         // �Z�����J���ꂽ�Ftrue
    public bool isOpened { get => _isOpened; set => _isOpened = value; }
    private bool _oneFlameActive = false;   // 1�t���[���̂�Update��L�����Ftrue
    public bool oneFlameActive { set => _oneFlameActive = value; }
   

    // ----- �����o�֐� ----- //
    private void OnValidate()
    {
        // ---- Inspector�X�V���ďo ---- //
        // Scene����Z���̏�ԕω��Ƃ���View���Q�Ƃ��邽�ߗp��
        OnCellStateChanged();
    }

    private void Update()
    {
        // ---- �A�b�v�f�[�g���� ---- //

        // ---- 1�t���[���Ԃ̂ݗL�� ---- //
        if (!_oneFlameActive) { return; }
        _oneFlameActive = false;
        OnCellStateChanged();
    }

    private void OnCellStateChanged()
    {
        // ---- Cell��Ԃɉ����F, �\�������ω� ---- //
        if (_view == null) { return; }

        // --- �������� --- //
        // �I�[�v�����ĂȂ��ꍇ
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
        // �I�[�v���ς݂̏ꍇ
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

