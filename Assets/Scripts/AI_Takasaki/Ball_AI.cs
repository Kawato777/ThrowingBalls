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
        // 空気抵抗（-kv）
        rb.AddForce(-airResistantNum * GetComponent<Rigidbody>().velocity);
    }
}
