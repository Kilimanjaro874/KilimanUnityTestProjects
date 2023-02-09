using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class GridBoxScript : MonoBehaviour
{
    //// ---- Memver variables ---- ////
    [SerializeField]
    private Transform picePutPos_;
    public Transform PicePutPos { get { return picePutPos_; } }
    private BoardManager.State state_ = BoardManager.State.None;      // set pice state.
    public BoardManager.State State { get { return state_; } set { state_ = value; } }
    //// ---- Member functions ---- ////



    private void OnTriggerEnter(Collider other)
    {
        // valid : PiceScript only
        var piece = other.GetComponent<PieceScript>();
        if (!piece) { return; }
        state_= piece.State;    // get piece state
    }

}
