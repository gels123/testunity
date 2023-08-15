using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestPhysicsScript2d : MonoBehaviour
{
    public GameObject ply;
    private Transform tf;
    // Start is called before the first frame update
    void Start()
    {
        tf = ply.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("OnCollisionEnter2D col="+col.gameObject.name+" tf="+tf);
    }
}
