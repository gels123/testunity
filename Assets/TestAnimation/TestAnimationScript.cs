using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnimationScript : MonoBehaviour
{
    public GameObject cube1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            Animation ani = cube1.GetComponent<Animation>();
            // ani.Play("AnimationX");
            ani.CrossFade("AnimationX");
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            Animation ani = cube1.GetComponent<Animation>();
            ani.CrossFade("AnimationY");
        }
    }

    void OnAnimationHalfX()
    {
        Debug.Log("==OnAnimationHalfX==");
    }
}
