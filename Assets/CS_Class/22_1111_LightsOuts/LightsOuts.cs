using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LightsOuts : MonoBehaviour, IPointerClickHandler
{
    [SerializeField, Tooltip("GridLayoutGroup : Fixed Row Count")]
    private int _row = 5;
    [SerializeField]
    private int _col = 6;
    [SerializeField]
    private Color _on_color = Color.white;
    [SerializeField]
    private Color _off_color = Color.black;
    private int _num_of_move = 0;   // �萔���L�^

    private struct Cellst
    {   // Cellst : �Q�Ƃ��\���̂Ŏ���
        public GameObject _cell;
        public CellScript _scr;
        public Image _img;
    }
    Cellst[,] _cellSt;

    private void Start()
    {
        GetComponent<GridLayoutGroup>().constraintCount = _row;
        _cellSt = new Cellst[_row, _col];

        for (var r = 0; r < _row; r++)
        {
            for (var c = 0; c < _col; c++)
            {
                var cell = new GameObject($"Cell({r}, {c})");
                cell.transform.parent = transform;
                var col = cell.AddComponent<Image>();
                var scr = cell.AddComponent<CellScript>();        // Cell�p�̃X�N���v�g�A�^�b�`
                // --- Cell�Ǘ��\���̂ɃI�u�W�F�N�g���X�N���v�g��� --- //
                _cellSt[r, c]._cell = cell;
                _cellSt[r, c]._scr = scr;
                _cellSt[r, c]._img = col;
                // --- �Ֆʂ̏�����Ԍ��� --- //
                scr.Row = r;                // �s���
                scr.Col = c;                // ����
            }
        }
        // --- �����_������ --- //
        RandCellMake();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var cell = eventData.pointerCurrentRaycast.gameObject;
        // --- �Q�[���I�u�W�F�N�g��Cell�X�N���v�g�������Ă邩�m�F --- //
        if(cell.gameObject.GetComponent<CellScript>()== null) { return; }
        // --- �萔�̋L�^ --- //
        _num_of_move++;
        // --- cell�̏�Ԏ擾 --- //
        var scr = cell.gameObject.GetComponent<CellScript>();
        var s_row = scr.Row;
        var s_col = scr.Col;
        // --- cell�̏�ԕύX --- //
        scr.LightOn = !scr.LightOn;                             // on / off�؂�ւ�
        var image = cell.GetComponent<Image>();
        image.color = scr.LightOn ? _on_color : _off_color;     // �摜�̐F�ύX 
        // --- ���̋�̏�Ԃ��ω������� --- //
        if(CellStateChangeAndCheck(s_row, s_col))
        {
            // -- Cell���S�Ĕ��̎��̏��� -- //
            Debug.Log("Complete");
            Debug.Log("Clear time(s) = " + Time.time.ToString());
            Debug.Log("Num of move = " + _num_of_move.ToString());
        }

    }

    bool CellStateChangeAndCheck(int s_row, int s_col)
    {
        // ---- s_row, s_col�őI������Cell�̏㉺���E�̏�Ԃ�ω� ---- //
        int black_num = 0;
        for(var r = 0; r < _cellSt.GetLength(0); r++)
        {
            for(var c = 0; c < _cellSt.GetLength(1); c++)
            {
                // --- ��A���A���A�E��Cell�m�F --- //
                if(r == s_row - 1 && c == s_col){ CellLightChange(r, c); }
                if(r == s_row + 1 && c == s_col){ CellLightChange(r, c); }
                if(c == s_col - 1 && r == s_row){ CellLightChange(r, c); }
                if(c == s_col + 1 && r == s_row){ CellLightChange(r, c); }
                // --- ��̏�Ԃ��m�F --- //
                if (!_cellSt[r, c]._scr.LightOn) { black_num++; }
            }
        }
        if(black_num == 0) { return true; }     // Cell���S�Ĕ��I
        return false;
    }

    void CellLightChange(int row, int col)
    {
        // ---- �I������Cell�̏�Ԃ�ω������� ---- //
        if(row > _cellSt.GetLength(0)) { return; }
        if(col > _cellSt.GetLength(1)) { return; }
        var scr = _cellSt[row, col]._scr;
        var img = _cellSt[row, col]._img;
        scr.LightOn = !scr.LightOn;
        img.color = scr.LightOn ? _on_color : _off_color;     // �摜�̐F�ύX 
    }

    void RandCellMake()
    {
        for (var r = 0; r < _cellSt.GetLength(0); r++)
        {
            for (var c = 0; c < _cellSt.GetLength(1); c++)
            {
                // -- �ȉ��̈ꔭ�ŃN���A�o���Ȃ��悤�Ɏ��s���鏈���̃e�X�g -- //
                // -> �����đS�Ă�Cell���A���������̔��ɕύX���� 
                _cellSt[r,c]._scr.LightOn = true;           // LightOn = true
                _cellSt[r, c]._img.color = _on_color;       // ��
                int tempRand = Random.Range((int)0, (int)1);
                //if(tempRand == 0)
                //{
                //    _cellSt[r, c]._scr.LightOn = false;             // LightOn = false
                //    _cellSt[r, c]._img.color = _off_color;          // ��
                //}
            }
        }
        // --- �ꔭ�ŃN���A�ł��Ȃ��p�ɑ������s�� --- //
        // ���Ȃ��Ƃ��A�u�����\���ő��݂���ꏊ���ꂩ���������v�Ȃ���v
        // ���̕���������΁A���̓�1~2�ӏ������h��Ԃ�
        for (var r = 0; r < _cellSt.GetLength(0); r++)
        {
            int count_white = 0;
            for (var c = 0; c < _cellSt.GetLength(1); c++)
            {
                if(_cellSt[r, c]._scr.LightOn) { count_white++; }
                else { count_white = 0; }
                if(count_white >= 2)
                {
                    // --- 2�������ɘA���F�\�������݂��Ȃ����`�F�b�N���Ă��� --- //
                    if(c + 1 >= _cellSt.GetLength(1)) { continue; }     // �E�͔͈͊O�AOK
                    if(!_cellSt[r,c + 1]._scr.LightOn) { continue; }    // �E�͍��AOK 
                    if (r - 1 < 0) { continue; }                        // ��͔͈͊O�AOK
                    if(!_cellSt[r-1, c]._scr.LightOn) { continue; }     // ��͍��AOK
                    if(r + 1 >= _cellSt.GetLength(0)) { continue; }     // ���͔͈͊O�AOK
                    if(!_cellSt[r+1, c]._scr.LightOn) { continue; }     // ���͍��@OK
                    // -- ���\�����������ꂽ���̏��� -- //
                    int tmp_roop = Random.Range((int)1, (int)3);
                    for (int i = 0; i < tmp_roop; i++)
                    {
                        // - ���\���̓��A���������ɕύX - //
                        int tmp_r = Random.Range((int)-1, (int)1);
                        int tmp_c = Random.Range((int)-1, (int)1);
                        _cellSt[r + tmp_r, c + tmp_c]._scr.LightOn = false;
                        _cellSt[r + tmp_r, c + tmp_c]._img.color = _off_color;
                    }

                }
            }
        }

    }
}
