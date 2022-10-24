using UnityEngine;

[ExecuteInEditMode]     // Editモードでも本クラスを実行のため記述
public class DieCuttingRef : MonoBehaviour
{
    [SerializeField, Tooltip("水の画像を登録　※氷とピクセル数を揃える事")]
    private Sprite im_water_;
    string im_water_name_;                  // im_waterの名前を取得(型抜きとの名前比較用)
    public string im_water_name { get { return im_water_name_; } }
    [SerializeField, Tooltip("氷の画像を登録  ※水とピクセル数を揃える事")]
    private Sprite im_snow_;
    string im_snow_name_;                   // im_snowの名前を取得(型抜きとの名前比較用)
    public string im_snow_name { get { return im_snow_name_; } }
    [SerializeField, Tooltip("型抜きの行の数:正方行列想定")]
    private int row_num_ = 5;
    public int row_num { get { return row_num_; } }
    [SerializeField, Tooltip("型抜きの行の間隔調整スペースを指定")]
    private float row_space_num_ = 0.1f;
    [SerializeField, Tooltip("型抜きの列の数:正方行列想定")]
    private int column_num_ = 5;
    public int column_num { get { return column_num_; } }
    [SerializeField, Tooltip("型抜きの行の間隔調整スペースを指定")]
    private float column_space_num_ = 1.0f;
    [SerializeField, Tooltip("こちらをtrueとして->Game画面をクリックすると、本参照クラスを初期化する")]
    private bool is_reset_ = false;

    private GameObject[,] object2D_;        // ゲームオブジェクト保持 
    private SpriteRenderer[,] spriteRn2D;   // 画像切り替えのため、2Dのスプライトレンダー取得
    private BoxCollider2D[,] boxCol2D;      // 当たり判定のため、2Dのコライダー取得

    private bool is_init_ = false;
   
    private void OnGUI()
    {
        // ---- 主にEditモードにおいて[Game]ウィンドウクリック時、本関数が実施される ---- //

        if (is_reset_) { Reset();}      // クラス初期化処理(インスペクタから is_resetをtrueとする)                   
        if (is_init_) { return; }       // 既に初期化済み：以下処理を実施しない
        // --- 初期化処理 --- //
        object2D_ = new GameObject[row_num_, column_num_];
        spriteRn2D = new SpriteRenderer[row_num_, column_num_];
        boxCol2D = new BoxCollider2D[row_num_, column_num_];
        im_water_name_ = im_water_.name;
        im_snow_name_ = im_snow_.name;
        float image_width = 0;  // 画像の横幅サイズ取得
        float image_height = 0; // 画像の縦幅サイズ取得
        for (var r = 0; r < row_num_; r++)
        {
            for (var c = 0; c < column_num_; c++)
            {
                // --- オブジェクト生成 --- //
                object2D_[r, c] = new GameObject($"Cell{r}, {c})");
                object2D_[r, c].transform.parent = transform;                   // まず親に座標設定
                var sprite = object2D_[r, c].AddComponent<SpriteRenderer>();    // SpriteRendererを与える
                sprite.sprite = im_snow_;                           // 参照画像を与える(デフォルト：雪)

                if (r == 0 && c == 0)
                {
                    image_width = sprite.size.x;             // 画像横幅取得
                    image_height = sprite.size.y;            //     縦
                }
                var collider = object2D_[r, c].AddComponent<BoxCollider2D>();   // BOXコライダー貼り付け
                // --- オブジェクトの位置・コライダーサイズ調整 --- //
                object2D_[r,c].transform.position = new Vector3(transform.position.x + c * (image_width + row_space_num_),
                    transform.position.y - r * (image_height + column_space_num_), transform.position.z);
                collider.size = new Vector2(image_width, image_height);
                // --- 参照の取得 --- //
                spriteRn2D[r, c] = sprite;
                boxCol2D[r, c] = collider;
            }
        }
        is_init_ = true;
    }

    private void Reset()
    {
        if (object2D_ == null)
        {
            // 全くObjectが空の時
            is_reset_ = false;
            is_init_ = false;
        }
        else
        {
            // 既にObjectが存在する場合
            for (var r = 0; r < object2D_.GetLength(0); r++)
            {
                for (var c = 0; c < object2D_.GetLength(1); c++)
                {
                    if (object2D_[r, c] != null)
                    {
                        DestroyImmediate(object2D_[r, c]);
                    }
                }
            }
            is_reset_ = false;
            is_init_ = false;
        }
    }

}
