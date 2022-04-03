using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopBackGround : MonoBehaviour
{

    public float sizeBackgroundX = 10.24f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(transform.position.x);
        if(transform.position.x < - sizeBackgroundX)
        {
            transform.position = new Vector3(0, transform.position.y, transform.position.z);
        }
    }
}
