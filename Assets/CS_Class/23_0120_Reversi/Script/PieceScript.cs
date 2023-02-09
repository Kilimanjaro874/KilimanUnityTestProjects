using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceScript : MonoBehaviour
{
    //// ---- Member variables ---- ////
    [SerializeField]
    private float move_speed_ = 2.0f;
    private BoardManager.State state_;
    public BoardManager.State State { get { return state_; } set { state_ = value; } }
    private Vector3 piece_black_rotate_angle_ = new Vector3(180, 0, 0);     // black rotation (deg)
    private Vector3 piece_white_rotate_angle_ = new Vector3(0, 0, 0);       // white rotation (deg)

    //// ---- Member functions ---- ////
    public void pullOutPice(Vector3 target_pos)
    {
        StartCoroutine(pull(target_pos));
    }

    public IEnumerator pull(Vector3 target_pos)
    {
        // move to up : world(0, y, 0)
        Vector3 move_pos = transform.position + Vector3.up * 2.5f;
        Vector3 length = move_pos - transform.position;
        float velocity = length.magnitude;
        yield return IEMoveTransform(move_pos, 0.4f);
        Vector3 rot_ref = state_ == BoardManager.State.White ? piece_white_rotate_angle_ : piece_black_rotate_angle_;
        yield return IEMoveRotateTransform(target_pos, rot_ref, 0.3f);

        yield return null;
    }

    private IEnumerator IEMoveTransform(Vector3 target_pos, float duration)
    {
        // move translation only
        Vector3 start_pos = transform.position;
        Vector3 length = target_pos - start_pos;
        float velocity = length.magnitude / duration;
        for(var t = 0f; t <= duration; t += Time.deltaTime)
        {
            transform.position =
                Vector3.MoveTowards(transform.position, target_pos, velocity * Time.deltaTime);
            yield return null;
        }
        transform.position = target_pos;    // adjustment
    }

   private IEnumerator IEMoveRotateTransform(Vector3 target_pos, Vector3 angle_ref, float duration)
    {
        // move translation and rotate 
        // translation
        Vector3 start_pos = transform.position;
        Vector3 length = target_pos - start_pos;
        float velocity = length.magnitude / duration;
        // rotation
        Vector3 current_angle = transform.rotation.eulerAngles;
        Vector3 angle_diff = angle_ref - current_angle;
        Vector3 rot_vel = angle_diff / duration;
        for(var t = 0f; t <= duration; t += Time.deltaTime)
        {
            transform.position =
                Vector3.MoveTowards(transform.position, target_pos, velocity * Time.deltaTime);
            transform.Rotate(rot_vel * Time.deltaTime, Space.World);
            yield return null;
        }
        transform.position = target_pos;        // adjustment
        transform.eulerAngles = angle_ref;      // adjustment
    }


}
