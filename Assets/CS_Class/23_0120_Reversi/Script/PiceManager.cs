using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PiceManager : MonoBehaviour
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
    private PiceScript[] piceScripts;
    
    public bool GeneratePices()
    {
        if(piceScripts == null)
        {
            piceScripts = new PiceScript[pieceNum_];
        }
        for (var c = 0; c < pieceNum_; c++)
        {
            var piece = Instantiate(piece_);
            piece.transform.SetParent(pieceGenTransform_);
            piece.transform.position = pieceGenTransform_.position + pieceGenTransform_.forward * (float)c * pieceSpan_;
            var piceScript = piece.GetComponent<PiceScript>();
            if(piceScript == null) {
                Debug.Log("null error : piceScript");
                return false;
            }
            piceScripts[c] = piceScript;            
        }
        return true;
    }

    
    public PiceScript PopPiceScript()
    {
        if (pulledPieceCount >= 0)
        {
            PiceScript piceS = piceScripts[pulledPieceCount];
            pulledPieceCount--;
            return piceS;
        } else
        {
            return null;
        }
    }
}
