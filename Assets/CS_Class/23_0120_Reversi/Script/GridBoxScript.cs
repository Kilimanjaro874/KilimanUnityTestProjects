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
    [SerializeField]
    private BoardManager.State state_ = BoardManager.State.None;      // set pice state.
    public BoardManager.State State { get { return state_; } set { state_ = value; } }
    [SerializeField]
    private bool isCanPlacePiece_ = false;   // judge: can place white.
    [SerializeField]
    private GameObject Ball;
    // check: can place a piece.
    public bool IsCanPlacePiece { get { return isCanPlacePiece_; } set { isCanPlacePiece_ = value; } }
    // box column & row
    private int column_;
    private int row_;
    // Placed piece script 
    private PieceScript placed_piece_script_= null;
    public PieceScript PlacedPieceScript { get { return placed_piece_script_; } }

    private void Update()
    {
        if (isCanPlacePiece_)
        {
            Ball.SetActive(true);
        } else
        {
            Ball.SetActive(false);
        }
    }

    //// ---- Member functions ---- ////
    private void OnTriggerEnter(Collider other)
    {
        // valid : PiceScript only
        var piece = other.GetComponent<PieceScript>();
        if (!piece) { return; }
        state_= piece.State;            // get piece state.
        placed_piece_script_ = piece;   // get piece script.
    }

    public void SetBoxColAndRow(int col, int row)
    {
        column_ = col;
        row_ = row;
    }
    public void GetBoxColAndRow(out int col, out int row)
    {
        col = column_;
        row = row_;
    }

    public void TurnPiece()
    {
        if (!placed_piece_script_) {
            Debug.Log("hey");
            return; 
        }
        placed_piece_script_.turnPiece();   // trun move
        
    }
}
