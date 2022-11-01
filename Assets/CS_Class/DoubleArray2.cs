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
        // image�񎟌��z�񁕃Z���N�^�[�Ɋ֘A����������F�\���̂Ƃ��Ĉ���
        public Image[,] im2d_;
        public int s_row_;  // �I�𒆂̍s
        public int s_col_;  // �I�𒆂̗�
    }
    ImageMap2D imMap2D_;


    void Start()
    {
        // --- Grid Layout Group�̏����� --- //
        var grid_ = GetComponent<GridLayoutGroup>();
        grid_.constraintCount = init_column_;  // �s��F�����s���z��B��\���čs������
        // --- ImageMap2D������ ---- //
        imMap2D_.im2d_ = new Image[init_row_, init_column_];
        imMap2D_.s_row_ = 0;
        imMap2D_.s_col_ = 0;
        // --- �I�u�W�F�N�g�������摜�Q�Ǝ擾 --- //
        for (var r = 0; r < imMap2D_.im2d_.GetLength(0); r++)
        {
            for (var c = 0; c < imMap2D_.im2d_.GetLength(1); c++)
            {
                var obj = new GameObject($"Cell({r}, {c})");
                obj.transform.parent = transform;

                var image = obj.AddComponent<Image>();
                if (r == 0 && c == 0) { image.color = Color.red; }
                else { image.color = Color.white; }
                imMap2D_.im2d_[r, c] = image;    // �z��Ɋi�[
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // --- ����擾 --- //
        int in_row = 0;
        int in_col = 0;
        if (Input.GetKeyDown(KeyCode.Space)) { Image2D_Erase(ref imMap2D_); }  // �摜�폜
        if (Input.GetKeyDown(KeyCode.RightArrow)) { in_col = 1; };
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { in_col = -1; };
        if (Input.GetKeyDown(KeyCode.UpArrow)) { in_row = -1; };
        if (Input.GetKeyDown(KeyCode.DownArrow)) { in_row = 1; };
        if (in_row == 0 && in_col == 0) { return; }     // ���얳���F�I��
        Image2D_Selector(in_row, in_col, ref imMap2D_);
    }

    void Image2D_Erase(ref ImageMap2D m2d)
    {
        // ---- ImageMap2D�̑I�𒆂̉摜���������� --- //
        // �Z���N�^�[�͏����摜�̈ʒu�ɂƂǂ܂�F�����ɃZ���N�^�[���ړ������鎖�Ƃ���
        Destroy(m2d.im2d_[m2d.s_row_, m2d.s_col_]);
    }
    void Image2D_Selector(int in_row, int in_col, ref ImageMap2D m2d)
    {
        bool is_scan_end_ = false;
        bool is_scan_column = false;
        // ---- ImageMap2D�̑I���摜�ύX�F�Ԃ����� ---- //
        for(var r = 0; r < m2d.im2d_.GetLength(0); r++)
        {
            for(var c = 0; c < m2d.im2d_.GetLength(1); c++)
            {
                // --- �I�𒆂̉摜�𔒂����� --- //
                if(m2d.im2d_[m2d.s_row_, m2d.s_col_] != null)
                {
                    m2d.im2d_[m2d.s_row_, m2d.s_col_].color = Color.white;
                }
                m2d.s_col_ += in_row;
                // ��z���� or ���������F�Z���N�^�[��z�񉺌� or ����Ɉړ�
                if (m2d.s_col_ > m2d.im2d_.GetLength(1) - 1) { m2d.s_col_ = 0; }
                if (m2d.s_col_ < 0) { m2d.s_col_ = m2d.im2d_.GetLength(1) - 1; }
                // �Q�Ƃ��L���Ȃ�摜��Ԃ�����
                if (m2d.im2d_[m2d.s_row_, m2d.s_col_] != null)
                {
                    m2d.im2d_[m2d.s_row_, m2d.s_col_].color = Color.red;
                    is_scan_column = true;
                    break;
                }
                // --- ����ɗL���ȉ摜�����݂��Ȃ���΁A�s��i�߂� --- //
                if (c >= m2d.im2d_.GetLength(1) - 1)
                {
                    m2d.s_row_++;
                }
            }

            if (is_scan_column) { return; }

            // �I�𒆂̉摜�𔒂�����
            if (m2d.im2d_[m2d.s_row_, m2d.s_col_] != null)
            {
                m2d.im2d_[m2d.s_row_, m2d.s_col_].color = Color.white;
            }
            m2d.s_row_ += in_row;
            // �s�z���� or ���������F�Z���N�^�[��z�񉺌� or ����Ɉړ�
            if (m2d.s_row_ > m2d.im2d_.GetLength(0) - 1) { m2d.s_row_ = 0; }
            if (m2d.s_row_ < 0) { m2d.s_row_ = m2d.im2d_.GetLength(0) - 1; }
            // �Q�Ƃ��L���Ȃ�摜��Ԃ�����
            if (m2d.im2d_[m2d.s_row_, m2d.s_col_] != null)
            {
                m2d.im2d_[m2d.s_row_, m2d.s_col_].color = Color.red;
                break;
            }
            // �s�̉摜���S�ĎQ�Ɩ����̎��F��̑����ֈڍs����
            if (r >= m2d.im2d_.GetLength(0) - 1)
            {
                is_scan_end_ = false;
            }




        }

    }
}
