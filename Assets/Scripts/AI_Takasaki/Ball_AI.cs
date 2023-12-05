using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball_AI : MonoBehaviour
{
    [SerializeField]
    float airResistantNum;   // 空気抵抗係数
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // 空気抵抗（-kv^2）
        Vector3 vPow = rb.velocity;
        vPow.x = Mathf.Pow(vPow.x, 2);
        vPow.y = Mathf.Pow(vPow.y, 2);
        vPow.z = Mathf.Pow(vPow.z, 2);
        rb.AddForce(-airResistantNum * vPow);
    }
}
