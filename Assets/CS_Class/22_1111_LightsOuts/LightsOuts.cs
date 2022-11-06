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
    GameObject[,] _cellSt;          // Cellゲームオブジェクト格納
    private int _num_of_move = 0;   // 手数を記録

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
                var scr = cell.AddComponent<CellScript>();        // Cell用のスクリプトアタッチ
                // --- Cell管理構造体にオブジェクト代入 --- //
                _cellSt[r, c] = cell;
                // --- 盤面の初期状態決定 --- //
                scr.Row = r;                // 行代入
                scr.Col = c;                // 列代入
                // --- ランダム生成 --- //
                scr.LightOn = false;        // 
                col.color = _off_color;     // 色の決定

            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var cell = eventData.pointerCurrentRaycast.gameObject;
        if(cell.gameObject.GetComponent<CellScript>()== null) { return; }
        // --- 手数の記録 --- //
        _num_of_move++;
        // --- cellの状態取得 --- //
        var scr = cell.gameObject.GetComponent<CellScript>();
        var s_row = scr.Row;
        var s_col = scr.Col;
        // --- cellの状態変更 --- //
        scr.LightOn = !scr.LightOn;                             // on / off切り替え
        var image = cell.GetComponent<Image>();
        image.color = scr.LightOn ? _on_color : _off_color;     // 画像の色変更 
        // --- 他の駒の状態も変化させる --- //
        if(CellStateChangeAndCheck(s_row, s_col))
        {
            // -- Cellが全て白の時の処理 -- //
            Debug.Log("Complete");
            Debug.Log("Clear time(s) = " + Time.time.ToString());
            Debug.Log("Num of move = " + _num_of_move.ToString());
        }

    }

    bool CellStateChangeAndCheck(int s_row, int s_col)
    {
        // ---- s_row, s_colで選択したCellの上下左右の状態を変化 ---- //
        int black_num = 0;
        for(var r = 0; r < _cellSt.GetLength(0); r++)
        {
            for(var c = 0; c < _cellSt.GetLength(1); c++)
            {
                // --- 上、下、左、右のCell確認 --- //
                if(r == s_row - 1 && c == s_col){ CellLightChange(r, c); }
                if(r == s_row + 1 && c == s_col){ CellLightChange(r, c); }
                if(c == s_col - 1 && r == s_row){ CellLightChange(r, c); }
                if(c == s_col + 1 && r == s_row){ CellLightChange(r, c); }
                // --- 駒の状態を確認 --- //
                if (!_cellSt[r, c].GetComponent<CellScript>().LightOn) { black_num++; }
            }
        }
        if(black_num == 0) { return true; }     // Cellが全て白！
        return false;
    }

    void CellLightChange(int row, int col)
    {
        // ---- 選択したCellの状態を変化させる ---- //
        if(row > _cellSt.GetLength(0)) { return; }
        if(col > _cellSt.GetLength(1)) { return; }
        var scr = _cellSt[row, col].GetComponent<CellScript>();
        scr.LightOn = !scr.LightOn;
        var image = _cellSt[row, col].GetComponent<Image>();
        image.color = scr.LightOn ? _on_color : _off_color;     // 画像の色変更 
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
