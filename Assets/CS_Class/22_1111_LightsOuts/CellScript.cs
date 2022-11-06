using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellScript : MonoBehaviour
{
    // Cell‚Ìó‘Ô“™‚ğŠi”[
    [SerializeField]
    private bool _is_on = false;
    public bool LightOn { get { return _is_on; } set { _is_on = value; } }
    [SerializeField]
    private int row_;
    public int Row { get { return row_; } set { row_ = value; } }
    [SerializeField]
    private int col_;
    public int Col { get { return col_; } set { col_ = value; } }
}
