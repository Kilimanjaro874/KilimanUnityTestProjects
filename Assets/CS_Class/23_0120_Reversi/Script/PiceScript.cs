using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiceScript : MonoBehaviour
{
    //// ---- Member variables ---- ////
    [SerializeField]
    private float move_speed_ = 2.0f;

    //// ---- Member functions ---- ////
    public void pullOutPice(Vector3 target_pos, Vector3 angle_ref)
    {
        StartCoroutine(pull(target_pos, angle_ref));
    }

    private IEnumerator pull(Vector3 target_pos, Vector3 angle_ref)
    {
        // move to up : world(0, y, 0)
        Vector3 move_pos = transform.position + Vector3.up * 2.5f;
        Vector3 length = move_pos - transform.position;
        float velocity = length.magnitude;
        yield return IEMoveTransform(move_pos, 0.5f);
        yield return IEMoveTransform(target_pos, 0.5f);

        yield return null;
    }

    private IEnumerator IEMoveTransform(Vector3 target_pos, float duration)
    {
        Vector3 start_pos = transform.position;
        Vector3 length = target_pos - start_pos;
        float velocity = length.magnitude / duration;
        for(var t = 0f; t < duration; t += Time.deltaTime)
        {
            transform.position =
                Vector3.MoveTowards(transform.position, target_pos, velocity * Time.deltaTime);
            yield return null;
        }
    }


}
