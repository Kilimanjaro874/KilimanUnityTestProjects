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

    private Cell[,] _cells;                 // cell2�����z��p��
    private bool _isFirstToched = false;    // ����n���h�~�p�F�����^�b�`�ڂ̓Q�[���I�[�o�[�ɂȂ�Ȃ�
    private bool _isToched = false;         // �Q�[�����A�Z���N���b�N�̓x : true
   
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
        if (!_isToched) { return; }     // �Z���N���b�N�����F�I��

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // ----- UI��̃Q�[���I�u�W�F�N�g�N���b�N���ɔ��� ----- //
        var cell = eventData.pointerCurrentRaycast.gameObject;
        if(cell.GetComponent<Cell>() == null) { return; }   // �N���b�N�I�u�W�F�N�g���Z���ȊO�Freturn
        var scr = cell.GetComponent<Cell>();
        _isToched = true;                                   // �Z���N���b�N�t���Otrue;
        // ---- �n�߂ăN���b�N�������n�߂Ēn�����}�X�ɃZ�b�g���� ----- //
        if (_isFirstToched) { return; }
        SetMine(ref _cells, scr.row, scr.col);              // �N���b�N�����n�_�ȊO�̃Z���ɒn����Ԃ�^����
        _isFirstToched = true;                              // �n�߂ăN���b�N�������̃t���O�����グ
    }


    void SetMine(ref Cell[,] cells, int void_row, int void_col)
    {
        // ----- �n���̏�Ԃ��Z���ɗ^����(void_row, void_col�ȊO�̃Z��) ----- //
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
        // ----- Cell���ӂɑ��݂���n������SCell�\�� ----- //
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
}