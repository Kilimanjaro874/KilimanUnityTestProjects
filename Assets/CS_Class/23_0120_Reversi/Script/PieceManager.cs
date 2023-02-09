using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
    //// ---- Memver variables ---- ////
    // --- Define --- //
    const int pieceNum_ = 32;
    int pulledPieceCount = pieceNum_ - 1;
    const float pieceSpan_ = 0.15f;
    [SerializeField]
    private Transform pieceGenTransform_;
    [SerializeField]
    GameObject piece_;      // reversi piece;
    private PieceScript[] piceScripts;
    
    public bool GeneratePices(BoardManager.State state)
    {
        if(piceScripts == null)
        {
            piceScripts = new PieceScript[pieceNum_];
        }
        for (var c = 0; c < pieceNum_; c++)
        {
            var piece = Instantiate(piece_);
            piece.transform.SetParent(pieceGenTransform_);
            piece.transform.position = pieceGenTransform_.position + pieceGenTransform_.forward * (float)c * pieceSpan_;
            var piceScript = piece.GetComponent<PieceScript>();
            piceScript.State = state;   // define White / Black
            if(piceScript == null) {
                Debug.Log("null error : piceScript");
                return false;
            }
            
            piceScripts[c] = piceScript;            
        }
        return true;
    }

    
    public PieceScript PopPiceScript()
    {
        if (pulledPieceCount >= 0)
        {
            PieceScript piceS = piceScripts[pulledPieceCount];
            pulledPieceCount--;
            return piceS;
        } else
        {
            return null;
        }
    }
}
