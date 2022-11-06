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
    private int _num_of_move = 0;   // 手数を記録

    private struct Cellst
    {   // Cellst : 参照を構造体で持つ
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
                var scr = cell.AddComponent<CellScript>();        // Cell用のスクリプトアタッチ
                // --- Cell管理構造体にオブジェクト＆スクリプト代入 --- //
                _cellSt[r, c]._cell = cell;
                _cellSt[r, c]._scr = scr;
                _cellSt[r, c]._img = col;
                // --- 盤面の初期状態決定 --- //
                scr.Row = r;                // 行代入
                scr.Col = c;                // 列代入
            }
        }
        // --- ランダム生成 --- //
        RandCellMake();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var cell = eventData.pointerCurrentRaycast.gameObject;
        // --- ゲームオブジェクトがCellスクリプトを持ってるか確認 --- //
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
                if (!_cellSt[r, c]._scr.LightOn) { black_num++; }
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
        var scr = _cellSt[row, col]._scr;
        var img = _cellSt[row, col]._img;
        scr.LightOn = !scr.LightOn;
        img.color = scr.LightOn ? _on_color : _off_color;     // 画像の色変更 
    }

    void RandCellMake()
    {
        for (var r = 0; r < _cellSt.GetLength(0); r++)
        {
            for (var c = 0; c < _cellSt.GetLength(1); c++)
            {
                // -- 以下の一発でクリア出来ないように実行する処理のテスト -- //
                // -> あえて全てのCellを、成功条件の白に変更する 
                _cellSt[r,c]._scr.LightOn = true;           // LightOn = true
                _cellSt[r, c]._img.color = _on_color;       // 白
                int tempRand = Random.Range((int)0, (int)1);
                //if(tempRand == 0)
                //{
                //    _cellSt[r, c]._scr.LightOn = false;             // LightOn = false
                //    _cellSt[r, c]._img.color = _off_color;          // 黒
                //}
            }
        }
        // --- 一発でクリアできない用に走査を行う --- //
        // 少なくとも、「白が十字で存在する場所が一か所も無い」なら大丈夫
        // その部分があれば、その内1~2箇所黒く塗りつぶす
        for (var r = 0; r < _cellSt.GetLength(0); r++)
        {
            int count_white = 0;
            for (var c = 0; c < _cellSt.GetLength(1); c++)
            {
                if(_cellSt[r, c]._scr.LightOn) { count_white++; }
                else { count_white = 0; }
                if(count_white >= 2)
                {
                    // --- 2個白が横に連続：十字が存在しないかチェックしていく --- //
                    if(c + 1 >= _cellSt.GetLength(1)) { continue; }     // 右は範囲外、OK
                    if(!_cellSt[r,c + 1]._scr.LightOn) { continue; }    // 右は黒、OK 
                    if (r - 1 < 0) { continue; }                        // 上は範囲外、OK
                    if(!_cellSt[r-1, c]._scr.LightOn) { continue; }     // 上は黒、OK
                    if(r + 1 >= _cellSt.GetLength(0)) { continue; }     // 下は範囲外、OK
                    if(!_cellSt[r+1, c]._scr.LightOn) { continue; }     // 下は黒　OK
                    // -- 白十字が発見された時の処理 -- //
                    int tmp_roop = Random.Range((int)1, (int)3);
                    for (int i = 0; i < tmp_roop; i++)
                    {
                        // - 白十字の内、数か所黒に変更 - //
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
