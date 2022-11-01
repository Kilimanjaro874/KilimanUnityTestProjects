using UnityEngine;
using UnityEngine.UI;

public class DoubleArray2 : MonoBehaviour
{
    [SerializeField]
    private int init_row_ = 5;
    [SerializeField]
    private int init_column_ = 5;
    private struct ImageMap2D
    {
        // image二次元配列＆セレクターに関連を持たせる：構造体として扱う
        public Image[,] im2d_;
        public int s_row_;  // 選択中の行
        public int s_col_;  // 選択中の列
    }
    ImageMap2D imMap2D_;


    void Start()
    {
        // --- Grid Layout Groupの初期化 --- //
        var grid_ = GetComponent<GridLayoutGroup>();
        grid_.constraintCount = init_column_;  // 行列：正方行列を想定。代表して行数を代入
        // --- ImageMap2D初期化 ---- //
        imMap2D_.im2d_ = new Image[init_row_, init_column_];
        imMap2D_.s_row_ = 0;
        imMap2D_.s_col_ = 0;
        // --- オブジェクト生成＆画像参照取得 --- //
        for (var r = 0; r < imMap2D_.im2d_.GetLength(0); r++)
        {
            for (var c = 0; c < imMap2D_.im2d_.GetLength(1); c++)
            {
                var obj = new GameObject($"Cell({r}, {c})");
                obj.transform.parent = transform;

                var image = obj.AddComponent<Image>();
                if (r == 0 && c == 0) { image.color = Color.red; }
                else { image.color = Color.white; }
                imMap2D_.im2d_[r, c] = image;    // 配列に格納
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // --- 操作取得 --- //
        int in_row = 0;
        int in_col = 0;
        if (Input.GetKeyDown(KeyCode.Space)) { Image2D_Erase(ref imMap2D_); }  // 画像削除
        if (Input.GetKeyDown(KeyCode.RightArrow)) { in_col = 1; };
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { in_col = -1; };
        if (Input.GetKeyDown(KeyCode.UpArrow)) { in_row = -1; };
        if (Input.GetKeyDown(KeyCode.DownArrow)) { in_row = 1; };
        if (in_row == 0 && in_col == 0) { return; }     // 操作無し：終了
        Image2D_Selector(in_row, in_col, ref imMap2D_);
    }

    void Image2D_Erase(ref ImageMap2D m2d)
    {
        // ---- ImageMap2Dの選択中の画像を消去する --- //
        // セレクターは消去画像の位置にとどまる：操作後にセレクターを移動させる事とする
        Destroy(m2d.im2d_[m2d.s_row_, m2d.s_col_]);
    }
    void Image2D_Selector(int in_row, int in_col, ref ImageMap2D m2d)
    {
        bool is_scan_end_ = false;
        bool is_scan_column = false;
        // ---- ImageMap2Dの選択画像変更：赤くする ---- //
        for(var r = 0; r < m2d.im2d_.GetLength(0); r++)
        {
            for(var c = 0; c < m2d.im2d_.GetLength(1); c++)
            {
                // --- 選択中の画像を白くする --- //
                if(m2d.im2d_[m2d.s_row_, m2d.s_col_] != null)
                {
                    m2d.im2d_[m2d.s_row_, m2d.s_col_].color = Color.white;
                }
                m2d.s_col_ += in_row;
                // 列配列上限 or 下限処理：セレクターを配列下限 or 上限に移動
                if (m2d.s_col_ > m2d.im2d_.GetLength(1) - 1) { m2d.s_col_ = 0; }
                if (m2d.s_col_ < 0) { m2d.s_col_ = m2d.im2d_.GetLength(1) - 1; }
                // 参照が有効なら画像を赤くする
                if (m2d.im2d_[m2d.s_row_, m2d.s_col_] != null)
                {
                    m2d.im2d_[m2d.s_row_, m2d.s_col_].color = Color.red;
                    is_scan_column = true;
                    break;
                }
                // --- 列内に有効な画像が存在しなければ、行を進める --- //
                if (c >= m2d.im2d_.GetLength(1) - 1)
                {
                    m2d.s_row_++;
                }
            }

            if (is_scan_column) { return; }

            // 選択中の画像を白くする
            if (m2d.im2d_[m2d.s_row_, m2d.s_col_] != null)
            {
                m2d.im2d_[m2d.s_row_, m2d.s_col_].color = Color.white;
            }
            m2d.s_row_ += in_row;
            // 行配列上限 or 下限処理：セレクターを配列下限 or 上限に移動
            if (m2d.s_row_ > m2d.im2d_.GetLength(0) - 1) { m2d.s_row_ = 0; }
            if (m2d.s_row_ < 0) { m2d.s_row_ = m2d.im2d_.GetLength(0) - 1; }
            // 参照が有効なら画像を赤くする
            if (m2d.im2d_[m2d.s_row_, m2d.s_col_] != null)
            {
                m2d.im2d_[m2d.s_row_, m2d.s_col_].color = Color.red;
                break;
            }
            // 行の画像が全て参照無効の時：列の走査へ移行する
            if (r >= m2d.im2d_.GetLength(0) - 1)
            {
                is_scan_end_ = false;
            }




        }

    }
}
