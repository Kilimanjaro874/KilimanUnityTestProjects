using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string test = "Cell(2,3)";
        string r_s = test.Substring(5, 1);
        string c_s = test.Substring(7, 1);
        Debug.Log(r_s);
        Debug.Log(c_s);
        int r_ = int.Parse(r_s);
        int c_ = int.Parse(c_s);
        Debug.Log(r_);
        Debug.Log(c_);
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
