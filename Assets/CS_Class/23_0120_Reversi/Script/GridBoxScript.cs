using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class GridBoxScript : MonoBehaviour
{
    //// ---- Memver variables ---- ////
    private BoardManager.State state_;      // set pice state.
    public BoardManager.State State { get { return state_; } set { state_ = value; } }

    
}
