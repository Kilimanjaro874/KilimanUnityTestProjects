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
    private TextMeshProUGUI _timeText = null;       // �o�ߎ��Ԏ��ԃe�L�X�g

    [SerializeField]
    private TextMeshProUGUI _gameClearText = null;  // �Q�[���N���A�e�L�X�g

    [SerializeField]
    private TextMeshProUGUI _gameOverText = null;   // �Q�[���I�[�o�[�e�L�X�g

    private Cell[,] _cells;                 // cell2�����z��p��
    private bool _isFirstToched = false;    // ����n���h�~�p�F�����^�b�`�ڂ̓Q�[���I�[�o�[�ɂȂ�Ȃ�
    private bool _isToched = false;         // �Q�[�����A�Z���N���b�N�̓x : true

    private float _time = 0f;               // �o�ߎ��Ԋi�[
    private bool _isTime = true;            // ���ԃJ�E���g�L��/����
    private bool _isGameClear = false;          // �Q�[���N���A�t���O
    private bool _isGameOver = false;       // �Q�[���I�[�o�[�t���O

    GameObject _activeCell;                 // �v���C���[���I�������Z���Q�[���I�u�W�F�N�g��s�x�i�[
   
    void Start()
    {
        // ----- Minesweeper�X�e�[�W������ ------ //
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
                cell.row = r;        // cell��row�Z�b�g
                cell.col = c;        // cell��col�Z�b�g
                _cells[r,c] = cell;
            }
        }
        // ----- �Q�[���I�[�o�[ / �N���A�e�L�X�g��\�� ----- //
        _gameClearText.gameObject.SetActive(false);
        _gameOverText.gameObject.SetActive(false);

    }

    private void Update()
    {
        // ----- �T. �ʏ폈�� ----- //
        if (_isGameOver)    // �Q�[���I�[�o�[����
        {
            _gameOverText.gameObject.SetActive(true);
            return;
        }
        if (_isGameClear)   // �Q�[���N���A����
        {
            _gameClearText.gameObject.SetActive(true);
            return;
        }
        TimeTextUpdate(Time.deltaTime, ref _timeText, _isTime); // �Q�[�����ԊǗ�


        // ----- �U. Cell�̏��� ----- //
        if (!_isToched) { return; }     // �Z���N���b�N�����F�I��
        _isToched = false;              // �t���O��܂�
        // ---- 0. �A�N�e�B�u�ȃZ�����Q��(OnPointerClick�Ŏ擾�ς�) ---- //
        var cell = _activeCell;                             // �A�N�e�B�u�ȃZ���Q�[���I�u�W�F�N�g�擾
        var scr = cell.GetComponent<Cell>();                // �X�N���v�g�Q��
        // ---- 1. ���߂ăv���C���[���Z���ɐG�������̏��� ---- //
        if (!_isFirstToched)
        {
            _isFirstToched = true;                          // �t���O��܂�
            SetMine(ref _cells, scr.row, scr.col);          // �n�����Z�b�g
            SetCellState(ref _cells);                       // �Z���̏�Ԃ��X�V
        }
        // ---- 2. Cell��ԍX�V��̔��菈�� ---- //
        scr.isOpened = true;                                // �I�[�v���ς݂Ƃ��ăt���O��ݒ�
        scr.oneFlameActive = true;                          // 1�t���[���Ԃ����Z���̊O�ς��X�V����֐����s
        // --- 2.1 �Q�[���I�[�o�[���� --- //
        _isGameOver = GameOverCheck(ref scr);
        // --- 2.2 �N���A���� --- //
        _isGameClear = GameClearCheck(ref _cells);
        // --- 2.3 �����W�J���� --- //
        OpenCells(ref _cells, scr.row, scr.col);
    }

    private bool GameOverCheck(ref Cell cell)
    {
        // ---- �Q�[���I�[�o�[���� ---- //
        if(cell.cellState == G_Manager.CellState.Mine) { return true; }
        return false;
    }

    private bool GameClearCheck(ref Cell[,] cells)
    {
        // ---- �Q�[���N���A���� ----- //
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
        // ---- �����Ȃ��Z���ד��m�̎����W�J ---- //
        // --- �Z�������W�J���� --- //
        if(1 <= (int)cells[row, col].cellState || 8 >= (int) cells[row, col].cellState)
        {
            // �Z���������������ꍇ�͂����܂łŎ����W�J�X�g�b�v
            cells[row, col].isOpened = true;
            cells[row, col].oneFlameActive = true;
        }
        if(cells[row, col].cellState != G_Manager.CellState.None) { return; }   // �Z�����󔒁F�ċA�𑱂���
        cells[row, col].isOpened = true;
        cells[row, col].oneFlameActive = true;
        // --- �ċA���� --- //
        // -- �z��T�C�Y�`�F�b�N -- //
        for(int d_r = -1; d_r <= 1; d_r++)
        {
            for(int d_c = -1; d_c <= 1; d_c++)
            {
                int tmp_r = row + d_r;
                int tmp_c = col + d_c;
                if (row == tmp_r && col == tmp_c) { continue; }
                if (tmp_r < 0 || tmp_r >= cells.GetLength(0)) { continue; }
                if (tmp_c < 0 || tmp_c >= cells.GetLength(1)) { continue; }
                // �܂��J���ĂȂ��Z���ɑ΂��A�ċA�I�ɖ{�֐����s
                if(!cells[tmp_r, tmp_c].isOpened) { OpenCells(ref cells, tmp_r, tmp_c); }
            }
        }
        return;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // ----- UI��̃Q�[���I�u�W�F�N�g�N���b�N���ɔ��� ----- //
        // ---- 1. ���N���b�N ---- //
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            var cell = eventData.pointerCurrentRaycast.gameObject;
            if (cell.GetComponent<Cell>() == null) { return; }   // �N���b�N�I�u�W�F�N�g���Z���ȊO�Freturn
            var scr = cell.GetComponent<Cell>();                // �X�N���v�g�擾
            if (scr.isOpened == true) { return; }               // ���ɃI�[�v���ς݁Freturn
            _activeCell = cell;                                 // �A�N�e�B�u�ȃZ���Ƃ��ăZ�b�g
            _isToched = true;                                   // �Z���N���b�N�t���Otrue;
        }
        //---- 2. �E�N���b�N ---- //
        else if(eventData.button == PointerEventData.InputButton.Right)
        {
            var cell = eventData.pointerCurrentRaycast.gameObject;
            if (cell.GetComponent<Cell>() == null) { return; }   // �N���b�N�I�u�W�F�N�g���Z���ȊO�Freturn
            var scr = cell.GetComponent<Cell>();                // �X�N���v�g�擾
            if (scr.isOpened == true) { return; }               // ���ɃI�[�v���ς݁Freturn
            scr.optionState++;                                  // ����H�}�[�N�ɕύX
            if(scr.optionState >= G_Manager.OptionState.End) { scr.optionState = G_Manager.OptionState.None; }
        }
    }

    void SetMine(ref Cell[,] cells, int void_row, int void_col)
    {
        // ----- �n���̏�Ԃ��Z���ɗ^����(void_row, void_col�ȊO�̃Z��) ----- //
        // ---- Cell�̐����n���������F�G���[ ---- //
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
        // ---- Cell�̏�Ԃ��X�V���� ---- //
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
        // ----- Cell�����ɑ��݂���n������Ԃ��B�n���̏ꍇ��-1��Ԃ� ----- //
        if(cells[row, col] == null) { Debug.Log("Error!"); }      // �G���[
        if(cells[row, col].cellState == G_Manager.CellState.Mine) { return -1; }    // �n���̏ꍇ
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
        // ----- ���Ԃ��J�E���g�����ԃe�L�X�g�X�V�\�� ----- //
        if (!active) { return; }
        _time += delta_time;
        int min = (int)(_time / 60f);
        int sec = (int)(_time - min * 60);
        string time = string.Format("{0:00}:{1:00}",min,sec);
        t_text.text = "Time : " + time;
    }

    public void OpenAnswer(){
        // ----- �}�C���X�C�[�p�[�̓����\���F�{�^������\�� ----- //
        for(int r = 0; r < _cells.GetLength(0); r++)
        {
            for(int c = 0; c < _cells.GetLength(1); c++)
            {
                _cells[r, c].isOpened = true;
                _cells[r,c].oneFlameActive = true;
            }
        }
        _isToched = true;   // ���f
    }
}