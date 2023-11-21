using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball_AI : MonoBehaviour
{
    [SerializeField]
    float airResistantNum;   // 空気抵抗係数
    Rigidbody rb;
    public BallManager manager;
    bool beenHigh;
    void Start()
    {
        beenHigh = false;
        rb = GetComponent<Rigidbody>();
        manager = transform.parent.GetComponent<BallManager>();
    }

    void FixedUpdate()
    {
        if(!beenHigh && transform.localPosition.y > 4.2f) // 4.2mを超えたらプラスの報酬
        {
            beenHigh = true;
            manager.CountHighBall();
        }
        if(beenHigh && transform.localPosition.y <= 4.2f)
        {
            beenHigh = false;
        }
        // 空気抵抗（-kv^2）
        Vector3 vPow = rb.velocity;
        vPow.x = Mathf.Pow(vPow.x, 2);
        vPow.y = Mathf.Pow(vPow.y, 2);
        vPow.z = Mathf.Pow(vPow.z, 2);
        rb.AddForce(-airResistantNum * vPow);
    }
}
