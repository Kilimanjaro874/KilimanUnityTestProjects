using UnityEngine;

[ExecuteInEditMode]     // Edit���[�h�ł��{�N���X�����s�̂��ߋL�q
public class DieCuttingRef : MonoBehaviour
{
    [SerializeField, Tooltip("���̉摜��o�^�@���X�ƃs�N�Z�����𑵂��鎖")]
    private Sprite im_water_;
    string im_water_name_;                  // im_water�̖��O���擾(�^�����Ƃ̖��O��r�p)
    public string im_water_name { get { return im_water_name_; } }
    [SerializeField, Tooltip("�X�̉摜��o�^  �����ƃs�N�Z�����𑵂��鎖")]
    private Sprite im_snow_;
    string im_snow_name_;                   // im_snow�̖��O���擾(�^�����Ƃ̖��O��r�p)
    public string im_snow_name { get { return im_snow_name_; } }
    [SerializeField, Tooltip("�^�����̍s�̐�:�����s��z��")]
    private int row_num_ = 5;
    public int row_num { get { return row_num_; } }
    [SerializeField, Tooltip("�^�����̍s�̊Ԋu�����X�y�[�X���w��")]
    private float row_space_num_ = 0.1f;
    [SerializeField, Tooltip("�^�����̗�̐�:�����s��z��")]
    private int column_num_ = 5;
    public int column_num { get { return column_num_; } }
    [SerializeField, Tooltip("�^�����̍s�̊Ԋu�����X�y�[�X���w��")]
    private float column_space_num_ = 1.0f;
    [SerializeField, Tooltip("�������true�Ƃ���->Game��ʂ��N���b�N����ƁA�{�Q�ƃN���X������������")]
    private bool is_reset_ = false;

    private GameObject[,] object2D_;        // �Q�[���I�u�W�F�N�g�ێ� 
    private SpriteRenderer[,] spriteRn2D;   // �摜�؂�ւ��̂��߁A2D�̃X�v���C�g�����_�[�擾
    private BoxCollider2D[,] boxCol2D;      // �����蔻��̂��߁A2D�̃R���C�_�[�擾

    private bool is_init_ = false;
   
    private void OnGUI()
    {
        // ---- ���Edit���[�h�ɂ�����[Game]�E�B���h�E�N���b�N���A�{�֐������{����� ---- //

        if (is_reset_) { Reset();}      // �N���X����������(�C���X�y�N�^���� is_reset��true�Ƃ���)                   
        if (is_init_) { return; }       // ���ɏ������ς݁F�ȉ����������{���Ȃ�
        // --- ���������� --- //
        object2D_ = new GameObject[row_num_, column_num_];
        spriteRn2D = new SpriteRenderer[row_num_, column_num_];
        boxCol2D = new BoxCollider2D[row_num_, column_num_];
        im_water_name_ = im_water_.name;
        im_snow_name_ = im_snow_.name;
        float image_width = 0;  // �摜�̉����T�C�Y�擾
        float image_height = 0; // �摜�̏c���T�C�Y�擾
        for (var r = 0; r < row_num_; r++)
        {
            for (var c = 0; c < column_num_; c++)
            {
                // --- �I�u�W�F�N�g���� --- //
                object2D_[r, c] = new GameObject($"Cell{r}, {c})");
                object2D_[r, c].transform.parent = transform;                   // �܂��e�ɍ��W�ݒ�
                var sprite = object2D_[r, c].AddComponent<SpriteRenderer>();    // SpriteRenderer��^����
                sprite.sprite = im_snow_;                           // �Q�Ɖ摜��^����(�f�t�H���g�F��)

                if (r == 0 && c == 0)
                {
                    image_width = sprite.size.x;             // �摜�����擾
                    image_height = sprite.size.y;            //     �c
                }
                var collider = object2D_[r, c].AddComponent<BoxCollider2D>();   // BOX�R���C�_�[�\��t��
                // --- �I�u�W�F�N�g�̈ʒu�E�R���C�_�[�T�C�Y���� --- //
                object2D_[r,c].transform.position = new Vector3(transform.position.x + c * (image_width + row_space_num_),
                    transform.position.y - r * (image_height + column_space_num_), transform.position.z);
                collider.size = new Vector2(image_width, image_height);
                // --- �Q�Ƃ̎擾 --- //
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
            // �S��Object����̎�
            is_reset_ = false;
            is_init_ = false;
        }
        else
        {
            // ����Object�����݂���ꍇ
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
