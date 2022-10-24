using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class Code : MonoBehaviour
{
    [SerializeField]
    private int object_num_ = 10;
    int select_num_ = 0;
    private Image[] images_;
    private void Start()
    {
        images_ = new Image[object_num_];
        for (var i = 0; i < object_num_; i++)
        {
            var obj = new GameObject($"Cell{i}");
            obj.transform.parent = transform;
            var image = obj.AddComponent<Image>();
            if (i == 0) { image.color = Color.red; }
            else { image.color = Color.white; }
            // 配列に格納
            images_[i] = image;
        }
    }
    void Update()
    {
        // --- 画像の消去 --- //
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // imageだけ消去
            Destroy(images_[select_num_]);
            for (var i = 0; i < images_.Length; i++)
            {
                select_num_++;
                if (select_num_ > images_.Length -1) { select_num_ = 0; }
                if (images_[select_num_] != null)
                {
                    images_[select_num_].color = Color.red;
                    break;
                }
                if (i == images_.Length - 1)
                {
                    Debug.Log("画像は全て消去済みです");
                }
            }
        }
        // --- 操作の受付 --- //
        int input = 0;
        int count = 0;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { input = -1; }
        if (Input.GetKeyDown(KeyCode.RightArrow)) { input = 1; }
        if (input == 0) return;     // 操作がなければ終了
        for (var i = 0; i < images_.Length; i++)
        {
            // 現在選択中の画像を白くする
            if (images_[select_num_] != null)
            {
                images_[select_num_].color = Color.white;
            }
            select_num_ += input;
            // 上限・加減値処理
            if (select_num_ > images_.Length - 1) select_num_ = 0;
            if (select_num_ < 0) select_num_ = images_.Length - 1;
            // 画像のサーチ＆色変え
            if (images_[select_num_] != null)
            {
                images_[select_num_].color = Color.red;
                break;
            }
            // 移動カウントを進める
            count++;
            // カウントが配列の大きさと揃った時：配列のImageが全て消去済み
            if (count >= images_.Length - 1)
            {
                Debug.Log("全ての画像は消去済みです\n");
            }
        }
    }
}