using UnityEngine;

public class DieCutting : MonoBehaviour
{
    [SerializeField, Tooltip("�^�����̎Q�l�N���X���Z�b�g����:DieCuttingRef")]
    private DieCuttingRef dcRef_;
    [SerializeField, Tooltip("���̉摜��o�^�@���X�ƃs�N�Z�����𑵂��鎖")]
    private Sprite im_water_;
    [SerializeField, Tooltip("�X�̉摜��o�^  �����ƃs�N�Z�����𑵂��鎖")]
    private Sprite im_snow_;
    [SerializeField, Tooltip("�^�����̍s�̐�:�����s��z��")]
    private int row_num_ = 5;
    [SerializeField, Tooltip("�^�����̍s�̊Ԋu�����X�y�[�X���w��")]
    private float row_space_num_ = 0.1f;
    [SerializeField, Tooltip("�^�����̗�̐�:�����s��z��")]
    private int column_num_ = 5;
    [SerializeField, Tooltip("�^�����̍s�̊Ԋu�����X�y�[�X���w��")]
    private float column_space_num_ = 1.0f;
    [SerializeField, Tooltip("�Q�ƃN���X�̐��摜�̖��O���L�q���Ă�������")]
    private string im_water_ref_name_;
    [SerializeField, Tooltip("�Q�ƃN���X�̕X�摜�̖��O���L�q���Ă�������")]
    private string im_ice_ref_name_;

    private SpriteRenderer[,] spriteRn2D;   //  �摜�؂�ւ��̂��߁A2D�̃X�v���C�g�����_�[�擾
    private BoxCollider2D[,] boxCol2D;      //  �����蔻��̂��߁A2D�̃R���C�_�[�擾

    private int[,] ref_shaphe_;

    void Start()
    {
        // ---- �^�����Ώۂ𐶐� ---- //
        if(dcRef_ == null)
        {
            Debug.Log("�^�����̎Q�ƂƂȂ�N���X���Z�b�g���Ă��������FdcRef_");
            return;
        }
        // ---- ���������� ---- //
        spriteRn2D = new SpriteRenderer[dcRef_.row_num, dcRef_.column_num];
        boxCol2D = new BoxCollider2D[dcRef_.row_num, dcRef_.column_num];
        float image_width = 0;      // �摜�̉����T�C�Y�ꎞ�擾
        float image_height = 0;     // �摜�̏c���T�C�Y�ꎞ�擾
        for(var r = 0; r < dcRef_.row_num; r++)
        {
            for(var c = 0; c < dcRef_.column_num; c++)
            {
                // --- �I�u�W�F�N�g���� --- //
                var obj = new GameObject($"Cell{r}, {c})");
                obj.transform.parent = transform;                   // �܂��e�ɍ��W�ݒ�
                var sprite = obj.AddComponent<SpriteRenderer>();    // SpriteRenderer��^����
                sprite.sprite = im_snow_;                           // �Q�Ɖ摜��^����(�f�t�H���g�F��)
                if (r == 0 && c == 0)
                {
                    image_width = sprite.size.x;             // �摜�����擾
                    image_height = sprite.size.y;            //     �c
                }
                var collider = obj.AddComponent<BoxCollider2D>();   // BOX�R���C�_�[�\��t��
                                                                    // --- �I�u�W�F�N�g�̈ʒu�E�R���C�_�[�T�C�Y�E�I�u�W�F�N�g�T�C�Y���� --- //
                obj.transform.position = new Vector3(transform.position.x + c * (image_width + row_space_num_),
                    transform.position.y - r * (image_height + column_space_num_), transform.position.z);
                collider.size = new Vector2(image_width, image_height);
                // --- �Q�Ƃ̎擾 --- //
                spriteRn2D[r, c] = sprite;
                boxCol2D[r, c] = collider;
            }
        }
    }

    void Update()
    {
        
    }


}
