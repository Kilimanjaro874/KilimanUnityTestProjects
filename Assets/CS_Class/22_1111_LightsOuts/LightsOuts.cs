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
    GameObject[,] _cellSt;          // Cell�Q�[���I�u�W�F�N�g�i�[
    private int _num_of_move = 0;   // �萔���L�^

    private void Start()
    {
        GetComponent<GridLayoutGroup>().constraintCount = _row;
        _cellSt = new GameObject[_row, _col];

        for (var r = 0; r < _row; r++)
        {
            for (var c = 0; c < _col; c++)
            {
                var cell = new GameObject($"Cell({r}, {c})");
                cell.transform.parent = transform;
                var col = cell.AddComponent<Image>();
                var scr = cell.AddComponent<CellScript>();        // Cell�p�̃X�N���v�g�A�^�b�`
                // --- Cell�Ǘ��\���̂ɃI�u�W�F�N�g��� --- //
                _cellSt[r, c] = cell;
                // --- �Ֆʂ̏�����Ԍ��� --- //
                scr.Row = r;                // �s���
                scr.Col = c;                // ����
                // --- �����_������ --- //
                scr.LightOn = false;        // 
                col.color = _off_color;     // �F�̌���

            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var cell = eventData.pointerCurrentRaycast.gameObject;
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
                if (!_cellSt[r, c].GetComponent<CellScript>().LightOn) { black_num++; }
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
        var scr = _cellSt[row, col].GetComponent<CellScript>();
        scr.LightOn = !scr.LightOn;
        var image = _cellSt[row, col].GetComponent<Image>();
        image.color = scr.LightOn ? _on_color : _off_color;     // �摜�̐F�ύX 
    }

    void RandCellMake()
    {
        for (var r = 0; r < _cellSt.GetLength(0); r++)
        {
            for (var c = 0; c < _cellSt.GetLength(1); c++)
            {
               
            }
        }
    }
}
