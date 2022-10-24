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
            // �z��Ɋi�[
            images_[i] = image;
        }
    }
    void Update()
    {
        // --- �摜�̏��� --- //
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // image��������
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
                    Debug.Log("�摜�͑S�ď����ς݂ł�");
                }
            }
        }
        // --- ����̎�t --- //
        int input = 0;
        int count = 0;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { input = -1; }
        if (Input.GetKeyDown(KeyCode.RightArrow)) { input = 1; }
        if (input == 0) return;     // ���삪�Ȃ���ΏI��
        for (var i = 0; i < images_.Length; i++)
        {
            // ���ݑI�𒆂̉摜�𔒂�����
            if (images_[select_num_] != null)
            {
                images_[select_num_].color = Color.white;
            }
            select_num_ += input;
            // ����E�����l����
            if (select_num_ > images_.Length - 1) select_num_ = 0;
            if (select_num_ < 0) select_num_ = images_.Length - 1;
            // �摜�̃T�[�`���F�ς�
            if (images_[select_num_] != null)
            {
                images_[select_num_].color = Color.red;
                break;
            }
            // �ړ��J�E���g��i�߂�
            count++;
            // �J�E���g���z��̑傫���Ƒ��������F�z���Image���S�ď����ς�
            if (count >= images_.Length - 1)
            {
                Debug.Log("�S�Ẳ摜�͏����ς݂ł�\n");
            }
        }
    }
}