using UnityEngine;
using UnityEngine.UI;

public class ImageSelector : MonoBehaviour
{
    [SerializeField]
    private int image_num_ = 10;
    private struct imageMap_
    {
        // image配列＆セレクターに関連を持たせたい：構造体として扱う
        public Image[] images_;
        public int select_num_;
    }
    imageMap_ imMap1;

    void Start()
    {
        // --- image_Map初期化 --- //
        imMap1.images_ = new Image[image_num_];
        imMap1.select_num_ = 0;
        // --- オブジェクト生成＆画像参照取得 --- //
        for (var i = 0; i < image_num_; i++)
        {
            var obj = new GameObject($"Cell{i}");
            obj.transform.parent = transform;
            var image = obj.AddComponent<Image>();
            if (i == 0) { image.color = Color.red; }
            else { image.color = Color.white; }
            // 配列に格納
            imMap1.images_[i] = image;
        }
    }

    void Update()
    {
        // --- 操作 --- //
        int input = 0;
        if (Input.GetKeyDown(KeyCode.Space)) { DeleteImage(ref imMap1); }   // 画像削除
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { input = -1; }
        if (Input.GetKeyDown(KeyCode.RightArrow)) { input = 1; }
        if(input == 0) { return; }  // 操作が無ければ終了
        IMapSelector(input, ref imMap1);    // 画像選択
    }

    void IMapSelector(int imput, ref imageMap_ imMap)
    {
        // --- imageMapの選択画像変更：赤くする --- //
        for (var i = 0; i < imMap.images_.Length; i++)
        {
            if(imMap.images_[imMap.select_num_] != null)
            {
                // 現在選択中の画像を白くする
                imMap.images_[imMap.select_num_].color = Color.white;   
            }
            imMap.select_num_ += imput;
            // 配列上限 or 下限処理 : セレクターを下限 or 上限に移動 
            if (imMap.select_num_ > imMap.images_.Length - 1) { imMap.select_num_ = 0; }
            if (imMap.select_num_ < 0) { imMap.select_num_ = imMap.images_.Length - 1; }
            // 参照のある画像サーチ＆色変え
            if(imMap.images_[imMap.select_num_] != null)
            {
                imMap.images_[imMap.select_num_].color = Color.red;
                break;
            }
            // カウントが配列サイズと揃った時：配列のimageが全て消去済み
            if(i >= imMap.images_.Length - 1)
            {
                Debug.Log("imageMapの全ての画像は消去済みです\n");
            }
        }
    }

    void DeleteImage(ref imageMap_ imMap)
    {
        // --- imageMapから選択中の画像消去 --- //
        Destroy(imMap.images_[imMap.select_num_]);
        for(var i = 0; i < imMap.images_.Length; i++)
        {
            // 参照のある画像サーチ＆色変え(配列右側へ移動する事とする)
            imMap.select_num_++;
            if(imMap.select_num_ > imMap.images_.Length - 1 ) { imMap.select_num_ = 0; }
            if(imMap.images_[imMap.select_num_] != null)
            {
                imMap.images_[imMap.select_num_].color = Color.red;
                break;
            }
            // カウントが配列サイズと揃った時：配列のimageが全て消去済み
            if (i >= imMap.images_.Length - 1)
            {
                Debug.Log("imageMapの全ての画像は消去済みです\n");
            }
        }
    }
}
