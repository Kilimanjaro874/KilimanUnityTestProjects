using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PieceStocker : MonoBehaviour
{
    // ---- Member variables ---- //
    // --- Define --- //
    const int pieceNum_ = 32;
    const float pieceSpan_ = 0.15f;
    private BoardManager.State state_ = BoardManager.State.None;
    // --- Objects --- //
    [SerializeField] private Transform pieceGenTransform_;
    [SerializeField] private GameObject piceObj_;
    private PieceScript[] pieceScripts_;
    // --- num --- //
    int remainingPieceNumCount_ = pieceNum_ - 1;
    
    // ---- Memver functions ---- //
    public bool Init(BoardManager.State state)
    {
        // --- generate pieces & stock this class --- //
        // -- set state -- //
        state_ = state;             // white or black
        // -- gen pieces -- //
        if (pieceScripts_ == null)
        {
            pieceScripts_ = new PieceScript[pieceNum_];
        }
        for(var i = 0; i < pieceNum_; i++)
        {
            var piece = Instantiate(piceObj_);
            piece.transform.SetParent(pieceGenTransform_);
            piece.transform.position = 
                pieceGenTransform_.position + 
                pieceGenTransform_.forward * (float)i * pieceSpan_;
            var pieceScript = piece.GetComponent<PieceScript>();
            if (!pieceScript) { Debug.Log("error : null of PieceScript"); return false; }
            pieceScript.State = state;      // set state 
            pieceScripts_[i] = pieceScript;
        }
        return true;
    }

    public PieceScript PopPiece()
    {
        // -- return pop out Piece -- //
        if(remainingPieceNumCount_ >= 0)
        {
            var pieceScript = pieceScripts_[remainingPieceNumCount_];
            remainingPieceNumCount_--;
            return pieceScript;
        }
        else
        {
            Debug.Log("Piece : out of stock.");
            return null;
        }
    }

    public bool IsPiecesMovingCheck()
    {
        for(var i = 0; i < pieceNum_; i++)
        {
            var piece = pieceScripts_[i];
            if (piece.IsMoving)
            {
                return true;
            }
        }
        return false;
    }



}
