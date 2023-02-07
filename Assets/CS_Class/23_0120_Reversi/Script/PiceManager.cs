using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PiceManager : MonoBehaviour
{
    //// ---- Memver variables ---- ////
    // --- Define --- //
    const int PieceNum_ = 32;
    const float PieceSpan_ = 0.15f;
    [SerializeField]
    private Transform PieceGenTransform_;
    [SerializeField]
    GameObject Piece_;      // reversi piece;
    private PiceScript[] piceScripts;
    
    public bool GeneratePices()
    {
        if(piceScripts == null)
        {
            piceScripts = new PiceScript[PieceNum_];
        }
        for (var c = 0; c < PieceNum_; c++)
        {
            var piece = Instantiate(Piece_);
            piece.transform.SetParent(PieceGenTransform_);
            piece.transform.position = PieceGenTransform_.position + PieceGenTransform_.forward * (float)c * PieceSpan_;
            var piceScript = piece.GetComponent<PiceScript>();
            if(piceScript == null) {
                Debug.Log("null error : piceScript");
                return false;
            }
            piceScripts[c] = piceScript;            
        }
        return true;
    }

}
