using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using static BoardManager;
using UnityEngine.EventSystems;

public class GridBoxScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // ---- Member variables ---- //
    // --- Define --- //
    // --- Objects --- //
    [SerializeField] private Transform piecePutPosition_;
    public Transform PiecePutPosition { get { return piecePutPosition_; } }
    [SerializeField] GameObject target_;                       // isCanPiecePlace_= true : appear.
    [SerializeField] GameObject hilight_;                      // if on mouse ray = true : appera.
    private PieceScript placedPieceScript_ = null;             // get : placed piece script.
    public PieceScript PlacedPieceScript { get { return placedPieceScript_; } set { placedPieceScript_ = value; } }
    // --- flags --- //
    [SerializeField] private bool isCanPiecePlace_ = false;
    public bool IsCanPiecePlace { get { return isCanPiecePlace_; } set { isCanPiecePlace_ = value; } }
    [SerializeField] private bool isHilighPermit_ = true;
    public bool IsHilightPermit { get { return isHilighPermit_; } set { isHilighPermit_ = value; } }
    // --- Nums --- //
    private int col_;
    private int row_;

    private void Start()
    {
        // ---- Initialize ---- //
        hilight_.SetActive(false);
        isCanPiecePlace_ = false;
        isHilighPermit_ = false;
    }

    // ---- Member functions ---- //
    public void SetColAndRow(int col, int row) { col_ = col; row_ = row; }
    public void GetColAndRow(out int col, out int row) { col = col_; row = row_; }
    private void FixedUpdate()
    {
        if (isCanPiecePlace_)
        {
            target_.SetActive(true);
        } else
        {
            target_.SetActive(false);
        }
    }

    // --- Collider --- //
    private void OnTriggerEnter(Collider other)
    {
        // --- Get piece information --- //
        var piece = other.GetComponent<PieceScript>();
        if(!piece) { return; }
        placedPieceScript_ = piece;     // get piece info.
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // --- select a box -> status update --- //
        if (!isHilighPermit_) { return; }
        if (placedPieceScript_) { return; }
        hilight_.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData) {
        hilight_.SetActive(false);
    }
}
