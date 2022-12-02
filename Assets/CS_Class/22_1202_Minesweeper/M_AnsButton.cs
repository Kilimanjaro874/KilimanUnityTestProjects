using UnityEngine;

public class M_AnsButton : MonoBehaviour
{
    [SerializeField]
    private Minesweeper _minesweeper;

    public void GetAnswer()
    {
        _minesweeper.OpenAnswer();
    }
}
