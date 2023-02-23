using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceScript : MonoBehaviour
{
    // ---- Member variables ---- //
    // --- Define --- //
    [SerializeField]
    private BoardManager.State state_ = BoardManager.State.None;
    public BoardManager.State State { get { return state_; } set { state_ = value; } }
    private Vector3 pieceBlackRotateAngle_ = new Vector3(180, 0, 0);     // black rotation (deg)
    private Vector3 pieceWhiteRotateAngle_ = new Vector3(0, 0, 0);       // white rotation (deg)
    [SerializeField] float pieceUpHeight_ = 2.5f;        // piece move height : Piece pull out from PieceStocker.
    [SerializeField] float piecePullOutTime_ = 0.3f;     // piece move height time.
    [SerializeField] float pieceMoveTime_ = 0.2f;        // piece move time to target pos.
    [SerializeField] float pieceTurnTime = 0.2f;         // piece turn move time in current pos.
    // --- flags --- //
    private bool isMoving_ = false;
    public bool IsMoving { get { return isMoving_;} set { isMoving_ = value; } }

    // ---- Member functions ---- //
    public IEnumerator pull(Vector3 targetPos)
    {
        // --- pull out Piece from PieceStocker --- //
        isMoving_ = true;       // piece moving flag.
        Vector3 movePos = transform.position + Vector3.up * pieceUpHeight_;
        yield return TranslateMove(movePos, piecePullOutTime_);
        Vector3 rotRef = (state_ == BoardManager.State.White) ? pieceWhiteRotateAngle_ : pieceBlackRotateAngle_;
        yield return TranslateAndRotMove(targetPos, rotRef, pieceMoveTime_);
        isMoving_= false;

        yield return null;
    }

    public IEnumerator put(Vector3 putPos)
    {
        Vector3 movePos = putPos + Vector3.up * pieceUpHeight_;
        yield return TranslateMove(movePos, pieceMoveTime_);
        yield return TranslateMove(putPos, pieceMoveTime_);

        yield return null;
    }

    public IEnumerator turnPieceInCurrentPos(BoardManager.State state)
    {
        Vector3 angleRef = (state == BoardManager.State.White) ? pieceWhiteRotateAngle_ : pieceBlackRotateAngle_;
        yield return TranslateAndRotMove(transform.position, angleRef, pieceTurnTime);

        yield return null;
    }

    public IEnumerator turnPiece(BoardManager.State state)
    {
        Vector3 currentPos = transform.position;
        Vector3 movePos = transform.position + Vector3.up * pieceUpHeight_;
        Vector3 rotRef = (state == BoardManager.State.White) ? pieceWhiteRotateAngle_ : pieceBlackRotateAngle_;
        yield return TranslateMove(movePos, pieceTurnTime);
        yield return TranslateAndRotMove(currentPos, rotRef, pieceTurnTime);

        yield return null;
    }

    public IEnumerator TranslateMove(Vector3 targetPos, float duration)
    {
        // --- move tranlate to targetPos -- //
        Vector3 startPos = transform.position;
        Vector3 length = targetPos - startPos;
        float velocity = length.magnitude / duration;
        for(var t = 0f; t <= duration; t += Time.deltaTime)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, targetPos, velocity * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos; // adjustment.

        yield return null;
    }
    
    public IEnumerator TranslateAndRotMove(Vector3 targetPos, Vector3 angleRef, float duration)
    {
        // -- move translate and rotate to targetPos -- //
        // - translate - //
        Vector3 startPos = transform.position;
        Vector3 length = targetPos - startPos;
        float velocity = length.magnitude / duration;
        // - rotation - //
        Vector3 currentAngle = transform.rotation.eulerAngles;
        Vector3 angleDiff = angleRef - currentAngle;
        Vector3 rotVel = angleDiff / duration;
        for(var t = 0f; t <= duration; t += Time.deltaTime)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, targetPos, velocity * Time.deltaTime);
            transform.Rotate(rotVel * Time.deltaTime, Space.World);
            yield return null;
        }
        transform.position = targetPos;     // adjustment.
        transform.eulerAngles = angleRef;   // adjustment.

        yield return null;
    }
}