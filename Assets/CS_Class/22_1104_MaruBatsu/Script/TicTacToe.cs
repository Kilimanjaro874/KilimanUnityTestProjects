using UnityEngine;
using UnityEngine.UI;

public class TicTacToe : MonoBehaviour
{
    // --- �ܖڕ��ד��ł��ʗp����悤�A��ʐ����������ĊJ������ --- //
    [SerializeField]
    private const int _Size = 5;
    //private GameObject[,] _cells;     // �\���̂ŊǗ�����悤�ύX
    [SerializeField]
    private const int _lineUpNum = 4;   // ���{�ϐ��ȏ�ɕ��ׂ��Ƃ��F����

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
        // ��U�A��U��񋓌^�ŊǗ�  
        _circleTurn, _crossTurn, end                    
    }
    [SerializeField, Tooltip("��s�^�[�����w��")]
    private turn _turn = turn._circleTurn;

    enum state
    {
        // ��̏��(�����A�Z�A�~)��񋓌^�ŊǗ�
        _blank, _circle, _cross
    }

    struct cellAndState
    {
        // �}�X���Ǘ�����Q�[���I�u�W�F�N�g����̏��(�񋓌^)���Ǘ�
        public GameObject _cell;
        public state _state;
    }
    private cellAndState[,] _cellSt;    

    struct scan_dir
    {
        // ���u���A���̔����ɓ�����������݂��邩�A��������������`���Ă����\����
        public Vector2 _dir1;
        public Vector2 _dir2;
    }
    private scan_dir[] _scanDir;


    private void Start()
    {
        // --- �}�X�ڂ̐��ύX�ɑΉ� --- //
        var glg = GetComponent<GridLayoutGroup>();
        glg.constraintCount = _Size;
        // --- �f�t�H���g --- //
        _cellSt = new cellAndState[_Size, _Size];
        for (var r = 0; r < _cellSt.GetLength(0); r++)
        {
            for (var c = 0; c < _cellSt.GetLength(1); c++)
            {
                var cell = new GameObject($"Cell({r},{c})");
                cell.transform.parent = transform;
                cell.AddComponent<Image>();
                _cellSt[r, c]._cell = cell;
                _cellSt[r, c]._state = state._blank;    // ��̏�Ԃ�o�^
            }
        }
        // --- ��̏�Ԃ𑖍�����\���̂��`���Ă��� --- //
        // �����ƊȒP�ɋL�q�ł�����@���m�肽��...
        _scanDir = new scan_dir[4];         
        _scanDir[0]._dir1 = new Vector2(0, -1);     // ����������
        _scanDir[0]._dir2 = new Vector2(0, 1);
        _scanDir[1]._dir1 = new Vector2(-1, 0);     // �c��������
        _scanDir[1]._dir2 = new Vector2(1, 0);
        _scanDir[2]._dir1 = new Vector2(-1, 1);     // �E�΂ߏ�A���΂߉�����
        _scanDir[2]._dir2 = new Vector2(1, -1);
        _scanDir[3]._dir1 = new Vector2(-1, -1);    // ���΂ߏ�A�E�΂߉�����
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
            // --- �^�[���ɉ����ĕ\���摜�ύX --- //
            if (turn._circleTurn == _turn) {
                image.sprite = _circle;
                _cellSt[_selectedRow, _selectedColumn]._state = state._circle;
            }
            else if(turn._crossTurn == _turn) {
                image.sprite = _cross; 
                _cellSt[_selectedRow, _selectedColumn]._state = state._cross;
            }
            // --- �}�[�N�������Ă��邩�̔��聕���� --- //
            if(CheckBoxes(ref _cellSt, _selectedRow, _selectedColumn, _turn)){
                // -- ���������̏��� -- //
                Debug.Log("Complete!!!!, Win : " + _turn);
            } else
            {
                // -- �����Ė������̏��� -- //
                Debug.Log("not Complete");
            }
            // --- �^�[���̊Ǘ� --- //
            _turn++;
            if(_turn == turn.end) { _turn = turn._circleTurn; }
        }
    }

    private bool CheckBoxes(ref cellAndState[,] cs, int s_row, int s_col, turn tu)
    {
        // --- �Ֆʂ� �Z or �~ �ŎO�A����Ă��邩�̔��茋�ʂ�Ԃ� --- //
        state check_state = state._circle;
        if (turn._crossTurn == tu) { check_state = state._cross; }
        // -- �����ɑ����J�n --
        for (var i = 0; i < _scanDir.GetLength(0); i++)
        {
            // -- �u������A�ړ���iVector2)�̋�Ƒ����Ă��邩���ċA�I�ɃJ�E���g --
            int count1 = 0;
            int count2 = 0;
            ScanBoxes(ref cs, ref count1, s_row, s_col, ref check_state, ref _scanDir[i]._dir1);
            ScanBoxes(ref cs, ref count2, s_row, s_col, ref check_state, ref _scanDir[i]._dir2);
            if (1 + count1 + count2 >= _lineUpNum)    // ���g�̋�܂߂āA_lineUpNum�ȏ���ׂĂ����true
            {
                return true;
            }
        }

        return false;
    }

    private void ScanBoxes(ref cellAndState[,] cs, ref int count, int s_row, int s_col, ref state st, ref Vector2 dir)
    {   // --- �Ֆ�cs�̏�Ԃ𑖍��A�ċA�I�Ɏg�p --- //
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
