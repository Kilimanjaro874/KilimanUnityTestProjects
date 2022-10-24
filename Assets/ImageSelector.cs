using UnityEngine;
using UnityEngine.UI;

public class ImageSelector : MonoBehaviour
{
    [SerializeField]
    private int image_num_ = 10;
    private struct imageMap_
    {
        // image�z�񁕃Z���N�^�[�Ɋ֘A�������������F�\���̂Ƃ��Ĉ���
        public Image[] images_;
        public int select_num_;
    }
    imageMap_ imMap1;

    void Start()
    {
        // --- image_Map������ --- //
        imMap1.images_ = new Image[image_num_];
        imMap1.select_num_ = 0;
        // --- �I�u�W�F�N�g�������摜�Q�Ǝ擾 --- //
        for (var i = 0; i < image_num_; i++)
        {
            var obj = new GameObject($"Cell{i}");
            obj.transform.parent = transform;
            var image = obj.AddComponent<Image>();
            if (i == 0) { image.color = Color.red; }
            else { image.color = Color.white; }
            // �z��Ɋi�[
            imMap1.images_[i] = image;
        }
    }

    void Update()
    {
        // --- ���� --- //
        int input = 0;
        if (Input.GetKeyDown(KeyCode.Space)) { DeleteImage(ref imMap1); }   // �摜�폜
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { input = -1; }
        if (Input.GetKeyDown(KeyCode.RightArrow)) { input = 1; }
        if(input == 0) { return; }  // ���삪������ΏI��
        IMapSelector(input, ref imMap1);    // �摜�I��
    }

    void IMapSelector(int imput, ref imageMap_ imMap)
    {
        // --- imageMap�̑I���摜�ύX�F�Ԃ����� --- //
        for (var i = 0; i < imMap.images_.Length; i++)
        {
            if(imMap.images_[imMap.select_num_] != null)
            {
                // ���ݑI�𒆂̉摜�𔒂�����
                imMap.images_[imMap.select_num_].color = Color.white;   
            }
            imMap.select_num_ += imput;
            // �z���� or �������� : �Z���N�^�[������ or ����Ɉړ� 
            if (imMap.select_num_ > imMap.images_.Length - 1) { imMap.select_num_ = 0; }
            if (imMap.select_num_ < 0) { imMap.select_num_ = imMap.images_.Length - 1; }
            // �Q�Ƃ̂���摜�T�[�`���F�ς�
            if(imMap.images_[imMap.select_num_] != null)
            {
                imMap.images_[imMap.select_num_].color = Color.red;
                break;
            }
            // �J�E���g���z��T�C�Y�Ƒ��������F�z���image���S�ď����ς�
            if(i >= imMap.images_.Length - 1)
            {
                Debug.Log("imageMap�̑S�Ẳ摜�͏����ς݂ł�\n");
            }
        }
    }

    void DeleteImage(ref imageMap_ imMap)
    {
        // --- imageMap����I�𒆂̉摜���� --- //
        Destroy(imMap.images_[imMap.select_num_]);
        for(var i = 0; i < imMap.images_.Length; i++)
        {
            // �Q�Ƃ̂���摜�T�[�`���F�ς�(�z��E���ֈړ����鎖�Ƃ���)
            imMap.select_num_++;
            if(imMap.select_num_ > imMap.images_.Length - 1 ) { imMap.select_num_ = 0; }
            if(imMap.images_[imMap.select_num_] != null)
            {
                imMap.images_[imMap.select_num_].color = Color.red;
                break;
            }
            // �J�E���g���z��T�C�Y�Ƒ��������F�z���image���S�ď����ς�
            if (i >= imMap.images_.Length - 1)
            {
                Debug.Log("imageMap�̑S�Ẳ摜�͏����ς݂ł�\n");
            }
        }
    }
}
