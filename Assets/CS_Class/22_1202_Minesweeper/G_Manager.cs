using UnityEngine;

public class G_Manager : MonoBehaviour
{
    // ----- �����o�ϐ� ----- //
    public enum CellState
    {
        // ---- Cell��Ԓ�` ---- //
        None = 0,       //
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,

        Mine = -1,
    }

    public enum OptionState
    {
        // ---- �����̎��o�I�ȏ��(�N���A���ɉe���Ȃ�)���` ---- //
        None = 0,
        Flag = 1,
        Question = 2,
        End,
    }

}
