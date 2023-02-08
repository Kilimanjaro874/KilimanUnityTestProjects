using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    private float deg = 0f;
    private float length = 5.0f;
    private float length_calc = 0f;
    public int healts = 5;
    public float width = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        length_calc = length +  width * Mathf.Sin(healts * deg);

        transform.position = new Vector3(length_calc * Mathf.Cos(deg), 0.5f, length_calc * Mathf.Sin(deg));
        deg += 2f * Mathf.PI / 360f;
    }
}
