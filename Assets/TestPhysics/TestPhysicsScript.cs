using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPhysicsScript : MonoBehaviour
{
    private Transform tf;
    private float maxSpeed = 10.0f;
    private float acceSpeed = 20.0f;
    private bool jump = false;
    private bool ground = false;
    private Rigidbody rigidbody;
    private Vector3 vole = new Vector3(0.0f, 0.0f, 0.0f);
    
    // Start is called before the first frame update
    void Start()
    {
        tf = gameObject.GetComponent<Transform>();
        rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (ground && jump)
        {
            ground = false;
            jump = false;
            rigidbody.AddForce(new Vector3(0,0.3f,0.5f) * 2.5f, ForceMode.Impulse);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ground)
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            //=vec.Normalize()=vec.normalized单位向量, 确保矢量不大于1可以让斜着走和其它方向的移动保持相同速度
            // Vector2 vec = Vector2.ClampMagnitude(new Vector2(x, z), 1.0f);
            Vector2 desiredV = new Vector2(x, z).normalized * maxSpeed; //最大期望速度
            // if (desiredV != Vector2.zero)
            // {
            //     Debug.Log("Update desiredV=" + desiredV);
            // }
            float changeV = acceSpeed * Time.deltaTime; //本帧速度变化
            if (vole.x < desiredV.x)
            {
                vole.x = Math.Min(vole.x + changeV, desiredV.x);
            } else if (vole.x > desiredV.x)
            {
                vole.x = Math.Max(vole.x - changeV, desiredV.x);
            }
            if (vole.z < desiredV.y)
            {
                vole.z = Math.Min(vole.z + changeV, desiredV.y);
            } else if (vole.z > desiredV.y)
            {
                vole.z = Math.Max(vole.z - changeV, desiredV.y);
            }
            Vector3 distance = vole * Time.deltaTime;
            tf.transform.position += distance;
            // rigidbody.velocity = vole;
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("forword===" + tf.forward);
            jump = true;
        }
        
        // // 单位化向量, vec2被改变
        // Vector2 vec1 = new Vector2(2, 2);
        // vec1.Normalize();
        // // 两个向量的夹角
        // Vector2 vec2 = new Vector2(1, 0);
        // float agl = Vector2.Angle(vec1, vec2);
        // // 向量的长度, 且不超过上限
        // Vector2 len = Vector2.ClampMagnitude(vec1, 1.0f);
        // // 向量差值, 第3个参数范围[0,1], 
        // Vector2 lerp1 = Vector2.Lerp(vec1, vec2, 0.5f);
        // // 向量差值, 每次步长不超过第3个参数, 差值效果比Lerp好, 也更消耗性能
        // Vector2 lerp2 = Vector2.MoveTowards(vec1, vec2, 0.1f);
        // // 向量缩放, 返回向量的乘积
        // Vector2 vec3 = Vector2.Scale(vec1, vec2);
    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter collision=" + collision.gameObject.name);
        if (collision.gameObject.name == "Plane")
        {
            ground = true;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter other=" + other);
    }
}
