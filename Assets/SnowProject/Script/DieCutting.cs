using UnityEngine;

public class DieCutting : MonoBehaviour
{
    [SerializeField, Tooltip("型抜きの参考クラスをセットする:DieCuttingRef")]
    private DieCuttingRef dcRef_;
    [SerializeField, Tooltip("水の画像を登録　※氷とピクセル数を揃える事")]
    private Sprite im_water_;
    [SerializeField, Tooltip("氷の画像を登録  ※水とピクセル数を揃える事")]
    private Sprite im_snow_;
    [SerializeField, Tooltip("型抜きの行の数:正方行列想定")]
    private int row_num_ = 5;
    [SerializeField, Tooltip("型抜きの行の間隔調整スペースを指定")]
    private float row_space_num_ = 0.1f;
    [SerializeField, Tooltip("型抜きの列の数:正方行列想定")]
    private int column_num_ = 5;
    [SerializeField, Tooltip("型抜きの行の間隔調整スペースを指定")]
    private float column_space_num_ = 1.0f;
    [SerializeField, Tooltip("参照クラスの水画像の名前を記述してください")]
    private string im_water_ref_name_;
    [SerializeField, Tooltip("参照クラスの氷画像の名前を記述してください")]
    private string im_ice_ref_name_;

    private SpriteRenderer[,] spriteRn2D;   //  画像切り替えのため、2Dのスプライトレンダー取得
    private BoxCollider2D[,] boxCol2D;      //  当たり判定のため、2Dのコライダー取得

    private int[,] ref_shaphe_;

    void Start()
    {
        // ---- 型抜き対象を生成 ---- //
        if(dcRef_ == null)
        {
            Debug.Log("型抜きの参照となるクラスをセットしてください：dcRef_");
            return;
        }
        // ---- 初期化処理 ---- //
        spriteRn2D = new SpriteRenderer[dcRef_.row_num, dcRef_.column_num];
        boxCol2D = new BoxCollider2D[dcRef_.row_num, dcRef_.column_num];
        float image_width = 0;      // 画像の横幅サイズ一時取得
        float image_height = 0;     // 画像の縦幅サイズ一時取得
        for(var r = 0; r < dcRef_.row_num; r++)
        {
            for(var c = 0; c < dcRef_.column_num; c++)
            {
                // --- オブジェクト生成 --- //
                var obj = new GameObject($"Cell{r}, {c})");
                obj.transform.parent = transform;                   // まず親に座標設定
                var sprite = obj.AddComponent<SpriteRenderer>();    // SpriteRendererを与える
                sprite.sprite = im_snow_;                           // 参照画像を与える(デフォルト：雪)
                if (r == 0 && c == 0)
                {
                    image_width = sprite.size.x;             // 画像横幅取得
                    image_height = sprite.size.y;            //     縦
                }
                var collider = obj.AddComponent<BoxCollider2D>();   // BOXコライダー貼り付け
                                                                    // --- オブジェクトの位置・コライダーサイズ・オブジェクトサイズ調整 --- //
                obj.transform.position = new Vector3(transform.position.x + c * (image_width + row_space_num_),
                    transform.position.y - r * (image_height + column_space_num_), transform.position.z);
                collider.size = new Vector2(image_width, image_height);
                // --- 参照の取得 --- //
                spriteRn2D[r, c] = sprite;
                boxCol2D[r, c] = collider;
            }
        }
    }

    void Update()
    {
        
    }


}
