using UnityEngine;

public class G_Manager : MonoBehaviour
{
    // ----- メンバ変数 ----- //
    public enum CellState
    {
        // ---- Cell状態定義 ---- //
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
        // ---- 旗等の視覚的な状態(クリア等に影響なし)を定義 ---- //
        None = 0,
        Flag = 1,
        Question = 2,
        End,
    }

}
